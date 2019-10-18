public interface INote
{
    //accessor
    bool isReset { get; }
    int LaneNumber { get; }// set; }
    float DownTime { get; }// set; }

    //function
    void Setup(uint laneNumber, float downTime);
    void Register();
    void Unregister();
    void Move();
}
