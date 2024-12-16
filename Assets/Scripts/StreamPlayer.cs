using UnityEngine;
using UnityEngine.Video;
using Newtonsoft.Json.Linq;
using System.Collections;

/// <summary>
/// Handles playback of a local video synchronized with match data received from GSIDataReceiver.
/// </summary>
public class PlayLocalVideo : MonoBehaviour
{
    //Video player component for video playback.
    public VideoPlayer videoPlayer;

    //GSI receiver component for receiving GSI data.
    public GSIDataReceiver gsiDataReceiver;

    /// <summary>
    /// Unity method called when the script is initialized.
    /// Initializes components and prepares the video for playback.
    /// </summary>
    void Start()
    {
        //Check if the VideoPlayer component is assigned, log an error if not.
        if (videoPlayer == null)
        {
            Debug.LogError("VideoPlayer component is missing.");
            return;
        }

        //If GSIDataReceiver is not assigned in the inspector, attempt to find it in the scene.
        if (gsiDataReceiver == null)
        {
            gsiDataReceiver = FindObjectOfType<GSIDataReceiver>();
            if (gsiDataReceiver == null)
            {
                Debug.LogError("GSIDataReceiver component is missing.");
                return;
            }
        }

        //Prepare the video to ensure the texture is ready.
        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += OnVideoPrepared;

        //Subscribe to the event that handles incoming match data from the GSIDataReceiver.
        gsiDataReceiver.OnMatchDataReceived += HandleMatchData;
    }

    /// <summary>
    /// Unity method called when the script is destroyed.
    /// Unsubscribes from events.
    /// </summary>
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

    /// <summary>
    /// Called when the video has been prepared and the texture is ready for application.
    /// </summary>
    /// <param name="vp">The VideoPlayer that has been prepared.</param>
    private void OnVideoPrepared(VideoPlayer vp)
    {
        Debug.Log("Video is prepared and texture is ready.");
        ApplyVideoTexture(vp);
    }

    /// <summary>
    /// Applies the video texture to the object's material renderer.
    /// This ensures that the video is rendered onto the surface of the object.
    /// </summary>
    /// <param name="vp">The VideoPlayer that provides the video texture.</param>
    private void ApplyVideoTexture(VideoPlayer vp)
    {
        //Apply the video texture to the plane's material.
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.mainTexture = vp.texture;
        }
    }

    /// <summary>
    /// Handles incoming match data from the GSIDataReceiver.
    /// Triggers video playback based on the data.
    /// </summary>
    /// <param name="data">The match data as a JSON string.</param>
    private void HandleMatchData(string data)
    {
        JObject matchData = JObject.Parse(data);
        int round = matchData["round"]?.Value<int>() ?? -1; //Default to -1 if null or missing.

        Debug.Log("round " + round);

        if (round == 0)
        {
            //Start a coroutine to delay the video playback by 20 seconds.
            StartCoroutine(DelayedVideoPlayback(20f));
        }
    }

    /// <summary>
    /// Coroutine that delays the start of video playback by the specified duration.
    /// Ensures the video starts after delay.
    /// </summary>
    /// <param name="delay">The duration (in seconds) to wait before starting video playback.</param>
    /// <returns>Returns an enumerator for coroutine execution.</returns>
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
