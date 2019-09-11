using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INote 
{
    //accessor
    bool isReset { get; }
    int LaneNumber { get; }// set; }
    float DownTime { get; }// set; }

    //function
    void Setup(int laneNumber, float downTime);
    void Register();
    void Unregister();
    void Move();
}
