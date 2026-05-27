using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void LoadScene(string sceneName)
        {
            Debug.Log($"[SceneLoader] Transitioning to scene: {sceneName}");
            SceneManager.LoadScene(sceneName);
        }
    }
}
