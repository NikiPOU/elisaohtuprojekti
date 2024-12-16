using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using TMPro;
using Newtonsoft.Json.Linq;

/// <summary>
/// Handles the display and management of player statistics based on received GSI data.
/// </summary> public class Statistics : MonoBehaviour

public class Statistics : MonoBehaviour
{
    /// <summary> 
    /// The TextMeshPro component used to display statistics on the UI.
    /// </summary>
    public TMP_Text text;
    /// <summary> 
    /// The GSIDataReceiver component responsible for receiving GSI data updates. 
    /// </summary
    public GSIDataReceiver gsiDataReceiver;

    /// <summary>
    /// Table to store statistics for Counter-Terrorist players.
    /// </summary>
    private DataTable cTerroristTable;

    /// <summary>
    /// Table to store statistics for Terrorist players.
    /// </summary>
    private DataTable terroristTable;

    /// <summary>
    /// Initializes the script for testing purposes.
    /// </summary>
    public void Initialize()
    {
        if (text == null)
        {
            Debug.LogError("Ei toimi: Teksti-komponentti puuttuu.");
            return;
        }
        // Default placeholder text
        text.text = "Game statistics here";
    }

    /// <summary>
    /// Unity's Start method initializes tables and subscribes to GSI data updates.
    /// </summary>
    void Start()
    {
        // Ensure the TextMeshPro text component is assigned
        if (text == null)
        {
            Debug.LogError("Teksti komponentissa ongelma");
            return; //Exit if TMP text component is empty
        }
        // Find the GSIDataReceiver in the scene
        gsiDataReceiver = FindObjectOfType<GSIDataReceiver>();
        if (gsiDataReceiver == null)
        {
            Debug.LogError("Ei toimi: GSI datan haussa ongelma.");
            return; //Exit if GSI data receiver is nout found
        }

        // Create separate tables for both teams
        cTerroristTable = CreateTable("Counterterrorists");
        terroristTable = CreateTable("Terrorists");

       // Subscribe to the data update event
        gsiDataReceiver.OnStatisticsDataReceived += StatisticsUpdate;
    }


    /// <summary>
    /// Ensures that the event subscription is removed when the object is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        if (gsiDataReceiver != null)
        {
            gsiDataReceiver.OnStatisticsDataReceived -= StatisticsUpdate;
        }
    }


    /// <summary>
    /// Updates player statistics on receiving new GSI data.
    /// </summary>
    /// <param name="jsonData">JSON string containing updated statistics.</param>
    void StatisticsUpdate(string jsonData)
    {
        if (gsiDataReceiver != null && text != null)
        {
            // Clear previous UI content and data
            text.text = "";
            cTerroristTable.Clear();
            terroristTable.Clear();
            // Extract important data from GSI and update the display text
            string important = ParseGSI(gsiDataReceiver.statisticsData);
            text.text = important; // Update the UI with server data
        }
    }


    /// <summary>
    /// Parses the GSI JSON data and fills team statistics tables.
    /// </summary>
    /// <param name="jsonData">JSON data string.</param>
    /// <returns>Formatted string representation of player statistics.</returns>
    string ParseGSI(string jsonData)
    {
        JObject allPlayers = JObject.Parse(jsonData); // Parse JSON using Newtonsoft
        if (allPlayers == null)
        {
            return "No players found.";
        }

        List<string> terroristDetails = new List<string>();
        List<string> CTerroristDetails = new List<string>();

        // Iterate over the properties of allPlayers
        foreach (var property in ((JObject)allPlayers).Properties())
        {
            string steamId = property.Name; // Access the key (Steam ID)
            JArray playerData = (JArray)property.Value; // Access the value (player data array)

            // Extract details from the array
            string playerName = playerData[0]?.ToString() ?? "Unknown";
            string team = playerData[1]?.ToString() ?? "No Team";
            string health = playerData[2]?.ToString() ?? "0";
            string kills = playerData[3]?.ToString() ?? "0";
            string assists = playerData[4]?.ToString() ?? "0";
            string deaths = playerData[5]?.ToString() ?? "0";
            string kdr = playerData[6]?.ToString() ?? "0.00";

            // Add player data to the appropriate team's table
            AddStatistics(playerName, team, kills, deaths, assists, health, kdr);
        }

        // Convert the tables to strings
        string cTerroristsString = ConvertTableToString(cTerroristTable, true); // Include headers for CT
        string terroristString = ConvertTableToString(terroristTable, false); // Exclude headers for Terrorists
        return "\n" + cTerroristsString + "\nCounter-Terrorists\n" + "\nTerrorists\n\n" + terroristString;
    }


    /// <summary>
    /// Creates a DataTable with columns for player statistics.
    /// </summary>
    /// <param name="tableName">Name of the table.</param>
    /// <returns>A DataTable object initialized with the required columns.</returns>
    DataTable CreateTable(string tableName)
    {
        DataTable dataTable = new DataTable(tableName);

        // Add columns for each player statistic
        dataTable.Columns.Add("Name", typeof(string));
        dataTable.Columns.Add("Kills", typeof(string));
        dataTable.Columns.Add("Deaths", typeof(string));
        dataTable.Columns.Add("Assists", typeof(string));
        dataTable.Columns.Add("Health", typeof(string));
        dataTable.Columns.Add("KDR", typeof(string));

        return dataTable;
    }


    /// <summary>
    /// Adds a player's statistics to the appropriate team table.
    /// </summary>
    /// <param name="name">Player's name.</param>
    /// <param name="team">Player's team ("CT" or "Terrorist").</param>
    /// <param name="kills">Number of kills.</param>
    /// <param name="deaths">Number of deaths.</param>
    /// <param name="assists">Number of assists.</param>
    /// <param name="health">Current health.</param>
    /// <param name="kdr">Kill/Death Ratio.</param>
    void AddStatistics(string name, string team, string kills, string deaths, string assists, string health, string kdr)
    {
        if (team == "CT")
        {
            cTerroristTable.Rows.Add(name, kills, deaths, assists, health, kdr);
        }
        else
        {
            terroristTable.Rows.Add(name, kills, deaths, assists, health, kdr);
        }
        
    }


    /// <summary>
    /// Converts a DataTable into a string for display.
    /// </summary>
    /// <param name="dataTable">The DataTable to convert.</param>
    /// <param name="includeHeaders">Whether to include column headers in the output.</param>
    /// <returns> string representation of the DataTable.</returns>
    string ConvertTableToString(DataTable dataTable, bool includeHeaders)
    {
        const int columnWidth = 9; // Define a fixed width for all columns
        string tableString = "";

        if (includeHeaders)
        {
            tableString += " ";
            // Add column headers
            foreach (DataColumn column in dataTable.Columns)
            {
                tableString += (column.ColumnName == "KDR") ? " " : ""; 
                tableString += column.ColumnName.PadRight(columnWidth); // Pad column headers
            }
            tableString += "\n\n";
        }

        // Add rows
        foreach (DataRow row in dataTable.Rows)
        {
            foreach (var item in row.ItemArray)
            {
                string value = item.ToString();

                // Calculate padding to align the first character
                int leadingSpaces = columnWidth - value.Length; // Total space left after placing the value
                int firstCharPadding = (value.Length > 0 && char.IsDigit(value[0])) ? leadingSpaces / 2 : 0;

                // Add padding before and after the value
                string paddedValue = new string(' ', firstCharPadding) + value + new string(' ', leadingSpaces - firstCharPadding);

                tableString += paddedValue;
            }
            tableString += "\n";
        }

        return tableString;
    }


}
