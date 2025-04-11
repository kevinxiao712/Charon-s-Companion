using Unity.VisualScripting;
using UnityEngine;

public class SpiritWorldFog : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        RenderSettings.fog = true;
    }
        
    
}
