//参考:https://qiita.com/satotin/items/4f9fb20cefc057d7641b
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;

namespace Game
{
    public class ScrollWall : MonoBehaviour
    {
        [SerializeField] Vector3 direction = Vector3.back;
        [SerializeField] float speed = 1.0f;
        [SerializeField] float resetDistance = 10;
        Vector3 add;
        Vector3 initPosition;

        SynchronizationContext context = null;//メインスレッドのコンテキスト保持用

        // Start is called before the first frame update
        void Start()
        {
            Debug.Log("start func threadNo:" + Thread.CurrentThread.ManagedThreadId);
            add = Vector3.zero;
            initPosition = gameObject.transform.position;
        }

        // Update is called once per frame
       void Update()
        {
            context = SynchronizationContext.Current;
            Work();
        }

        private async Task ScrollTaskAsync()
        {
            //Debug.Log("start threadNo:" + Thread.CurrentThread.ManagedThreadId);
            await Task.Run(
                () =>
                {
                    add += direction * speed * NotesController.Instance.NotesSpeed;// * Time.deltaTime;
                    //Debug.Log("threadNo:" + Thread.CurrentThread.ManagedThreadId);
                }
                );

        }

        async Task Work()
        {
            //Debug.Log("work:" + Thread.CurrentThread.ManagedThreadId);
            await Task.Run(() =>
            {
                //Debug.Log("work task:" + Thread.CurrentThread.ManagedThreadId);
                Execute();
            });
        }

        private void Execute()
        {
            add += direction * speed;// * Time.deltaTime;
            //Debug.Log("execute:" + Thread.CurrentThread.ManagedThreadId);
            //メインスレッド側の処理
            context.Post(
                (state) =>
                {
                    gameObject.transform.position += add;
                    if(resetDistance<=Vector3.Distance(gameObject.transform.position,initPosition))
                    {
                        gameObject.transform.position = initPosition;
                    }
                    //Debug.Log("execute main:" + Thread.CurrentThread.ManagedThreadId);//スレッドは1で実行
                    add = Vector3.zero;
                }, null);

        }
    }
}