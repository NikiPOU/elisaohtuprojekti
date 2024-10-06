using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Linq;

public class TerroristMovement : MonoBehaviour
{
    public float speed = 0.2f;
    Vector3 targetPosition = new Vector3(-0.8f, 0.501f, 0.2f);
    GSIDataReceiver gsiDataReceiver;

    // Start is called before the first frame update
    void Start()
    {
        gsiDataReceiver = FindObjectOfType<GSIDataReceiver>();

        if (gsiDataReceiver == null)
        {
            return;
        }

    }

    // Update is called once per frame
    void Update()
    {
        MovePlayers();
    }
    
    Dictionary<string, Vector3> ParseData(string jsonData)
    {
        JObject data = JObject.Parse(jsonData);
        var allPlayers = data["allPlayers"];

        Dictionary<string, Vector3> terrorists = new Dictionary<string, Vector3>();

        foreach (var player in allPlayers)
        {
            string team = player.First["team"]?.ToString() ?? "No team assigned yet";

            if (team == "T")
            {
                string playerName = player.First["name"]?.ToString();
                Vector3 position = player.First["position"]?.ToObject<Vector3>() ?? new Vector3(0, 0, 0);
                terrorists.Add(playerName, position);
            }
            
        }
        
        return terrorists;
    }

    public void MovePlayers()
    {
        Dictionary<string, Vector3> players = ParseData(gsiDataReceiver.gsiData);

        GameObject terrorist1 = GameObject.FindWithTag("Terrorist1");
        Vector3 targetPos1 = players.ElementAt(0).Value;
        terrorist1.transform.position = Vector3.MoveTowards(terrorist1.transform.position, targetPos1, speed * Time.deltaTime);

        GameObject terrorist2 = GameObject.FindWithTag("Terrorist2");
        Vector3 targetPos2 = players.ElementAt(1).Value;
        terrorist2.transform.position = Vector3.MoveTowards(terrorist2.transform.position, targetPos2, speed * Time.deltaTime);

        GameObject terrorist3 = GameObject.FindWithTag("Terrorist3");
        Vector3 targetPos3 = players.ElementAt(2).Value;
        terrorist3.transform.position = Vector3.MoveTowards(terrorist3.transform.position, targetPos3, speed * Time.deltaTime);

        GameObject terrorist4 = GameObject.FindWithTag("Terrorist4");
        Vector3 targetPos4 = players.ElementAt(3).Value;
        terrorist4.transform.position = Vector3.MoveTowards(terrorist4.transform.position, targetPos4, speed * Time.deltaTime);

        GameObject terrorist5 = GameObject.FindWithTag("Terrorist5");
        Vector3 targetPos5 = players.ElementAt(4).Value;
        terrorist5.transform.position = Vector3.MoveTowards(terrorist5.transform.position, targetPos5, speed * Time.deltaTime);
    }
    public void UpdateTargetPosition()
    {
        float x_coord = (float) GetRandomCoordinate('x');
        float z_coord = (float) GetRandomCoordinate('z');
        targetPosition = new Vector3(x_coord, 0.501f, z_coord);
    }
    
}
