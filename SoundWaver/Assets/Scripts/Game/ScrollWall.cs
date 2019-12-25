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
        AudioSource source;
        float baseSpeed;
        SynchronizationContext context = null;//メインスレッドのコンテキスト保持用

        // Start is called before the first frame update
        void Start()
        {
            add = Vector3.zero;
            initPosition = gameObject.transform.position;
        }

        public void Setup(AudioSource source)
        {
            this.source = source;
            baseSpeed = NotesController.Instance.NotesSpeed;
        }

        public void Run()
        {
            context = SynchronizationContext.Current;
            this.StartCoroutine(MainRoutine());
        }

        private IEnumerator MainRoutine()
        {
            while (true)
            {
                Work();
                yield return null;
            }
        }

        //async Task Work()
        //{
        //    await Task.Run(() =>
        //    {
        //        Execute();
        //    });
        //}

        private void Work()
        {
            add += direction.normalized * speed * baseSpeed * source.pitch;
            gameObject.transform.position += add;
            if (resetDistance <= Vector3.Distance(gameObject.transform.position, initPosition))
            {
                gameObject.transform.position = initPosition;
            }
            add = Vector3.zero;
        }

        private void Execute()
        {
            add += direction * speed * baseSpeed;// * Time.deltaTime
            //メインスレッド側の処理
            context.Post(
                (state) =>
                {
                    add *= source.pitch;
                    gameObject.transform.position += add;
                    if(resetDistance<=Vector3.Distance(gameObject.transform.position,initPosition))
                    {
                        gameObject.transform.position = initPosition;
                    }
                    add = Vector3.zero;
                }, null);

        }
    }
}