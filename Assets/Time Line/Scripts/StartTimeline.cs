using UnityEngine;
using UnityEngine.Playables;

public class Scripts : MonoBehaviour
{
    [SerializeField] private PlayableDirector timline;

    // Start is called before the first frame update
    void Start()
    {
        timline = GetComponent<PlayableDirector>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            timline.Play();
        }
    }
}
