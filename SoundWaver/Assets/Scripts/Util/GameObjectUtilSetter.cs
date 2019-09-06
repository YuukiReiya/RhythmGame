using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectUtilSetter : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] GameObject target;
    [SerializeField] GameObject self;


    void Reset()
    {
        self = this.gameObject;
    }

    // Start is called before the first frame update
    void Start()
    {
        Destroy(this);
    }

    [ContextMenu("Set World Position")]
    void SetWorldPosotion()
    {
        self.transform.position = target.transform.position;
    }

    [ContextMenu("Set World Rotation")]
    void SetWorldRotation()
    {
        self.transform.rotation = target.transform.rotation;
    }

    [ContextMenu("Set World Scale")]
    void SetWorldScale()
    {
        self.transform.localScale = target.transform.lossyScale;
    }

    [ContextMenu("Set Local Position")]
    void SetLocalPosotion()
    {
        self.transform.position = target.transform.localPosition;
    }

    [ContextMenu("Set Local Rotation")]
    void SetLocalRotation()
    {
        self.transform.rotation = target.transform.localRotation;
    }

    [ContextMenu("Set Local Scale")]
    void SetLocalScale()
    {
        self.transform.localScale = target.transform.localScale;
    }
#endif

}
