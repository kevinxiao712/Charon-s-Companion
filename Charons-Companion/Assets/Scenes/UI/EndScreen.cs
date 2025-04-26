using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreen : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RestartBTN()
    {
        SceneManager.LoadScene("Living Room Scene"); 
    }

    public void MainMenuBTN()
    {
        SceneManager.LoadScene("MainMenuFinished"); 
    }

    public void GoToCreditScreen()
    {
        SceneManager.LoadScene("CreditScreen");
    }

    public void BackToEndScreen()
    {
        SceneManager.LoadScene("EndScreen");
    }
}
