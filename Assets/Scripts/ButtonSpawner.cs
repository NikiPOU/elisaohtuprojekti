using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

public class ButtonSpawner : MonoBehaviour
{
    public Button buttonPrefab;
    public Transform buttonParent;
    public GSIDataReceiver gsiDataReceiver;
    private List<string> playerNames = new List<string>();
    private List<Button> buttons = new List<Button>();

    void Start()
    {
        gsiDataReceiver = FindObjectOfType<GSIDataReceiver>();
        if (gsiDataReceiver == null)
        {
            Debug.LogError("Ei toimi: GSI datan haussa ongelma.");
            return;
        }

        gsiDataReceiver.OnStatisticsDataReceived += StatisticsUpdate;
        StatisticsUpdate(gsiDataReceiver.statisticsData);
    }

    private void OnDestroy()
    {
        if (gsiDataReceiver != null)
        {
            gsiDataReceiver.OnStatisticsDataReceived -= StatisticsUpdate;
        }
    }

    void StatisticsUpdate(string jsonData)
    {
        if (!string.IsNullOrEmpty(jsonData))
        {
            List<string> newPlayerNames = ParseGSI(jsonData);
            if (!ArePlayerListsEqual(playerNames, newPlayerNames))
            {
                UpdateButtons(newPlayerNames);
                playerNames = new List<string>(newPlayerNames);
            }
        }
        else
        {
            Debug.LogError("Ei json dataa.");
        }
    }

    void UpdateButtons(List<string> newPlayerNames)
    {
        // Hoitaa nappien päivityksen, kun pelaajat vaihtuvat
        while (buttons.Count > newPlayerNames.Count)
        {
            Destroy(buttons[buttons.Count - 1].gameObject);
            buttons.RemoveAt(buttons.Count - 1);
        }

        for (int i = 0; i < newPlayerNames.Count; i++)
        {
            if (i < buttons.Count)
            {
                TextMeshProUGUI buttonText = buttons[i].GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = newPlayerNames[i];
                }
                AssignButtonAction(buttons[i], newPlayerNames[i]);
            }
            else
            {
                Button newButton = Instantiate(buttonPrefab, buttonParent);
                
                Vector3 newPosition = newButton.transform.localPosition;
                newPosition.x += i * 0.8f; // Jokainen nappi 0,75 välillä edelliseen
                newButton.transform.localPosition = newPosition;

                TextMeshProUGUI buttonText = newButton.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = newPlayerNames[i];
                    buttonText.color = Color.white;
                }

                AssignButtonAction(newButton, newPlayerNames[i]);
                buttons.Add(newButton);
            }
        }
    }

        void AssignButtonAction(Button button, string playerName)
    {
        // Remove previous listeners to avoid duplication
        button.onClick.RemoveAllListeners();

        // Add a listener to call ButtonClicked with the player's name
        button.onClick.AddListener(() => ButtonClicked(playerName));
    }

    void ButtonClicked(string playerName)
    {
        // Call the method in the other script (replace 'OtherScript' with the actual script name)
        HeatMapPlayerMovement HeatmapScript = FindObjectOfType<HeatMapPlayerMovement>();
        if (HeatmapScript != null)
        {
            HeatmapScript.ButtonClicked(playerName);
        }
        else
        {
            Debug.LogError("HeatmapScript not found.");
        }
    }

    bool ArePlayerListsEqual(List<string> oldList, List<string> newList)
    // Tarkistaa, onko pelaajia vaihtunut
    {
        if (oldList.Count != newList.Count) return false;

        for (int i = 0; i < oldList.Count; i++)
        {
            if (oldList[i] != newList[i]) return false;
        }

        return true;
    }

    List<string> ParseGSI(string jsonData)
    // Etsii datasta pelaaja nimet listaan
    {
        JObject allPlayers = JObject.Parse(jsonData);
        if (allPlayers == null)
        {
            Debug.LogError("Pelaajia ei löytynyt datasta.");
            return new List<string>();
        }

        List<string> playerDetails = new List<string>();

        foreach (var property in ((JObject)allPlayers).Properties())
        {
            JArray playerData = (JArray)property.Value;
            string playerName = playerData[0]?.ToString();
            if (!string.IsNullOrEmpty(playerName))
            {
                playerDetails.Add(playerName);
            }
        }
        return playerDetails;
    }
}
