using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PressurePlate : MonoBehaviour
{
    [Header("Visual & Motion")]
    [Tooltip("Child mesh that should move down/up. " + "Leave empty to move the whole GameObject (not recommended).")]
    [SerializeField] private Transform plateVisual;
    public float pressDepth = 0.1f;

    [Header("Activation")]
    public LayerMask activatorLayer;
    public GameObject linkedObject;

    Vector3 visualStartPos;
    readonly HashSet<Collider> activators = new();

    void Awake()
    {
        visualStartPos = plateVisual != null ? plateVisual.localPosition: Vector3.zero;

        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }


    void OnTriggerEnter(Collider other)
    {
        if (IsActivator(other)) activators.Add(other);
    }

    void OnTriggerExit(Collider other)
    {
        if (IsActivator(other)) activators.Remove(other);
    }


    void FixedUpdate()
    {
        bool shouldBePressed = activators.Count > 0;
        ApplyVisual(shouldBePressed);
        NotifyLinkedObject(shouldBePressed);
    }


    bool IsActivator(Collider c)
        => ((1 << c.gameObject.layer) & activatorLayer) != 0;

    void ApplyVisual(bool pressed)
    {
        if (plateVisual == null) return;

        plateVisual.localPosition = visualStartPos + (pressed ? Vector3.down * pressDepth : Vector3.zero);
    }

    void NotifyLinkedObject(bool pressed)
    {
        if (linkedObject == null) return;

        linkedObject.SendMessage(pressed ? "Activate" : "Deactivate", SendMessageOptions.DontRequireReceiver);
    }
}
