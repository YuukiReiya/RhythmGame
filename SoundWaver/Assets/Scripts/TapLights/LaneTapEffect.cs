using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yuuki.MethodExpansions;

public class LaneTapEffect : MonoBehaviour
{
    Material mat;
    MeshRenderer renderer;
    Color color { get { return renderer.material.color; } set { renderer.material.color = value; } }
    float maxAlpha { get; set; }
    [SerializeField, TooltipAttribute("Alpha値が０～任意の値に変化するまでの時間")]
    float startTime = 0.2f;
    [SerializeField, TooltipAttribute("Alpha値が任意の値～０に変化するまでの時間")]
    float endTime = 0.0f;

    public IEnumerator Routine { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        if (!TryGetComponent<MeshRenderer>(out renderer)) { return; }

        //  マテリアルをコピーし、そちらを弄る
        //※大本は弄らない
        mat = Instantiate<Material>(renderer.material);

        //  コピーの参照をセット
        renderer.material = mat;

        var cr = this.color;
        maxAlpha = cr.a;
        cr.a = 0;
        this.color = cr;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {

            Execute();
        }
    }
    public void Execute()
    {
        if (!renderer)
        {
            if (!TryGetComponent<MeshRenderer>(out renderer))
                return;
        }

        //  ０ → maxAlpha
        Routine = TapEffectRoutine(startTime, 0, maxAlpha);

        //  コルーチン呼び出し
        this.StartCoroutine(
            Routine,
            //  コルーチン終了時に呼び出される関数
            () =>
            {
                //  maxAlpha → 0
                Routine = TapEffectRoutine(endTime, maxAlpha, 0);
                //  コルーチン呼び出し
                this.StartCoroutine(Routine,
                //  終了時に保存用変数を初期化
                () => { Routine = null; });
            }
            );
    }

    private IEnumerator TapEffectRoutine(float duration, float from, float to)
    {
        float time = Time.time;
        while (Time.time < duration + time)
        {
            float rate = duration > 0.0f ? (Time.time - time) / duration : 1.0f;
            var cr = this.color;
            cr.a = Mathf.Lerp(from, to, rate);
            this.color = cr;
            yield return null;
        }
        var color = this.color;
        color.a = to;
        this.color = color;
    }
}
