using UnityEngine;

public class makeBooksFall : MonoBehaviour
{
    [SerializeField] Animation objectToFall;

    
        [Header("References")]
       
        public string playerTag = "Player";

        [Header("Pull Settings")]
        public KeyCode pullKey = KeyCode.F;
        public float pullDuration = 1.5f;     // How long the pull takes (seconds)

        private bool playerInside = false;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(playerTag))
            {
                playerInside = true;
               
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(playerTag))
            {
                playerInside = false;

               
            }
        }

        private void Update()
        {
            // If inside the trigger, not currently pulling, and user presses F
            if (playerInside && Input.GetKeyDown(pullKey))
            {
             objectToFall.Play();
            }
        }

        


    public void MakeObjectFall()
    {

    }
}
