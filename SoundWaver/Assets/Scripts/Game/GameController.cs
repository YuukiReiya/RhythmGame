using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class GameController : Yuuki.SingletonMonoBehaviour<GameController>
    {
        //serialize param
        //private param
        //public param
        public AudioSource source;
        //accessor
        public float ElapsedTime { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            //楽曲データロード済み
            if (GameMusic.Instance.Clip)
            {
                source.clip = GameMusic.Instance.Clip;
                Music.CurrentSetup();
                source.Play();
            }
            else
            {
                //エラー処理
                //(確認用のダイアログを出す.etc)
            }
        }
        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            //経過時間の更新
            ElapsedTime = source.time;
        }
    }
}