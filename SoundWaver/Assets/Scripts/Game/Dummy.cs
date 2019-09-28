using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Dummy : Yuuki.SingletonMonoBehaviour<Dummy>
{
    // Start is called before the first frame update
    void Start()
    {
        Game.GameMusic.Instance.LoadAndFunction(
            "a.mp3",
            () => 
            {
                SceneManager.LoadScene("SampleScene");
            }
            );
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void Execute()
    {

    }
}
