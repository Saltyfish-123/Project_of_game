using UnityEngine;
using UnityEngine.SceneManagement;

namespace CGUsed
{
    /// <summary>
    /// Shows an end UI panel after dialogue, and loads Scene2 when button is clicked.
    /// </summary>
    public class Scene2LoadUI : MonoBehaviour
    {
        [Header("UI")]
        public GameObject root;

        [Header("Scene")]
        [Tooltip("Scene name to load (must be added in Build Settings).")]
        public string sceneName = "Scene2";

        void Awake()
        {
            if (root != null)
                root.SetActive(false);
        }

        public void Show()
        {
            if (root != null)
                root.SetActive(true);
        }

        public void Hide()
        {
            if (root != null)
                root.SetActive(false);
        }

        public void LoadScene2()
        {
            if (string.IsNullOrWhiteSpace(sceneName))
                return;

            SceneManager.LoadScene(sceneName);
        }
    }
}

