using UnityEngine;

public class ToggleVisibility : MonoBehaviour
{
    public GameObject targetObject;

    public void ToggleActiveState()
    {
        if (targetObject != null)
        {
            targetObject.SetActive(!targetObject.activeSelf);
        }
    }
}
