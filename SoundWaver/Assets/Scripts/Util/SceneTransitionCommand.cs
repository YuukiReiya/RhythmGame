using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneTransitionCommand : MonoBehaviour
{
    [SerializeField] uint sceneIndex;
    public void Execute()
    {
        SceneManager.LoadScene((int)sceneIndex);
    }
}
