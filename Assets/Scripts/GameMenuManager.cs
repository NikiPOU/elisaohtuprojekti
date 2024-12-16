using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Manages the display and positioning of the menu.
/// </summary>
public class GameMenuManager : MonoBehaviour
{
    public Transform head; //Reference to user head, eg. VR headset.
    public float spawnDistance = 2; //Distance from the head where the menu spawns.
    public GameObject menu; //The menu GameObject.
    public InputActionProperty showButton; //Input action for toggling the menu.

    /// <summary>
    /// Called once per frame. Handles menu toggling and position updates.
    /// </summary>
    void Update()
    {
        //Only proceed if showButton action is set and enabled
        if (showButton.action != null && showButton.action.WasPressedThisFrame())
        {
            ToggleMenu();
        }

        //Ensure the menu position is updated only when it's active
        if (menu.activeSelf)
        {
            UpdateMenuPosition();
        }
    }

    /// <summary>
    /// Toggles the menu's active state on or off.
    /// </summary>
    public void ToggleMenu()
    {
        menu.SetActive(!menu.activeSelf);
    }

    /// <summary>
    /// Updates the menu's position and orientation relative to the user's head.
    /// </summary>
    public void UpdateMenuPosition()
    {
        //Update the menu's position based on the head's position
        menu.transform.position = head.position + new Vector3(head.forward.x, 0, head.forward.z) * spawnDistance;

        //Ensure the menu is always facing the player
        menu.transform.LookAt(new Vector3(head.position.x, menu.transform.position.y, head.position.z));

        //Invert the forward direction so the menu faces the correct way
        menu.transform.forward *= -1;
    }
}
