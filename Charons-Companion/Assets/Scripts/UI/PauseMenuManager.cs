using UnityEngine;
using UnityEngine.SceneManagement;



public class PauseMenu : MonoBehaviour
{
    [Header("UI Objects")]
    [Tooltip("Entire pause‑menu canvas or panel")]
    public GameObject pauseCanvas;
    [Tooltip("Controls / How‑to‑Play panel (optional)")]
    public GameObject controlsPanel;

    [Header("Keys")]
    public KeyCode pauseKey = KeyCode.Escape;

    [Header("Behaviour")]
    public bool pauseAudio = true;             
    [Tooltip("Scripts (camera look, player input, etc.) that should turn off while paused")]
    public MonoBehaviour[] scriptsToDisable;

    bool isPaused;

    void Start()
    {
        if (pauseCanvas != null) pauseCanvas.SetActive(false);
        if (controlsPanel != null) controlsPanel.SetActive(false);
        SetCursorLock(true);                 
    }

    void Update()
    {
        if (!Input.GetKeyDown(pauseKey)) return;

        if (!isPaused)
            PauseGame();
        else
        {
            if (controlsPanel != null && controlsPanel.activeSelf)
                HideControls();
            else
                ResumeGame();
        }
    }


    public void ResumeGame() => ResumeGameInternal();
    public void QuitGame()
    {
        Time.timeScale = 1f;                    // just in case

        Application.Quit();
    }
    public void ShowControls()
    {
        if (controlsPanel != null) controlsPanel.SetActive(true);
        if (pauseCanvas != null) pauseCanvas.SetActive(true); // keep backdrop
    }
    public void HideControls()
    {
        if (controlsPanel != null) controlsPanel.SetActive(false);
    }

    void PauseGame()
    {
        if (pauseCanvas != null) pauseCanvas.SetActive(true);
        if (pauseAudio) AudioListener.pause = true;
        ToggleScripts(false);

        Time.timeScale = 0f;
        isPaused = true;
        SetCursorLock(false);
    }

    void ResumeGameInternal()
    {
        if (pauseCanvas != null) pauseCanvas.SetActive(false);
        if (controlsPanel != null) controlsPanel.SetActive(false);
        if (pauseAudio) AudioListener.pause = false;
        ToggleScripts(true);

        Time.timeScale = 1f;
        isPaused = false;
        SetCursorLock(true);
    }

    void ToggleScripts(bool enabled)
    {
        foreach (var mb in scriptsToDisable)
            if (mb != null) mb.enabled = enabled;
    }

    void SetCursorLock(bool lockCursor)
    {
        Cursor.lockState = lockCursor ? CursorLockMode.Locked
                                      : CursorLockMode.None;
        Cursor.visible = !lockCursor;
    }
}
