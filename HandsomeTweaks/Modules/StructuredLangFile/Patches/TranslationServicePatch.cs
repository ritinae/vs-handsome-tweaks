using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using HarmonyLib;

using Newtonsoft.Json.Linq;

using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Util;

namespace Jakojaannos.HandsomeTweaks.Modules.StructuredLangFile.Patches;

[HarmonyPatchCategory(StructuredLangFile.PATCH_CATEGORY)]
[HarmonyPatch(typeof(TranslationService))]
public static class TranslationServicePatch {
	private static readonly JsonLoadSettings JSON_LOAD_SETTINGS = new() {
		CommentHandling = CommentHandling.Ignore,
		DuplicatePropertyNameHandling = DuplicatePropertyNameHandling.Error,
	};

	// FIXME: this could be easier implemented as a transpiler
	[HarmonyPrefix]
	[HarmonyPatch(nameof(TranslationService.Load))]
	public static bool StructuredLoad(
		TranslationService __instance,
		ref string? ___preLoadAssetsPath,
		ref bool ___loaded,
		ILogger ___logger,
		IAssetManager ___assetManager,
		Dictionary<string, string> ___entryCache,
		Dictionary<string, KeyValuePair<Regex, string>> ___regexCache,
		Dictionary<string, string> ___wildcardCache,
		bool lazyLoad = false
	) {
		Load(ref ___preLoadAssetsPath, ref ___loaded, ___logger, ___assetManager, __instance.LanguageCode, ref ___entryCache, ref ___regexCache, ref ___wildcardCache, lazyLoad);
		// Prevent the original code from running
		return false;
	}

	/* Method mostly copy-pasted from the original TranslationService. */
	// FIXME: just PR these changes to VSApi
	private static void Load(
		ref string? preLoadAssetsPath,
		ref bool loaded,
		ILogger logger,
		IAssetManager assetManager,
		string LanguageCode,
		ref Dictionary<string, string> entryCache,
		ref Dictionary<string, KeyValuePair<Regex, string>> regexCache,
		ref Dictionary<string, string> wildcardCache,
		bool lazyLoad = false
	) {
		preLoadAssetsPath = null;
		if (lazyLoad) return;
		loaded = true;

		// Don't work on dicts directly for thread safety (client and local server access the same dict)
		var localEntryCache = new Dictionary<string, string>();
		var localRegexCache = new Dictionary<string, KeyValuePair<Regex, string>>();
		var localWildcardCache = new Dictionary<string, string>();

		var origins = assetManager.Origins;

		foreach (var asset in origins.SelectMany(p => p.GetAssets(AssetCategory.lang).Where(a => a.Name.Equals($"{LanguageCode}.json") || a.Name.Equals($"worldconfig-{LanguageCode}.json")))) {

			try {
				var json = asset.ToText();

				/* === MODIFIED CODE STARTS === */
				LoadEntries(entryCache, regexCache, wildcardCache, JToken.Parse(json), asset.Location.Domain);
				/* === MODIFIED CODE ENDS === */
			} catch (Exception ex) {
				logger.Error($"Failed to load language file: {asset.Name}");
				logger.Error(ex);
			}
		}

		entryCache = localEntryCache;
		regexCache = localRegexCache;
		wildcardCache = localWildcardCache;
	}

	private static void LoadEntries(Dictionary<string, string> entryCache, Dictionary<string, KeyValuePair<Regex, string>> regexCache, Dictionary<string, string> wildcardCache, JToken json, string domain = GlobalConstants.DefaultDomain) {
		var key = new StringBuilder(domain, 256)
			.Append(AssetLocation.LocationSeparator);
		LoadEntries(entryCache, regexCache, wildcardCache, json, key, domain, isFirstPart: true);
	}

	private static void LoadEntries(Dictionary<string, string> entryCache, Dictionary<string, KeyValuePair<Regex, string>> regexCache, Dictionary<string, string> wildcardCache, JToken json, StringBuilder key, string domain, bool isFirstPart) {
		switch (json) {
			case JObject jsonObject:
				if (!isFirstPart) {
					key.Append('-');
				}

				var prefixLength = key.Length;
				foreach (var property in jsonObject.Properties()) {
					key.Length = prefixLength;
					key.Append(property.Name);
					LoadEntries(entryCache, regexCache, wildcardCache, property.Value, key, domain, isFirstPart: false);
				}
				break;
			case JValue jsonValue when jsonValue.Type == JTokenType.String && !isFirstPart:
				LoadEntry(entryCache, regexCache, wildcardCache, key, jsonValue.ToString(), domain);
				break;
			default:
				throw new InvalidOperationException($"Unexpected token: {json.Type}");
		}
	}

	private static void LoadEntry(Dictionary<string, string> entryCache, Dictionary<string, KeyValuePair<Regex, string>> regexCache, Dictionary<string, string> wildcardCache, StringBuilder keyBuilder, string value, string domain) {
		var key = EnsureSingleDomainPrefix(keyBuilder, domain);
		switch (key.CountChars('*')) {
			case 0:
				entryCache[key] = value;
				break;
			case 1 when key.EndsWith('*'):
				wildcardCache[key.TrimEnd('*')] = value;
				break;
			// we can probably do better here, as we have our own wildcardsearch now
			default: {
					var regex = new Regex("^" + key.Replace("*", "(.*)") + "$", RegexOptions.Compiled);
					regexCache[key] = new KeyValuePair<Regex, string>(regex, value);
					break;
				}
		}
	}

	private static string EnsureSingleDomainPrefix(StringBuilder keyBuilder, string domain = GlobalConstants.DefaultDomain) {
		var key = keyBuilder.ToString();
		var defaultDomainEndIndex = domain.Length + 1;
		if (key.IndexOf(AssetLocation.LocationSeparator, defaultDomainEndIndex) >= 0) {
			// Key contains a custom prefix, drop the default prefix
			return key[defaultDomainEndIndex..];
		}

		return key;
	}

	[HarmonyReversePatch]
	[HarmonyPatch(typeof(TranslationService), "LoadEntry")]
	public static void LoadEntry(
		object instance,
		Dictionary<string, string> entryCache,
		Dictionary<string, KeyValuePair<Regex, string>> regexCache,
		Dictionary<string, string> wildcardCache,
		KeyValuePair<string, string> entry,
		string domain
	) => throw new NotImplementedException("Reverse patch stub called!");
}
