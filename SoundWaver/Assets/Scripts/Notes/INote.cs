public interface INote
{
    //accessor
    bool isReset { get; }
    uint LaneNumber { get; }// set; }
    float DownTime { get; }// set; }

    //function
    //void Setup();
    void Register(uint laneNumber, float downTime);
    void Unregister();
    void Move();
}
