using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Networking;
using Common;
using Yuuki.FileIO;
using System.IO;
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
        private void Awake()
        {
            if (parent.activeSelf)
            {
                parent.SetActive(false);
            }
        }
#if UNITY_EDITOR
        private void Start()
        {
            if(parent.activeSelf)
            {
                parent.SetActive(false);
            }
        }
#endif

        public void Open()
        {
            //ファイルが存在するか判定
            if (File.Exists(Define.c_SettingFilePath))
            {
                StartCoroutine(ReadSettingFileRoutine());
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
                     IniFile iniFile = new IniFile();
                     iniFile.CurrentPath = Define.c_InitialCurrentPath;
                     iniFile.BGMVol = Define.c_InitialVol;
                     iniFile.SEVol = Define.c_InitialVol;
                     iniFile.NotesSpeed = Define.c_InitialNotesSpeed;
                     //ファイルを上書きモードで生成
                     io.CreateFile(
                      Define.c_SettingFilePath,
                      JsonUtility.ToJson(iniFile),
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
            return;
            //var io = new FileIO();
            //var content = io.GetContents(Define.c_SettingFilePath);
            ////読み込みに失敗
            //if (content == string.Empty)
            //{
            //    DialogController.Instance.Open(
            //        "設定ファイルの読み込みに\n失敗しました。\n\nキャッシュをクリアし\n再生成します。",
            //        () =>
            //        {
            //            //初期データを入れておく
            //            IniFile iniFile = new IniFile();
            //            iniFile.CurrentPath = Define.c_InitialCurrentPath;
            //            iniFile.BGMVol = Define.c_InitialVol;
            //            iniFile.SEVol = Define.c_InitialVol;
            //            iniFile.NotesSpeed = Define.c_InitialNotesSpeed;
            //            //ファイルを上書きモードで生成
            //            io.CreateFile(
            //                Define.c_SettingFilePath,
            //                JsonUtility.ToJson(iniFile),
            //                FileIO.FileIODesc.Overwrite
            //                );
            //            BGMVolValue = Define.c_InitialVol;
            //            SEVolValue = Define.c_InitialVol;
            //            notesSpeedValue = Define.c_InitialNotesSpeed;
            //            //ラベルに反映
            //            bgmVolLabel.text = BGMVolValue.ToString();
            //            seVolLabel.text = SEVolValue.ToString();
            //            notesSpeedLabel.text = notesSpeedValue.ToString();
            //            //ウィジェット
            //            SetupBothArrow(BGM, BGMVolValue, Define.c_MinVolume, Define.c_MaxVolume);
            //            SetupBothArrow(SE, SEVolValue, Define.c_MinVolume, Define.c_MaxVolume);
            //            SetupBothArrow(notesSpeed, notesSpeedValue, Define.c_MinNoteSpeed, Define.c_MaxNoteSpeed);
            //            //ウィンドウのアクティブ化
            //            parent.SetActive(true);
            //        });
            //}
            //else
            //{
            //    var ini = JsonUtility.FromJson<IniFile>(content);
            //    //データ取得
            //    BGMVolValue = ini.BGMVol;
            //    SEVolValue = ini.SEVol;
            //    notesSpeedValue = ini.NotesSpeed;

            //    //ウィジェット
            //    SetupBothArrow(BGM, BGMVolValue, Define.c_MinVolume, Define.c_MaxVolume);
            //    SetupBothArrow(SE, SEVolValue, Define.c_MinVolume, Define.c_MaxVolume);
            //    SetupBothArrow(notesSpeed, notesSpeedValue, Define.c_MinNoteSpeed, Define.c_MaxNoteSpeed);

            //    //ラベルに反映
            //    bgmVolLabel.text = BGMVolValue.ToString();
            //    seVolLabel.text = SEVolValue.ToString();
            //    notesSpeedLabel.text = notesSpeedValue.ToString();

            //    //ウィンドウのアクティブ化
            //    parent.SetActive(true);
            //}
        }
        private IEnumerator ReadSettingFileRoutine()
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
            yield break;
        }
#if false
        private IEnumerator ReadSettingFileRoutine()
        {
            using (var request = UnityWebRequest.Get(Define.c_SettingFilePath))
            {
                yield return request.SendWebRequest();
                var io = new FileIO();
                //読み込み失敗
                //※Cannot connect to destination host で必ずエラーが出る
                //無視して読み込みデータを参照する
#if false
                if (request.isNetworkError || request.isHttpError)
                {
                    Debug.LogError("Option.cs line52: UnityWebRequest Error\n" + "file path is \"" + Define.c_SettingFilePath + "\"" + request.error);
                    ErrorManager.Save();
                    DialogController.Instance.Open(
                            "設定ファイルの読み込みに\n失敗しました。\n\nキャッシュをクリアし\n再生成します。",
                            () =>
                            {
                                //初期データを入れておく
                                IniFile iniFile = new IniFile();
                                iniFile.CurrentPath = Define.c_InitialCurrentPath;
                                iniFile.BGMVol = Define.c_InitialVol;
                                iniFile.SEVol = Define.c_InitialVol;
                                iniFile.NotesSpeed = Define.c_InitialNotesSpeed;
                                //ファイルを上書きモードで生成
                                io.CreateFile(
                                    Define.c_SettingFilePath,
                                    JsonUtility.ToJson(iniFile),
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
                    yield break;
                }
#endif
                //読み込み成功
                var ini = JsonUtility.FromJson<IniFile>(request.downloadHandler.text);

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
            yield break;
        }
#endif
                public void Close()
        {
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
                //上書き保存
                io.CreateFile(
                    Define.c_SettingFilePath,
                    JsonUtility.ToJson(ini),
                    FileIO.FileIODesc.Overwrite
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
        }

        public void SubBGMVol()
        {
            BGMVolValue -= BGMVolValue <= Define.c_MinVolume ? (uint)0 : (uint)1;
            SetupBothArrow(BGM, BGMVolValue, Define.c_MinVolume, Define.c_MaxVolume);
            bgmVolLabel.text = BGMVolValue.ToString();
        }

        public void AddSEVol()
        {
            SEVolValue += SEVolValue >= Define.c_MaxVolume ? (uint)0 : (uint)1;
            SetupBothArrow(SE, SEVolValue, Define.c_MinVolume, Define.c_MaxVolume);
            seVolLabel.text = SEVolValue.ToString();
        }

        public void SubSEVol()
        {
            SEVolValue -= SEVolValue <= Define.c_MinVolume ? (uint)0 : (uint)1;
            SetupBothArrow(SE, SEVolValue, Define.c_MinVolume, Define.c_MaxVolume);
            seVolLabel.text = SEVolValue.ToString();
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