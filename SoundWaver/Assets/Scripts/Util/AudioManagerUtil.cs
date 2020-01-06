using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Audio
{
    public class AudioManagerUtil : MonoBehaviour
    {
        [SerializeField] string key;

        public void PlayBGM()
        {
            AudioManager.Instance.PlayBGM(key);
        }
        public void PlaySE()
        {
            AudioManager.Instance.PlaySE(key);
        }

    }
}
