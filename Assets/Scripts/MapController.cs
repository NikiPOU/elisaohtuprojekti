using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Handles interactions with the map object and changing the material of the map object dynamically.
/// </summary>
public class MapController : MonoBehaviour
{

    /// <summary>
    /// Reference to the GameObject representing the map in the scene.
    /// </summary>
    public GameObject mapObject;

    /// <summary>
    /// Array of materials available for the map object.
    /// </summary>
    public Material[] materials = new Material[8];

    /// <summary>
    /// Changes the material of the map object.
    /// </summary>
    /// <param name="index">The index of the material to apply..</param>
    public void changeMaterial(int index)
    {
     mapObject.GetComponent<MeshRenderer>().material = materials[index];

    }
    
}
