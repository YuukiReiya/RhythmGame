﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Networking;
using Common;
using Yuuki.FileIO;
using System.IO;
using Game.Audio;
namespace Game
{

    public class Option : MonoBehaviour
    {
        [SerializeField] private GameObject parent;
        [Header("Save Values")]
        [SerializeField] private UILabel bgmVolLabel;
        [SerializeField] private UILabel seVolLabel;
        [SerializeField] private UILabel notesSpeedLabel;

        [Serializable]
        class BothArrow
        {
            public UIWidget leftArrowWidget;
            public UIWidget rightArrowWidget;
        }
        [Header("NGUI")]
        [SerializeField] private BothArrow BGM;
        [SerializeField] private BothArrow SE;
        [SerializeField] private BothArrow notesSpeed;

        //  private param
        private uint BGMVolValue;
        private uint SEVolValue;
        private uint notesSpeedValue;
        //  const param
        private const float c_SoundFadeTime = 0.25f;

        private void Awake()
        {
            if (parent.activeSelf)
            {
                parent.SetActive(false);
            }
        }
        public void Open()
        {
            //ファイルが存在するか判定
            if (File.Exists(Define.c_SettingFilePath))
            {
                var io = new FileIO();
                var ini = JsonUtility.FromJson<IniFile>(io.GetContents(Define.c_SettingFilePath));
                //データ取得
                BGMVolValue = ini.BGMVol;
                SEVolValue = ini.SEVol;
                notesSpeedValue = ini.NotesSpeed;

                //ウィジェット
                SetupBothArrow(BGM, BGMVolValue, Define.c_MinVolume, Define.c_MaxVolume);
                SetupBothArrow(SE, SEVolValue, Define.c_MinVolume, Define.c_MaxVolume);
                SetupBothArrow(notesSpeed, notesSpeedValue, Define.c_MinNoteSpeed, Define.c_MaxNoteSpeed);

                //ラベルに反映
                bgmVolLabel.text = BGMVolValue.ToString();
                seVolLabel.text = SEVolValue.ToString();
                notesSpeedLabel.text = notesSpeedValue.ToString();

                //ウィンドウのアクティブ化
                parent.SetActive(true);
            }
            //無いので生成
            else
            {
                DialogController.Instance.Open(
                 "設定ファイルの読み込みに\n失敗しました。\n\nキャッシュをクリアし\n再生成します。",
                 () =>
                 {
                     var io = new FileIO();
                     //初期データを入れておく
                     IniFile ini = new IniFile();
                     ini.Setup();
                     //ファイルを上書きモードで生成
                     io.CreateFile(
                      Define.c_SettingFilePath,
                      JsonUtility.ToJson(ini),
                      FileIO.FileIODesc.Overwrite
                      );
                     BGMVolValue = Define.c_InitialVol;
                     SEVolValue = Define.c_InitialVol;
                     notesSpeedValue = Define.c_InitialNotesSpeed;
                     //ラベルに反映
                     bgmVolLabel.text = BGMVolValue.ToString();
                     seVolLabel.text = SEVolValue.ToString();
                     notesSpeedLabel.text = notesSpeedValue.ToString();
                     //ウィジェット
                     SetupBothArrow(BGM, BGMVolValue, Define.c_MinVolume, Define.c_MaxVolume);
                     SetupBothArrow(SE, SEVolValue, Define.c_MinVolume, Define.c_MaxVolume);
                     SetupBothArrow(notesSpeed, notesSpeedValue, Define.c_MinNoteSpeed, Define.c_MaxNoteSpeed);
                     //ウィンドウのアクティブ化
                     parent.SetActive(true);
                 });
            }
        }
        public void Close()
        {
            var audio = AudioManager.Instance;
            var io = new FileIO();
            var content = io.GetContents(Define.c_SettingFilePath);
            if (content == string.Empty)
            {
                //再生成
                //※ここに入るのは、設定画面開いている中で"*.ini"を削除した時なのでそうそう入らない
                Debug.LogError("Option.cs line154 File.GetContent error");
                ErrorManager.Save();
                DialogController.Instance.Open(
                    "予期せぬエラーが\n発生しました。",
                    () =>
                    {
                        parent.SetActive(false);
                    });
            }
            else
            {
                //相対パスはココで変化させないので一旦ロードしなおす
                var ini = JsonUtility.FromJson<IniFile>(content);
                ini.BGMVol = BGMVolValue;
                ini.SEVol = SEVolValue;
                ini.NotesSpeed = notesSpeedValue;
                audio.BGMVolume = BGMVolValue;
                audio.SEVolume = SEVolValue;
                //上書き保存
                io.CreateFile(
                    Define.c_SettingFilePath,
                    JsonUtility.ToJson(ini),
                    FileIO.FileIODesc.Overwrite
                    );

                //音量の反映
                audio.FadeBGM(
                    c_SoundFadeTime,
                    audio.SourceBGM.volume,
                    audio.GetConvertVolume(audio.BGMVolume)
                    );
                audio.FadeSE(
                    c_SoundFadeTime,
                    audio.SourceSE.volume,
                    audio.GetConvertVolume(audio.SEVolume)
                    );
            }
            parent.SetActive(false);
        }

