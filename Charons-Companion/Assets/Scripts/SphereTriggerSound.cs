using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class TriggerToggleObjects : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip audioClip;

    [Header("Objects to Toggle")]
    public GameObject firstObject;  // initially enabled
    public GameObject secondObject; // initially disabled


    [SerializeField] private Light pointLight;        
    [SerializeField] private Color newLightColor = Color.red;
    private void Start()
    {

        SphereCollider sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.isTrigger = true;

        if (firstObject != null)
            firstObject.SetActive(true);
        if (secondObject != null)
            secondObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (audioSource != null && audioClip != null)
            {
                audioSource.PlayOneShot(audioClip);
            }
            else
            {
                Debug.LogWarning("AudioSource or AudioClip not set on " + gameObject.name);
            }

            if (firstObject != null)
                firstObject.SetActive(false);
            if (secondObject != null)
                secondObject.SetActive(true);

            if (pointLight != null)
                pointLight.color = newLightColor;
        }
    }
}
