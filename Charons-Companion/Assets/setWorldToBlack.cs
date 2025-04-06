using System.ComponentModel;
using System.Threading;
using UnityEngine;

public class setWorldToBlack : MonoBehaviour
{
    RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
    Color beginColour = RenderSettings.ambientLight;
    bool enteredSpiritWorld = false;
    float timer = 0;
    private void OnTriggerEnter(Collider other)
    {
        
        
        enteredSpiritWorld = true;
    }

    private void Update()
    {
        
        if (enteredSpiritWorld)
        {
            timer += Time.deltaTime;
            RenderSettings.ambientLight = Color.Lerp(beginColour, Color.black, timer);
        }
    }
}
