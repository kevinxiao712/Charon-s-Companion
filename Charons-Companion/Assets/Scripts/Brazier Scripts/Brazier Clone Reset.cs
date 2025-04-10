using UnityEngine;

public class BrazierCloneReset : MonoBehaviour
{
    [SerializeField] clone playerScript;

    private void OnTriggerEnter(Collider other)
    {
        if (playerScript.CloneExists)
        {
            playerScript.DestroyClone();
        }
        
    }
}
