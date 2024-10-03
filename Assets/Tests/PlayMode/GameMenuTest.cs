using UnityEngine;
using UnityEngine.TestTools; // Use Unity Test Framework
using NUnit.Framework;
using System.Collections;

public class GameMenuTest
{
    private GameObject testObject;
    private GameMenuManager gameMenuManager;
    private GameObject menu; // Stub for Map Menu
    private Transform head;  // Stub for Main Camera

    [SetUp]
    public void SetUp()
    {
        // Create a stub for the Main Camera
        GameObject cameraStub = new GameObject("CameraStub");
        head = cameraStub.transform;

        // Create a stub for the Map Menu
        menu = new GameObject("MenuStub");

        // Set up the GameObject for the test and add the GameMenuManager component
        testObject = new GameObject("TestObject");
        gameMenuManager = testObject.AddComponent<GameMenuManager>();

        // Set the menu and head (camera) references
        gameMenuManager.menu = menu;
        gameMenuManager.head = head;

        // Set spawn distance
        gameMenuManager.spawnDistance = 2.0f;

        // Initially, the menu should be inactive
        menu.SetActive(false);
    }

    [TearDown]
    public void TearDown()
    {
        // Clean up objects after each test
        Object.Destroy(testObject);
        Object.Destroy(menu);
        if (head != null)
        {
            Object.Destroy(head.gameObject);
        }
    }

    [UnityTest]
    public IEnumerator MenuOpensOnMethodCall()
    {
        // Assert: The menu should be inactive at the start
        Assert.IsFalse(gameMenuManager.menu.activeSelf, "Menu should be inactive at the start");

        // Call the method to toggle the menu (simulated)
        gameMenuManager.ToggleMenu(); // Make sure this method exists in GameMenuManager

        // Wait for the next frame to ensure any updates are processed
        yield return null;

        // Assert: Check if the menu has been activated
        Assert.IsTrue(gameMenuManager.menu.activeSelf, "Menu should be active after calling the method");

        // Call the method again to toggle it off
        gameMenuManager.ToggleMenu();

        // Wait for the next frame to ensure any updates are processed
        yield return null;

        // Assert: Check if the menu has been deactivated
        Assert.IsFalse(gameMenuManager.menu.activeSelf, "Menu should be inactive after toggling off");
    }

    [UnityTest]
    public IEnumerator MenuPositionIsCorrectWhenOpened()
    {
        // Set the camera's position and rotation
        head.position = new Vector3(0, 0, 0); // Starting position
        head.rotation = Quaternion.Euler(0, 0, 0); // Facing forward

        // Activate the menu
        gameMenuManager.ToggleMenu();

        // Call UpdateMenuPosition to reposition the menu based on head's position
        gameMenuManager.UpdateMenuPosition();

        // Wait for the next frame to ensure any updates are processed
        yield return null;

        // Calculate the expected position of the menu
        Vector3 expectedPosition = head.position + head.forward * gameMenuManager.spawnDistance;

        // Assert that the menu's position is correct
        Assert.AreEqual(expectedPosition, menu.transform.position, 
            "Menu position is incorrect when opened. Expected: " + expectedPosition + ", Actual: " + menu.transform.position);

        // Also check if the menu is looking at the camera correctly
        Vector3 lookAtPosition = new Vector3(head.position.x, menu.transform.position.y, head.position.z);
        menu.transform.LookAt(lookAtPosition);
        menu.transform.forward *= -1;

        // Assert that the menu is looking at the head correctly
        Assert.AreEqual(menu.transform.forward, head.forward, "Menu is not looking at the camera correctly.");
    }
}


