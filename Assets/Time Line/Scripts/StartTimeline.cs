using UnityEngine;
using UnityEngine.Playables;

public class Scripts : MonoBehaviour
{
    public void StartTimeLine(PlayableDirector timline)
    {
        timline.Play();
    }
}
