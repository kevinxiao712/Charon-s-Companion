using UnityEngine;

public class Door : MonoBehaviour
{
    private Vector3 openPosition;
    private Vector3 closedPosition;
    private bool isOpen = false;

    void Start()
    {
        closedPosition = transform.position;
        openPosition = transform.position + new Vector3(0, 3, 0); // Move up when opening
    }

    public void Activate()
    {
        if (!isOpen)
        {
            isOpen = true;
            StopAllCoroutines();
            StartCoroutine(MoveDoor(openPosition));
        }
    }

    public void Deactivate()
    {
        if (isOpen)
        {
            isOpen = false;
            StopAllCoroutines();
            StartCoroutine(MoveDoor(closedPosition));
        }
    }

    private System.Collections.IEnumerator MoveDoor(Vector3 target)
    {
        float time = 0;
        Vector3 startPos = transform.position;
        while (time < 1)
        {
            transform.position = Vector3.Lerp(startPos, target, time);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = target;
    }
}
