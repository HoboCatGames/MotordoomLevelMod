using System;
using UnityEngine;

public class BezierCurve : Curve
{

    public Vector3[] points;

    public int lineSteps = 10;
    public LineRenderer lineRenderer;

    //[Range(0f, 1f)]
    //public float t = 0;

    private void Start()
    {
        if (this.lineRenderer != null && this.lineSteps > 0)
        {
            this.lineRenderer.positionCount = ((this.points.Length - 1) / 2 * this.lineSteps) + 1;
            this.UpdateLineRenderer();
        }
    }

    private void LateUpdate()
    {
        this.UpdateLineRenderer();
    }

    public void UpdateLineSteps(int steps)
    {
        this.lineSteps = steps;
        if (this.lineRenderer != null && this.lineSteps > 0)
        {
            this.lineRenderer.positionCount = ((this.points.Length - 1) / 2 * this.lineSteps) + 1;
            this.UpdateLineRenderer();
        }
    }

    void UpdateLineRenderer()
    {
        if (this.lineRenderer != null && this.lineSteps > 0)
        {
            int curveCounter = 0;
            int lastI = 1;
            for (int i = 1; i < this.points.Length; i += 2)
            {
                if (this.points.Length <= i + 1 || this.points.Length <= i)
                    break;
                for (int j = 0; j < lineSteps; j++)
                {
                    this.lineRenderer.SetPosition(j + curveCounter, this.lineRenderer.useWorldSpace ? this.transform.TransformPoint(this.GetPointLocal(i - 1, j / (float)lineSteps)) : this.GetPointLocal(i - 1, j / (float)lineSteps));
                }
                lastI = i;
                curveCounter += lineSteps;
            }
            this.lineRenderer.SetPosition(this.lineRenderer.positionCount - 1, this.lineRenderer.useWorldSpace ? this.transform.TransformPoint(this.GetPointLocal(lastI - 1, 1)) : this.GetPointLocal(lastI - 1, 1));
        }
    }

    public void Reset()
    {
        points = new Vector3[] {
            new Vector3(1f, 0f, 0f),
            new Vector3(2f, 0f, 0f),
            new Vector3(3f, 0f, 0f)
        };
    }

    public Vector3 GetPointLocal(int startIndex, float t)
    {
        return Bezier.GetPoint(points[startIndex], points[startIndex + 1], points[startIndex + 2], t);
    }

    public Vector3 GetPoint(int startIndex, float t)
    {
        return transform.TransformPoint(Bezier.GetPoint(points[startIndex], points[startIndex + 1], points[startIndex + 2], t));
    }

    public override Vector3 GetPoint(float t)
    {
        int i;
        if (t >= 1f)
        {
            i = points.Length - 3;
			t = 1;
        }
        else
        {
            t = Mathf.Clamp01(t) * (this.points.Length - 1) / 2;
            i = (int)t;
            t -= i;
            i *= 2;
        }
        
        return this.GetPoint(i, t);
    }

	public Vector3 GetVelocity(float t)
	{
		int i;
		if (t >= 1f)
		{
			i = points.Length - 3;
			t = 1;
		}
		else
		{
			t = Mathf.Clamp01(t) * (this.points.Length - 1) / 2;
			i = (int)t;
			t -= i;
			i *= 2;
		}

		return transform.TransformPoint(Bezier.GetFirstDerivative(
			points[i], points[i + 1], points[i + 2], t)) - transform.position;
	}

	public override Vector3 GetDirection(float t)
	{
		return GetVelocity(t).normalized;
	}

	public void SetPointsCount(int length)
    {
        Array.Resize(ref points, length);
    }

    public void AddCurve()
    {
        Vector3 point = points[points.Length - 1];
        Array.Resize(ref points, points.Length + 2);
        point.x += 1f;
        points[points.Length - 2] = point;
        point.x += 1f;
        points[points.Length - 1] = point;
    }
}

public static class Bezier
{

    public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        return
            oneMinusT * oneMinusT * p0 +
            2f * oneMinusT * t * p1 +
            t * t * p2;
    }

	public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, float t)
	{
		return
			2f * (1f - t) * (p1 - p0) +
			2f * t * (p2 - p1);
	}
}