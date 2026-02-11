using SoulboundBackend.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.UI.Screens {
	public abstract class Screen {
		protected IScreenNavigator screenNavigator;

		public void Init(IScreenNavigator navigator) {
			this.screenNavigator = navigator;
		}

		public virtual IScreenObject BuildObject(IScreenObjectFactory objFactory) {
			GameObject gameObject = objFactory.CreateGameObject();
			IScreenObject obj = objFactory.CreateSceneObject(this, gameObject);
			OnBuild(obj);
			return obj;
		}

		protected abstract void OnBuild(IScreenObject screenObject);

		public virtual void OnShow(IScreenObject obj) { }

		public virtual void OnHide(IScreenObject obj) { }

		public virtual void OnDispose(IScreenObject obj) { }
	}
}