        private void SetupBothArrow(BothArrow arrow,uint val,uint min,uint max)
        {
            arrow.leftArrowWidget.alpha = val <= min ? 0 : 1;
            arrow.rightArrowWidget.alpha = val >= max ? 0 : 1;
        }

        public void AddBGMVol()
        {
            BGMVolValue += BGMVolValue >= Define.c_MaxVolume ? (uint)0 : (uint)1;
            SetupBothArrow(BGM, BGMVolValue, Define.c_MinVolume, Define.c_MaxVolume);
            bgmVolLabel.text = BGMVolValue.ToString();
            //反映
            AudioManager.Instance.SourceBGM.volume = AudioManager.Instance.GetConvertVolume(BGMVolValue);
        }

        public void SubBGMVol()
        {
            BGMVolValue -= BGMVolValue <= Define.c_MinVolume ? (uint)0 : (uint)1;
            SetupBothArrow(BGM, BGMVolValue, Define.c_MinVolume, Define.c_MaxVolume);
            bgmVolLabel.text = BGMVolValue.ToString();
            //反映
            AudioManager.Instance.SourceBGM.volume = AudioManager.Instance.GetConvertVolume(BGMVolValue);
        }

        public void AddSEVol()
        {
            SEVolValue += SEVolValue >= Define.c_MaxVolume ? (uint)0 : (uint)1;
            SetupBothArrow(SE, SEVolValue, Define.c_MinVolume, Define.c_MaxVolume);
            seVolLabel.text = SEVolValue.ToString();
            //反映
            AudioManager.Instance.SourceSE.volume = AudioManager.Instance.GetConvertVolume(SEVolValue);
        }

        public void SubSEVol()
        {
            SEVolValue -= SEVolValue <= Define.c_MinVolume ? (uint)0 : (uint)1;
            SetupBothArrow(SE, SEVolValue, Define.c_MinVolume, Define.c_MaxVolume);
            seVolLabel.text = SEVolValue.ToString();
            //反映
            AudioManager.Instance.SourceSE.volume = AudioManager.Instance.GetConvertVolume(SEVolValue);
        }

        public void AddNoteSpeed()
        {
            notesSpeedValue += notesSpeedValue >= Define.c_MaxNoteSpeed ? (uint)0 : (uint)1;
            SetupBothArrow(notesSpeed, notesSpeedValue, Define.c_MinNoteSpeed, Define.c_MaxNoteSpeed);
            notesSpeedLabel.text = notesSpeedValue.ToString();
        }

        public void SubNoteSpeed()
        {
            notesSpeedValue -= notesSpeedValue <= Define.c_MinNoteSpeed ? (uint)0 : (uint)1;
            SetupBothArrow(notesSpeed, notesSpeedValue, Define.c_MinNoteSpeed, Define.c_MaxNoteSpeed);
            notesSpeedLabel.text = notesSpeedValue.ToString();
        }
    }

}