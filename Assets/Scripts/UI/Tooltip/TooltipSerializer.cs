using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Obsolete]
public abstract class TooltipSerializer : ScriptableObject {
	public abstract ITooltipDeserializer GetDeserializer(Item item);
}