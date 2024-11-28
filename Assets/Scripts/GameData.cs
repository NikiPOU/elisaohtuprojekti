using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using TMPro;
using Newtonsoft.Json.Linq;

public class Statistics : MonoBehaviour
{
    public TMP_Text text; // TextMeshPro component
    public GSIDataReceiver gsiDataReceiver; // GSIDataReceiver script component
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

        DataTable cTerroristTable = CreateTable("Counterterrorists");
        DataTable terroristTable = CreateTable("Terrorists");

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

            // Format the data
            //string handledData = $"{playerName} | " +
                                //$"Team: {team} | Health: {health} | " +
                                //$"Kills: {kills} | Assists: {assists} | Deaths: {deaths} \n";

           AddStatistics(name, team, health, kills, deaths, assists);
        }

        //string cTerrorists = string.Join("\n", CTerroristDetails);
        //string terrorists = string.Join("\n", terroristDetails);
        //return cTerrorists + "\n \n" + terrorists;
        string cTerroristsString = ConvertTableToString(cTerroristTable);
        string terroristString = ConvertTableToString(terroristTable);
        return cTerroristsString + "\n" + terroristString;
    }

    DataTable CreateTable(string tableName)
    {
        DataTable dataTable = new DataTable(tableName);

        dataTable.Columns.Add("Name", typeof(string));
        dataTable.Columns.Add("Team", typeof(string));
        dataTable.Columns.Add("Health", typeof(string));
        dataTable.Columns.Add("Kills", typeof(string));
        dataTable.Columns.Add("Assists", typeof(string));
        dataTable.Columns.Add("Deaths", typeof(string));

        return dataTable;
    }

    void AddStatistics(string name, string team, string health, string kills, string assists, string deaths)
    {
        if (team == "CT")
        {
            cTerroristTable.Rows.Add(name, team, health, kills, assists, deaths);
        }
        else
        {
            terroristTable.Rows.Add(name, team, health, kills, assists, deaths);
        }
        
    }

    string ConvertTableToString(DataTable dataTable)
        {
            string tableString = "";

            // Add column headers
            foreach (DataColumn column in dataTable.Columns)
            {
                tableString += column.ColumnName + "\t";
            }
            tableString += "\n";

            // Add rows
            foreach (DataRow row in dataTable.Rows)
            {
                foreach (var item in row.ItemArray)
                {
                    tableString += item.ToString() + "\t";
                }
                tableString += "\n";
            }

            return tableString;
        }

}
