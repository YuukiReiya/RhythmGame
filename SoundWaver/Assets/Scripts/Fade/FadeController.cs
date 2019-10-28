using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yuuki;
using Yuuki.MethodExpansions;

namespace API.Util
{
    public class FadeController : SingletonMonoBehaviour<FadeController>
    {
        //serialize param
        private UIPanel panel;

        //private param
        IEnumerator routine;

        //public param

        //accessor
        public Queue<System.Action> EventQueue { get; private set; }
        public IEnumerator Routine { get { return routine; } }

        protected override void Awake()
        {
            base.Awake();
            EventQueue = new Queue<System.Action>();
            routine = null;
            if (!TryGetComponent(out panel))
            {
                Debug.LogError("Panelがアタッチされていない");
                return;
            }
            //panel.alpha = 0.0f;
        }

        public void FadeIn(float time, float from = 0.0f, float to = 1.0f)
        {
            if (routine != null) { return; }
            routine = Execute(from, to, time);
            this.StartCoroutine(routine, () => { routine = null; });
        }
        public void FadeOut(float time, float from = 1.0f, float to = 0.0f)
        {
            if (routine != null) { return; }
            Debug.Log("yes");
            routine = Execute(from, to, time);
            this.StartCoroutine(routine, () => { routine = null; });
        }
        private IEnumerator Execute(float from, float to, float time)
        {
            float startTime = Time.time;
            panel.alpha = from;
            while (Time.time <= startTime + time)
            {
                float rate = time > 0.0f ? (Time.time - startTime) / time : 1.0f;
                var alpha = Mathf.Lerp(from, to, rate);
                panel.alpha = alpha;
                yield return null;
            }
            panel.alpha = to;
            foreach(var ev in EventQueue)
            {
                ev?.Invoke();
            }
            EventQueue.Clear();
        }

        public void Stop(bool callbackExecute=false)
        {
            if (routine != null) { 
                StopCoroutine(routine);
                routine = null;
            }
            if (callbackExecute)
            {
                foreach(var e in EventQueue)
                {
                    e?.Invoke();
                }
            }
            EventQueue.Clear();
        }
        public void SetAlpha(float alpha)
        {
            panel.alpha = alpha;
        }
    }
}