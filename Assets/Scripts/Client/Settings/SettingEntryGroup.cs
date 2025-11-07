using SoulboundBackend.Client.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Reflection;

namespace SoulboundBackend.Client.Settings {
	public class SettingEntryGroup : MonoBehaviour {
		public void AddEntry<T>(SettingEntry<T> entry) {
			SettingVisual<T> visual = entry.valueSet.GetVisual(transform);
			visual.Show(entry);
		}

		public void AddEntry(AbstractSettingEntry entry) {
			const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
			Type entryType = typeof(SettingEntry<>).MakeGenericType(entry.valueType);

			FieldInfo valueSetField = entryType.GetField("valueSet", bindingFlags);
			object valueSet = valueSetField.GetValue(entry);

			MethodInfo getVisualMethod = valueSet.GetType().GetMethod("GetVisual", bindingFlags);
			object visual = getVisualMethod.Invoke(valueSet, new object[] { transform });

			MethodInfo bindMethod = visual.GetType().GetMethod("Bind", bindingFlags);
			bindMethod.Invoke(visual, new object[] { entry });
		}
	}
}
