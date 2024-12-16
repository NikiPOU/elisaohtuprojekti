using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

/// <summary>
/// Dynamically spawns buttons based on received GSI data.
/// </summary>
public class ButtonSpawner : MonoBehaviour
{
    public Button buttonPrefab; //Prefab for creating buttons
    public Transform buttonParent; //Parent object for positioning buttons
    public GSIDataReceiver gsiDataReceiver; //Script for receiving GSI data
    private List<string> playerNames = new List<string>(); //List of current player names
    private List<Button> buttons = new List<Button>(); //List of spawned buttons

    private Button lastClickedButton = null; //keep track what button is being cliked

    /// <summary>
    /// Initialize GSI data receiver and set up event subscriptions.
    /// </summary>
    void Start()
    {
        gsiDataReceiver = FindObjectOfType<GSIDataReceiver>();
        if (gsiDataReceiver == null)
        {
            Debug.LogError("Ei toimi: GSI datan haussa ongelma.");
            return;
        }

        // Subscribe to GSI data updates
        gsiDataReceiver.OnStatisticsDataReceived += StatisticsUpdate;

        // Initialize buttons with the initial data
        StatisticsUpdate(gsiDataReceiver.statisticsData);
    }

    /// <summary>
    /// Cancel event subscriptions on destruction.
    /// </summary>
    private void OnDestroy()
    {
        if (gsiDataReceiver != null)
        {
            gsiDataReceiver.OnStatisticsDataReceived -= StatisticsUpdate;
        }
    }

    /// <summary>
    /// Handles incoming JSON data, updates button list if players have changed.
    /// </summary>
    /// <param name="jsonData">JSON data string containing player information.</param>
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

    /// <summary>
    /// Updates the buttons in UI to match the list of current players.
    /// </summary>
    /// <param name="newPlayerNames">List of new player names.</param>
    void UpdateButtons(List<string> newPlayerNames)
    {
        //Remove extra buttons if there are fewer players now
        while (buttons.Count > newPlayerNames.Count)
        {
            Destroy(buttons[buttons.Count - 1].gameObject);
            buttons.RemoveAt(buttons.Count - 1);
        }

        //Update or create buttons
        for (int i = 0; i < newPlayerNames.Count; i++)
        {
            if (i < buttons.Count)
            {
                //Update existing button
                TextMeshProUGUI buttonText = buttons[i].GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = newPlayerNames[i];
                }
                AssignButtonAction(buttons[i], newPlayerNames[i]);
            }
            else
            {
                //Create new button
                Button newButton = Instantiate(buttonPrefab, buttonParent);
                
                //Adjust position for better alignment
                Vector3 newPosition = newButton.transform.localPosition;
                newPosition.x += i * 0.8f; //Space out buttons horizontally
                newButton.transform.localPosition = newPosition;

                //Set button text
                TextMeshProUGUI buttonText = newButton.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = newPlayerNames[i];
                    buttonText.color = Color.white;
                }

                AssignButtonAction(newButton, newPlayerNames[i]);
                buttons.Add(newButton); //Add to button list
            }
        }
    }

    /// <summary>
    /// Assigns a specific action to a button (click behaviour).
    /// </summary>
    /// <param name="button">The button to assign behavior to.</param>
    /// <param name="playerName">The player name associated with the button.</param>
        void AssignButtonAction(Button button, string playerName)
    {
        // Remove previous listeners to avoid duplication
        button.onClick.RemoveAllListeners();

        // Add a listener to call ButtonClicked with the player's name
        button.onClick.AddListener(() => ButtonClicked(button, playerName));
    }

    /// <summary>
    /// Handles button click, updates the UI and used to notify HeatMapPlayerMovement script.
    /// </summary>
    /// <param name="clickedButton">The button that was clicked.</param>
    /// <param name="playerName">The player name associated with the clicked button.</param>
    void ButtonClicked(Button clickedButton, string playerName)
    {
        if (lastClickedButton != null)
        {
            ColorBlock colors = lastClickedButton.colors;
            colors.normalColor = Color.white; // Reset the color to the default one
            Debug.Log("Normal color: " + colors.normalColor);
            lastClickedButton.colors = colors;
        }

        // Set the clicked button to a selected color
        ColorBlock clickedButtonColors = clickedButton.colors;
        clickedButtonColors.normalColor = new Color(102/255f, 176/255f, 195/255f); //Here the selected button
        clickedButton.colors = clickedButtonColors;

        //Update the last clicked button reference
        lastClickedButton = clickedButton;

        //Notify the HeatMapPlayerMovement script
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

    /// <summary>
    /// Checks if two lists of player names are identical.
    /// </summary>
    /// <param name="oldList">The old list of player names.</param>
    /// <param name="newList">The new list of player names.</param>
    /// <returns>True if the lists are identical, false otherwise.</returns>
    bool ArePlayerListsEqual(List<string> oldList, List<string> newList)
    {
        if (oldList.Count != newList.Count) return false;

        for (int i = 0; i < oldList.Count; i++)
        {
            if (oldList[i] != newList[i]) return false;
        }

        return true;
    }

    /// <summary>
    /// Parses JSON data to extract a list of player names.
    /// </summary>
    /// <param name="jsonData">JSON data string.</param>
    /// <returns>List of player names.</returns>
    List<string> ParseGSI(string jsonData)
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
