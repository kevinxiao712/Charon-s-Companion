using UnityEngine;
using UnityEngine.Rendering.Universal; // For DecalProjector in URP

public class Decal : MonoBehaviour
{
    public DecalProjector decal;    // Assign the Decal Projector here
    public Transform player;        // The player transform (to measure height)
    public float minHeight = 0f;    // Height at which decal is smallest
    public float maxHeight = 5f;    // Height at which decal is largest
    public float minSize = 1f;      // Decal size (width/height) at minHeight
    public float maxSize = 3f;      // Decal size (width/height) at maxHeight

    public LayerMask groundLayers;  // Layers considered "ground" if you want to do a raycast

    void Update()
    {
        // 1) Calculate the player's height above ground
        float groundY = GetGroundHeight();
        float playerHeight = player.position.y - groundY;

        // 2) Normalize height (0 to 1)
        float t = Mathf.InverseLerp(minHeight, maxHeight, playerHeight);

        // 3) Lerp the decal size
        float currentSize = Mathf.Lerp(maxSize, minSize, t);


        var newSize = decal.size;
        newSize.x = currentSize;
        newSize.y = currentSize;
        decal.size = newSize;
    }

    private float GetGroundHeight()
    {
        Ray ray = new Ray(player.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayers))
        {
            return hit.point.y;
        }

        return 0f;
    }
}
