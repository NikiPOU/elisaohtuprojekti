using UnityEngine;
using UnityEngine.Video;

public class PlayLocalVideo : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    void Start()
    {
        if (videoPlayer == null)
        {
            Debug.LogError("VideoPlayer component is missing!");
            return;
        }

        // Play the video
        videoPlayer.Play();
    }
}
