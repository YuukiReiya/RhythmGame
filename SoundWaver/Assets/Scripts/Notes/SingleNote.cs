using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleNote : MonoBehaviour, INote
{
    public bool isReset  { get; }

    int laneNumber;
    public int LaneNumber
    {
        get { return 1; }
    }

    float dt;
    public float DownTime { get { return this.dt; } }

    [SerializeField] float ts = 1.0f;

    public void Setup(int laneNumber, float downTime)
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        Register();
    }

    protected void OnEnable()
    {
        Register();
    }

    protected void OnDisable()
    {
        Unregister();
    }

    // Update is called once per frame
    [SerializeField] float sp = 1.0f;
    void Update()
    {
    }
    public void Move()
    {
        var x = this.transform.position.x;
        float timing = ts - NotesController.Instance.elapsedTime;

        //TODO:汚い
        //Vector3 ndir = (this.transform.position - NotesController.Instance.JustTimingPosition).normalized;
        Vector3 ndir = (-this.transform.up.normalized);

        var pos = NotesController.Instance.JustTimingPosition - ndir * timing * sp;
        pos.x = x;
        this.transform.position = pos;
    }

    public void Register()
    {
        NotesController.Instance.notes.Add(this);
    }

    public void Unregister()
    {
        NotesController.Instance.notes.Remove(this);
    }
}
