using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    public TutorialManagement tutorialManagement;
    public int tutorialID; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (tutorialManagement != null)
            {
                tutorialManagement.ShowTutorial(tutorialID);
            }
        }
    }
}
