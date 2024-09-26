using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerMovementTest
{
    GameObject playerPrefab = Resources.Load<GameObject>("Prefabs/Terrorist");


    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator TestPlayerMovement()
    {
        Vector3 playerPos = new Vector3(0, 0.501f, 1);
        Quaternion playerDir = Quaternion.identity;
        GameObject terrorist = GameObject.Instantiate(playerPrefab, playerPos, playerDir);

        yield return new WaitForSeconds(3f);

        Vector3 newPlayerPos = (Vector3) terrorist.transform.position;

        Assert.AreNotEqual(newPlayerPos, (0, 0.501f, 1));
    }
}
