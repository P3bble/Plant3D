using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadLevel1() // load Level 1 scene
    {
        SceneManager.LoadScene("Level_1");
    }

    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void QuitGame() // quit the application
    {
  
#if UNITY_EDITOR
      
        UnityEditor.EditorApplication.isPlaying = false;
#else
        
       Application.Quit(); // think this dosent work will try closing the build differently if not
#endif
    }
}