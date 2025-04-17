using UnityEngine;
using UnityEngine.SceneManagement;


public class StartMenu : MonoBehaviour
{

    public string sceneToLoad = "SampleScene";

    public GameObject buttonsGroup;
    public GameObject infoPanel;

    [Header("Keys")]
    public KeyCode closeInfoKey = KeyCode.Escape;

    void Start()
    {
        // Make sure the expected objects are in a safe default state.
        if (buttonsGroup != null) buttonsGroup.SetActive(true);
        if (infoPanel != null) infoPanel.SetActive(false);

        // A start menu usually wants a visible, unlocked cursor.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        if (infoPanel != null && infoPanel.activeSelf && Input.GetKeyDown(closeInfoKey))
        {
            HideInfo();              // close panel with Esc
        }
    }


    public void StartGame()
    {
        if (int.TryParse(sceneToLoad, out int index))
            SceneManager.LoadScene(index);
        else
            SceneManager.LoadScene(sceneToLoad);
    }

    public void QuitGame()
    {

        Application.Quit();
    }

    public void ShowInfo()
    {
        if (infoPanel != null) infoPanel.SetActive(true);
        if (buttonsGroup != null) buttonsGroup.SetActive(false);
    }

    public void HideInfo()
    {
        if (infoPanel != null) infoPanel.SetActive(false);
        if (buttonsGroup != null) buttonsGroup.SetActive(true);
    }
}
