using UnityEngine;
using UnityEngine.UI;
using TMPro;
[System.Serializable]
public class TutorialData
{
    public GameObject tutorialUI;
    [HideInInspector]
    public bool hasShown = false; 
}

public class TutorialManagement : MonoBehaviour
{
    [Header("TutorialData")]
    public TutorialData[] tutorials;

    [Header("Player")]
    public GameObject player; 
    private PlayerMovement playerMovement; 


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (var t in tutorials)
        {
            if (t.tutorialUI != null)
                t.tutorialUI.SetActive(false);
        }

        if (player != null)
            playerMovement = player.GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            for (int i = 0; i < tutorials.Length; i++)
            {
                if (tutorials[i].tutorialUI.activeSelf)
                {
                    HideTutorial(i);
                    break;
                }
            }
        }
    }

    public void ShowTutorial(int index)
    {
        if (index >= 0 && index < tutorials.Length)
        {
            TutorialData t = tutorials[index];
            if (!t.hasShown)
            {
                t.hasShown = true;
                if (t.tutorialUI != null)
                    t.tutorialUI.SetActive(true);
                // Disable player movement
                if (playerMovement != null)
                    playerMovement.enabled = false;
            }
        }
    }

    public void HideTutorial(int index)
    {
        if (index >= 0 && index < tutorials.Length)
        {
            TutorialData t = tutorials[index];
            if (t.tutorialUI != null)
                t.tutorialUI.SetActive(false);
            
            if (playerMovement != null)
            {
                bool anyActive = false;
                foreach (var tut in tutorials)
                {
                    if (tut.tutorialUI.activeSelf)
                    {
                        anyActive = true;
                        break;
                    }
                }
                if (!anyActive)
                    playerMovement.enabled = true;
            }
        }
    }
}
