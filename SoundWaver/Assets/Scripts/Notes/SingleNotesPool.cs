﻿using UnityEngine;

namespace Game
{
    public sealed class SingleNotesPool : SingletonObjectPool<SingleNotesPool>
    {
        protected override void Awake()
        {
            base.Awake();
            Setup();
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public override GameObject GetObject()
        {
            return base.GetObject();
        }

        #region コンテキストメニュー
        [ContextMenu("Create Pool Object Instances")]
        protected override void CreatePoolObjects()
        {
            base.CreatePoolObjects();
        }
        [ContextMenu("Clear Pool Object Instances")]
        private void ClearPoolObjects()
        {
            GetAllChild<SingleNote>();
            foreach (var it in poolList)
            {
                DestroyImmediate(it.gameObject);
            }
        }
        #endregion
    }
}
