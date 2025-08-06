using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

#nullable enable

public abstract class Block {
	public abstract string name { get; }
	public abstract TileBase tileReference { get; }
	public abstract BlockItem? itemReference { get; }
	public abstract BlockState defaultState { get; }

	public virtual BlockState CreateState(Dictionary<string, object> properties) {
		return defaultState;
	}

[CustomEditor(typeof(Block))]
public class BlockEditor : Editor {
	private SerializedProperty stateBehaviorProperty;

	private List<Type> behaviorTypes;
	private string[] dropdownOptions;
	private int selectedIndex = -1;

    private void OnEnable() {
        stateBehaviorProperty = serializedObject.FindProperty("stateBehavior");
		behaviorTypes = Implementations.GetAllImplementationsOf<IBlockStateBehavior>();
		dropdownOptions = behaviorTypes.Select(t => t.Name).ToArray();
		UpdateSelectedIndex();
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();
		DrawDefaultInspector();

		int newIndex = EditorGUILayout.Popup("State Behavior", selectedIndex, dropdownOptions);
		if (newIndex != selectedIndex) {
			Type selectedType = behaviorTypes[newIndex];
			stateBehaviorProperty.managedReferenceValue = Activator.CreateInstance(selectedType);
			selectedIndex = newIndex;
        }
		EditorGUILayout.Space();

		if (stateBehaviorProperty.managedReferenceValue is IBlockStateBehavior stateBehavior) {
			EditorGUILayout.Space(-8);
			EditorGUILayout.HelpBox(stateBehavior.Description, MessageType.None);

            SerializedProperty iterator = stateBehaviorProperty.Copy();
			SerializedProperty end = iterator.GetEndProperty();
			bool enterChildren = true;

			while (iterator.NextVisible(enterChildren) && !SerializedProperty.EqualContents(iterator, end)) {
				EditorGUILayout.PropertyField(iterator, true);
				enterChildren = false;
            }
		}
		serializedObject.ApplyModifiedProperties();
    }

	private void UpdateSelectedIndex() {
		if (stateBehaviorProperty.managedReferenceValue == null) {
			selectedIndex = -1;
			return;
        }
		Type currentType = stateBehaviorProperty.managedReferenceValue.GetType();
		selectedIndex = behaviorTypes.IndexOf(currentType);
    }
}