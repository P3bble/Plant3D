using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelFail : MonoBehaviour
{
    public void Retry()
    {
        SceneManager.LoadScene("StarterLevel");
    }

    public void QuitToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
