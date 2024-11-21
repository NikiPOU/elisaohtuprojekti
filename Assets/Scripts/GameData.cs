using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json.Linq;

public class Statistics : MonoBehaviour
{
    public TMP_Text text; // TextMeshPro component
    public GSIDataReceiver gsiDataReceiver; // GSIDataReceiver script component

    // Initialize method for testing
    public void Initialize()
    {
        if (text == null)
        {
            Debug.LogError("Ei toimi: Teksti-komponentti puuttuu.");
            return;
        }

        text.text = "Game statistics here"; // Set the default text
    }

    void Start()
    {
        // Check TMP_Text assignment and log an error if necessary
        if (text == null)
        {
            Debug.LogError("Teksti komponentissa ongelma");
            return; //Exit if TMP text component is empty
        }

        gsiDataReceiver = FindObjectOfType<GSIDataReceiver>();
        if (gsiDataReceiver == null)
        {
            Debug.LogError("Ei toimi: GSI datan haussa ongelma.");
            return; //Exit if GSI data receiver is nout found
        }

        gsiDataReceiver.OnStatisticsDataReceived += StatisticsUpdate;
    }

    private void OnDestroy()
    {
        if (gsiDataReceiver != null)
        {
            gsiDataReceiver.OnStatisticsDataReceived -= StatisticsUpdate;
        }
    }

    void StatisticsUpdate(string jsaonData)
    {
        if (gsiDataReceiver != null && text != null)
        {
            // Extract important data from GSI and update the display text
            string important = ParseGSI(gsiDataReceiver.statisticsData);
            text.text = important; // Update the UI with server data
        }
    }

    string ParseGSI(string jsonData)
    {
        JObject allPlayers = JObject.Parse(jsonData); // Parse JSON using Newtonsoft
        if (allPlayers == null)
        {
            return "No players found.";
        }

        List<string> playerDetails = new List<string>();

        // Iterate over the properties of allPlayers
        foreach (var property in ((JObject)allPlayers).Properties())
        {
            string steamId = property.Name; // Access the key (Steam ID)
            JArray playerData = (JArray)property.Value; // Access the value (player data array)

            // Extract details from the array
            string playerName = playerData[0]?.ToString() ?? "Unknown";
            string team = playerData[1]?.ToString() ?? "No Team";
            int health = playerData[2]?.ToObject<int>() ?? 0;
            int kills = playerData[3]?.ToObject<int>() ?? 0;
            int assists = playerData[4]?.ToObject<int>() ?? 0;
            int deaths = playerData[5]?.ToObject<int>() ?? 0;

            // Format the data
            string handledData = $"Player: {playerName} | " +
                                $"Team: {team} | Health: {health} | " +
                                $"Kills: {kills} | Assists: {assists} | Deaths: {deaths}";

            playerDetails.Add(handledData);
        }
        return string.Join("\n", playerDetails);
    }

}
