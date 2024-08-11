using System;
using UnityEngine;

public enum BezierControlPointMode
{
	Free,
	Aligned,
	Mirrored
}

[System.Serializable]
public class ArcLength{
	public float[] segmentLengths;
	public float totalArcLength = 0;

	public float SampleCurve(int nSegments, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
	{
		float segIncrease = 1f / nSegments;
		segmentLengths = new float[nSegments + 1];
		segmentLengths[0] = 0;

		Vector3 previous = CubicBezier.GetPoint(p0, p1, p2, p3, 0);

		float t = 0f;
		totalArcLength = 0;

		for (int i = 1; i < nSegments + 1; i++)
		{
			t += segIncrease;
			// Get the current sampling step for our t value
			Vector3 current = CubicBezier.GetPoint(p0, p1, p2, p3, t);
			//Debug.DrawRay(current, Vector3.up, Color.blue, 3f);
			// Calculate the delta between the previous point and our current point
			// This will give us the arc length, which we accumulate in clen
			//Vector3 delta = previous - current;
			//clen += Mathf.Sqrt(delta.x * delta.x + delta.y * delta.y);

			// Store the arclength in our array of arc length segments
			totalArcLength += (previous - current).magnitude;
			segmentLengths[i] = totalArcLength;
			previous = current;
		}
		return totalArcLength;
	}

	public float GetParametricValueForTargetLength(float targetLength)
	{
		int low = 0;
		int index = 0;
		int high = segmentLengths.Length - 1;

		// We do a simple binary search for the largest length that's smaller than our target length
		while (low < high)
		{
			index = low + (((high - low) / 2) | 0);

			if (segmentLengths[index] < targetLength)
			{
				low = index + 1;
			}
			else
			{
				high = index;
			}
		}

		if (segmentLengths[index] > targetLength)
		{
			index--;
		}

		float lengthBefore = segmentLengths[index];

		// If we found the exact length we need in our array, return its t value
		if (lengthBefore == targetLength)
		{
			return index / (segmentLengths.Length - 1);
		}
		else
		{
			// Otherwise, return the interpolation represented by the remainder (targetLength - lengthBefore)
			return (index + (targetLength - lengthBefore) / (segmentLengths[index + 1] - lengthBefore)) / (segmentLengths.Length - 1);
		}
	}
}

public class CubicBezierSpline : Curve
{
	public int arcSegments = 10;
	public float totalSplineLength;
	//[HideInInspector]
	[SerializeField]
	public Vector3[] points;

	[SerializeField]
	public BezierControlPointMode[] modes;

	[SerializeField]
	public ArcLength[] arcLengths;
	[SerializeField]
	private bool loop;

	public bool Loop
	{
		get
		{
			return loop;
		}
		set
		{
			loop = value;
			if (value == true)
			{
				modes[modes.Length - 1] = modes[0];
				SetControlPoint(0, points[0]);
			}
		}
	}



	public int ControlPointCount
	{
		get
		{
			return points.Length;
		}
	}

	public Vector3 GetControlPoint(int index)
	{
		return points[index];
	}

	public void SetControlPoint(int index, Vector3 point)
	{
		if (index % 3 == 0)
		{
			Vector3 delta = point - points[index];
			if (loop)
			{
				if (index == 0)
				{
					points[1] += delta;
					points[points.Length - 2] += delta;
					points[points.Length - 1] = point;
				}
				else if (index == points.Length - 1)
				{
					points[0] = point;
					points[1] += delta;
					points[index - 1] += delta;
				}
				else
				{
					points[index - 1] += delta;
					points[index + 1] += delta;
				}
			}
			else
			{
				if (index > 0)
				{
					points[index - 1] += delta;
				}
				if (index + 1 < points.Length)
				{
					points[index + 1] += delta;
				}
			}
		}
		points[index] = point;
		EnforceMode(index);
		CalculateLength();
	}

	public BezierControlPointMode GetControlPointMode(int index)
	{
		return modes[(index + 1) / 3];
	}

	public void SetControlPointMode(int index, BezierControlPointMode mode)
	{
		int modeIndex = (index + 1) / 3;
		modes[modeIndex] = mode;
		if (loop)
		{
			if (modeIndex == 0)
			{
				modes[modes.Length - 1] = mode;
			}
			else if (modeIndex == modes.Length - 1)
			{
				modes[0] = mode;
			}
		}
		EnforceMode(index);
	}


	public override Vector3 GetPoint(float t)
	{
		int i;
		if (t >= 1f)
		{
			t = 1f;
			i = points.Length - 4;
		}
		else
		{
			/*
			t = Mathf.Clamp01(t) * CurveCount;
			i = (int)t;
			t -= i;
			i *= 3;*/
			i = GetArcByTotalT(t, out t);
		}
		//Debug.Log("index " + i + "; t " + t);
		return transform.TransformPoint(CubicBezier.GetPoint(
			points[i], points[i + 1], points[i + 2], points[i + 3], t));
	}

