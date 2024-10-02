using UnityEngine;
using UnityEngine.InputSystem;

public class GameMenuManager : MonoBehaviour
{
    public Transform head;
    public float spawnDistance = 2;
    public GameObject menu;
    public InputActionProperty showButton;

    void Update()
    {
        // Only proceed if showButton action is set and enabled
        if (showButton.action != null && showButton.action.WasPressedThisFrame())
        {
            ToggleMenu();
        }

        // Ensure the menu position is updated only when it's active
        if (menu.activeSelf)
        {
            UpdateMenuPosition();
        }
    }

    public void ToggleMenu()
    {
        // Toggle the menu's active state
        menu.SetActive(!menu.activeSelf);
    }

    public void UpdateMenuPosition()
    {
        // Update the menu's position based on the head's position
        menu.transform.position = head.position + new Vector3(head.forward.x, 0, head.forward.z) * spawnDistance;
        menu.transform.LookAt(new Vector3(head.position.x, menu.transform.position.y, head.position.z));
        menu.transform.forward *= -1;
    }
}
