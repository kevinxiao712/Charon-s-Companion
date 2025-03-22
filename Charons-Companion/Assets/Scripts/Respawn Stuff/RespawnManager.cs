using Unity.Cinemachine;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    [SerializeField] private Transform[] respawnPoints;

    /// <summary>
    /// Respawns a player at the specified respawn point index.
    /// </summary>
    public void RespawnPlayer(GameObject player, int respawnIndex)
    {
        if (respawnIndex < 0 || respawnIndex >= respawnPoints.Length)
        {
            Debug.LogError("Respawn index out of range!");
            return;
        }

        // Move player to the chosen respawn point's position & rotation
        player.transform.position = respawnPoints[respawnIndex].position;
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
