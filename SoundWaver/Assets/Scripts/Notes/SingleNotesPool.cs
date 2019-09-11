using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class SingleNotesPool : SingletonObjectPool<SingleNotesPool>
{
    protected override void Awake()
    {
        Setup();
    }
    // Start is called before the first frame update
    void Start ()
    {

    }

    // Update is called once per frame
    void Update ()
    {

    }

    [ContextMenu ("Create Pool Object Instances")]
    protected override void CreatePoolObjects()
    {
        base.CreatePoolObjects();
    }
}