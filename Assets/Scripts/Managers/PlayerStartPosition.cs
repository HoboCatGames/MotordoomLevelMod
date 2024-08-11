using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStartPosition : MonoBehaviour
{
    private Transform startPosition;

    private List<Transform> respawnPositions = new List<Transform>();

    public Transform StartPosition { get => startPosition; }

    private void Awake()
    {
        startPosition = this.transform;
    }

    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            respawnPositions.Add(transform.GetChild(i));
        }
    }

    public Transform GetClosestRespawnPosition(Transform currentPosition)
    {
        if(respawnPositions.Count == 0)
        {
            return startPosition;
        }
        else
        {
            Transform closestPosition = null;
            float closestDistance = float.MaxValue;

            foreach (Transform respawnPosition in respawnPositions)
            {
                float distance = Vector3.Distance(currentPosition.position, respawnPosition.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPosition = respawnPosition;
                }
            }
            closestPosition.position = new Vector3(closestPosition.position.x, closestPosition.position.y + 3, closestPosition.position.z);
            return closestPosition;

        }
    }
}

