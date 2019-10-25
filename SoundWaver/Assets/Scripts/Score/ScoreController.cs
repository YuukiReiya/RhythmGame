using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yuuki.MethodExpansions;

public class ScoreController : Yuuki.SingletonMonoBehaviour<ScoreController>
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
        public Sprite perfect;
        public Sprite great;
        public Sprite good;
        public Sprite miss;
    }
    //serialize param
    [SerializeField] ScoreSprites sprites;
    [SerializeField] DecorateScoreEffectParameter effectParam;
    [SerializeField] float displayTime;
    [SerializeField] Image image;
    //private param
    IEnumerator routine;
    //accessor
    public uint Score { get; private set; }
    protected override void Awake()
    {
        base.Awake();
        if (image.GetComponent<Image>() == null) { return; }
        image.gameObject.SetActive(false);
    }
    public void StartScoreEffect(Judge judge)
    {
        if (image.GetComponent<Image>() == null) { return; }
        //アクティブ化
        image.gameObject.SetActive(true);
        if (routine != null) { StopCoroutine(routine); }

        //評価分け
        switch (judge)
        {
            case Judge.PERFECT: Perfect(); break;
            case Judge.GREAT: Great(); break;
            case Judge.GOOD: Good(); break;
            case Judge.MISS: Miss(); break;
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
            image.rectTransform.localScale = new Vector3(scale.x, scale.y, 1);
            yield return null;
        }
        image.rectTransform.localScale = new Vector3(effectParam.endScale.x, effectParam.endScale.y, 1);
        //表示時間
        time = Time.time;
        while (Time.time < time + displayTime) { yield return null; }

        //非アクティブ化
        image.gameObject.SetActive(false);
    }

    #region 判定ごとの処理
    //TODO:コンボ処理とコンボ時のUI表示もココに入れてるけど
    //本来ならコールバック用のイベントキューを用意したほうがいい…
    private void Perfect()
    {
        image.sprite = sprites.perfect;
        Game.GameController.Instance.Comb++;
        CombEffectCanvas.Instance.Execute(Game.GameController.Instance.Comb);
    }
    private void Great()
    {
        image.sprite = sprites.great;
        Game.GameController.Instance.Comb++;
        CombEffectCanvas.Instance.Execute(Game.GameController.Instance.Comb);
    }
    private void Good()
    {
        image.sprite = sprites.good;
        Game.GameController.Instance.Comb = 0;
    }
    private void Miss()
    {
        image.sprite = sprites.miss;
        Game.GameController.Instance.Comb = 0;
    }
    #endregion

#if UNITY_EDITOR
    private void Reset()
    {
        image = GetComponentInChildren<Image>();
    }
#endif
}
