using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Linq;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 0.2f; // Speed for movement
    public GameObject counterTerroristPrefab; // Prefab for Counter-Terrorists
    public GameObject terroristPrefab; // Prefab for Terrorists

    // Parent objects for the players under the map in the hierarchy
    public Transform counterTerroristsParent;
    public Transform terroristsParent;
    
    private GSIDataReceiver gsiDataReceiver;

    // Dictionary to store player objects based on their name
    private Dictionary<string, GameObject> playerGameObjects = new Dictionary<string, GameObject>();

    void Start()
    {
        gsiDataReceiver = FindObjectOfType<GSIDataReceiver>();

        if (gsiDataReceiver == null)
        {
            Debug.LogError("GSIDataReceiver not found in the scene.");
            return;
        }

        // Subscribe to the data update event
        gsiDataReceiver.OnDataReceived += UpdatePlayers;
    }

    private void OnDestroy()
    {
        // Unsubscribe from the data update event
        if (gsiDataReceiver != null)
        {
            gsiDataReceiver.OnDataReceived -= UpdatePlayers;
        }
    }

    void Update()
    {
        // Move players every frame
        MovePlayers();
    }

    // Updates player positions and creates/destroys objects if necessary
    void UpdatePlayers(string jsonData)
    {
        // Parse the received data
        JObject data = JObject.Parse(jsonData);
        var allPlayers = data["allplayers"];

        if (allPlayers == null)
        {
            return;
        }

        // Store the new player data in a temporary dictionary
        Dictionary<string, Vector3> newPlayerPositions = new Dictionary<string, Vector3>();
        Dictionary<string, string> newPlayerTeams = new Dictionary<string, string>();

        foreach (var player in allPlayers)
        {
            string playerName = player.First["name"]?.ToString();
            string team = player.First["team"]?.ToString();
            string positionString = player.First["position"]?.ToString();

            if (string.IsNullOrEmpty(playerName) || string.IsNullOrEmpty(team) || string.IsNullOrEmpty(positionString))
                continue;

            string[] coords = positionString.Split(", ");
            float x_coord = float.Parse(coords[0], System.Globalization.CultureInfo.InvariantCulture);
            float z_coord = float.Parse(coords[1], System.Globalization.CultureInfo.InvariantCulture);
            float y_coord = float.Parse(coords[2], System.Globalization.CultureInfo.InvariantCulture);

            // Apply scaling to the coordinates
            Vector3 position = new Vector3(0.0006f * x_coord - 0.02f, 0.501f, 0.0006f * z_coord - 0.65f);

            // Store player data
            newPlayerPositions[playerName] = position;
            newPlayerTeams[playerName] = team;
        }

        // Remove old players who are not in the new data
        var playersToRemove = playerGameObjects.Keys.Except(newPlayerPositions.Keys).ToList();
        foreach (string playerName in playersToRemove)
        {
            Destroy(playerGameObjects[playerName]);
            playerGameObjects.Remove(playerName);
        }

        // Create new players and update existing ones
        foreach (var kvp in newPlayerPositions)
        {
            string playerName = kvp.Key;
            Vector3 targetPosition = kvp.Value;
            string team = newPlayerTeams[playerName];

            // If player already exists, update position
            if (playerGameObjects.ContainsKey(playerName))
            {
                GameObject playerObject = playerGameObjects[playerName];
                playerObject.transform.position = targetPosition;
            }
            // If player doesn't exist, create a new object under the correct parent
            else
            {
                GameObject prefab = (team == "CT") ? counterTerroristPrefab : terroristPrefab;
                Transform parent = (team == "CT") ? counterTerroristsParent : terroristsParent;
                
                // Instantiate under the correct parent
                GameObject newPlayerObject = Instantiate(prefab, targetPosition, Quaternion.identity, parent);
                newPlayerObject.name = playerName;
                playerGameObjects[playerName] = newPlayerObject;
            }
        }
    }

    // Moves players to their target positions
    void MovePlayers()
    {
        foreach (var kvp in playerGameObjects)
        {
            GameObject playerObject = kvp.Value;
            string playerName = kvp.Key;

            // Get the target position from the current player data
            if (playerGameObjects.TryGetValue(playerName, out GameObject obj))
            {
                Vector3 targetPos = obj.transform.position;
                playerObject.transform.position = Vector3.MoveTowards(playerObject.transform.position, targetPos, speed * Time.deltaTime);
            }
        }
    }
}
