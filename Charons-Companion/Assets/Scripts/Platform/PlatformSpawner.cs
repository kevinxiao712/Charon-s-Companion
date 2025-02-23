using System.Collections.Generic;
using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpotPair
    {
        public Collider spotA;         // The trigger collider for spot A
        public Collider spotB;         // The trigger collider for spot B
        public GameObject platform;    // The line/platform to activate
    }
    [Header("Configure your pairs below")]
    public SpotPair[] spotPairs;

    // Maps each spot collider -> occupant type: "Player" or "Clone"
    private Dictionary<Collider, string> occupantMap = new Dictionary<Collider, string>();

    // Called by SpotTrigger when a Player or Clone enters a spot
    public void SetOccupant(Collider spot, string occupant)
    {
        occupantMap[spot] = occupant;
    }

    // Called by SpotTrigger when a Player or Clone exits a spot
    public void ClearOccupant(Collider spot)
    {
        if (occupantMap.ContainsKey(spot))
        {
            occupantMap.Remove(spot);
        }
    }



    private void Update()
    {
        // Check all pairs
        foreach (var pair in spotPairs)
        {
            // Read occupant for spotA and spotB (if present)
            string occupantA = occupantMap.ContainsKey(pair.spotA) ? occupantMap[pair.spotA] : "";
            string occupantB = occupantMap.ContainsKey(pair.spotB) ? occupantMap[pair.spotB] : "";



            bool isPlayerInEither = occupantA == "Player" || occupantB == "Player";

            if (isPlayerInEither)
            {
                Debug.Log("Player is in triggers for platform: " + pair.platform.name);
            }
            bool cloneis = occupantA == "Clone" || occupantB == "Clone";

            if (cloneis)
            {
                Debug.Log("Player is in triggers for platform: " + pair.platform.name);
            }




            bool correctOccupants = (occupantA == "Player" && occupantB == "Clone") || (occupantA == "Clone" && occupantB == "Player");
            Debug.Log(correctOccupants);
            // If both spots are occupied by Player & Clone, check for key press
            if (correctOccupants)
            {

                // Use F key (KeyCode.F)
                if (Input.GetKeyDown(KeyCode.F))
                {
                    pair.platform.SetActive(true);
                }
            }
        }
    }

}
