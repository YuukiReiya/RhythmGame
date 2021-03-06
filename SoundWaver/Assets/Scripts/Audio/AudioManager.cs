﻿using System.Collections;
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
        [Header("Local Audio Clip")]
        [SerializeField, Tooltip("ダイアログのような一部の常にアクセスされる可能性のあるAudioClipのリスト")] private AudioClipTable localAudioClipTable;
        private IEnumerator fadeRoutineBGM;
        private IEnumerator fadeRoutineSE;
        private IEnumerator runPlaySE;
        //  public param
        [NonSerialized] public uint SEVolume;
        [NonSerialized] public uint BGMVolume;
        [NonSerialized] public AudioClipTable clips;
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
            if (vol < Define.c_MinVolume || Define.c_MaxVolume < vol)
            {
                Debug.LogError("AudioManager.cs line 33 GetConvertVolume value is invlid.");
                return 0.0f;
            }
#endif
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
                if (HasLocalClipsKey(key))
                {
                    //TODO:リファイン必須
                    sourceBGM.clip = localAudioClipTable.Table[key];
                    sourceBGM.loop = isLoop;
                    sourceBGM.Play();
                    return;
                }
                Debug.LogError("AudioManager.cs line65 PlayBGM not found sound key.\n" + key);
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
                if (HasLocalClipsKey(key))
                {
                    //TODO:リファイン必須
                    sourceSE.clip = localAudioClipTable.Table[key];
                    sourceSE.loop = isLoop;
                    sourceSE.Play();
                    return;
                }
                Debug.LogError("AudioManager.cs line89 PlaySE not found sound key.\n" + key);
                ErrorManager.Save();
                return;
            }
            sourceSE.clip = clips.Table[key];
            sourceSE.loop = isLoop;
            sourceSE.Play();
        }

        public void PlaySEEx(string key, bool isLoop = false, CoroutineExpansion.CoroutineFinishedFunc func = null)
        {
            if (!HasTableClipsKey(key))
            {
                if (HasLocalClipsKey(key))
                {
                    //TODO:リファイン必須
                    sourceSE.clip = localAudioClipTable.Table[key];
                    sourceSE.loop = isLoop;
                    sourceSE.Play();
                    return;
                }
                Debug.LogError("AudioManager.cs line66 PlaySEEx not found sound key.\n" + key);
                ErrorManager.Save();
                return;
            }

            if (runPlaySE != null)
            {
#if UNITY_EDITOR
                Debug.LogError("AudioManager.cs line123 PlaySEEx runPlaySE is not run until.");
#endif
                StopCoroutine(runPlaySE);
            }
            runPlaySE = PlayRoutine(sourceSE, func);
            this.StartCoroutine(runPlaySE,()=> { runPlaySE = null; });
            sourceSE.clip = clips.Table[key];
            sourceSE.loop = isLoop;
            sourceSE.Play();
        }

        /// <summary>
        /// 音の再生終了時に関数の実行を行いたかったので急遽拡張したCallback
        /// </summary>
        /// <param name="source"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        private IEnumerator PlayRoutine(AudioSource source,CoroutineExpansion.CoroutineFinishedFunc func)
        {
            yield return new WaitUntil(() => { return source.time == 0.0f && !source.isPlaying; });
            //while(true)
            //{
            //    yield return new WaitForFixedUpdate();
            //    if (!source.isPlaying) { break; }
            //}
            func?.Invoke();
        }

        private bool HasTableClipsKey(string key)
        {
            return clips.Table.Keys.Contains(key);
        }

        private bool HasLocalClipsKey(string key)
        {
            return localAudioClipTable.Table.Keys.Contains(key);
        }

        public void FadeBGM(float time, float from, float to, CoroutineExpansion.CoroutineFinishedFunc func = null)
        {
            if (fadeRoutineBGM != null) { StopCoroutine(fadeRoutineBGM); }
            fadeRoutineBGM = FadeRoutine(sourceBGM, time, from, to);
            func += () => { fadeRoutineBGM = null; };
            this.StartCoroutine(fadeRoutineBGM, func);
        }
        public void FadeSE(float time, float from, float to, CoroutineExpansion.CoroutineFinishedFunc func = null)
        {
            if (fadeRoutineSE != null) { StopCoroutine(fadeRoutineSE); }
            fadeRoutineSE = FadeRoutine(sourceSE, time, from, to);
            func += () => { fadeRoutineSE = null; };
            this.StartCoroutine(fadeRoutineSE, func);
        }

        /// <summary>
        /// ゲームシーンのAudioSourceはMusicEngineのものを使用するため急遽任意のインスタンスのフェードができる関数を用意
        /// ※BGM,SEのフェードはこちらでは出来ない(コルーチンの都合上？):左記回避策としてラッピングした専用の関数を用意している。
        /// </summary>
        /// <param name="source"></param>
        /// <param name="time"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="func"></param>
        public void Fade(AudioSource source, float time, float from, float to, CoroutineExpansion.CoroutineFinishedFunc func = null)
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