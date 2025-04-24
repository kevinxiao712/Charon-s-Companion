using UnityEngine;

public class Rope : MonoBehaviour
{
    public enum Axis { X, Y, Z }
    [Tooltip("Local axis the player can slide along.")]
    public Axis slideAxis = Axis.Y;

    public Vector3 SlideDir =>
        transform.TransformDirection( slideAxis == Axis.X ? Vector3.right : slideAxis == Axis.Y ? Vector3.up :Vector3.forward).normalized;


    public int AxisIndex => slideAxis == Axis.X ? 0 : slideAxis == Axis.Y ? 1 : 2;
}
