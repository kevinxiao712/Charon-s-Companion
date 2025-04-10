using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [Header("Settings")]
    public LayerMask activatorLayer; // Define layers that can activate the plate
    public GameObject linkedObject;  // The object this plate controls (door, platform, etc.)
    public float pressDepth = 0.1f;  // How much the plate moves down when stepped on
    public bool willMove = false;
    public GameObject otherPosition;
    private Vector3 originalPosition;
    private bool isPressed = false;

    void Start()
    {
        originalPosition = transform.position;

        if (willMove)
        {
            originalPosition = otherPosition.transform.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & activatorLayer) != 0)
        {
            if (!isPressed)
            {
                isPressed = true;
                PressPlate();
                if (linkedObject != null)
                {
                    linkedObject.SendMessage("Activate", SendMessageOptions.DontRequireReceiver);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & activatorLayer) != 0)
        {
            isPressed = false;
            ReleasePlate();
            if (linkedObject != null)
            {
                linkedObject.SendMessage("Deactivate", SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    void PressPlate()
    {
        transform.position = originalPosition - new Vector3(0, pressDepth, 0);
    }

    void ReleasePlate()
    {
        transform.position = originalPosition;
    }
}
