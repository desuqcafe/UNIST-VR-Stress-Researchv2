using UnityEngine;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{
    public VideoPlayer videoPlayer1;
    public VideoPlayer videoPlayer2;

    void Start()
    {
        videoPlayer1.Play();
        videoPlayer2.Play();
    }
}