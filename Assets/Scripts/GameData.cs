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

        JObject data = JObject.Parse(jsonData); //parse newtonsoftilla
        string playerName = data["player"]?["name"]?.ToString();
        string team = data["player"]?["team"]?.ToString() ?? "No team assigned yet";
        int health = data["player"]?["state"]?["health"]?.ToObject<int>() ?? 0;
        int kills = data["player"]?["match_stats"]?["kills"]?.ToObject<int>() ?? 0;
        int assists = data["player"]?["match_stats"]?["assists"]?.ToObject<int>() ?? 0;
        int deaths = data["player"]?["match_stats"]?["deaths"]?.ToObject<int>() ?? 0;
        string handledData = $"Player: {playerName}\n"+
                             $"Team: {team}\n"+
                             $"Health: {health}\n"+
                             $"Kills: {kills} | Assists: {assists} | Deaths: {deaths}";

        return handledData;
    }



}