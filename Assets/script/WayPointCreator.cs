using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointCreator : MonoBehaviour
{
    public GameObject road; // Reference to the road model
public GameObject nodePrefab; // Prefab for the node visual marker

private List<Vector3> nodes = new List<Vector3>(); // List to store generated nodes
public float nodeSpacing = 1000f; // Spacing between nodes

void Start()
{
    GenerateNodes();
    VisualizeNodes();
}

   void GenerateNodes()
    {
        // Get the bounds of the road model
        Bounds bounds = road.GetComponent<Renderer>().bounds;

        // Calculate the centerline of the road model
        float centerX = (bounds.min.x + bounds.max.x) / 2;

        // Iterate through the bounds along the centerline
        for (float z = bounds.min.z; z <= bounds.max.z; z += nodeSpacing)
        {
            Vector3 startPoint = new Vector3(centerX, bounds.max.y, z);

            // Perform a raycast to find the road surface
            if (Physics.Raycast(startPoint, Vector3.down, out RaycastHit hit))
            {
                if (hit.collider.gameObject == road)
                {
                    Vector3 nodePosition = hit.point;
                    nodes.Add(nodePosition);
                }
            }
        }
    }

    void VisualizeNodes()
    {
        foreach (Vector3 node in nodes)
        {
            Instantiate(nodePrefab, node, Quaternion.identity);
        }
    }
}