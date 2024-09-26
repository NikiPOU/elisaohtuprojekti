using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json.Linq;


public class Statistics : MonoBehaviour
{

    public TMP_Text text; //textMeshPro komponentti!
    public GSIDataReceiver gsiDataReceiver; //GSIDataReceiver scriptistä komponentti
    void Start()
    {
        if (text == null)
        {
            Debug.Log("Teksti komponentissa ongelma"); 
        }
        
        gsiDataReceiver = FindObjectOfType<GSIDataReceiver>();
        if (gsiDataReceiver == null)
        {
            Debug.LogError("Ei toimi: GSI datan haussa ongelma.");
        }
    }

    void Update()
    {
        if (gsiDataReceiver != null && text != null)
        {
            //tässä eritellään tärkeä data turhasta
            string important = ParseGSI(gsiDataReceiver.gsiData);

            text.text = important; //päivittää serveriltä saatua dataa näytölle
        }

    }
    string ParseGSI(string jsonData)
    {
        JObject data = JObject.Parse(jsonData); // parse JSON with Newtonsoft
        var allPlayers = data["allplayers"];
        if (allPlayers == null)
        {
            return "No players found.";
        }

        List<string> playerDetails = new List<string>();

        foreach (var player in allPlayers)
        {
            string playerName = player.First["name"]?.ToString();
            string team = player.First["team"]?.ToString() ?? "No team assigned yet";
            int health = player.First["state"]?["health"]?.ToObject<int>() ?? 0;
            int kills = player.First["match_stats"]?["kills"]?.ToObject<int>() ?? 0;
            int assists = player.First["match_stats"]?["assists"]?.ToObject<int>() ?? 0;
            int deaths = player.First["match_stats"]?["deaths"]?.ToObject<int>() ?? 0;

            string handledData = $"Player: {playerName} | " +
                                $"Team: {team} | " +
                                $"Health: {health} | " +
                                $"Kills: {kills} | Assists: {assists} | Deaths: {deaths}\n";
            playerDetails.Add(handledData);
    }
    return string.Join("\n", playerDetails);
}



}