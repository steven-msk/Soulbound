using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine.InputSystem.Android;

public interface IGameInitializable<TInstance> {
	public TInstance OnGameInit();
}
