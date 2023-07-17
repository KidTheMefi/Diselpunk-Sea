using UnityEngine;
using UnityEngine.SceneManagement;

namespace DefaultNamespace
{
    public class AppMenu : MonoBehaviour
    {

        public void RestartGame()
        {
            SceneManager.LoadScene(0, LoadSceneMode.Single);
        }

        public void ExitGame()
        {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
                   #endif
            Application.Quit();
        }
    }
}