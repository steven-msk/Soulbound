using SoulboundEngine.Common;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace SoulboundEngine.UnityClient.Settings.View {
	using Component = UnityEngine.Component;

	[PROTOTYPICAL]
	public class SettingEntryGroup : MonoBehaviour {
		private List<GameObject> toDestroy = new();

		public SettingContainerBuilder AddEntry<T>(SettingEntry<T> entry) {
			SettingContainerBuilder containerBuilder = new(this, entry);
			GameObject container = containerBuilder.ConstructContainer();
			this.toDestroy.Add(container);

			SettingVisual<T> visual = entry.valueSet.GetVisual(this.transform);
			visual.transform.SetParent(container.transform, false);
			visual.Show(entry);

			//TooltipTrigger tooltipTrigger = container.AddComponent<TooltipTrigger>();
			//tooltipTrigger.Init(entry.tooltipSupplier());

			return containerBuilder;
		}

		public SettingContainerBuilder AddEntry(SettingEntry entry) {
			const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
			Type entryType = typeof(SettingEntry<>).MakeGenericType(entry.valueType);

			FieldInfo valueSetField = entryType.GetField("valueSet", bindingFlags);
			object valueSet = valueSetField.GetValue(entry);

			MethodInfo getVisualMethod = valueSet.GetType().GetMethod("GetVisual", bindingFlags);
			object visual = getVisualMethod.Invoke(valueSet, new object[] { this.transform });

			SettingContainerBuilder containerBuilder = new(this, entry);
			GameObject container = containerBuilder.ConstructContainer();
			(visual as Component).transform.SetParent(container.transform);
			this.toDestroy.Add(container);

			MethodInfo bindMethod = visual.GetType().GetMethod("Show", bindingFlags);
			bindMethod.Invoke(visual, new object[] { entry });

			//TooltipTrigger tooltipTrigger = container.AddComponent<TooltipTrigger>();
			//tooltipTrigger.Init(entry.tooltipSupplier());

			return containerBuilder;
		}

		private void OnDisable() => this.DestroyVisuals();

		public void DestroyVisuals() {
			foreach (var obj in this.toDestroy) {
				GameObject.Destroy(obj);
			}
			this.toDestroy.Clear();
		}
	}
}
