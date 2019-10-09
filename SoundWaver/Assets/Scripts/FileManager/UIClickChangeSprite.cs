using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 同じスプライトアトラスに登録されたテクスチャの差し替え
/// </summary>
public class UIClickChangeSprite : MonoBehaviour
{
    //serialize param
    [SerializeField] UISprite sprite;
    [SerializeField] UIButton button;
    [SerializeField] string secondSprite;
    [SerializeField] Color changeColor;

    //private param
    string firstSprite;
#if UNITY_EDITOR
    private void Reset()
    {
        sprite = GetComponent<UISprite>();
        button = GetComponent<UIButton>();
        changeColor = sprite.color;
    }

    private void OnValidate()
    {
        if (secondSprite != string.Empty && sprite.GetSprite(secondSprite) == null)
        {
            Debug.LogWarning(secondSprite + " という名前のスプライトは存在しません");
        }
        else
        {
            Debug.Log(secondSprite + " が設定されました");
        }
    }
#endif

    // Start is called before the first frame update
    void Start()
    {
        firstSprite = sprite.spriteName;
        //EventDelegate ev = new EventDelegate(() => { button.normalSprite = button.normalSprite == firstSprite ? secondSprite : firstSprite; });
        EventDelegate.Callback callback = null;
        callback += () => { var cr = button.defaultColor; button.defaultColor = changeColor; changeColor = cr; };
        if (secondSprite != string.Empty)
        {
            callback += () => { button.normalSprite = button.normalSprite == firstSprite ? secondSprite : firstSprite; };
        }
        EventDelegate ev = new EventDelegate(callback);
        button.onClick.Add(ev);
    }
}
