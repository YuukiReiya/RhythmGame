using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NotesController : Yuuki.SingletonMonoBehaviour<NotesController>
{
    [Header("Notes Control Parameter")]
    [SerializeField] float noteSpeed;
    public float NotesSpeed { get { return noteSpeed; } }
    [System.Serializable]
    struct TimingLine
    {
        public float y, z;
    }
    [SerializeField]
    TimingLine timingLine;
    public List<INote> notes;
    [SerializeField] AudioSource audioSource;

    public Vector3 JustTimingPosition { get { return new Vector3(0, timingLine.y, timingLine.z); } } 

    public float elapsedTime { get; private set; }

    protected override void Awake ()
    {
        base.Awake ();
        notes = new List<INote> ();
        Debug.Log("NM");
    }

    // Start is called before the first frame update
    void Start () { }

    // Update is called once per frame
    void Update ()
    {
        Move();
        elapsedTime = audioSource.time;
    }

    /// <summary>
    /// 管理リストの更新
    /// </summary>
    void Renewal()
    {
        notes.RemoveAll(it => it.isReset);
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