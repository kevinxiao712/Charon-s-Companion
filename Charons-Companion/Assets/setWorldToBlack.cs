using System.ComponentModel;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;

public class setWorldToBlack : MonoBehaviour
{

    Color beginColour;
    bool enteredSpiritWorld = false;
    float timer = 0;

    private void Start()
    {
        beginColour = RenderSettings.ambientSkyColor;
        
    }
    private void OnTriggerEnter(Collider other)
    {
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;

        enteredSpiritWorld = true;
    }

    private void Update()
    {
        
        if (enteredSpiritWorld)
        {
            timer += (Time.deltaTime / 2);
            RenderSettings.ambientLight = Color.Lerp(beginColour, Color.black, timer);
        }
    }
}
