using UnityEngine;

public class startPortal : MonoBehaviour
{
    [SerializeField] ParticleSystem _particleSystem;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter(Collider other)
    {
        _particleSystem.Play();
        Debug.Log("THIS SHOULD BE ON HELLOOO");
    }
}
