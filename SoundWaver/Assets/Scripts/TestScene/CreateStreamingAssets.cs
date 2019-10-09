using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CreateStreamingAssets : MonoBehaviour
{
    const string c_Path= "StreamingAssets";

    // Start is called before the first frame update
    void Start()
    {
        if(Directory.Exists(Application.persistentDataPath+"/"+c_Path))
        {

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
