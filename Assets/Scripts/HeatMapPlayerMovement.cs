using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 2.0f;
    public Vector3 targetPosition = new Vector3(2.795f, 2f, 1f);
    public GameObject tracePrefab;

    void Start()
    {
        UpdateTargetPosition();
    }

    void Update()
    {
        // Update target position every 2 seconds
        if (Time.frameCount % 120 == 0)
        {
            UpdateTargetPosition();
        }

        // Move towards the target position at the specified speed
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Leave a trace at the current position
        LeaveTrace();
    }

    void LeaveTrace()
    {
        if (tracePrefab != null)
        {
            Instantiate(tracePrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogError("TracePrefab is not assigned in the Inspector!");
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
        float x_coord = (float)GetRandomCoordinate('x');
        float z_coord = (float)GetRandomCoordinate('z');
        targetPosition = new Vector3(x_coord, 0.501f, z_coord);
    }
}
