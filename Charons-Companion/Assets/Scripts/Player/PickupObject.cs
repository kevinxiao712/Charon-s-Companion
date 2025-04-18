using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LetterPickup : MonoBehaviour
{
    [Header("UI References")]
    public GameObject letterPanel;          // The panel with the letter background + text
    public TextMeshProUGUI descriptionText; // The text shown on the letter
    [TextArea] public string letterMessage = "Hello! This is a letter.";

    [Header("Player Movement Reference")]
    public PlayerMovement pm;  

    [Header("Pickup Settings")]
    public KeyCode interactKey = KeyCode.G;
    public float interactRange = 2f;   // how close player must be to pick up

    private bool isShowingLetter = false;
    static readonly string[] interactTags = { "Player", "Clone" };

    Transform currentInteractor;
    void Start()
    {

        if (letterPanel != null)
        {
            letterPanel.SetActive(false);
        }
    }

    void Update()
    {
        // 1) Check for input
        if (!Input.GetKeyDown(interactKey)) return;

        // 2) Determine if someone is in range
        if (!isShowingLetter)
        {
            if (TryFindInteractor(out currentInteractor))
                ShowLetter();
        }
        else
            CloseLetterAndDestroyItem();
    }

    private bool PlayerIsInRange()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return false;

        float dist = Vector3.Distance(transform.position, player.transform.position);
        return dist <= interactRange;
    }
    bool TryFindInteractor(out Transform found)
    {
        foreach (string tag in interactTags)
        {
            foreach (var obj in GameObject.FindGameObjectsWithTag(tag))
            {
                if (Vector3.Distance(transform.position, obj.transform.position) <= interactRange)
                {
                    found = obj.transform;
                    return true;
                }
            }
        }
        found = null;
        return false;
    }
    private void ShowLetter()
    {
        // 1) Activate the UI
        letterPanel.SetActive(true);
        // 2) Set the text
        if (descriptionText != null)
        {
            descriptionText.text = letterMessage;
        }
        // 3) Freeze player movement
        if (pm != null)
        {
            pm.freeze = true;       // This sets velocity to zero each frame
            pm.restricted = true;   // This stops MovePlayer() from applying any movement
        }

        isShowingLetter = true;
    }

    private void CloseLetterAndDestroyItem()
    {
        // 1) Deactivate the letter panel
        letterPanel.SetActive(false);

        // 2) Unfreeze player
        if (pm != null)
        {
            pm.freeze = false;
            pm.restricted = false;
        }

        // 3) Destroy this item from the game world
        Destroy(gameObject);
    }
}
