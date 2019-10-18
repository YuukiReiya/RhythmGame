using DG.Tweening;
using UnityEngine;
public class NoteDecorate : MonoBehaviour
{
    [SerializeField] Vector3 from;
    [SerializeField] Vector3 to;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Music.GetCurrentMusic() != null && Music.IsJustChangedBeat())
        {
            DOTween.To(t => OnScale(t), 0, 1, 0.1f);
        }
    }

    public void OnScale(float t)
    {
        var scale = transform.localScale;
        scale = Vector3.Lerp(from, to, t);
        transform.localScale = scale;
    }
}
