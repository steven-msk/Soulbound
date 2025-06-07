using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

[CustomEditor(typeof(Item))]
public class ItemEditor : Editor {
	private SerializedProperty tagsProperty;
	private string[] tagNames;
	private bool[] selectedTags;

	private void OnEnable() {
		tagsProperty = serializedObject.FindProperty("tags");
		ItemTag[] allTags = Enum.GetValues(typeof(ItemTag)).Cast<ItemTag>().ToArray();
		tagNames = allTags.Select(tag => tag.ToString()).ToArray();
		selectedTags = new bool[allTags.Length];
		for (int i = 0; i < allTags.Length; i++) {
			selectedTags[i] = ContainsTag(allTags[i]);
		}
	}

	public override void OnInspectorGUI() {
		serializedObject.Update();
		DrawDefaultInspector();

		/// if ItemTag behavior gets real, uncomment this and adapt to ScriptableObject instead of Enum
		
		//EditorGUILayout.Space(10);
		//EditorGUILayout.LabelField("Item Tags", EditorStyles.boldLabel);
		//ItemTag[] allTags = Enum.GetValues(typeof(ItemTag)).Cast<ItemTag>().ToArray();
		//for (int i = 0; i < allTags.Length; i++) {
		//	bool wasSelected = selectedTags[i];
		//	selectedTags[i] = EditorGUILayout.ToggleLeft(tagNames[i], selectedTags[i]);

		//	if (selectedTags[i] != wasSelected) {
		//		if (selectedTags[i]) {
		//			tagsProperty.InsertArrayElementAtIndex(tagsProperty.arraySize);
		//			tagsProperty.GetArrayElementAtIndex(tagsProperty.arraySize - 1).enumValueIndex = (int)allTags[i];
		//		} else {
		//			for (int j = 0; j < tagsProperty.arraySize; j++) {
		//				if (tagsProperty.GetArrayElementAtIndex(j).enumValueIndex == (int)allTags[i]) {
		//					tagsProperty.DeleteArrayElementAtIndex(j);
		//					break;
		//				}
		//			}
		//		}
		//	}
		//}
		serializedObject.ApplyModifiedProperties();
	}

	private bool ContainsTag(ItemTag tag) {
		for (int i = 0; i < tagsProperty.arraySize; i++) {
			if (tagsProperty.GetArrayElementAtIndex(i).enumValueIndex == (int)tag)
				return true;
		}
		return false;
	}
}
