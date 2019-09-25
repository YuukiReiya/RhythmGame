using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test1 : MonoBehaviour
{
    public UILabel label;
    // Start is called before the first frame update
    void Start()
    {
        Execute();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Execute()
    {
        label.text = Application.persistentDataPath;
    }
}
