using UnityEngine;

/// <summary>
/// Toggles the visibility of a specified GameObject by enabling or disabling it.
/// This script allows for showing or hiding the target object.
/// </summary>
public class ToggleVisibility : MonoBehaviour
{
    //The GameObject whose visibility will be toggled.
    public GameObject targetObject;

    /// <summary>
    /// Toggles the active state of the GameObject, activated/deactivated.
    /// </summary>
    public void ToggleActiveState()
    {
        if (targetObject != null)
        {
            targetObject.SetActive(!targetObject.activeSelf);
        }
    }
}
