/*using UnityEngine;

public class JumpIndicator3D : MonoBehaviour
{
    [Header("3D Cube Indicator")]
    public GameObject jumpCube;          // Assign a simple cube or mesh
    public float verticalOffset = 2f;    // How high above the player to place the cube
    public bool followPlayer = true;     // Reposition the cube each frame
    public float defaultScale = 1f;      // Default scale when you first start charging

    private PlayerMovement pm;

    void Awake()
    {
        // Get the PlayerMovement component that¡¯s also on this GameObject
        pm = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        // If references are missing, do nothing
        if (pm == null || jumpCube == null) return;

        // Show & scale the cube while charging
        if (pm.isCharging)
        {
            jumpCube.SetActive(true);

            // Charge ratio: how long have we charged vs. max possible
            float chargeRatio = pm.holdTime / pm.maxHoldTime;

            // Lerp from defaultScale up to 'indicatorMaxScale'
            float currentScale = Mathf.Lerp(defaultScale, pm.indicatorMaxScale, chargeRatio);

            // Apply that scale
            jumpCube.transform.localScale = Vector3.one * currentScale;

        }
        else
        {
            // Hide the cube if not charging
            jumpCube.SetActive(false);
        }
    }
}
*/