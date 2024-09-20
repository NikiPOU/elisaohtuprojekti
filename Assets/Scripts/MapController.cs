using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public GameObject mapObject;
    public Material[] materials = new Material[8];

    public void changeMaterial(int index)  // Changes the material on the map gameobject
    {

     mapObject.GetComponent<MeshRenderer>().material = materials[index];

    }
    
}
