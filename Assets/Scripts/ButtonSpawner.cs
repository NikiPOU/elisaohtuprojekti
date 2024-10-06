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

    // This will hold the current player names
    private List<string> playerNames = new List<string>();

    // This will hold the instantiated buttons
    private List<Button> buttons = new List<Button>();

    void Start()
    {
        if (buttonParent == null)
        {
            Debug.LogError("Button Parent is not set!");
            return;
        }

        gsiDataReceiver = FindObjectOfType<GSIDataReceiver>();
        if (gsiDataReceiver == null)
        {
            Debug.LogError("Ei toimi: GSI datan haussa ongelma.");
            return;
        }

        // Subscribe to the OnDataReceived event
        gsiDataReceiver.OnDataReceived += StatisticsUpdate;

        // Initial update if data is already available
        StatisticsUpdate(gsiDataReceiver.gsiData);
    }

    private void OnDestroy()
    {
        if (gsiDataReceiver != null)
        {
            gsiDataReceiver.OnDataReceived -= StatisticsUpdate;
        }
    }

    void StatisticsUpdate(string jsonData)
    {
        if (!string.IsNullOrEmpty(jsonData))
        {
            List<string> newPlayerNames = ParseGSI(jsonData);

            // If there are changes in the player list, update the UI
            if (!ArePlayerListsEqual(playerNames, newPlayerNames))
            {
                UpdateButtons(newPlayerNames);
                playerNames = new List<string>(newPlayerNames); // Update the current player list
            }
        }
        else
        {
            Debug.LogError("Received empty JSON data.");
        }
    }

    // Update buttons only when there is a change in the player list
    void UpdateButtons(List<string> newPlayerNames)
    {
        // If there are more buttons than players, remove the extra buttons
        while (buttons.Count > newPlayerNames.Count)
        {
            Destroy(buttons[buttons.Count - 1].gameObject); // Destroy the excess button
            buttons.RemoveAt(buttons.Count - 1); // Remove from the list
        }

        // For each player, update the button or create a new one if needed
        for (int i = 0; i < newPlayerNames.Count; i++)
        {
            if (i < buttons.Count)
            {
                // Update the existing button's text
                TextMeshProUGUI buttonText = buttons[i].GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = newPlayerNames[i];
                }
            }
            else
            {
                // Instantiate a new button if needed
                Button newButton = Instantiate(buttonPrefab, buttonParent);
                TextMeshProUGUI buttonText = newButton.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = newPlayerNames[i];
                    buttonText.color = Color.white;
                }
                buttons.Add(newButton); // Add the new button to the list
            }
        }
    }

    // Compare two player lists to see if they are the same
    bool ArePlayerListsEqual(List<string> oldList, List<string> newList)
    {
        if (oldList.Count != newList.Count) return false;

        for (int i = 0; i < oldList.Count; i++)
        {
            if (oldList[i] != newList[i]) return false;
        }

        return true;
    }

    // Parse JSON data and return a list of player names
    List<string> ParseGSI(string jsonData)
    {
        JObject data = JObject.Parse(jsonData); // Parse JSON using Newtonsoft
        var allPlayers = data["allplayers"];
        if (allPlayers == null)
        {
            Debug.LogError("No players found in JSON data.");
            return new List<string>(); // Return an empty list if no players are found
        }

        List<string> playerDetails = new List<string>();

        // Loop through all players and extract their names
        foreach (var player in allPlayers)
        {
            string playerName = player.First["name"]?.ToString();
            if (!string.IsNullOrEmpty(playerName))
            {
                playerDetails.Add(playerName);
            }
        }
        return playerDetails; // Return the list of player names
    }
}
