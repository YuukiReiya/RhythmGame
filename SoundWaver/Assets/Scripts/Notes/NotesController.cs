using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotesController : Yuuki.SingletonMonoBehaviour<NotesController>
{
    [System.Serializable]
    struct TimingLine
    {
        public float y, z;
    }

    [SerializeField]
    TimingLine timingLine;
    public List<INote> notes;

    public float elapsedTime { get; private set; }

    void Awake ()
    {
        base.Awake ();
        notes = new List<INote> ();
    }

    // Start is called before the first frame update
    void Start () { }

    // Update is called once per frame
    void Update ()
    {
        Move ();
    }

    void Move ()
    {

        foreach (var it in notes)
        {
            it.Move ();
        }
    }

    void OnDrawGizmos ()
    {
        Gizmos.color = Color.red;

        float left = -10, right = 10;
        Gizmos.DrawLine (new Vector3 (left, timingLine.y, timingLine.z), new Vector3 (right, timingLine.y, timingLine.z));
    }
}