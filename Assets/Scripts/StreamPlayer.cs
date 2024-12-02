using UnityEngine;
using UnityEngine.Video;

public class StreamPlayer : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    void Start()
    {
        videoPlayer.url = "https://www.youtube.com/watch?v=ebcrEHPEZKg"; // Use a direct HLS or DASH URL
        videoPlayer.Play();
    }
}

