using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class MatchScore : MonoBehaviour
{
    public GSIDataReceiver gsiDataReceiver;
    public TMP_Text textMeshPro;

    void Start()
    {
        // Find the GSIDataReceiver instance
        gsiDataReceiver = FindObjectOfType<GSIDataReceiver>();
        if (gsiDataReceiver == null)
        {
            Debug.LogError("Ei toimi: GSI datan haussa ongelma.");
            return;
        }

        // Ensure TextMeshPro is assigned
        if (textMeshPro == null)
        {
            Debug.LogError("TextMeshPro not assigned.");
            return;
        }

        // Subscribe to data updates
        gsiDataReceiver.OnMatchDataReceived += StatisticsUpdate;

        // Initialize text with existing data
        StatisticsUpdate(gsiDataReceiver.statisticsData);
    }

    private void StatisticsUpdate(string data)
    {
        if (gsiDataReceiver != null) {
            JObject matchData = JObject.Parse(data);
            int scoreCT = matchData["score_ct"]?.Value<int>() ?? 0; 
            int scoreT = matchData["score_t"]?.Value<int>() ?? 0;
            // Update the TextMeshPro text
            textMeshPro.text = $"{scoreCT} - {scoreT}";
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        if (gsiDataReceiver != null)
        {
            gsiDataReceiver.OnMatchDataReceived -= StatisticsUpdate;
        }
    }
}
