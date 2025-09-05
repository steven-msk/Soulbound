using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ItemIcon {
	public Sprite sprite { get; }
	public float intendedPixelsPerUnit { get; }

	public float importedPixelsPerUnit => sprite.pixelsPerUnit;

	public ItemIcon(Sprite sprite, float pixelsPerUnit) {
		this.sprite = sprite;
		this.intendedPixelsPerUnit = pixelsPerUnit;
	}
}
