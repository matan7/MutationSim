#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Physics 2D/Arc Collider 2D")]

[RequireComponent(typeof(PolygonCollider2D))]
public class ArcCollider2D : MonoBehaviour
{

    [Range(1, 25), HideInInspector]
    public float radius = 3;

    [Range(1, 25), HideInInspector]
    public float Thickness = 0.4f;

    [Range(10, 360)]
    public int totalAngle = 360;

    public int offsetRotation = 0;

    Vector2 origin, center;

    public Vector2[] getPoints(Vector2 off)
    {
        List<Vector2> points = new List<Vector2>();

        origin = transform.localPosition;
        center = origin + off;
        offsetRotation = 360 - (totalAngle / 2);
        float ang = offsetRotation;
        points.Add(center);

        for (int i = 0; i <= 5; i++)
        {
            float x = radius * Mathf.Cos(ang * Mathf.Deg2Rad);
            float y = radius * Mathf.Sin(ang * Mathf.Deg2Rad);

            points.Add(new Vector2(x, y));
            ang += (float)totalAngle / 5;
        }
        return points.ToArray();
    }
}
#endif