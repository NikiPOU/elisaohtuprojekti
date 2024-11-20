using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Linq;
using System;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 0.2f;
    public GameObject counterTerroristPrefab;
    public GameObject terroristPrefab;
    public Transform parent;
    public Color damageFlashColor = Color.red;
    public float flashDuration = 0.2f;
    [NonSerialized] public Transform counterTerroristsParent;
    [NonSerialized] public Transform terroristsParent;
    private GSIDataReceiver gsiDataReceiver;
    private Dictionary<string, GameObject> playerGameObjects = new Dictionary<string, GameObject>();
    private Dictionary<string, int> previousPlayerHealth = new Dictionary<string, int>();
    private Dictionary<string, bool> playerAliveState = new Dictionary<string, bool>();

    void Start()
    {
        gsiDataReceiver = FindObjectOfType<GSIDataReceiver>();

        if (gsiDataReceiver == null)
        {
            Debug.LogError("GSIDataReceiver not found in the scene.");
            return;
        }

        gsiDataReceiver.OnDataReceived += UpdatePlayers;
    }

    private void OnDestroy()
    {
        if (gsiDataReceiver != null)
        {
            gsiDataReceiver.OnDataReceived -= UpdatePlayers;
        }
    }

    void Update()
    {
        MovePlayers();
    }

    void UpdatePlayers(string jsonData)
    {
        JObject data = JObject.Parse(jsonData);
        var allPlayers = data["allplayers"];

        if (allPlayers == null)
        {
            return;
        }

        // Store the new player data
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
            Vector3 position = new Vector3(0.00063f * x_coord, 0.00063f * z_coord - 0.7f, -0.05f) + parent.position;

            // Store player data
            newPlayerPositions[playerName] = position;
            newPlayerTeams[playerName] = team;

            // Check for health
            if (previousPlayerHealth.ContainsKey(playerName))
            {
                int previousHealth = previousPlayerHealth[playerName];
                if (currentHealth < previousHealth)
                {
                    // Player damaged
                    if (playerGameObjects.ContainsKey(playerName))
                    {
                        GameObject playerObject = playerGameObjects[playerName];
                        Debug.Log("Player hit: " + playerName);
                        StartCoroutine(FlashColor(playerObject));
                    }
                }
            }

            previousPlayerHealth[playerName] = currentHealth;

            // Check if the player is dead
            if (currentHealth <= 0)
            {
                // If player died
                playerAliveState[playerName] = false;

                if (playerGameObjects.ContainsKey(playerName))
                {
                    GameObject playerObject = playerGameObjects[playerName];
                    playerObject.SetActive(false); // Hide the player object
                }
            }
            else
            {
                playerAliveState[playerName] = true;
            }
        }

        // Remove old players who are not in the new data
        var playersToRemove = playerGameObjects.Keys.Except(newPlayerPositions.Keys).ToList();
        foreach (string playerName in playersToRemove)
        {
            Destroy(playerGameObjects[playerName]);
            playerGameObjects.Remove(playerName);
            previousPlayerHealth.Remove(playerName);
            playerAliveState.Remove(playerName);
        }

        // Create new players and update existing ones
        foreach (var kvp in newPlayerPositions)
        {
            string playerName = kvp.Key;
            Vector3 targetPosition = kvp.Value;
            string team = newPlayerTeams[playerName];

            if (playerGameObjects.ContainsKey(playerName))
            {
                // If player already exists, update position
                GameObject playerObject = playerGameObjects[playerName];
                playerObject.transform.position = targetPosition;

                // If player is alive set visibility
                if (playerAliveState[playerName])
                {
                    playerObject.SetActive(true);
                }
            }
            else
            {
                // If player doesn't exist -> make new player object
                GameObject prefab = (team == "CT") ? counterTerroristPrefab : terroristPrefab;
                //Transform parent = (team == "CT") ? counterTerroristsParent : terroristsParent;

                GameObject newPlayerObject = Instantiate(prefab, parent, false);
                newPlayerObject.transform.position = targetPosition;
                newPlayerObject.name = playerName;
                playerGameObjects[playerName] = newPlayerObject;

                previousPlayerHealth[playerName] = 100;
                playerAliveState[playerName] = true;
            }
        }
    }

    void MovePlayers()
    {
        foreach (var kvp in playerGameObjects)
        {
            GameObject playerObject = kvp.Value;
            string playerName = kvp.Key;

            if (playerGameObjects.TryGetValue(playerName, out GameObject obj))
            {
                Vector3 targetPos = obj.transform.position;
                playerObject.transform.position = Vector3.MoveTowards(playerObject.transform.position, targetPos, speed * Time.deltaTime);
            }
        }
    }

    private System.Collections.IEnumerator FlashColor(GameObject playerObject)
    {
        // Player object flashes red when damage taken
        Renderer renderer = playerObject.GetComponent<Renderer>();

        if (renderer == null)
            yield break;

        Color originalColor = renderer.material.color;
        renderer.material.color = damageFlashColor;
        yield return new WaitForSeconds(flashDuration);
        renderer.material.color = originalColor;
    }
}