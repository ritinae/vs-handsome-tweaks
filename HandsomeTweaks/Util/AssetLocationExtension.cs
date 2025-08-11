using Vintagestory.API.Common;

namespace Jakojaannos.HandsomeTweaks.Util;

public static class AssetLocationExtension {
	public static AssetLocation WithoutPathPart(this AssetLocation @this, string value) {
		var basePath = @this.Path;
		var suffix = $"-{value}";
		if (basePath.EndsWith(suffix)) {
			basePath = basePath[..^suffix.Length];
		} else if (basePath.Contains($"-{value}-")) {
			basePath = basePath.Replace($"-{value}-", "-");
		}

		return new(@this.Domain, basePath);
	}
}