	public Vector3 GetVelocity(float t)
	{
		int i;
		if (t >= 1f)
		{
			t = 1f;
			i = points.Length - 4;
		}
		else
		{
			/*t = Mathf.Clamp01(t) * CurveCount;
			i = (int)t;
			t -= i;
			i *= 3;*/
			i = GetArcByTotalT(t, out t);
		}
		
		return transform.TransformPoint(CubicBezier.GetFirstDerivative(
			points[i], points[i + 1], points[i + 2], points[i + 3], t)) - transform.position;
	}

	int GetArcByTotalT(float t, out float arcT)
	{
		float positionOnSpline = totalSplineLength * t;
		int pointsIndex = 0;
		float countingArcLength = 0;
		for (int i = 0; i < arcLengths.Length; i++)
		{
			if (positionOnSpline > arcLengths[i].totalArcLength + countingArcLength)
				countingArcLength += arcLengths[i].totalArcLength;
			else{
				arcT = arcLengths[i].GetParametricValueForTargetLength(positionOnSpline - countingArcLength);
				return pointsIndex;
			}
			pointsIndex += 3;
		}
		arcT = Mathf.Clamp01(t) * CurveCount;
		pointsIndex = (int)arcT;
		arcT -= pointsIndex;
		pointsIndex *= 3;
		return pointsIndex;
	}



	public int CurveCount
	{
		get
		{
			return (points.Length - 1) / 3;
		}
	}

	public override Vector3 GetDirection(float t)
	{
		return GetVelocity(t).normalized;
	}


	private void EnforceMode(int index)
	{
		int modeIndex = (index + 1) / 3;
		BezierControlPointMode mode = modes[modeIndex];
		if (mode == BezierControlPointMode.Free || index == 0 || index == points.Length - 1)
		{
			return;
		}

		int middleIndex = modeIndex * 3;
		int fixedIndex, enforcedIndex;
		if (index <= middleIndex)
		{
			fixedIndex = middleIndex - 1;
			enforcedIndex = middleIndex + 1;
		}
		else
		{
			fixedIndex = middleIndex + 1;
			enforcedIndex = middleIndex - 1;
		}

		Vector3 middle = points[middleIndex];
		Vector3 enforcedTangent = middle - points[fixedIndex];
		if (enforcedIndex >= 0 && enforcedIndex < points.Length)
		{
			if (mode == BezierControlPointMode.Aligned)
			{
				enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, points[enforcedIndex]);
			}
			points[enforcedIndex] = middle + enforcedTangent;
		}
	}

	public void Reset()
	{
		points = new Vector3[] {
			new Vector3(1f, 0f, 0f),
			new Vector3(2f, 0f, 0f),
			new Vector3(3f, 0f, 0f),
			new Vector3(4f, 0f, 0f)
		};
		modes = new BezierControlPointMode[] {
			BezierControlPointMode.Free,
			BezierControlPointMode.Free
		};
		arcLengths = new ArcLength[] {
			new ArcLength()
		};
	}

	public void AddPoint()
	{
		Vector3 point = points[points.Length - 1];
		Vector3 direction = (points[points.Length - 1] - points[points.Length - 2]).normalized;
		Array.Resize(ref points, points.Length + 3);

		for (int i = 3; i > 0; i--)
		{
			point += direction;
			points[points.Length - i] = point;
		}

		Array.Resize(ref modes, modes.Length + 1);
		modes[modes.Length - 1] = modes[modes.Length - 2];
		EnforceMode(points.Length - 4);
		Array.Resize(ref arcLengths, arcLengths.Length + 1);
		arcLengths[arcLengths.Length - 1] = new ArcLength();
		CalculateLength();
	}

	public void RemoveLastPoint()
	{
		Array.Resize(ref points, points.Length - 3);
		Array.Resize(ref modes, modes.Length - 1);
		Array.Resize(ref arcLengths, arcLengths.Length - 1);
		CalculateLength();
	}

	void CalculateLength(){
		//Debug.Log("Calculate length " + arcLengths.Length + "; " + points.Length);
		int pointsIndex = 0;
		totalSplineLength = 0;
		for (int i = 0; i < arcLengths.Length; i++)
		{
			totalSplineLength += arcLengths[i].SampleCurve(arcSegments, points[pointsIndex], points[pointsIndex + 1], points[pointsIndex + 2], points[pointsIndex + 3]);
			pointsIndex += 3;
		}
	}
}

public static class CubicBezier
{
	public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
	{
		t = Mathf.Clamp01(t);
		float oneMinusT = 1f - t;
		return
			oneMinusT * oneMinusT * oneMinusT * p0 +
			3f * oneMinusT * oneMinusT * t * p1 +
			3f * oneMinusT * t * t * p2 +
			t * t * t * p3;
	}

	public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
	{
		t = Mathf.Clamp01(t);
		float oneMinusT = 1f - t;
		return
			3f * oneMinusT * oneMinusT * (p1 - p0) +
			6f * oneMinusT * t * (p2 - p1) +
			3f * t * t * (p3 - p2);
	}
}
