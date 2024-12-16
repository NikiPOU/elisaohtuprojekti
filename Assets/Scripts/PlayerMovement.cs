using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Linq;
using System;

/// <summary>
/// Manages player objects, their positions, rotations, health, and visual effects.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    //Prefab for counter-Terrorist players.
    public GameObject counterTerroristPrefab;

    //Prefab for Terrorist players.
    public GameObject terroristPrefab;

    //Parent transform for organizing player objects.
    public Transform parent;

    //Duration for smooth position and rotation transformations.
    public float lerpDuration = 0.3f;

    //Color used for flash when a player takes damage.
    public Color damageFlashColor = Color.red;

    //Duration of the damage flash.
    public float flashDuration = 0.2f;

    //Internal state for organizing player objects and data.
    [NonSerialized] public Transform counterTerroristsParent;
    [NonSerialized] public Transform terroristsParent;
    private GSIDataReceiver gsiDataReceiver;
    private Dictionary<string, GameObject> playerGameObjects = new Dictionary<string, GameObject>();
    private Dictionary<string, int> previousPlayerHealth = new Dictionary<string, int>();
    private Dictionary<string, bool> playerAliveState = new Dictionary<string, bool>();
    private Dictionary<string, Coroutine> playerMoveCoroutines = new Dictionary<string, Coroutine>();
    private Dictionary<string, Vector3> newPlayerForward = new Dictionary<string, Vector3>();
    private Dictionary<string, Coroutine> playerFlashCoroutines = new Dictionary<string, Coroutine>();
    private Dictionary<string, Color> originalPlayerColors = new Dictionary<string, Color>();
    
    /// <summary>
    /// Initialize player positions and subscribe to GSI data updates.
    /// </summary>
    void Start()
    {
        gsiDataReceiver = FindObjectOfType<GSIDataReceiver>();

        if (gsiDataReceiver == null)
        {
            Debug.LogError("GSIDataReceiver not found in the scene.");
            return;
        }
        
        gsiDataReceiver.OnPositionsDataReceived += UpdatePlayers;
    }


    /// <summary>
    /// Unsubscribe from GSI updates when the object is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        if (gsiDataReceiver != null)
        {
            gsiDataReceiver.OnPositionsDataReceived -= UpdatePlayers;
        }
    }

    /// <summary>
    /// Processes player GSI data and updates their positions, rotations, and states.
    /// </summary>
    /// <param name="jsonData">JSON string containing player data.</param>
    void UpdatePlayers(string jsonData)
    {
        JObject allPlayers = JObject.Parse(jsonData);

        if (allPlayers == null)
        {
            return;
        }

        //Store the new player data in dictionairies.
        Dictionary<string, Vector3> newPlayerPositions = new Dictionary<string, Vector3>();
        Dictionary<string, string> newPlayerTeams = new Dictionary<string, string>();
        Dictionary<string, Quaternion> newPlayerRotations = new Dictionary<string, Quaternion>();
        Dictionary<string, Vector3> newPlayerScales = new Dictionary<string, Vector3>();

        //Parse player data.
        foreach (var property in ((JObject)allPlayers).Properties())
        {
            JArray playerData = (JArray)property.Value;

            string positionString = playerData[0]?.ToString();
            string playerName = playerData[1]?.ToString();
            string team = playerData[2]?.ToString();
            int currentHealth = playerData[3]?.ToObject<int>() ?? 100;
            string forwardString = playerData[4]?.ToString();

            if (string.IsNullOrEmpty(playerName) || string.IsNullOrEmpty(team) || string.IsNullOrEmpty(positionString) || string.IsNullOrEmpty(forwardString))
                continue;

            //Parse position.
            string[] coords = positionString.Split(", ");
            float x_coord = float.Parse(coords[0], System.Globalization.CultureInfo.InvariantCulture);
            float z_coord = float.Parse(coords[1], System.Globalization.CultureInfo.InvariantCulture);
            float y_coord = float.Parse(coords[2], System.Globalization.CultureInfo.InvariantCulture);
            Vector3 position = new Vector3(0.00059f * x_coord + 0.1f, 0.00059f * z_coord - 0.6f, -0.05f) + parent.position;

            //Apply map rotation and scale to the position.
            Vector3 initialScale = new Vector3(2.68f, 2.68f, 1f);
            Vector3 scaleFactor = new Vector3(
                parent.localScale.x / initialScale.x,
                parent.localScale.y / initialScale.y,
                parent.localScale.z / initialScale.z
            );
            position = parent.rotation * Vector3.Scale(position - parent.position, scaleFactor) + parent.position;

            //Parse forward vector.
            string[] forwardCoords = forwardString.Split(", ");
            float fx = float.Parse(forwardCoords[0], System.Globalization.CultureInfo.InvariantCulture);
            float fz = float.Parse(forwardCoords[1], System.Globalization.CultureInfo.InvariantCulture);
            float fy = float.Parse(forwardCoords[2], System.Globalization.CultureInfo.InvariantCulture);
            Vector3 forward = new Vector3(fx, fz, fy);

            //Apply map rotation to the forward vector.
            forward = parent.rotation * forward;


            //Store player data.
            newPlayerPositions[playerName] = position;
            newPlayerTeams[playerName] = team;
            newPlayerForward[playerName] = forward;

            //Calculate target rotation.
            Quaternion targetRotation = Quaternion.LookRotation(forward, Vector3.up);
            targetRotation = Quaternion.Euler(targetRotation.eulerAngles.x, targetRotation.eulerAngles.y, 0f);
            newPlayerRotations[playerName] = targetRotation;

            //Check for health.
            if (previousPlayerHealth.ContainsKey(playerName))
            {
                int previousHealth = previousPlayerHealth[playerName];
                if (currentHealth < previousHealth)
                {
                    //Player damage detection and effects handling.
                    if (playerGameObjects.ContainsKey(playerName))
                    {
                        GameObject playerObject = playerGameObjects[playerName];
                        Debug.Log("Player hit: " + playerName);
                        
                        if (playerFlashCoroutines.ContainsKey(playerName))
                        {
                            StopCoroutine(playerFlashCoroutines[playerName]);
                        }
                        playerFlashCoroutines[playerName] = StartCoroutine(FlashColor(playerObject));
                    }
                }
            }

            previousPlayerHealth[playerName] = currentHealth;

            //Check if the player is dead.
            if (currentHealth <= 0)
            {
                playerAliveState[playerName] = false;

                if (playerGameObjects.ContainsKey(playerName))
                {
                    GameObject playerObject = playerGameObjects[playerName];
                    playerObject.SetActive(false);
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
            if (playerMoveCoroutines.ContainsKey(playerName))
            {
                StopCoroutine(playerMoveCoroutines[playerName]);
                playerMoveCoroutines.Remove(playerName);
            }

            Destroy(playerGameObjects[playerName]);
            playerGameObjects.Remove(playerName);
            previousPlayerHealth.Remove(playerName);
            playerAliveState.Remove(playerName);
            originalPlayerColors.Remove(playerName);
        }

        //Create new players and update existing ones.
        foreach (var kvp in newPlayerPositions)
        {
            string playerName = kvp.Key;
            Vector3 targetPosition = kvp.Value;
            string team = newPlayerTeams[playerName];
            Quaternion targetRotation = newPlayerRotations[playerName];

            if (playerGameObjects.ContainsKey(playerName))
            {
                GameObject playerObject = playerGameObjects[playerName];

                //If player is alive, set visibility.
                if (playerAliveState[playerName])
                {
                    playerObject.SetActive(true);
                }

                //Start or restart the movement coroutine for this player.
                if (playerMoveCoroutines.ContainsKey(playerName))
                {
                    StopCoroutine(playerMoveCoroutines[playerName]);
                }
                playerMoveCoroutines[playerName] = StartCoroutine(SmoothMove(playerObject, targetPosition, targetRotation));

                if (!playerFlashCoroutines.ContainsKey(playerName) && originalPlayerColors.ContainsKey(playerName))
                {
                    Renderer renderer = playerObject.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        renderer.material.color = originalPlayerColors[playerName];
                    }
                }
            }
            else
            {
                //If player doesn't exist -> make new player object.
                GameObject prefab = (team == "CT") ? counterTerroristPrefab : terroristPrefab;

                GameObject newPlayerObject = Instantiate(prefab, parent, false);
                newPlayerObject.transform.position = targetPosition;
                newPlayerObject.name = playerName;
                playerGameObjects[playerName] = newPlayerObject;

                previousPlayerHealth[playerName] = 100;
                playerAliveState[playerName] = true;

                Renderer renderer = newPlayerObject.GetComponent<Renderer>();
                if (renderer != null)
                {
                    originalPlayerColors[playerName] = renderer.material.color;
                }

                //Start the movement coroutine for this new player.
                playerMoveCoroutines[playerName] = StartCoroutine(SmoothMove(newPlayerObject, targetPosition, targetRotation));
            }
        }

        //Apply the new scales to the player objects.
        foreach (var kvp in newPlayerScales)
        {
            string playerName = kvp.Key;
            Vector3 targetScale = kvp.Value;

            if (playerGameObjects.ContainsKey(playerName))
            {
                GameObject playerObject = playerGameObjects[playerName];
                playerObject.transform.localScale = targetScale;
            }
        }
    }


    /// <summary>
    /// Smoothly moves a player object to a new position and rotation.
    /// </summary>
    private System.Collections.IEnumerator SmoothMove(GameObject playerObject, Vector3 targetPosition, Quaternion targetRotation)
    {
        float time = 0f;
        Vector3 startPosition = playerObject.transform.position;
        Quaternion startRotation = playerObject.transform.rotation;

        while (time < lerpDuration)
        {
            //Lerp from the start position to the target position over time.
            playerObject.transform.position = Vector3.Lerp(startPosition, targetPosition, time / lerpDuration);
            playerObject.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, time / lerpDuration);
            time += Time.deltaTime;
            yield return null;
        }

        //Ensure the final position and rotation are set.
        playerObject.transform.position = targetPosition;
        playerObject.transform.rotation = targetRotation;
    }

    /// <summary>
    /// Temporarily changes a player's color when they take damage.
    /// </summary>
    private System.Collections.IEnumerator FlashColor(GameObject playerObject)
    {
        Renderer renderer = playerObject.GetComponent<Renderer>();

        if (renderer == null)
            yield break;

        Color originalColor = originalPlayerColors[playerObject.name];
        renderer.material.color = damageFlashColor;
        yield return new WaitForSeconds(flashDuration);
        renderer.material.color = originalColor;
    }
}
