using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Curve : MonoBehaviour
{
    public virtual Vector3 GetPoint(float t)
	{
		return Vector3.zero;
	}


	public virtual Vector3 GetDirection(float t)
	{
		return Vector3.zero;
	}
}
