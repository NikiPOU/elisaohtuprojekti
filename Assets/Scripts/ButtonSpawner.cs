using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonSpawner : MonoBehaviour
{
    // Reference to the button prefab
    public Button buttonPrefab;

    // Parent object for the buttons (Panel with Grid Layout Group)
    public Transform buttonParent;

    // Number of buttons to create
    public int numberOfButtons = 10;

    void Start()
    {
        // Ensure buttonParent is set (Panel)
        if (buttonParent == null)
        {
            Debug.LogError("Button Parent is not set!");
            return;
        }

        // Loop to spawn buttons
        for (int i = 0; i < numberOfButtons; i++)
        {
            // Instantiate a new button
            Button newButton = Instantiate(buttonPrefab, buttonParent);

            // Set button's text (optional)
            TextMeshProUGUI buttonText = newButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = "Button" + i;
            }
        }
    }
}

