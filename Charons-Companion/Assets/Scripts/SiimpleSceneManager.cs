using UnityEngine;
using UnityEngine.SceneManagement;
public class SiimpleSceneManager : MonoBehaviour
{
    // Call this to load the next scene in build order
    public void StartGame()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
    }

    // Call this to quit the application
    public void QuitGame()
    {
        Debug.Log("Quit Game");  // This will show in editor
        Application.Quit();      // This will work in a built version
    }
}