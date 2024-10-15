using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Linq;

public class PlayerMovement : MonoBehaviour
{
    public GSIDataReceiver gsiDataReceiver;
    Vector3 position = new Vector3(0f, 0f, 0f);
    public Vector3 targetPosition = new Vector3(2.795f, 2f, 1f);
    public GameObject tracePrefab;
    public Transform parent;
    List<Vector3> positions = new List<Vector3>();
    Dictionary<string, int> coordinates = new Dictionary<string, int>();
    Dictionary<string, GameObject> gameObjects = new Dictionary<string, GameObject>();
    Dictionary<int, List<double>> colors = new Dictionary<int, List<double>>()
    {
        {5, new List<double> { 0, 255, 135, 0.8 }},
        {10, new List<double> { 120, 255, 0, 0.8 }},
        {15, new List<double> { 247, 255, 0, 0.8 }},
        {20, new List<double> { 255, 162, 0, 0.8 }},
        {25, new List<double> { 255, 98, 0, 0.8 }},
        {30, new List<double> { 161, 0, 0, 0.8 }}
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
        if (tracePrefab != null)
        {
            Vector3 currentPosition = targetPosition;
            Color traceColor;

            if (coordinates.ContainsKey(currentPosition.ToString()))
            {
                Debug.Log("Koordinaatti löytyy sanakirjasta");
                coordinates[currentPosition.ToString()]+=1;
                Debug.Log(coordinates);
                if (colors.ContainsKey(coordinates[currentPosition.ToString()]))
                {
                    GameObject currentGameObject = gameObjects[currentPosition.ToString()];
                    Renderer renderer = currentGameObject.GetComponent<Renderer>();
                    Material existingMaterial = renderer.material;
                    List<double> color = colors[coordinates[currentPosition.ToString()]];
                    Color newColor = new Color((float)(color[0]/255.0), (float)(color[1]/255.0), 
                    (float)(color[2]/255.0), (float)color[3]);
                    existingMaterial.color = newColor;
                }
            }
            else
            {
                Debug.Log("Koordinaatti ei löydy sanakirjasta");
                coordinates.Add(currentPosition.ToString(),1);
                GameObject newTrace = Instantiate(tracePrefab, currentPosition, Quaternion.identity);
                newTrace.transform.SetParent(parent);
                newTrace.transform.localPosition = currentPosition;
                newTrace.transform.localRotation = Quaternion.identity;
                gameObjects.Add(currentPosition.ToString(),newTrace);
            }
        }
    }
    public void UpdateTargetPosition(string jsonData)
    {
        if (gsiDataReceiver != null)
        {
            targetPosition = ParseGSI(gsiDataReceiver.gsiData);
        }
        LeaveTrace();
    }

    Vector3 ParseGSI(string jsonData)
    {
        List<Vector3> positions = new List<Vector3>();
        JObject data = JObject.Parse(jsonData); // Parse JSON using Newtonsoft
        var allPlayers = data["allplayers"];
        if (allPlayers == null)
        {
            return position;
        }

        foreach (var player in allPlayers)
        {
            string team = player.First["team"]?.ToString() ?? "No team assigned yet";
            if (team == "T")
            {
                string stringPosition = player.First["position"]?.ToString();
                string[] coords = stringPosition.Split(", ");
                float x_coord = float.Parse(coords[0], System.Globalization.CultureInfo.InvariantCulture);
                float y_coord = float.Parse(coords[1], System.Globalization.CultureInfo.InvariantCulture);
                float z_coord = float.Parse(coords[2], System.Globalization.CultureInfo.InvariantCulture);
                Vector3 position = new Vector3(0.00022f * x_coord + 0.2f, 0.00022f * y_coord - 0.2f, 0f);
                positions.Add(position);
            }
        }
        Debug.Log("Position" + position);
        position = positions[0];
        return position;
    }
}
// x: -2203.84, y: -1031.97, z: 128.23 vasenala
// x: -2093.97, y: 3117.97, z: 35.71 vasen ylä