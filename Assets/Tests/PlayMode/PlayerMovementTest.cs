using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerMovementTest
{
    GameObject playerPrefab = Resources.Load<GameObject>("Prefabs/Terrorist");

    [UnityTest]
    public IEnumerator TestPlayerMovement()
    // Test that the player gameobject moves
    {
        Vector3 playerPos = new Vector3(0, 0.501f, 1);
        Quaternion playerDir = Quaternion.identity;
        GameObject terrorist = GameObject.Instantiate(playerPrefab, playerPos, playerDir);

        yield return new WaitForSeconds(3f);

        Vector3 newPlayerPos = (Vector3) terrorist.transform.position;

        Assert.AreNotEqual(newPlayerPos, (0, 0.501f, 1));
    }
}
