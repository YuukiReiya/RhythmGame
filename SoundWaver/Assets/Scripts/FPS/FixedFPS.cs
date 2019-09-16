using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedFPS : MonoBehaviour
{
    [SerializeField, Range (0, 120)]
    int fps = 60;
    void Awake ()
    {
        /// <summary>
        /// "ProjectSetting/Quality/Other/VSync Count" = Don't Sync の手動設定
        /// </summary>
        QualitySettings.vSyncCount = 0;
        /// <summary>
        /// 事前に"ProjectSetting/Quality/Other/VSync Count"をDon't Syncに設定
        /// </summary>
        Application.targetFrameRate = fps;
    }
}