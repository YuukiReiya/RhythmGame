using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace Game.UI
{
    public class RadioButtonGroop : MonoBehaviour
    {
        [System.Serializable]
        struct Item
        {
            public RadioButton radioButton;
            public bool initValue;
        }
        //serialize param
        [SerializeField] private Item[] list;
        //private param

        //public param
        public RadioButton isActiveButton { 
            get 
            {
                //ラジオボタンなのでアクティブなものは一つしかないはず！
                return list.First(it => it.radioButton.IsActive).radioButton; 
            } 
        }
        public RadioButton[] Buttons { 
            get 
            {
                return list.Select(it => it.radioButton).ToArray();
            }
        }
        /// <summary>
        /// 初期化
        /// </summary>
        public void Setup()
        {
            //コールバック
            foreach(var it in list)
            {
                //"自分以外のGroupアイテムを非アクティブ化"
                //する関数をラジオボタンがアクティブ化したら実行するようコールバックに設定
                {
                    var others = list.Where(p => it.radioButton != p.radioButton);
                    it.radioButton.onActiveFunc = () => 
                    {
                        foreach(var other in others)
                        {
                            other.radioButton.CallDisable();
                        }
                    };
                }

                //選択されている状態でもう一度押されたら、ボタンを入れなおす。
                {
                    var others = list.Where(p => it.radioButton != p.radioButton);
                    it.radioButton.onDisableFunc = () =>
                    {
                        bool reset = others.Where(p => p.radioButton.IsActive).Count() <= 0;
                        if (reset) { it.radioButton.CallActive(); }
                    };
                }
            }
            //アクティブ状況初期化
            foreach (var it in list)
            {
                switch (it.initValue)
                {
                    case true:it.radioButton.CallActive();break;
                    case false:it.radioButton.CallDisable();break;
                }
            }
        }

    }
}