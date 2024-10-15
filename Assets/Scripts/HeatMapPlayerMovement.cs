using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 2.0f;
    public Vector3 targetPosition = new Vector3(2.795f, 2f, 1f);
    public GameObject tracePrefab;
    public Vector3 testCoordinates = new Vector3(1f,2f,1f);
    //dictionary coordinates as keys
    Dictionary<string, int> coordinates = new Dictionary<string, int>();
    Dictionary<string, GameObject> gameObjects = new Dictionary<string, GameObject>();
    Dictionary<int, List<double>> colors = new Dictionary<int, List<double>>()
    {
        {1, new List<double> { 13, 59, 246, 0.8 }},
        {10, new List<double> { 0, 194, 255, 0.8 }},
        {20, new List<double> { 0, 255, 135, 0.8 }},
        {30, new List<double> { 120, 255, 0, 0.8 }},
        {40, new List<double> { 247, 255, 0, 0.8 }},
        {60, new List<double> { 255, 162, 0, 0.8 }},
        {70, new List<double> { 255, 98, 0, 0.8 }},
        {100, new List<double> { 161, 0, 0, 0.8 }}
    };
    public int i=0;

    private float timeOnCurrentCoordinate = 0f;
    private Vector3 lastPosition;

    void Start()
    {
        lastPosition = transform.position; //nykyinen sijainti tässä
        UpdateTargetPosition(); //hakee uuden sijainnin
    }

    void Update()
    {

        UpdateTargetPosition();


        // Move towards the target position at the specified speed
        //transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        transform.position = targetPosition;
        /*
        if (transform.position == lastPosition) //JOS PYSYY PAIKALLAAN
        {
            //time spent on this coordinate gets bigger
            timeOnCurrentCoordinate += Time.deltaTime;

            // Fetch the previous time spent on a coordinate
            float previousTimeSpent = GetTimeSpentAtPosition(lastPosition); //VOI OLLA VÄÄRIJ

            float totalSpentTime = previousTimeSpent + timeOnCurrentCoordinate; //uusi aika sinne tuple listaan --> pitää vielä tehdä
        }
        else
        {
            //TOIMIIKO ETTÄ TUOSTA YLEMMÄSTÄ HYPPÄÄ TÄHÄN JA SE ETTÄ SUORAAN HYPPÄÄ TÄHÄN
            // Player moved, save the time spent at the last position
            SaveTimeSpentAtPosition(lastPosition, timeOnCurrentCoordinate);

            // Reset the timer and update the last position
            lastPosition = transform.position;
            timeOnCurrentCoordinate = 0f;
        }
        // Leave a trace at the current position
        */
        LeaveTrace();
    }
    void ChangeColorBasedOnTime(GameObject newTrace, float timeSpent)
    {
        Renderer renderer = newTrace.GetComponent<Renderer>();

        if (renderer != null)
        {
            // Adjust the color's darkness based on the time spent
            float darkeningFactor = Mathf.Clamp(1f - timeSpent * 0.1f, 0.1f, 1f);
            //TESTAA TÄHÄN IF LAUSEITA KUINKA KAUAN ON OLU CORDINAATILLA
            //New material!
            Debug.Log("VÄRIN PITÄISI VBAIHTUA" + timeSpent);
            Material existingMaterial = renderer.material;

            //Create a new color
            Color newColor = new Color(0f, darkeningFactor, 0f, 1f);


            // Update the material color of the player (or any other object)
            existingMaterial.color = newColor;
        }
    }
    void LeaveTrace()
    {
        if (tracePrefab != null)
        {
            Vector3 currentPosition = transform.position;

            // Check for existing coordinates
            //float overlapThreshold = 0.1f; // Distance threshold for considering overlap
            //float timeSpent = GetTimeSpentAtPosition(currentPosition);
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
                gameObjects.Add(currentPosition.ToString(),newTrace);
            }
/*
            //Debug.Log("New trace instantiated at position: " + currentPosition);
            Vector3 testVector = new Vector3(2f, 2f, 2f); //TESTI
            //Debug.Log("New trace instantiated at position: " + currentPosition + testVector);
            if (currentPosition == testVector)
            {
                // Change the color of the trace based on the time spent
               // ChangeColorBasedOnTime(newTrace, timeSpent);
                //Debug.Log("MENI VÄRINVAIHTOON");
            }
            // Determine color based on whether we are overlapping with an existing coordinate


        }
    }*/
        }
    }
    public void UpdateTargetPosition()
    {
        //Tähän joku random koordinaatti
        //float x_coord = (float)GetRandomCoordinate('x');
        //float z_coord = (float)GetRandomCoordinate('z');
        //float x_coord = (2f);
        //float z_coord = (2f);
        Debug.Log(testCoordinates[0]);
        Debug.Log(testCoordinates[0].Equals(1.8f));
        //Console.WriteLine(float1.Equals(float2));
        if (i == 8)
        {
            testCoordinates = new Vector3(testCoordinates[0]-0.1f, 2f, 1f);
            Debug.Log("kääntyy");
            i -= 1;
        }
        else
        {
            testCoordinates = new Vector3(testCoordinates[0]+0.1f, 2f, 1f);
            i += 1;
        }
        targetPosition = testCoordinates;
    }















/*
    void SaveTimeSpentAtPosition(Vector3 position, float timeSpent)
    {
        // Round the position to avoid floating-point precision issues
        Vector3 roundedPosition = new Vector3(
            Mathf.Round(position.x * 100f) / 100f,
            Mathf.Round(position.y * 100f) / 100f,
            Mathf.Round(position.z * 100f) / 100f);

        // Check if this position is already in the list
        int index = coordinatesWithTime.FindIndex(item => item.coordinate == roundedPosition);

        if (index >= 0)
        {
            // If the position already exists, update the time spent there
            coordinatesWithTime[index] = (roundedPosition, coordinatesWithTime[index].timeSpent + timeSpent);
        }
        else
        {
            // Otherwise, add a new entry
            coordinatesWithTime.Add((roundedPosition, timeSpent));
        }
    }


    float GetTimeSpentAtPosition(Vector3 position)
    {
        // Round the position to avoid floating-point precision issues
        Vector3 roundedPosition = new Vector3(
            Mathf.Round(position.x * 100f) / 100f,
            Mathf.Round(position.y * 100f) / 100f,
            Mathf.Round(position.z * 100f) / 100f);

        // Find the index of the position in the list of coordinates
        int index = coordinatesWithTime.FindIndex(item => item.coordinate == roundedPosition);

        if (index >= 0)
        {
            // If the position is found this RETURNS TIME PREVIOUSLY SPENT THERE
            return coordinatesWithTime[index].timeSpent; 
        }

        // If the position is not found, return 0 (indicating no time spent at this position)
        return 0f;
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
    */
}