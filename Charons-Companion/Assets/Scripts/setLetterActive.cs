using UnityEngine;

public class setLetterActive : MonoBehaviour
{
    [SerializeField] GameObject letter;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter()
    {
        letter.SetActive(true);
    }
}
