using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Newtonsoft.Json.Linq;

/// <summary>
/// Handles the display and updating of match scores using GSI data reveiver.
/// </summary>
public class MatchScore : MonoBehaviour
{
    // The GSI data receiver for retrieving match data.
    public GSIDataReceiver gsiDataReceiver;

    //Reference to the TextMeshPro component for displaying scores.
    public TMP_Text textMeshPro;

    /// <summary>
    /// Initializes the script, and subscribes to match data updates.
    /// </summary>
    void Start()
    {
        //Find the GSIDataReceiver instance
        gsiDataReceiver = FindObjectOfType<GSIDataReceiver>();
        if (gsiDataReceiver == null)
        {
            Debug.LogError("Ei toimi: GSI datan haussa ongelma.");
            return;
        }

        //Ensure TextMeshPro is assigned
        if (textMeshPro == null)
        {
            Debug.LogError("TextMeshPro not assigned.");
            return;
        }

        //Subscribe to data updates
        gsiDataReceiver.OnMatchDataReceived += StatisticsUpdate;

        //Initialize text with existing data
        StatisticsUpdate(gsiDataReceiver.statisticsData);
    }

    /// <summary>
    /// Updates the score display when match data is received.
    /// </summary>
    /// <param name="data">JSON string containing match statistics.</param>
    private void StatisticsUpdate(string data)
    {
        if (gsiDataReceiver != null) {
            JObject matchData = JObject.Parse(data);
            int scoreCT = matchData["score_ct"]?.Value<int>() ?? 0; 
            int scoreT = matchData["score_t"]?.Value<int>() ?? 0;
            // Update the TextMeshPro text
            textMeshPro.text = $"<color=#51CDF3>{scoreCT}</color>  -  <color=#EFDE49>{scoreT}</color>";
        }
    }

    /// <summary>
    /// Unsubscribes from GSI data updates when the script is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        if (gsiDataReceiver != null)
        {
            gsiDataReceiver.OnMatchDataReceived -= StatisticsUpdate;
        }
    }
}
