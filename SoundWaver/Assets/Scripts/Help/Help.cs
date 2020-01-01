using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.UI;
namespace ChartCreate
{
    public class Help : MonoBehaviour
    {
        //  serialize param
        [SerializeField] private GameObject window;
        [SerializeField] private RadioButton autoMatchBPM;
        [SerializeField] private RadioButton bar;
        [SerializeField] private RadioButton beat;
        [SerializeField] private RadioButton interval;
        [SerializeField] private RadioButton threshold;

        private void Awake()
        {
            if (window.activeSelf)
            {
                window.SetActive(false);
            }
        }

        public void Open()
        {
            window.SetActive(true);
            autoMatchBPM.CallDisable();
            bar.CallDisable();
            beat.CallDisable();
            interval.CallDisable();
            threshold.CallDisable();
        }

        public void Close()
        {
            window.SetActive(false);
        }
    }
}