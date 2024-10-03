using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class MapMenuTest
{
    
    [Test]
    public void TestMapChanges()
    {
        GameObject mapPrefab = Resources.Load<GameObject>("Prefabs/Map");
        GameObject mapController = Resources.Load<GameObject>("Prefabs/mapController");
        var mapScript = mapController.GetComponent<MapController>();
        mapScript.mapObject = mapPrefab;

        mapScript.changeMaterial(1);

        Material material = mapPrefab.GetComponent<Renderer>().sharedMaterial;

        Assert.AreEqual(material.name, "Map_Anubis");

    }

}
