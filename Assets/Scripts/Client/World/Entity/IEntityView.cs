using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Core.Debug.Command {
	public interface IEntityView {
		Guid GetGuid();
		string GetID();
		Vector2 GetPos();
	}
}
