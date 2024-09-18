using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terrorist1Movement : MonoBehaviour
{
    public float speed = 3;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.frameCount % 20 == 0)
        {
            float x_coord = (float) GetRandomCoordinate('x');
            float z_coord = (float) GetRandomCoordinate('z');
            Vector3 targetPosition = new Vector3(x_coord, 0.501f, z_coord);
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
    
}
