using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Yuuki.MethodExpansions;
public class CombEffectCanvas : Yuuki.SingletonMonoBehaviour<CombEffectCanvas>
{
    [System.Serializable]
    struct DecorateParam
    {
        public float time;
        public Vector2 startScale;
        public Vector2 endScale;
    }
    //serialize param
    [SerializeField] private TextMeshProUGUI combCount;
    [SerializeField] private DecorateParam decorateParam;
    [SerializeField] private float displayTime;
    //private param
    private IEnumerator routine;
    //  accessor
    private GameObject CombUI{ get { return combCount.gameObject; } }
    protected override void Awake()
    {
        base.Awake();
        CombUI.SetActive(false);
    }
    public void Execute(uint comb)
    {
        if (!combCount.GetComponent<TextMeshProUGUI>()) { return; }
        CombUI.SetActive(true);
        combCount.text = comb.ToString();
        if (routine != null) { StopCoroutine(routine); }
        routine = DecorateCombEffectRoutine(decorateParam);
        this.StartCoroutine(routine, () => { routine = null; });
    }

    private IEnumerator DecorateCombEffectRoutine(DecorateParam arg)
    {
        //最初の大きさ
        CombUI.transform.localScale = arg.startScale;
        float time = Time.time;
        while (Time.time < time + arg.time)
        {
            float rate = arg.time > 0.0f ? (Time.time - time) / arg.time : 1.0f;
            var scale = Vector2.Lerp(arg.startScale, arg.endScale, rate);
            CombUI.transform.localScale = new Vector3(scale.x, scale.y, CombUI.transform.localScale.z);
            yield return null;
        }
        CombUI.transform.localScale = arg.endScale;
        time = Time.time;
        while (Time.time < time + displayTime) { yield return null; }

        //UI非アクティブ化
        CombUI.SetActive(false);
    }
}
