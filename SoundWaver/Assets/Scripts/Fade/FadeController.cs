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
            panel.alpha = 0.0f;
        }

        public void FadeIn(float time)
        {
            if (routine != null) { return; }
            routine = Routine(0.0f, 1.0f, time);
            this.StartCoroutine(routine,()=> { routine = null; });
        }
        public void FadeOut(float time)
        {
            if (routine != null) { return; }
            routine = Routine(1.0f, 0.0f, time);
            this.StartCoroutine(routine, () => { routine = null; });
        }

        private IEnumerator Routine(float from, float to, float time)
        {
            float startTime = Time.time;
            panel.alpha = from;
            while (Time.time < startTime + time)
            {
                float rate = time > 0.0f ? (Time.time - startTime) / time : 1.0f;
                var alpha = Mathf.Lerp(0, 1, rate);
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

    }
}