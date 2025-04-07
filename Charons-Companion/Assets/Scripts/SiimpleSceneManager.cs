using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class SiimpleSceneManager : MonoBehaviour
{
    public GameObject imagePanel;
    // Call this to load the next scene in build order
    public void StartGame()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
    }
    private void Start()
    {
        // Ensure the panel is initially hidden
        imagePanel.SetActive(false);
    }

        public void QuitGame()
    {
        Debug.Log("Quit Game");  // This will show in editor
        Application.Quit();      // This will work in a built version
    }
    private void Update()
    {
        if (imagePanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePanel();
        }
    }
    public void OpenPanel()
    {
        imagePanel.SetActive(true);
    }
    public void ClosePanel()
    {
        imagePanel.SetActive(false);
    }
}