using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Yuuki;
using Common;
using System;
using Yuuki.MethodExpansions;

namespace Game.Audio
{
    public class AudioManager : SingletonMonoBehaviour<AudioManager>
    {
        //serialize param
        [SerializeField] private AudioSource sourceBGM;
        [SerializeField] private AudioSource sourceSE;

        //  private param
        //  public param
        [System.NonSerialized] public uint SEVolume;
        [System.NonSerialized] public uint BGMVolume;
        [System.NonSerialized] public AudioClipTable clips;
        //accessor
        public AudioSource SourceBGM { get { return sourceBGM; } }
        public AudioSource SourceSE { get { return sourceSE; } }

        protected override void Awake()
        {
            base.Awake();
        }
         public float GetConvertVolume(uint vol)
        {
#if UNITY_EDITOR
            if(vol<Define.c_MinVolume||Define.c_MaxVolume<vol)
            {
                Debug.LogError("AudioManager.cs line 33 GetConvertVolume value is invlid.");
                return 0.0f;
            }
#endif
            var A = 1.0f / Define.c_MaxVolume;
            return (1.0f / Define.c_MaxVolume) * vol;
        }

        /// <summary>
        /// BGMチャネルで再生
        /// </summary>
        /// <param name="key"></param>
        /// <param name="isLoop"></param>
        public void PlayBGM(string key, bool isLoop = true)
        {
            if (!HasTableClipsKey(key))
            {
                Debug.LogError("AudioManager.cs line50 PlayBGM not found sound key.\n" + key);
                ErrorManager.Save();
                return;
            }
            sourceBGM.clip = clips.Table[key];
            sourceBGM.loop = isLoop;
            sourceBGM.Play();
        }

        /// <summary>
        /// SEチャネルで再生
        /// </summary>
        public void PlaySE(string key, bool isLoop = false)
        {
            if (!HasTableClipsKey(key))
            {
                Debug.LogError("AudioManager.cs line66 PlaySE not found sound key.\n" + key);
                ErrorManager.Save();
                return;
            }
            sourceSE.clip = clips.Table[key];
            sourceSE.loop = isLoop;
            sourceSE.Play();
        }

        private bool HasTableClipsKey(string key)
        {
            return clips.Table.Keys.Contains(key);
        }

        public void Fade(AudioSource source, float time, float from, float to,  CoroutineExpansion.CoroutineFinishedFunc func = null)
        {
            this.StartCoroutine(FadeRoutine(source, time, from, to), func);
        }

        private IEnumerator FadeRoutine(AudioSource source, float time, float from, float to)
        {
            var st = Time.time;
            while (Time.time <= st + time)
            {
                float rate = time > 0.0f ? (Time.time - st) / time : 1.0f;
                var vol = Mathf.Lerp(from, to, rate);
                source.volume = vol;
                yield return null;
            }
            source.volume = to;
        }
    }
}