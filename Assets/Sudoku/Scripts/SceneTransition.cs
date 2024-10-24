namespace Sudoku.Scripts
{
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class SceneTransition : MonoBehaviour
    {
        // Этот метод будет вызываться для загрузки сцены
        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }

}