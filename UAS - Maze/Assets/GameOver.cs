using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class GameOver : MonoBehaviour
{
    public void RetryGame()
    {
        Debug.Log("Retry button clicked!");
        SceneManager.LoadScene("PlayScene");
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Quit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        Debug.Log("Player has quit the game");
    }
}
