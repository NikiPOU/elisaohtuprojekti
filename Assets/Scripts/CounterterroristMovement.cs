using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Linq;

public class CounterterroristMovement : MonoBehaviour
{
    public float speed = 0.2f; // Speed for movement
    public GameObject counterTerroristPrefab; // Reference to the Counter-Terrorist prefab
    private GSIDataReceiver gsiDataReceiver;

    // Dictionary to hold the positions of the Counter-Terrorists
    private Dictionary<string, Vector3> cterroristsPositions = new Dictionary<string, Vector3>();
    private List<GameObject> cterroristsGameObjects = new List<GameObject>(); // To hold instantiated Counter-Terrorists

    void Start()
    {
        gsiDataReceiver = FindObjectOfType<GSIDataReceiver>();

        if (gsiDataReceiver == null)
        {
            Debug.LogError("GSIDataReceiver not found in the scene.");
            return;
        }

        gsiDataReceiver.OnDataReceived += UpdatePositions;

        // Spawn initial Counter-Terrorists
        SpawnCounterTerrorists();
    }

    private void OnDestroy()
    {
        if (gsiDataReceiver != null)
        {
            gsiDataReceiver.OnDataReceived -= UpdatePositions;
        }
    }

    void Update()
    {
        MovePlayers();
    }
    
    void SpawnCounterTerrorists()
    {
        // Instantiate the Counter-Terrorists based on the maximum number needed
        for (int i = 0; i < 15; i++) // Change the number based on the maximum number of Counter-Terrorists
        {
            GameObject newCT = Instantiate(counterTerroristPrefab, new Vector3(i * 2, 0, 0), Quaternion.identity);
            newCT.transform.parent = transform; // Make the Counter-Terrorist a child of this manager
            newCT.name = "CounterTerrorist" + (i + 1); // Naming for later reference
            cterroristsGameObjects.Add(newCT); // Add to the list
        }
    }

    void UpdatePositions(string jsonData)
    {
        cterroristsPositions.Clear();
        ParseData(jsonData);
    }

    void ParseData(string jsonData)
    {
        JObject data = JObject.Parse(jsonData);
        var allPlayers = data["allplayers"];
        
        if (allPlayers == null)
        {
            return;
        }

        foreach (var player in allPlayers)
        {
            string team = player.First["team"]?.ToString() ?? "No team assigned yet";

            if (team == "CT")
            {
                string playerName = player.First["name"]?.ToString();
                string stringPosition = player.First["position"]?.ToString();
                string[] coords = stringPosition.Split(", ");
                float x_coord = float.Parse(coords[0], System.Globalization.CultureInfo.InvariantCulture);
                float z_coord = float.Parse(coords[1], System.Globalization.CultureInfo.InvariantCulture);
                float y_coord = float.Parse(coords[2], System.Globalization.CultureInfo.InvariantCulture);
                Vector3 position = new Vector3(0.0005f * x_coord, 0.501f, 0.00065f * z_coord);
                cterroristsPositions[playerName] = position; // Add to the dictionary
            }
        }
    }

    void MovePlayers()
    {
        for (int i = 0; i < cterroristsGameObjects.Count; i++)
        {
            if (i < cterroristsPositions.Count)
            {
                string playerName = cterroristsPositions.ElementAt(i).Key; // Get player name
                if (cterroristsPositions.TryGetValue(playerName, out Vector3 targetPos))
                {
                    // Move the player towards the target position
                    GameObject cterrorist = cterroristsGameObjects[i];
                    cterrorist.transform.position = Vector3.MoveTowards(cterrorist.transform.position, targetPos, speed * Time.deltaTime);
                }
            }
        }
    }
}