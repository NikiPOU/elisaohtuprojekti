using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Linq;

public class HeatMapPlayerMovement : MonoBehaviour
{
    public GSIDataReceiver gsiDataReceiver;
    public string playerName = null;
    public Vector3 position;
    public GameObject tracePrefab;
    public Transform parent;
    Dictionary<string, int> coordinates = new Dictionary<string, int>();
    Dictionary<string, GameObject> gameObjects = new Dictionary<string, GameObject>();
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


    void Start()
    {
        gsiDataReceiver = FindObjectOfType<GSIDataReceiver>();
        if (gsiDataReceiver == null)
        {
            Debug.LogError("Ei toimi: GSI datan haussa ongelma.");
            return;
        }

        gsiDataReceiver.OnDataReceived += UpdateTargetPosition;
    }

    private void OnDestroy()
    {
        if (gsiDataReceiver != null)
        {
            gsiDataReceiver.OnDataReceived -= UpdateTargetPosition;
        }
    }

    void LeaveTrace()
    {
        if (tracePrefab != null && position != Vector3.zero && playerName != null)
        {
            Vector3 currentPosition = position; // Use the updated 'position'
            Color traceColor;

            if (coordinates.ContainsKey(currentPosition.ToString()))
            {
                coordinates[currentPosition.ToString()] += 1;
                Debug.Log(coordinates);
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
                coordinates.Add(currentPosition.ToString(), 1);
                GameObject newTrace = Instantiate(tracePrefab, currentPosition, Quaternion.identity);
                newTrace.transform.SetParent(parent);
                newTrace.transform.localPosition = currentPosition;
                newTrace.transform.localRotation = Quaternion.identity;
                gameObjects.Add(currentPosition.ToString(), newTrace);
            }
        }
    }

    public void UpdateTargetPosition(string jsonData)
    {
        if (gsiDataReceiver != null)
        {
            // Update the target position based on the parsed GSI data
            position = ParseGSI(gsiDataReceiver.gsiData);
            Debug.Log("Updated player position: " + position);
        }
        LeaveTrace(); // Leave trace after updating the position
    }

    Vector3 ParseGSI(string jsonData)
    {
        JObject data = JObject.Parse(jsonData); // Parse JSON using Newtonsoft
        var allPlayers = data["allplayers"];
        if (allPlayers == null || playerName == null)
        {
            Debug.Log("playername null");
            return position;
        }

        foreach (var player in allPlayers)
        {
            string name = player.First["name"]?.ToString();
            if (name == playerName)
            {
                string stringPosition = player.First["position"]?.ToString();
                if (!string.IsNullOrEmpty(stringPosition))
                {
                    string[] coords = stringPosition.Split(", ");
                    if (coords.Length == 3)
                    {
                        float x_coord = float.Parse(coords[0], System.Globalization.CultureInfo.InvariantCulture);
                        float y_coord = float.Parse(coords[1], System.Globalization.CultureInfo.InvariantCulture);
                        float z_coord = float.Parse(coords[2], System.Globalization.CultureInfo.InvariantCulture);

                        // Update the global position variable
                        position = new Vector3(0.00022f * x_coord + 0.2f, 0.00022f * y_coord - 0.2f, 0f);
                        return position; // Return the updated position
                    }
                }
            }
        }
        return position;
    }

    public void ButtonClicked(string player)
    {
        Debug.Log("Button clicked for player: " + player);
        playerName = player;
    }
}
