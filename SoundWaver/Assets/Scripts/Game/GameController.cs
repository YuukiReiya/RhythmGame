using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class GameController : Yuuki.SingletonMonoBehaviour<GameController>
    {
        public AudioSource source;
        protected override void Awake()
        {
            base.Awake();
            //Dummy通った場合
            if (GameMusic.Instance.Clip)
            {
                source.clip = GameMusic.Instance.Clip;
                source.Play();
            }
            else
            {
                //GameMusic.Instance.LoadAndPlayAudioClip("a.mp3");
                string c_Path = "/Sound/short_song_shiho_shining_star.mp3";
                GameMusic.Instance.LoadAndPlayAudioClip(Application.streamingAssetsPath + c_Path);
            }
            //GameMusic.Instance.LoadAndPlayAudioClip("a.mp3");
        }
        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}