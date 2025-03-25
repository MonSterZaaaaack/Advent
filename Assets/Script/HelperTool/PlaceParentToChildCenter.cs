using UnityEngine;
using UnityEditor;

public class PlaceParentToChildCenter : MonoBehaviour
{
    [ContextMenu("Center This Object To Children")]
    void CenterToChildren()
    {
        if (transform.childCount == 0)
        {
            Debug.LogWarning("No children to center on.");
            return;
        }

        Vector3 center = Vector3.zero;
        foreach (Transform child in transform)
        {
            center += child.position;
        }
        center /= transform.childCount;

        Vector3 offset = center - transform.position;

        // Move parent to center
        transform.position = center;

        // Compensate children to keep them visually in place
        foreach (Transform child in transform)
        {
            child.position -= offset;
        }

        Debug.Log("Parent centered to children.");
    }
}