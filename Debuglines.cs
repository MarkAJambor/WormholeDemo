using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debuglines : MonoBehaviour
{
    public static Debuglines debugLines;
    public Matrix4x4 matrix1;
    public Matrix4x4 matrix2;
    public Vector3 vector1;
    public Vector3 vector2;
    public bool vector1show;
    public bool vector2show;
    public bool matrix1show;
    public bool matrix2show;
    public float LengthScale = 1;

    private void Start()
    {
        debugLines = this;
        matrix1 = transform.worldToLocalMatrix;
        matrix2 = transform.worldToLocalMatrix;
        vector1 = new Vector3(0, 1, 0);
        vector2 = new Vector3(1, 0, 0);
    }
    // Update is called once per frame
    void Update()
    {
        
        if (vector1show)
        {
            Debug.DrawLine(transform.position + Vector3.zero, transform.position + vector1* LengthScale, Color.cyan);
        }
        if (vector2show)
        {
            Debug.DrawLine(transform.position + Vector3.zero, transform.position + vector2* LengthScale, Color.yellow);
        }

        Vector4 column1 = matrix1.GetColumn(0);
        Vector4 column2 = matrix1.GetColumn(1);
        Vector4 column3 = matrix1.GetColumn(2);
        if (matrix1show)
        {
            Debug.DrawLine(transform.position + Vector3.zero, transform.position + new Vector3(column1.x, column1.y, column1.z)* LengthScale, Color.red);
            Debug.DrawLine(transform.position + Vector3.zero, transform.position + new Vector3(column2.x, column2.y, column2.z)* LengthScale, Color.green);
            Debug.DrawLine(transform.position + Vector3.zero, transform.position + new Vector3(column3.x, column3.y, column3.z)* LengthScale, Color.blue);
        }
        column1 = matrix2.GetColumn(0);
        column2 = matrix2.GetColumn(1);
        column3 = matrix2.GetColumn(2);
        if (matrix2show)
        {
            Debug.DrawLine(transform.position + Vector3.zero, transform.position + new Vector3(column1.x, column1.y, column1.z)* LengthScale, Color.red);
            Debug.DrawLine(transform.position + Vector3.zero, transform.position + new Vector3(column2.x, column2.y, column2.z)* LengthScale, Color.green);
            Debug.DrawLine(transform.position + Vector3.zero, transform.position + new Vector3(column3.x, column3.y, column3.z)* LengthScale, Color.blue);
        }
    }
}
