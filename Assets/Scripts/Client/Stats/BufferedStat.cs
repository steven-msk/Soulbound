using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class BufferedStat : SerializableStat {
	[SerializeReference] private IBufferedTrigger applyBufferedTrigger;
	[SerializeReference] private IBufferedTrigger revokeBufferedTrigger;
	private bool applied = false;

	public BufferedStat(SerializedStatReference serializedReference, StatValueType valueType, StatApplicationType appliance, object value, bool applyAsBonus,
						IBufferedTrigger applyBufferedTrigger, IBufferedTrigger revokeBufferedTrigger)
			: base(serializedReference, valueType, appliance, value, applyAsBonus) {
		this.applyBufferedTrigger = applyBufferedTrigger;
		this.revokeBufferedTrigger = revokeBufferedTrigger;
	}

	public void EnableBuffers() {
		applyBufferedTrigger.Enable(this);
		revokeBufferedTrigger.Enable(this);
	}

	public void DisableBuffers() {
		applyBufferedTrigger.Disable(this);
		revokeBufferedTrigger.Disable(this);
	}
}

[CustomPropertyDrawer(typeof(BufferedStat))]
public class BufferedStatDrawer : PropertyDrawer {

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		EditorGUI.BeginProperty(position, label, property);
		float ypos = position.y;
		float lineHeight = EditorGUIUtility.singleLineHeight;
		float verticalSpacing = EditorGUIUtility.standardVerticalSpacing;
		SerializedProperty iterator = property.Copy();
		SerializedProperty endProperty = iterator.GetEndProperty();
		iterator.NextVisible(true);

		while(!SerializedProperty.EqualContents(iterator, endProperty)) {
			if (iterator.name != "applyBufferedTrigger" || iterator.name != "revokeBufferedTrigger") {
				Rect rect = new(position.x, ypos, position.width, lineHeight);
				EditorGUI.PropertyField(rect, iterator, true);
				ypos += lineHeight + verticalSpacing;
			}
			if (!iterator.NextVisible(false)) {
				break;
			}
		}

		// FIXME: buffer dropdown overlapping in inspector

		SerializedProperty applyProperty = property.FindPropertyRelative("applyBufferedTrigger");
		DrawBufferProperty(new Rect(position.x, ypos, position.width, lineHeight), applyProperty, "Apply Buffered Trigger");
		ypos += lineHeight + verticalSpacing;
		if (applyProperty.managedReferenceValue != null) {
			float height = EditorGUI.GetPropertyHeight(applyProperty, true);
			EditorGUI.PropertyField(new Rect(position.x, ypos, position.width, height), applyProperty, true);
			ypos += height + verticalSpacing;
		}

		SerializedProperty revokeProperty = property.FindPropertyRelative("revokeBufferedTrigger");
		DrawBufferProperty(new Rect(position.x, ypos, position.width, lineHeight), revokeProperty, "Revoke Buffered Trigger");
		ypos += lineHeight + verticalSpacing;
		if (revokeProperty.managedReferenceValue != null) {
			float height = EditorGUI.GetPropertyHeight(revokeProperty, true);
			EditorGUI.PropertyField(new Rect(position.x, ypos, position.width, height), revokeProperty, true);
			ypos += height + verticalSpacing;
		}

		EditorGUI.EndProperty();
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
		float height = 0f;
		float spacing = EditorGUIUtility.standardVerticalSpacing;

		SerializedProperty iterator = property.Copy();
		SerializedProperty endProperty = iterator.GetEndProperty();
		iterator.NextVisible(true);

		while (!SerializedProperty.EqualContents(iterator, endProperty)) {
			if (iterator.name != "applyBufferedTrigger" || iterator.name != "revokeBufferedTrigger") {
				height += EditorGUI.GetPropertyHeight(iterator, true) + spacing;
			}
			if (!iterator.NextVisible(false)) {
				break;
			}
		}

		var applyProperty = property.FindPropertyRelative("applyBufferedTrigger");
		height += EditorGUIUtility.singleLineHeight + spacing;
		if (applyProperty.managedReferenceValue != null) {
			height += EditorGUI.GetPropertyHeight(applyProperty, true) + spacing;
		}

		var revokeProperty = property.FindPropertyRelative("revokeBufferedTrigger");
		height += EditorGUIUtility.singleLineHeight + spacing;
		if (revokeProperty.managedReferenceValue != null) {
			height += EditorGUI.GetPropertyHeight(revokeProperty, true) + spacing;
		}

		return height;
	}


	private void DrawBufferProperty(Rect position, SerializedProperty property, string label) {
		EditorGUI.BeginProperty(position, GUIContent.none, property);
		void ShowTypeDropdown(SerializedProperty property, string label) {
			List<Type> allTypes = GetAllImplementationsOf<IBufferedTrigger>();
			GenericMenu menu = new();
			foreach(var type in allTypes) {
				menu.AddItem(new GUIContent(type.FullName), false, () => {
					property.serializedObject.Update();
					property.managedReferenceValue = Activator.CreateInstance(type);
					property.serializedObject.ApplyModifiedProperties();
				});
			}
			menu.ShowAsContext();
		}

		if (property.managedReferenceValue != null) {
			Type type = property.managedReferenceValue.GetType();
			if (GUI.Button(position, $"{label} ({type.Name})", EditorStyles.popup)) {
				ShowTypeDropdown(property, label);
			}
		} else {
			if (GUI.Button(position, $"Select {label} Type", EditorStyles.popup)) {
				ShowTypeDropdown(property, label);
			}
		}
		EditorGUI.EndProperty();
	}

	private static List<Type> GetAllImplementationsOf<T>() {
		return AppDomain.CurrentDomain.GetAssemblies()
			.SelectMany(assemblies => assemblies.GetTypes())
			.Where(type => typeof(T).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
			.ToList();
	}
}