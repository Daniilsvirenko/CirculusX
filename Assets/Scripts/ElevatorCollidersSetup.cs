using UnityEngine;

public class ElevatorCollidersSetup : MonoBehaviour
{
    void Awake()
    {
        // Recursively go through all children and add colliders
        AddCollidersRecursive(transform);
    }

    private void AddCollidersRecursive(Transform parent)
    {
        foreach (Transform child in parent)
        {
            // Only add colliders if the object has a MeshRenderer (so it's visible geometry)
            // and doesn't already have a collider
            MeshRenderer renderer = child.GetComponent<MeshRenderer>();
            Collider existingCollider = child.GetComponent<Collider>();

            if (renderer != null && existingCollider == null)
            {
                string childName = child.name.ToLower();

                if (childName.Contains("button"))
                {
                    BoxCollider bc = child.gameObject.AddComponent<BoxCollider>();
                    bc.size = bc.size * 3f; // Increase the button hitbox by 3 times
                }
                else
                {
                    // For doors, walls, and floors, we use MeshCollider for an exact fit to prevent invisible barriers.
                    MeshCollider mc = child.gameObject.AddComponent<MeshCollider>();
                    // Mesh colliders must NOT be convex if we want to walk inside them (like the cage)
                    mc.convex = false; 
                }
            }

            // Go deeper into the hierarchy
            AddCollidersRecursive(child);
        }
    }
}
