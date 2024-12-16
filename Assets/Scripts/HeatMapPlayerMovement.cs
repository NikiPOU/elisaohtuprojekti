using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Linq;

/// <summary>
///Implementation of the heatmap.
/// </summary>
public class HeatMapPlayerMovement : MonoBehaviour
{
    //Receives data from GSI.
    public GSIDataReceiver gsiDataReceiver;

    //The name of the player being tracked.
    public string playerName = null;

    //The current position of the tracked player.
    public Vector3 position;

    //The prefab used to create traces on the heatmap.
    public GameObject tracePrefab;

    //Parent transform for all trace objects in the scene.
    public Transform parent;

    //Dictionary to track the number of visits to each coordinate.
    Dictionary<string, int> coordinates = new Dictionary<string, int>();

    //Dictionary to store the trace objects corresponding to each coordinate.
    Dictionary<string, GameObject> gameObjects = new Dictionary<string, GameObject>();

    //Color gradients based on visit frequency.
    Dictionary<int, List<double>> colors = new Dictionary<int, List<double>>()
    {
        {5, new List<double> { 173, 216, 230, 0.8 }}, 
        {10, new List<double> { 0, 255, 255, 0.8 }},
        {15, new List<double> { 144, 238, 144, 0.8 }},
        {20, new List<double> { 255, 255, 0, 0.8 }},
        {25, new List<double> { 255, 165, 0, 0.8 }},
        {30, new List<double> { 255, 0, 0, 0.8 }},
        {35, new List<double> { 139, 0, 0, 0.8 }}
    };


    /// <summary>
    /// Subscribes to GSI data updates at the start of the script.
    /// </summary>
    void Start()
    {
        gsiDataReceiver = FindObjectOfType<GSIDataReceiver>();
        if (gsiDataReceiver == null)
        {
            Debug.LogError("Ei toimi: GSI datan haussa ongelma.");
            return;
        }

        gsiDataReceiver.OnPositionsDataReceived += UpdateTargetPosition;
    }

    /// <summary>
    /// Unsubscribes from GSI data updates when the script is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        if (gsiDataReceiver != null)
        {
            gsiDataReceiver.OnPositionsDataReceived -= UpdateTargetPosition;
        }
    }

    /// <summary>
    /// Creates a trace at the current position or updates an existing one with the correct color.
    /// </summary>
    void LeaveTrace()
    {
        if (tracePrefab != null && position != Vector3.zero && playerName != null)
        {
            Vector3 currentPosition = position; // Use the updated 'position'
            Color traceColor;

            //Check if the position has already been visited
            if (coordinates.ContainsKey(currentPosition.ToString()))
            {
                coordinates[currentPosition.ToString()] += 1;
                Debug.Log(coordinates);

                //Update the color if it exists in the color gradient
                if (colors.ContainsKey(coordinates[currentPosition.ToString()]))
                {
                    GameObject currentGameObject = gameObjects[currentPosition.ToString()];
                    Renderer renderer = currentGameObject.GetComponent<Renderer>();
                    Material existingMaterial = renderer.material;
                    List<double> color = colors[coordinates[currentPosition.ToString()]];
                    Color newColor = new Color((float)(color[0] / 255.0), (float)(color[1] / 255.0),
                    (float)(color[2] / 255.0), (float)color[3]);
                    existingMaterial.color = newColor;
                }
            }
            else
            {
                //Create a new trace at the position
                coordinates.Add(currentPosition.ToString(), 1);
                GameObject newTrace = Instantiate(tracePrefab, currentPosition, Quaternion.identity);
                newTrace.transform.SetParent(parent);
                newTrace.transform.localPosition = currentPosition;
                newTrace.transform.localRotation = Quaternion.identity;
                gameObjects.Add(currentPosition.ToString(), newTrace);
            }
        }
    }

    /// <summary>
    /// Updates the player's position based on the JSON GSI data received.
    /// </summary>
    /// <param name="jsonData">The JSON string containing player position data.</param>
    public void UpdateTargetPosition(string jsonData)
    {
        if (gsiDataReceiver != null)
        {
            // Update the target position based on the parsed GSI data
            position = ParseGSI(gsiDataReceiver.positionsData);
            Debug.Log("Updated player position: " + position);
        }
        LeaveTrace(); //Leave trace after updating the position
    }


    /// <summary>
    /// Parses the JSON data to extract the player's position.
    /// </summary>
    /// <param name="jsonData">The JSON string containing player data.</param>
    /// <returns>The player's position as a Vector3.</returns>
    Vector3 ParseGSI(string jsonData)
    {
        JObject allPlayers = JObject.Parse(jsonData); // Parse JSON using Newtonsoft
        if (allPlayers == null || playerName == null)
        {
            Debug.Log("Playernames null");
            return position;
        }

        foreach (var property in ((JObject)allPlayers).Properties())
        {
            JArray playerData = (JArray)property.Value;
            string name = playerData[1]?.ToString();
            if (name == playerName)
            {
                string stringPosition = playerData[0]?.ToString();
                Debug.Log("Pos: " + stringPosition);
                if (!string.IsNullOrEmpty(stringPosition))
                {
                    string[] coords = stringPosition.Split(", ");
                    if (coords.Length == 3)
                    {
                        float x_coord = float.Parse(coords[0], System.Globalization.CultureInfo.InvariantCulture);
                        float y_coord = float.Parse(coords[1], System.Globalization.CultureInfo.InvariantCulture);
                        float z_coord = float.Parse(coords[2], System.Globalization.CultureInfo.InvariantCulture);

                        // Transform coordinates into Unity 
                        position = new Vector3(0.000215f * x_coord + 0.052f, 0.000217f * y_coord - 0.21f, 0f);
                        //Vector3 position = new Vector3(0.0006f * x_coord - 0.02f, 0.501f, 0.0006f * z_coord - 0.65f);
                        return position; // Return the updated position
                    }
                }
            }
        }
        return position;
    }

    /// <summary>
    /// Handles player selection from button in UI.
    /// </summary>
    /// <param name="player">The selected player's name.</param>
    public void ButtonClicked(string player)
    {
        Debug.Log("Button clicked for player: " + player);
        playerName = player;
    }
}
