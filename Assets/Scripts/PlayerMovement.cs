using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Linq;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 0.2f; // Speed for movement
    public GameObject counterTerroristPrefab; // Prefab for Counter-Terrorists
    public GameObject terroristPrefab; // Prefab for Terrorists
    public Color damageFlashColor = Color.red; // Color to flash when the player takes damage
    public float flashDuration = 0.2f; // Duration for color flash

    // Parent objects for the players under the map in the hierarchy
    public Transform counterTerroristsParent;
    public Transform terroristsParent;

    private GSIDataReceiver gsiDataReceiver;

    // Dictionary to store player objects based on their name
    private Dictionary<string, GameObject> playerGameObjects = new Dictionary<string, GameObject>();
    // Dictionary to track each player's previous health
    private Dictionary<string, int> previousPlayerHealth = new Dictionary<string, int>();
    // Dictionary to track player alive state
    private Dictionary<string, bool> playerAliveState = new Dictionary<string, bool>();

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
            int currentHealth = player.First["state"]?["health"]?.ToObject<int>() ?? 100; // Get current health

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

            // Check for health drop
            if (previousPlayerHealth.ContainsKey(playerName))
            {
                int previousHealth = previousPlayerHealth[playerName];
                if (currentHealth < previousHealth)
                {
                    // The player has taken damage, flash their color to red
                    if (playerGameObjects.ContainsKey(playerName))
                    {
                        GameObject playerObject = playerGameObjects[playerName];
                        Debug.Log("Player hit: " + playerName);
                        StartCoroutine(FlashColor(playerObject));
                    }
                }
            }

            // Update the previous health value
            previousPlayerHealth[playerName] = currentHealth; // Correctly updating health here

            // Check if the player is dead
            if (currentHealth <= 0)
            {
                // Set player alive state to false and hide the object
                playerAliveState[playerName] = false;

                if (playerGameObjects.ContainsKey(playerName))
                {
                    GameObject playerObject = playerGameObjects[playerName];
                    playerObject.SetActive(false); // Hide the player object
                }
            }
            else
            {
                // Set player alive state to true
                playerAliveState[playerName] = true;
            }
        }

        // Remove old players who are not in the new data
        var playersToRemove = playerGameObjects.Keys.Except(newPlayerPositions.Keys).ToList();
        foreach (string playerName in playersToRemove)
        {
            Destroy(playerGameObjects[playerName]);
            playerGameObjects.Remove(playerName);
            previousPlayerHealth.Remove(playerName); // Remove health tracking for the removed player
            playerAliveState.Remove(playerName); // Remove alive state tracking
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

                // Check if the player is alive to set the visibility
                if (playerAliveState[playerName])
                {
                    playerObject.SetActive(true); // Show the player object if alive
                }
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

                // Initialize the health tracking for the new player
                previousPlayerHealth[playerName] = 100; // Correctly initializing health for new players
                playerAliveState[playerName] = true; // Initialize alive state
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

    // Coroutine to flash the player's color when they take damage
    private System.Collections.IEnumerator FlashColor(GameObject playerObject)
    {
        Renderer renderer = playerObject.GetComponent<Renderer>();

        if (renderer == null)
            yield break;

        // Store the original color
        Color originalColor = renderer.material.color;

        // Change to the damage color (red)
        renderer.material.color = damageFlashColor;

        // Wait for the duration of the flash
        yield return new WaitForSeconds(flashDuration);

        // Revert back to the original color
        renderer.material.color = originalColor;
    }
}


