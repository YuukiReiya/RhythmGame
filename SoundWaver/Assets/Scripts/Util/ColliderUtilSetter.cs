using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderUtilSetter : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] GameObject target;
    [Header("Single")]
    Collider col;
    [Header("Multiple")]
    [SerializeField] GameObject fromObj;
    [SerializeField] Collider[] attatchColliders;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(this);
    }
    [ContextMenu("Set Colliders data dest Inspector")]
    void SetCollidersDataInspector()
    {
        var param = fromObj.GetComponents<Collider>();
        if (param.Length <= 0) { return; }
        attatchColliders = param;
        foreach(var it in param)
        {
            //attatchColliders.
        }
    }
    [ContextMenu("Set Colliders data dest Object")]
    void SetCollidersDataObject()
    {
        if (attatchColliders.Length <= 0) { return; }
        Queue<Collider> queue=new Queue<Collider>();
        foreach(var it in attatchColliders)
        {
            if (it == null) { continue; }
            queue.Enqueue(it);
        }
        foreach(var it in queue)
        {
            if (!UnityEditorInternal.ComponentUtility.CopyComponent(it)) {
                Debug.LogError("copy failed.");
                continue; 
            }
            UnityEditorInternal.ComponentUtility.PasteComponentAsNew(target);
        }
    }
    [ContextMenu("All atttach collider clear")]
    void AllCollidersClear()
    {
        var val = target.GetComponents<Collider>();
        if (val.Length <= 0) { return; }
        foreach(var it in val)
        {
            DestroyImmediate(it);
        }
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
