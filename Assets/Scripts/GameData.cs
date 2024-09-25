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
            string important = ParseGSI(gsidataReceiver.gsiData);

            text.text = important; //päivittää serveriltä saatua dataa näytölle
        }

    }
    string ParseGSI(string jsonData)
    {

        JObject data = JObject.Parse(jsonData); //parse newtonsoftilla
        string playerName = data["player"]?["name"]?.ToString();
        string handledData = $"Player: {playerName}\n";

        return handledData;
    }



}