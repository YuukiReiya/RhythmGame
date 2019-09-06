using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleNote : MonoBehaviour, INote
{
    // Start is called before the first frame update
    void Start()
    {
        Register();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Move()
    {
        Debug.Log("hoge");
    }

    public void Register()
    {
        NotesController.Instance.notes.Add(this);
    }

    public void Unregister()
    {

    }
}
