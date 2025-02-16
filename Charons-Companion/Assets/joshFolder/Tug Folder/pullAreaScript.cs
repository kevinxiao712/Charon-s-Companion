using UnityEngine;

public class pullAreaScript : MonoBehaviour
{
    [SerializeField] tugObject tugObject;
    /// <summary>
    /// Calls when something enters the area. CURRENTLY activates when anything enters this area. we may want to
    /// change this to only trigger upon player entry
    /// </summary>
    /// <param name="other"></param>
    protected void OnTriggerEnter(Collider other)
    {
        if (other != null)
        {
            tugObject.setPullable(true);
        }
    }

    protected void OnTriggerExit(Collider other)
    {
        if (other != null)
        {
            tugObject.setPullable(false);
        }
    }
}
