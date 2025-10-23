using SoulboundBackend.Core;
using SoulboundBackend.Core.Resource;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SoulboundBackend.Tests {
    internal class PlayModeTesting {
        public static Scene CreateNewTestScene() {
            return SceneManager.CreateScene(Guid.NewGuid().ToString());
        }

        public static Scene CreateNewSceneAndSetActive() {
            var scene = CreateNewTestScene();
            SceneManager.SetActiveScene(scene);
            return scene;
        }

        public static IEnumerator UnloadSceneAsync(Scene scene) {
            var async = SceneManager.UnloadSceneAsync(scene);
            yield return new WaitUntil(() => async.isDone);
        }
    }
}
