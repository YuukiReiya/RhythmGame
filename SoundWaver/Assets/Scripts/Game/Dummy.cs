using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Dummy : Yuuki.SingletonMonoBehaviour<Dummy>
{
    public string musicFilePath;
    // Start is called before the first frame update
    void Start()
    {
        //シーン遷移後のGameMusicインスタンス差し替わるが、
        //ロードしたAudioClipの情報は引き継ぐ
        Game.GameMusic.Instance.LoadAndFunction(
            musicFilePath,
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
