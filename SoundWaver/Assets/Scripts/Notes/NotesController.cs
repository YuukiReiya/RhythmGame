using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotesController : Yuuki.SingletonMonoBehaviour<NotesController>
{
    public List<INote> notes;
    void Awake()
    {
        base.Awake();
        notes = new List<INote>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {
        foreach (var it in notes)
        {
            it.Move();
        }
    }
}
