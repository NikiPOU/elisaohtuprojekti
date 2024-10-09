using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Linq;

public class CounterterroristMovement : MonoBehaviour
{
    public float speed = 0.2f;
    Vector3 targetPosition = new Vector3(-0.8f, 0.501f, 0.2f);

    GSIDataReceiver gsiDataReceiver;

    Dictionary<string, Vector3> cterrorists = new Dictionary<string, Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        gsiDataReceiver = FindObjectOfType<GSIDataReceiver>();

        if (gsiDataReceiver == null)
        {
            return;
        }

        gsiDataReceiver.OnDataReceived += UpdatePositions;

    }

    private void OnDestroy()
    {
        if (gsiDataReceiver != null)
        {
            gsiDataReceiver.OnDataReceived -= UpdatePositions;
        }
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayers();
    }
    
    void UpdatePositions(string jsonData)
    {
        if (gsiDataReceiver != null)
        {
            cterrorists.Clear();
            ParseData(jsonData);
        }
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
                float y_coord = float.Parse(coords[1], System.Globalization.CultureInfo.InvariantCulture);
                float z_coord = float.Parse(coords[2], System.Globalization.CultureInfo.InvariantCulture);
                Vector3 position = new Vector3(0.001f * x_coord, 0.501f, 1 + 0.001f * z_coord);
                cterrorists.Add(playerName, position);
            }
            
        }
    }

    public void MovePlayers()
    {
        if (gsiDataReceiver != null)
        {
            foreach (int index in Enumerable.Range(0, 4))
            {   
                int tagIndex = index + 1;
                GameObject cterrorist = GameObject.FindWithTag("CTerrorist" + tagIndex);
                if (cterrorists.Count() != 0)
                {    
                    Vector3 targetPos = cterrorists.ElementAt(index).Value;
                    cterrorist.transform.position = Vector3.MoveTowards(cterrorist.transform.position, targetPos, speed * Time.deltaTime);
                }
        
            }
        }    
    }
    
}