using NUnit.Framework;
using SoulboundBackend.Client.UI.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Screen = SoulboundBackend.Client.UI.Screens.Screen;

namespace UITests {
	[TestFixture]
	public class ChildReferencingTests {
		[Test]
		public void Accessor_Uses_GameObjectName_When_NoOverride() {
			var obj = new GameObject("TestObj");
			var childRef = obj.AddComponent<ChildReference>();

			Assert.AreEqual("TestObj", childRef.accessor);

			GameObject.DestroyImmediate(obj);
		}

		[Test]
		public void Accessor_UsesOverride_When_Set() {
			var obj = new GameObject("GameObjectName");
			var childRef = obj.AddComponent<ChildReference>();

			var serializedObj = new SerializedObject(childRef);
			serializedObj.FindProperty("referenceOverride").stringValue = "CustomRef";
			serializedObj.ApplyModifiedPropertiesWithoutUndo();

			Assert.AreEqual("CustomRef", childRef.accessor);

			GameObject.DestroyImmediate(obj);
		}

		[Test]
		public void ChildReferenceMap_Registers_And_Returns_GameObject() {
			var obj = new GameObject("MappedObj");
			var childRef = obj.AddComponent<ChildReference>();
			var map = new ChildReferenceMap();

			map.RegisterChildReference(childRef);

			Assert.AreEqual(obj, map.GetChild(childRef.accessor));

			GameObject.Destroy(obj);
		}

		[Test]
		public void OnRegisterChildrenReferences_SendsMessageUpwards() {
			var parent = new GameObject("Parent");
			var receiver = parent.AddComponent<ChildReferenceReceiver>();

			var child = new GameObject("Child");
			child.transform.SetParent(parent.transform);
			var childRef = child.AddComponent<ChildReference>();

			childRef.OnRegisterChildrenReferences();

			Assert.IsTrue(receiver.received);

			GameObject.DestroyImmediate(receiver);
		}

		[Test]
		public void Integration_WithScreen_RegistersChild_InChildMap() {
			var parent = new GameObject("ParentScreen");
			var screen = parent.AddComponent<Screen>();

			var child = new GameObject("TestChild");
			child.transform.SetParent(parent.transform);
			var childRef = child.AddComponent<ChildReference>();

			parent.BroadcastMessage("OnRegisterChildrenReferences", SendMessageOptions.DontRequireReceiver);
			Assert.AreEqual(child, screen.childMap.GetChild("TestChild"));

			GameObject.DestroyImmediate(parent);
		}
	}

	public class ChildReferenceReceiver : MonoBehaviour {
		public bool received { get; private set; }

		public void RegisterChildReference(ChildReference reference) {
			received = true;
		}
	}
}
