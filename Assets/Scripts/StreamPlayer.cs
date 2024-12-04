using UnityEngine;
using UnityEngine.Video;
using Newtonsoft.Json.Linq;
using System.Collections;

public class PlayLocalVideo : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GSIDataReceiver gsiDataReceiver;

    void Start()
    {
        if (videoPlayer == null)
        {
            Debug.LogError("VideoPlayer component is missing.");
            return;
        }

        if (gsiDataReceiver == null)
        {
            gsiDataReceiver = FindObjectOfType<GSIDataReceiver>();
            if (gsiDataReceiver == null)
            {
                Debug.LogError("GSIDataReceiver component is missing.");
                return;
            }
        }

        // Prepare the video to ensure the texture is ready
        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += OnVideoPrepared;

        gsiDataReceiver.OnMatchDataReceived += HandleMatchData;
    }

    void OnDestroy()
    {
        if (gsiDataReceiver != null)
        {
            gsiDataReceiver.OnMatchDataReceived -= HandleMatchData;
        }

        if (videoPlayer != null)
        {
            videoPlayer.prepareCompleted -= OnVideoPrepared;
        }
    }

    private void OnVideoPrepared(VideoPlayer vp)
    {
        Debug.Log("Video is prepared and texture is ready.");
        ApplyVideoTexture(vp);
    }

    private void ApplyVideoTexture(VideoPlayer vp)
    {
        // Apply the video texture to the plane's material
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.mainTexture = vp.texture;
        }
    }

    private void HandleMatchData(string data)
    {
        JObject matchData = JObject.Parse(data);
        int round = matchData["round"]?.Value<int>() ?? -1; // Default to -1 if null or missing

        Debug.Log("round " + round);

        if (round == 0)
        {
            // Start a coroutine to delay the video playback by 20 seconds
            StartCoroutine(DelayedVideoPlayback(20f));
        }
    }

    private IEnumerator DelayedVideoPlayback(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Play the video if it’s not already playing
        if (!videoPlayer.isPlaying)
        {
            videoPlayer.Play();
        }
    }
}
