using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class $objectref$ : TooltipSerializer {
	public override ITooltipDeserializer GetDeserializer(Item item) => new $serializer$(item);
}

internal class $serializer$ : ITooltipSerializer {
	private Item item;

	public $serializer$(Item item) => this.item = item;
	
	public AbstractTooltip Generate() {

	}
}

internal class $tooltipOverride$ : $tooltipOverrideBase$ {


	public override void Update(ItemStack itemStack) {
		base.Update(itemStack);
		
	}
}