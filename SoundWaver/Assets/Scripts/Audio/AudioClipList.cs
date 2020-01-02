using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using API.Serialize;
namespace API
{
    namespace Serialize
    {
        /// <summary>
        /// 視覚化したいテーブルの基底クラス
        /// </summary>
        [System.Serializable]
        public class TableBase<TKey, TValue, Type> where Type : KeyAndValue<TKey, TValue>
        {
            //private param
            [SerializeField] List<Type> list;

            //accessory
            Dictionary<TKey, TValue> table { get; set; }

            //property
            public List<Type> List { get { return list; } }
            public Dictionary<TKey, TValue> Table
            {
                get
                {
                    //null check
                    if (table == null)
                    {
                        //convert List to Dictionary
                        table = ConvertListToDictionary(list);
                    }
                    return table;
                }
            }

            /// <summary>
            /// ListからDictionaryへ変換
            /// </summary>
            /// <param name="list">変換するリスト</param>
            /// <returns>変換したDictionary</returns>
            static private Dictionary<TKey, TValue> ConvertListToDictionary(List<Type> list)
            {
                Dictionary<TKey, TValue> dic = new Dictionary<TKey, TValue>();
                foreach (KeyAndValue<TKey, TValue> it in list) { dic.Add(it.key, it.value); }
                return dic;
            }

        }

        /// <summary>
        /// シリアル化して視覚化するテーブルの要素(キーと値)
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        [System.Serializable]
        public class KeyAndValue<TKey, TValue>
        {
            public TKey key;
            public TValue value;

            /// <summary>
            /// 空コンストラクタ
            /// </summary>
            //        public KeyAndValue() { }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="key">登録キー</param>
            /// <param name="value">登録値</param>
            public KeyAndValue(TKey key, TValue value)
            {
                this.key = key;
                this.value = value;
            }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="pair">登録するペア</param>
            public KeyAndValue(KeyAndValue<TKey, TValue> pair)
            {
                key = pair.key;
                value = pair.value;
            }
        }
    }

}

namespace Game.Audio
{
    [System.Serializable] public class AudioClipTable : TableBase<string, AudioClip, AudioPair> { };
    [System.Serializable]
    public class AudioPair : KeyAndValue<string, AudioClip>
    {

        public AudioPair(string key, AudioClip value) : base(key, value) { }
    }

    public class AudioClipList : MonoBehaviour
    {
        //  serialize param!
        [SerializeField, Tooltip("AudioClipを格納するテーブル")] AudioClipTable table;

        //  accessor
        public AudioClipTable Table { get { return table; } }
    }
}