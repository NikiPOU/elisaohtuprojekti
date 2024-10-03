using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class TerroristMovement : MonoBehaviour
{
    public float speed = 0.2f;
    Vector3 targetPosition = new Vector3(-0.8f, 0.501f, 0.2f);
    GSIDataReceiver gsiDataReceiver;

    // Start is called before the first frame update
    void Start()
    {
        gsiDataReceiver = FindObjectOfType<GSIDataReceiver>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.frameCount % 30 == 0)
        {
            UpdateTargetPosition();
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        }

        else
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        }

    }

    
    public double GetRandomCoordinate(char coordinate)
    {
        if (coordinate == 'x')
        { 
            double minimum = -0.8d;
            double maximum = 0.8d;
            System.Random random = new System.Random();
            return random.NextDouble() * (maximum - minimum) + minimum;
        }

        else
        {
            double minimum = 0.2d;
            double maximum = 1.9d;
            System.Random random = new System.Random();
            return random.NextDouble() * (maximum - minimum) + minimum;

        }

    }
    public void UpdateTargetPosition()
    {
        float x_coord = (float) GetRandomCoordinate('x');
        float z_coord = (float) GetRandomCoordinate('z');
        targetPosition = new Vector3(x_coord, 0.501f, z_coord);
    }
    
    Dictionary<string, Vector3> ParseData(string jsonData)
    {
        JObject data = JObject.Parse(jsonData);
        var allPlayers = data["allPlayers"];

        Dictionary<string, Vector3> terrorists = new Dictionary<string, Vector3>();

        foreach (var player in allPlayers)
        {
            string playerName = player.First["name"]?.ToString();
            string team = player.First["team"]?.ToString() ?? "No team assigned yet";
            Vector3 position = player.First["position"]?.ToObject<Vector3>() ?? new Vector3(0, 0, 0);
            if (team == "T")
            {
                terrorists.Add(playerName, position);
            }

        }
        return terrorists;
    }
}
