using UnityEngine;
using System.Collections;

public class DangerZone : MonoBehaviour
{
    [Header("Player References")]
    public PlayerMovement playerMovement;
    public clone cloneScriptOnPlayer;
    [Header("Assign your Player in the Inspector")]
    public GameObject player;    // The Player root object
    [SerializeField] private int respawnIndex;
    [SerializeField] private float respawnDelay = 1f;   // How long to wait before respawning
    public RespawnManager respawnManager;

    [Header("Fade UI Reference")]
    [SerializeField] private Fadeout fadeUI;             // Drag in your FadeUI script from the scene/prefab
    private void Awake()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Clone"))
        {
            if (cloneScriptOnPlayer != null)
            {
                cloneScriptOnPlayer.DestroyClone();
            }
            else
            {
                Debug.LogWarning("DangerZone: 'cloneScriptOnPlayer' is not assigned in the Inspector.");
            }

            // B) Play a random clip from the player's audio
            if (playerMovement != null && playerMovement.audioSource != null &&
                playerMovement.outOfJumpsClips != null && playerMovement.outOfJumpsClips.Length > 0)
            {
                PlayRandomClip(playerMovement.outOfJumpsClips, playerMovement.audioSource);
            }

        }
        else
        {
            GameObject rootObj = other.transform.root.gameObject;
            if (playerMovement != null && rootObj == playerMovement.gameObject)
            {
                // Start the death/respawn sequence for the *player*
                if (respawnManager != null)
                {
                    StartCoroutine(PlayerDeathAndRespawnRoutine(rootObj));
                }
            }
        }
    }

    // Example method for playing a random clip
    private void PlayRandomClip(AudioClip[] clips, AudioSource source)
    {
        int index = Random.Range(0, clips.Length);
        AudioClip chosenClip = clips[index];
        source.PlayOneShot(chosenClip);
    }
    private IEnumerator PlayerDeathAndRespawnRoutine(GameObject playerObj)
        {
            // 1) Disable player movement
            PlayerMovement playerMovement = playerObj.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.enabled = false;
            }

            // 2) Fade to black
            if (fadeUI != null)
            {
                yield return StartCoroutine(fadeUI.Fade(1f));  // alpha goes from current to 1.0
            }

            // 3) Optional: Wait additional delay if you want a pause while black
            yield return new WaitForSeconds(respawnDelay);

            // 4) Respawn the player
            respawnManager.RespawnPlayer(playerObj, respawnIndex);

            // 5) Fade from black
            if (fadeUI != null)
            {
                yield return StartCoroutine(fadeUI.Fade(0f));  // alpha from 1.0 back to 0
            }

            // 6) Re-enable player movement
            if (playerMovement != null)
            {
                playerMovement.enabled = true;
            }
        }
}
