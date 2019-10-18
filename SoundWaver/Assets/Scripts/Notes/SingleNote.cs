﻿using UnityEngine;

namespace Game
{
    public class SingleNote : MonoBehaviour, INote
    {
        public bool isReset { get; }

        uint laneNumber;
        public int LaneNumber
        {
            get { return 1; }
        }

        float dt;
        public float DownTime { get { return this.dt; } }

        public void Setup(uint laneNumber, float downTime)
        {
            this.laneNumber = laneNumber;
            dt = downTime;
        }

        // Start is called before the first frame update
        void Start()
        {
            Register();
        }

        protected void OnEnable()
        {
            //Register();
        }

        protected void OnDisable()
        {
            //Unregister();
        }

        // Update is called once per frame
        void Update()
        {
        }
        public void Move()
        {
            var x = this.transform.position.x;
            //float timing = DownTime - NotesController.Instance.elapsedTime;
            float timing = DownTime - GameController.Instance.ElapsedTime;

            //TODO:汚い
            //Vector3 ndir = (this.transform.position - NotesController.Instance.JustTimingPosition).normalized;
            Vector3 ndir = (-this.transform.up.normalized);

            var pos = NotesController.Instance.JustTimingPosition - ndir * timing * NotesController.Instance.NotesSpeed;
            pos.x = x;
            this.transform.position = pos;
        }

        /// <summary>
        /// ノーツの登録
        /// </summary>
        public void Register()
        {
            //空でOK?
            //NotesController.Instance.NotesQueue.Enqueue(this);
        }

        /// <summary>
        /// ノーツの登録解除
        /// </summary>
        public void Unregister()
        {
            this.gameObject.SetActive(false);
            NotesController.Instance.Notes.Remove(this);
        }
    }
}
