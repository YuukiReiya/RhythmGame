using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class PauseCanvas : MonoBehaviour
{
    //serialize param
    [SerializeField] Image fade;
    [SerializeField] GameObject menu;
    //private param
    AudioSource source;
    bool isPause;

    public void Setup(AudioSource source)
    {
        this.source = source;
        isPause = false;
        menu.SetActive(false);
        fade.gameObject.SetActive(false);
    }

    /// <summary>
    /// TODO:UnPause関数でn秒間待機してカウント表示からの再開？
    /// </summary>
    public void Pause()
    {
        isPause = !isPause;
        menu.SetActive(isPause);
        source.pitch = isPause ? 0 : 1;
    }

    public void TransitionSelect()
    {
        StartCoroutine(FadeIn(
            Define.c_FadeTime, 
            () => { SceneManager.LoadScene("Select"); })
            );
    }
    private IEnumerator FadeIn(float time, System.Action action = null)
    {
        fade.gameObject.SetActive(true);
        float startTime = Time.time;
        while (Time.time <= startTime + time)
        {
            float rate = time > 0.0f ? (Time.time - startTime) / time : 1.0f;
            var alpha = Mathf.Lerp(0, 1, rate);
            var cr = fade.color;
            cr.a = alpha;
            fade.color = cr;
            yield return null;
        }
        {
            var cr = fade.color;
            cr.a = 1.0f;
            fade.color = cr;
        }
        action?.Invoke();
    }
}
