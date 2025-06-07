using System;

[Serializable]
public struct Bool2 {
	public bool x;
	public bool y;

	public Bool2(bool x, bool y) {
		this.x = x;
		this.y = y;
	}

	public static readonly Bool2 True = new(true, true);
	public static readonly Bool2 False = new(false, false);
}