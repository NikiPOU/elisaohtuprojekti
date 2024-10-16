using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Linq;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 0.2f; // Speed for movement
    public GameObject counterTerroristPrefab; // Reference to the Counter-Terrorist prefab
    public GameObject terroristPrefab;
    private GSIDataReceiver gsiDataReceiver;

    // Dictionaries to hold the positions of the Counter-Terrorists and Terrorists
    private Dictionary<string, Vector3> cterroristsPositions = new Dictionary<string, Vector3>();
    private Dictionary<string, Vector3> terroristsPositions = new Dictionary<string, Vector3>();
    private List<GameObject> cterroristsGameObjects = new List<GameObject>(); // To hold instantiated Counter-Terrorists
    private List<GameObject> terroristsGameObjects = new List<GameObject>();

    void Start()
    {
        gsiDataReceiver = FindObjectOfType<GSIDataReceiver>();

        if (gsiDataReceiver == null)
        {
            Debug.LogError("GSIDataReceiver not found in the scene.");
            return;
        }

        gsiDataReceiver.OnDataReceived += UpdatePositions;

        // Spawn initial players
        SpawnPlayers();
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
    
    void SpawnPlayers()
    {
        // Instantiate the Counter-Terrorists based on the maximum number needed
        for (int i = 0; i < 5; i++) // Change the number based on the maximum number of Counter-Terrorists
        {
            GameObject newCT = Instantiate(counterTerroristPrefab, new Vector3(i * 0.05f - 0.15f, 0.501f, 0.3f), Quaternion.identity);
            newCT.transform.parent = transform; // Make the Counter-Terrorist a child of this manager
            newCT.name = "CounterTerrorist" + (i + 1); // Naming for later reference
            cterroristsGameObjects.Add(newCT); // Add to the list
        }
        // Instantiate terrorists
        for (int i = 0; i < 5; i++) 
        {
            GameObject newT = Instantiate(terroristPrefab, new Vector3(i * 0.05f - 0.6f, 0.501f, -1.2f), Quaternion.identity);
            newT.transform.parent = transform; // Make the Counter-Terrorist a child of this manager
            newT.name = "Terrorist" + (i + 1); // Naming for later reference
            terroristsGameObjects.Add(newT); // Add to the list
        }
    }     

    void UpdatePositions(string jsonData)
    {
        cterroristsPositions.Clear();
        terroristsPositions.Clear();
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
            string playerName = player.First["name"]?.ToString();
            string team = player.First["team"]?.ToString() ?? "No team assigned yet";
            string stringPosition = player.First["position"]?.ToString();
            string[] coords = stringPosition.Split(", ");
            float x_coord = float.Parse(coords[0], System.Globalization.CultureInfo.InvariantCulture);
            float z_coord = float.Parse(coords[1], System.Globalization.CultureInfo.InvariantCulture);
            float y_coord = float.Parse(coords[2], System.Globalization.CultureInfo.InvariantCulture);
            Vector3 position = new Vector3(0.0005f * x_coord, 0.501f, 0.00065f * z_coord);
            if (team == "CT")
            {
                cterroristsPositions[playerName] = position; // Add to the dictionary
            }

            else if (team == "T")
            {
                terroristsPositions[playerName] = position;
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

        for (int i = 0; i < terroristsGameObjects.Count; i++)
        {
            if (i < terroristsPositions.Count)
            {
                string playerName = terroristsPositions.ElementAt(i).Key; // Get player name
                if (terroristsPositions.TryGetValue(playerName, out Vector3 targetPos))
                {
                    // Move the player towards the target position
                    GameObject terrorist = terroristsGameObjects[i];
                    terrorist.transform.position = Vector3.MoveTowards(terrorist.transform.position, targetPos, speed * Time.deltaTime);
                }
            }
        }
    }
}