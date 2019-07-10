using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(ArcCollider2D))]
public class ArcCollider_Editor : Editor
{

    ArcCollider2D ac;
    PolygonCollider2D polyCollider;
    Vector2 off;

    void OnEnable()
    {
        ac = (ArcCollider2D)target;

        polyCollider = ac.GetComponent<PolygonCollider2D>();
        if (polyCollider == null)
        {
            ac.gameObject.AddComponent<PolygonCollider2D>();
            polyCollider = ac.GetComponent<PolygonCollider2D>();
        }
        polyCollider.points = ac.getPoints(polyCollider.offset);
    }

    public override void OnInspectorGUI()
    {
        GUI.changed = false;
        DrawDefaultInspector();

        if (GUI.changed || !off.Equals(polyCollider.offset))
        {
            polyCollider.points = ac.getPoints(polyCollider.offset);
        }
        off = polyCollider.offset;
    }

}