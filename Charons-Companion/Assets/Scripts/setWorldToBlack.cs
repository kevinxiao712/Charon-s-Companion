using System.ComponentModel;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;

public class setWorldToBlack : MonoBehaviour
{
    [SerializeField] ParticleSystem portal;
    Color beginColour;
    bool enteredSpiritWorld = false;
    float timer = 0;

    private void Start()
    {
        beginColour = RenderSettings.ambientSkyColor;
        
    }
    private void OnTriggerEnter(Collider other)
    {
        //RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;

        enteredSpiritWorld = true;
        RenderSettings.fog = true;
        portal.Play();
    }

    private void Update()
    {
        
        if (enteredSpiritWorld)
        {
            timer += (Time.deltaTime / 3);
            RenderSettings.fogStartDistance = Mathf.Lerp(100, 8, timer);
            RenderSettings.fogEndDistance = Mathf.Lerp(100, 50, timer);

        }
    }
}
