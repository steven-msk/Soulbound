using System.Collections.Generic;

public enum ItemTag {
	Consumable,
	//...
}

public static class ItemTagExt {
	private static readonly Dictionary<ItemTag, string> specializedNames = new() {
		
	};

	public static string ToDisplayString(this ItemTag tag) {
		if (specializedNames.TryGetValue(tag, out var name)) {
			return name;
		}
		return tag.ToString();
	}
}
