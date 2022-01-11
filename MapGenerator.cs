using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Wormhole;
    private WormholeScript WormholeScript;
    public GameObject Traveler;
    private WormholeTraveler TravelerScript;
    MeshRenderer MeshRenderer;
    MeshFilter Filter;
    float[] HeightLookupTable;
    public float MaxRadius=20;
    public int MeshHeight = 20;
    public int MeshWidth = 30;
    private int ActualMeshHeight;
    int LookuptableSize = 1000;
    Mesh mesh;

    void Start()
    {
        WormholeScript = Wormhole.GetComponent<WormholeScript>();
        TravelerScript = Traveler.GetComponent<WormholeTraveler>();
        MeshRenderer = GetComponent<MeshRenderer>();
        Filter = GetComponent<MeshFilter>();

        mesh = new Mesh();
        Filter.mesh = mesh;
        
        float PreviousValue = 0;
        float Area = 0;
        float dx = MaxRadius / LookuptableSize;
        HeightLookupTable = new float[LookuptableSize];
        ActualMeshHeight = MeshHeight * 2 + 1;
        
        for (int i = 0; i < LookuptableSize; i++)
        {
            float X = i * dx;
            float CurrentValue = DerivativeHeightFunction(X);
            HeightLookupTable[i] = Area;
            Area += dx * (CurrentValue + PreviousValue) / 2;
            PreviousValue = CurrentValue;
        }
        Vector3[] Vertecies = new Vector3[MeshWidth * ActualMeshHeight];
        Color[] Colors = new Color[MeshWidth * ActualMeshHeight];
        float ColorDistance = WormholeScript.Length * WormholeScript.Radius+ WormholeScript.Radius*4;
       
        for (int i = 0; i < MeshWidth; i++)
        {
            float Angle = 2*i * Mathf.PI / MeshWidth;
            for (int j = 0; j < ActualMeshHeight; j++)
            {
                int VertexIndex = ActualMeshHeight * i + j;
                float Rad = MaxRadius*(j - MeshHeight) / MeshHeight;

                
                Color Col = Color.Lerp(WormholeScript.Color2, WormholeScript.Color1, ((Rad / ColorDistance) + 1) / 2);
                Col = Color.Lerp(Col, Color.white, (i % 2) * 0.5f);
                Col = Color.Lerp(Col, Color.black, (j % 2) * 0.2f);
                
                Col.a = 0.4f;
                Colors[VertexIndex] = Col;
                Vector3 Vertex = GetShape(Angle, Rad);
                Vertecies[VertexIndex] = Vertex;
            }
        }
        mesh.vertices = Vertecies;
        mesh.colors = Colors;
        int[] Indecies = new int[12 * (MeshWidth * (ActualMeshHeight - 1))];
        int k = 0;
        for (int i = 0; i < MeshWidth; i++)
        {
            for (int j = 0; j < ActualMeshHeight - 1; j++)
            {
                int i2 = (i + 1) % MeshWidth;
                int u = ActualMeshHeight * i;
                int u2 = ActualMeshHeight * i2;
                AddMeshSquare(u + j, u + j + 1, u2 + j, u2 + j + 1);
                k++;
            }
        }
        void AddMeshSquare(int i1, int i2, int i3, int i4)
        {
            int m =12 * k;
            Indecies[m++] = i1;
            Indecies[m++] = i3;
            Indecies[m++] = i2;
            Indecies[m++] = i2;
            Indecies[m++] = i3;
            Indecies[m++] = i4;

            Indecies[m++] = i1;
            Indecies[m++] = i2;
            Indecies[m++] = i3;
            Indecies[m++] = i2;
            Indecies[m++] = i4;
            Indecies[m++] = i3;
        }
        mesh.triangles = Indecies;
    }
    
    float DerivativeHeightFunction(float X)
    {
        return Mathf.Sqrt(1-Sq(WormholeScript.GetPolarRadiusDerivative(X)));
    }
    float Sq(float X) => X * X;
    public Vector3 GetShape(float Angle,float Rad)
    {
        float r = WormholeScript.GetPolarRadius(Rad);
        /*float M = Mathf.Sign(Rad);
        Rad = Mathf.Abs(Rad);
        float r = WormholeScript.GetPolarRadius(Rad);
        float x = (LookuptableSize-1)*Rad / MaxRadius;
        int i = (int)x;
        x -= i;
        float h = Lerp(HeightLookupTable[i], HeightLookupTable[Mathf.Min(i+1, LookuptableSize-1)],x);*/

        return new Vector3(Mathf.Cos(Angle) * r, WormholeScript.GetHeight(Rad), Mathf.Sin(Angle) * r);
    }
    float Lerp(float a, float b, float t) => a * (1 - t) + b * t;

    // Update is called once per frame
    void Update()
    {
        
    }
}
