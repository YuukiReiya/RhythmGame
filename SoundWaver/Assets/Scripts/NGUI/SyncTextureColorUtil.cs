using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yuuki.MethodExpansions;

public class SyncTextureColorUtil : MonoBehaviour
{
    /// <summary>
    /// 同期するボタン
    /// </summary>
    [SerializeField] private UIButton button;

    /// <summary>
    /// 同期させるテクスチャ/スプライト
    /// </summary>
    [SerializeField] private UIBasicSprite syncSprite;

    [System.Serializable]
    struct Colors
    {
        public Color
            Normal,
            Hover,
            Pressed,
            Disabled;
    }
    [SerializeField] private Colors colors;
    /// <summary>
    /// コルーチン実装するための変数
    /// </summary>
    private IEnumerator routine;

    /// <summary>
    /// エディタ上のユーティリティ
    /// </summary>
#if UNITY_EDITOR
    private void Reset()
    {
        syncSprite = GetComponent<UIBasicSprite>();
    }

    [ContextMenu("Set Sync Button Color")]
    private void SyncColor()
    {
        if (button == null)
        {
            Debug.LogError("ボタンがアタッチされていません。処理をせずに関数を抜けます。");
            return;
        }
        colors.Normal = button.disabledColor;
        colors.Hover = button.hover;
        colors.Pressed = button.pressed;
        colors.Disabled = button.disabledColor;
    }
#endif

    private void Awake()
    {
        routine = MainRoutine();
        this.StartCoroutine(routine, () => { routine = null; });
    }

    /// <summary>
    /// コルーチンでやれば少なくともUpdateよりは軽い？
    /// </summary>
    /// <returns></returns>
    IEnumerator MainRoutine()
    {
        while(true)
        {
            switch (button.state)
            {
                case UIButtonColor.State.Normal:
                    syncSprite.color = colors.Normal;
                    break;
                case UIButtonColor.State.Hover:
                    syncSprite.color = colors.Hover;
                    break;
                case UIButtonColor.State.Pressed:
                    syncSprite.color = colors.Pressed;
                    break;
                case UIButtonColor.State.Disabled:
                    syncSprite.color = colors.Disabled;
                    break;
            }
            yield return null;
        }
    }

    private void OnEnable()
    {
        if (routine != null) { StopCoroutine(routine); }
        routine = MainRoutine();
        this.StartCoroutine(routine, () => { routine = null; });
    }

    private void OnDisable()
    {
        if (routine != null) { StopCoroutine(routine); }
        routine = null;
    }

    private void OnDestroy()
    {
        if (routine != null) { StopCoroutine(routine); }
        routine = null;
    }
}