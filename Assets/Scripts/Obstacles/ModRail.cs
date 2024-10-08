using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModRail : MonoBehaviour
{
    public Curve curve;
    public int railNodes = 10;
    public bool deactivateColliderWhenGrinding = false;
    public Collider colliderToDeactivate;
    public void Awake()
    {
        curve = gameObject.GetComponent<Curve>();
        SpawnRailNodes();
    }

    [ContextMenu("Spawn Objects")]
    public void SpawnRailNodes()
    {
        if (railNodes > 0)
        {
            for (int i = 0; i < railNodes; i++)
            {
                Spawn(((float)i + 1) / ((float)railNodes + 1), i);
            }
        }
    }


    void Spawn(float t, int index)
    {
        GameObject railNode = new GameObject();
        railNode.tag = "RailNode";
        railNode.transform.SetParent(transform);
        railNode.transform.position = curve.GetPoint(t);
        railNode.transform.forward = curve.GetDirection(t);
        if (index == 0)
        {
            railNode.transform.position = curve.GetPoint(0);
        }
        else if (index == railNodes - 1)
        {
            railNode.transform.position = curve.GetPoint(railNodes - 1);
        }
    }
}
