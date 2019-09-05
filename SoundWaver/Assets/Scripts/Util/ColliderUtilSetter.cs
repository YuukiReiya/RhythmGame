using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderUtilSetter : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] GameObject target;
    Collider col;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(this);
    }

    [ContextMenu("Set World Position")]
    void SetWorldPosotion()
    {
        if (!TryGetComponent<Collider>(out col)) { return; }
        col.transform.position = target.transform.position;
    }

    [ContextMenu("Set World Rotation")]
    void SetWorldRotation()
    {
        if (!TryGetComponent<Collider>(out col)) { return; }
        col.transform.rotation = target.transform.rotation;
    }

    [ContextMenu("Set World Scale")]
    void SetWorldScale()
    {
        if (!TryGetComponent<Collider>(out col)) { return; }
        col.transform.localScale = target.transform.lossyScale;
    }

    [ContextMenu("Set Local Position")]
    void SetLocalPosotion()
    {
        if (!TryGetComponent<Collider>(out col)) { return; }
        col.transform.position = target.transform.localPosition;
    }

    [ContextMenu("Set Local Rotation")]
    void SetLocalRotation()
    {
        if (!TryGetComponent<Collider>(out col)) { return; }
        col.transform.rotation = target.transform.localRotation;
    }

    [ContextMenu("Set Local Scale")]
    void SetLocalScale()
    {
        if (!TryGetComponent<Collider>(out col)) { return; }
        col.transform.localScale = target.transform.localScale;
    }
#endif

}
