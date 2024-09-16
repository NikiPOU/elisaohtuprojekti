using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CubeShootTest : MonoBehaviour
{
    public GameObject cubePrefab;
    public float spawnSpeed = 2;
    public InputActionProperty inputAction;

    // Update is called once per frame
    void Update()
    {
        if(inputAction.action.WasPressedThisFrame())
        {
            GameObject cube = Instantiate(cubePrefab, transform.position, transform.rotation);
            Rigidbody rb = cube.GetComponent<Rigidbody>();
            rb.velocity = transform.forward * spawnSpeed;
        }
        
    }
}
