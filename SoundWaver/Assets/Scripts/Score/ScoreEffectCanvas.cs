using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yuuki.MethodExpansions;

namespace Game.UI
{
    public class ScoreEffectCanvas : Yuuki.SingletonMonoBehaviour<ScoreEffectCanvas>
    {
        //enum
        public enum Judge
        {
            PERFECT,
            GREAT,
            GOOD,
            MISS,
        }
        //struct
        [System.Serializable]
        struct DecorateScoreEffectParameter
        {
            //TODO:現状はスケールのみの変化。
            //※α値を変えたりしたくなったらココの変数と関数を編集。
            public float scaleTime;
            public Vector2 startScale;
            public Vector2 endScale;
        }
        [System.Serializable]
        struct ScoreSprites
        {
            //public Sprite perfect;
            //public Sprite great;
            //public Sprite good;
            //public Sprite miss;
            public Texture2D perfect;
            public Texture2D great;
            public Texture2D good;
            public Texture2D miss;
        }

        //serialize param
        [SerializeField] ScoreSprites sprites;
        [SerializeField] DecorateScoreEffectParameter effectParam;
        //scaleTime:0.03
        //startScale:0.8,0.8
        //endScale:1,1


        [SerializeField] float displayTime = 0.3f;
        [SerializeField] UITexture image;
        //private param
        IEnumerator routine;
        //accessor
        protected override void Awake()
        {
            base.Awake();
#if UNITY_EDITOR
            if (image == null) { Debug.LogError("imageがアタッチされていません。"); }
#endif
            image.gameObject.SetActive(false);
        }
        public void StartScoreEffect(Judge judge)
        {
            //アクティブ化
            image.gameObject.SetActive(true);
            if (routine != null) { StopCoroutine(routine); }

            //評価分け
            switch (judge)
            {
                case Judge.PERFECT: image.mainTexture = sprites.perfect; break;
                case Judge.GREAT: image.mainTexture = sprites.great; break;
                case Judge.GOOD: image.mainTexture = sprites.good; break;
                case Judge.MISS: image.mainTexture = sprites.miss; break;
            }
            routine = DecorateScoreEffectRoutine();
            //コルーチンが終了したらroutineの初期化
            this.StartCoroutine(routine, () => { routine = null; });
        }

        IEnumerator DecorateScoreEffectRoutine()
        {
            float time = Time.time;
            //初期化
            //image.rectTransform.localScale = new Vector3(effectParam.startScale.x, effectParam.startScale.y, 1);
            //装飾時間
            while (Time.time < time + effectParam.scaleTime)
            {
                float rate = effectParam.scaleTime > 0.0f ? (Time.time - time) / effectParam.scaleTime : 1.0f;
                var scale = Vector2.Lerp(effectParam.startScale, effectParam.endScale, effectParam.scaleTime);
                //image.rectTransform.localScale = new Vector3(scale.x, scale.y, 1);
                image.transform.localScale = new Vector3(scale.x, scale.y, 1);
                yield return null;
            }
            //image.rectTransform.localScale = new Vector3(effectParam.endScale.x, effectParam.endScale.y, 1);
            image.transform.localScale = new Vector3(effectParam.endScale.x, effectParam.endScale.y, 1);
            //表示時間
            time = Time.time;
            while (Time.time < time + displayTime) { yield return null; }

            //非アクティブ化
            image.gameObject.SetActive(false);
        }

#if UNITY_EDITOR
        private void Reset()
        {
            image = this.GetComponentInChildren<UITexture>();
        }
#endif
    }
}