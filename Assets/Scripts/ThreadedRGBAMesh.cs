using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Jobs;
using UnityEngine.Rendering;
using Unity.Collections;


public struct MeshJob : IJob
{

    Vector3[] _vertices;
    int[] _triangles;
    int _myIndexX;
    int _myIndexY;
    int _mySizeX;
    int _mySizeY;

    public MeshJob(
        Vector3[] vertices,
        int[] triangles,
        int myIndexX,
        int myIndexY,
        int mySizeX,
        int mySizeY
        )
    {
        _vertices = vertices;
        _triangles = triangles;
        _myIndexX = myIndexX;
        _myIndexY = myIndexY;
        _mySizeX = mySizeX;
        _mySizeY = mySizeY;
    }
    public void Execute()
    {
        Camera c = Camera.main;

    }
}





[RequireComponent(typeof(MeshFilter))]
public class ThreadedRGBAMesh : MonoBehaviour
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
    [SerializeField] private int chuckSize;
    private int chunkExtra;
    private int chunks;

    private Mesh mesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Material mat;

    private bool textureState;
    int currentTriangle;
    private Mesh.MeshDataArray meshArray;

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
        if(chuckSize > 256)
        {
            Debug.LogWarning("Unity Doesn't Support Chunks bigger than 256. Max of 65535 vertices per mesh");
            chuckSize = 256;
        }
        chunks = height / chuckSize;
        //chunkExtra = height % threads;


        vertices = new Vector3[height * width];
        triangles = new int[(height - 1) * (width - 1) * 6];
        UVs = new Vector2[width * height];

        data = new RGBAHeightData(heightmap);

        meshArray = Mesh.AllocateWritableMeshData(1);
    }

    public JobHandle DrawMeshJob()
    {
        MeshJob job = new MeshJob();
        return job.Schedule();
    }


    void Update()
    {

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

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    struct Vertex
    {
        public Vector3 pos;
        public Vector2 uv;

        public Vertex(Vector3 _pos, Vector2 _uv)
        {
            pos = _pos;
            uv = _uv;
        }
    }

    //Possibly, use indexes as args and not the whole vertexPos array
    // https://forum.unity.com/threads/simple-mesh-creation-fails-while-setting-a-submesh.1513712/
    public void DrawVerticesCentered(float[][] vertices, int threadNbr)
    {

        var _mesh = meshArray[threadNbr];
        _mesh.SetVertexBufferParams(chuckSize * chuckSize,
          new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3),
          new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2),
          new VertexAttributeDescriptor(VertexAttribute.Normal, stream: 1));

        var t = _mesh.GetVertexData<Vertex>(0);
        
        float offsetZ = (height - 1) / 2f;
        float offsetX = (width - 1) / 2f;
        for (int j = 0; j < chuckSize; j++)
        {
            for (int i = 0; i < chuckSize; i++)
            {
                t[height * j + i] = new Vertex(new Vector3(offsetX - i, vertices[j][i], offsetZ - j), new Vector2(i / (float)width, j / (float)height));
            }
        }

        _mesh.SetIndexBufferParams(chuckSize * chuckSize, IndexFormat.UInt16);
        var ib = _mesh.GetIndexData<ushort>();
        for (ushort i = 0; i < chuckSize * chuckSize; ++i)
            ib[i] = i;

        // ITHINK index buffer is fucked because it crashes when _mesh.subMeshCount = 1; which has to do with ib
        _mesh.subMeshCount = 1;
        _mesh.SetSubMesh(0, new SubMeshDescriptor(0, ib.Length));

        mesh.Clear();
        Mesh.ApplyAndDisposeWritableMeshData(meshArray, mesh);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    
}
