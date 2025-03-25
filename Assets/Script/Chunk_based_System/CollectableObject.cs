using UnityEngine;

public class CollectableObject : MonoBehaviour
{
    public bool isCollected = false;

    // Simulate object being collected by the player.
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isCollected = true;
            gameObject.SetActive(false);  // Deactivate the object when collected.
        }
    }
}