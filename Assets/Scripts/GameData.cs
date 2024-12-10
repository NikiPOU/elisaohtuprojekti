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
    private DataTable cTerroristTable;
    private DataTable terroristTable;

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

        cTerroristTable = CreateTable("Counterterrorists");
        terroristTable = CreateTable("Terrorists");

        gsiDataReceiver.OnStatisticsDataReceived += StatisticsUpdate;
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
        if (gsiDataReceiver != null && text != null)
        {
            text.text = "";
            cTerroristTable.Clear();
            terroristTable.Clear();
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

            AddStatistics(playerName, team, kills, deaths, assists, health, kdr);
        }

        string cTerroristsString = ConvertTableToString(cTerroristTable, true); // Include headers for CT
        string terroristString = ConvertTableToString(terroristTable, false); // Exclude headers for Terrorists
        return "\n" + cTerroristsString + "\nCounter-Terrorists\n" + "\nTerrorists\n\n" + terroristString;
    }


    DataTable CreateTable(string tableName)
    {
        DataTable dataTable = new DataTable(tableName);

        dataTable.Columns.Add("Name", typeof(string));
        dataTable.Columns.Add("Kills", typeof(string));
        dataTable.Columns.Add("Deaths", typeof(string));
        dataTable.Columns.Add("Assists", typeof(string));
        dataTable.Columns.Add("Health", typeof(string));
        dataTable.Columns.Add("KDR", typeof(string));

        return dataTable;
    }


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
