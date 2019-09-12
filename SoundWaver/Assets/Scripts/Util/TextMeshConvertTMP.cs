using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TextMeshConvertTMP : MonoBehaviour
{
    [SerializeField]
    TextMesh textMesh;
    [SerializeField]
    TextMeshPro tmp;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        tmp.text = textMesh.text;
    }
}
