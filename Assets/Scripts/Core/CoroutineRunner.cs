using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Core {
    internal sealed class CoroutineRunner : MonoBehaviour {
        public static CoroutineRunner instance { get; private set; }

        private void Awake() {
            if (instance != null && instance != this) {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public static CoroutineRunner GetInstance() {
            if (instance != null) {
                return instance;
            }

            var gameObject = new GameObject("CoroutineRunner");
            instance = gameObject.AddComponent<CoroutineRunner>();
            DontDestroyOnLoad(gameObject);
            return instance;
        }
    }
}
