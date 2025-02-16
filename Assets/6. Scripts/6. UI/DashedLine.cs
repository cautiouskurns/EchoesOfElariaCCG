using UnityEngine;
using System.Collections.Generic;

public class DashedLine : MonoBehaviour
{
    public Transform startNode;
    public Transform endNode;
    public GameObject dashPrefab; // Assign the dash prefab in the Inspector
    public float dashSpacing = 0.5f; // The space between dashes

    private List<GameObject> instantiatedDashes = new List<GameObject>();

    void Start()
    {
        GenerateDashedLine();
    }

    void GenerateDashedLine()
    {
        if (startNode == null || endNode == null || dashPrefab == null)
        {
            Debug.LogError("[DashedLine] ❌ Missing references!");
            return;
        }

        // Clear old dashes
        foreach (GameObject dash in instantiatedDashes)
        {
            Destroy(dash);
        }
        instantiatedDashes.Clear();

        Vector3 startPos = startNode.position;
        Vector3 endPos = endNode.position;
        Vector3 direction = (endPos - startPos).normalized;
        float totalDistance = Vector3.Distance(startPos, endPos);

        // Calculate the number of dashes based on spacing
        int numDashes = Mathf.FloorToInt(totalDistance / dashSpacing);

        for (int i = 0; i < numDashes; i++)
        {
            Vector3 position = startPos + direction * (i * dashSpacing);
            GameObject dash = Instantiate(dashPrefab, position, Quaternion.identity, transform);

            // Rotate the dash to align with the line direction
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            dash.transform.rotation = Quaternion.Euler(0, 0, angle);

            // ✅ Ensure dashes are slightly behind nodes
            dash.transform.position += new Vector3(0, 0, 0.1f);

            instantiatedDashes.Add(dash);
        }

        // ✅ Ensure nodes render on top
        startNode.SetAsLastSibling();
        endNode.SetAsLastSibling();
    }
}

