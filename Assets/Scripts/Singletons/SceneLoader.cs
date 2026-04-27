using SingletonManager;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Singletons
{
    public class SceneLoader : SingletonPersistent
    {
        public string TargetScene { get; private set; } = "SampleScene";

        public void SceneLoad(string sceneName = "SampleScene")
        {
            TargetScene = sceneName;
            // Load the loading scene
            SceneManager.LoadScene("LoadingScene");
        }
    }
}
