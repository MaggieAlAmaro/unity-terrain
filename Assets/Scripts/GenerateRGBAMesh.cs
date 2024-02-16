using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Jobs;







public class RGBAHeightData
{
    public float[][] elevation;
    public Texture2D rgbTexture;
    public Texture2D aTexture;

    public RGBAHeightData(Texture2D heightmap)
    {
        Color[] colors = heightmap.GetPixels();

        elevation = new float[heightmap.height][];
        for (int i = 0; i < heightmap.height; i++)   
        {
            elevation[i] = new float[heightmap.width];
            elevation[i] = colors[(i * heightmap.width)..(((i + 1) * heightmap.width))].Select(color => color.a).ToArray();
        }
        Debug.Log("Image Processed");
        Debug.Log("Scaling...");
        for (int i = 0; i < elevation.GetLength(0); i++)
        {
            for (int j = 0; j < elevation[i].Length; j++)
            {
                elevation[i][j] = Scale(elevation[i][j]);
            }
        }


        Color[] aColors = heightmap.GetPixels();
        for (int i = 0; i < aColors.Length; i++)
        {
            aColors[i].r = aColors[i].a;
            aColors[i].g = aColors[i].a;
            aColors[i].b = aColors[i].a;
            aColors[i].a = 1;
        }

        Debug.Log(aColors[7]);

        aTexture = new Texture2D(heightmap.width, heightmap.height);

        for (int h = 0; h < heightmap.height; h++)
            for (int w = 0; w < heightmap.width; w++)
                aTexture.SetPixel(w, h, aColors[h * heightmap.width + w]);
        aTexture.Apply();


        for (int i = 0; i < colors.Length; i++)
        {
            colors[i].a = 1;
        }

        //colors.Select(color => color.a = 1);

        rgbTexture = new Texture2D(heightmap.width, heightmap.height);

        for (int h = 0; h < heightmap.height; h++)
            for (int w = 0; w < heightmap.width; w++)
                rgbTexture.SetPixel(w, h, colors[h * heightmap.width + w]);
        rgbTexture.Apply();



    }


    //Value will be between 0 and 1
    private float Scale(float value)
    {
        //Downsizing by x30
        float max = 291; //  8729;
        float min = -14;  // -415;

        //Using  Mathf.Exp((-1 * value) / 0.11f) scaling, the inverse is:
        float scaledVal = -0.11f * Mathf.Log(-value + 1);
        //Note: No need to normalize, result will be [0,1], if not then normalizedData = (value - min) / (max - min);

        float rescaledData = scaledVal * (max - min) + min;
        return rescaledData;
    }

}


[RequireComponent(typeof(MeshFilter))]
public class GenerateRGBAMesh : MonoBehaviour
{

    [SerializeField] private Texture2D heightmap;


    private Vector3[] vertices;
    private int[] triangles;
    private Vector2[] UVs;
    private RGBAHeightData data;

    public bool RGBTexture;

    //[SerializeField]
    private int height;
    //[SerializeField]
    private int width;
    [SerializeField] private int threads;
    private int chuckSize;
    private int chunkExtra;


    private Mesh mesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Material mat;

    private bool textureState;
    int currentTriangle;

    void Start()
    {

        mat = GetComponent<Material>();
        currentTriangle = 0;
        mesh = new Mesh();
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter.mesh = mesh;

        height = heightmap.height;
        width = heightmap.width;

        //ASSUMES height == width
        chuckSize = height / threads;
        chunkExtra = height % threads;


        vertices = new Vector3[height * width];
        triangles = new int[(height - 1) * (width - 1) * 6];
        UVs = new Vector2[width * height];

        data = new RGBAHeightData(heightmap);

        textureState = RGBTexture;
        if (!RGBTexture)
            meshRenderer.material.SetTexture("_MainTex", data.aTexture);
        else
            meshRenderer.material.SetTexture("_MainTex", data.rgbTexture);
        DrawMesh();
    }

    public JobHandle DrawMeshJob()
    {
        MeshJob job = new MeshJob();
        return job.Schedule();
    }


    void Update()
    {
        if(RGBTexture != textureState)
        {
            if (!RGBTexture)
                meshRenderer.material.SetTexture("_MainTex", data.aTexture);
            else
                meshRenderer.material.SetTexture("_MainTex", data.rgbTexture);
            textureState = RGBTexture;
        }
        //JobHandle jhandle = DrawMeshJob();
        //jhandle.Complete();
        /*
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;   

        for (var i = 0; i < vertices.Length; i++)
        {
            vertices[i] += normals[i] * Mathf.Sin(Time.time);
        }

        mesh.vertices = vertices;
        */
    }

    public void AddTriangle(int a, int b, int c)
    {
        triangles[currentTriangle] = a;
        triangles[currentTriangle + 1] = b;
        triangles[currentTriangle + 2] = c;
        currentTriangle += 3;
    }

    public void DrawVertices()
    {
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                vertices[height * j + i] = new Vector3(i, data.elevation[j][i], j);
                UVs[height * j + i] = new Vector2(i / (float)width, j / (float)height);
            }
        }
    }

    public void DrawVerticesCentered()
    {

        float offsetZ = (height - 1) / 2f;
        float offsetX = (width - 1) / 2f;
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                vertices[height * j + i] = new Vector3(offsetX - i, data.elevation[j][i], offsetZ - j);
                UVs[height * j + i] = new Vector2(i / (float)width, j / (float)height);
            }
        }
    }

    public void DrawTriangles()
    {
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                if (j < height - 1 && i < width - 1)
                {
                    int currVert = height * j + i;
                    AddTriangle(currVert, currVert + width, currVert + width + 1);
                    AddTriangle(currVert, currVert + width + 1, currVert + 1);
                }

            }
        }
    }

    void DrawMesh()
    {
        DrawVerticesCentered();
        DrawTriangles();

        mesh.Clear();
        //mesh = new Mesh();

        mesh.vertices = vertices;
        mesh.triangles = triangles; 
        mesh.uv = UVs;
        mesh.RecalculateNormals();
    }

    /*private void OnDrawGizmos()
    {

        if (vertices == null)
        {
            return;
        } 
        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], .1f);
        }   
    }*/
    
}
