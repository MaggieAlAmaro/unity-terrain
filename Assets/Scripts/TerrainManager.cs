using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;


public class ImportPNG
{
    public static Texture2D LoadPNG(string filePath)
    {

        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //WILL LOAD IN ARGB MODE 32 BIT!!!!!
        }
        return tex;
    }
}

public class TerrainManager : MonoBehaviour
{

    [SerializeField] private Texture2D heightmap;
    private int width;
    private int height;

    private bool isRGBA;
    private bool canToggleTexture;
    private bool textureState;
    [SerializeField] private bool displayRGBTexture;
    

    [SerializeField] private int threads;
    [SerializeField] private int chunkSize = 256;
    //private int chunkExtra;
    private int chunkRows;
    private int chunks;

    private IHeightData heightData;

    private GameObject[,] terrainObjs;



    void Start()
    {
        if(heightmap.format == TextureFormat.BC4)
        {
            heightData = new HeightData(heightmap);
            isRGBA = false;
            canToggleTexture = false;
        }
        else
        {
            
            heightData = new RGBAHeightData(heightmap);
            canToggleTexture = true; //TOGGLE TEXTURE ONLY POSSIBLE IF IS RGBA IMAGE
            isRGBA = true;
        }
        textureState = true;


       height = heightmap.height;
        width = heightmap.width;


        //ASSUMES height == width
        if (chunkSize > 256)
        {
            Debug.LogWarning("Unity Doesn't Support Chunks bigger than 256. Max of 65535 vertices per mesh");
            chunkSize = 256;
        }
        chunkRows = height / chunkSize;
        chunks = (int)Mathf.Pow(chunkRows, 2.0f);


        SplitIntoChunks();


    }


    void SplitIntoChunks()
    {
        terrainObjs = new GameObject[chunkRows, chunkRows];
        for (int i = 0; i < chunkRows; i++)
            for (int j = 0; j < chunkRows; j++)
                terrainObjs[i,j] = GenerateMeshObject();
            


        for (int i = 0; i < Mathf.Sqrt(chunks) * chunkSize; i += chunkSize)
        {
            for (int j = 0; j < Mathf.Sqrt(chunks) * chunkSize; j += chunkSize)
            {
                var tex = heightData.GetChunkOfTexture(i, j, i + chunkSize, j + chunkSize);

                float[][] subArr = heightData.elevation.Skip(i).Take(chunkSize).Select(
                    (each_row) => each_row.Skip(j).Take(chunkSize).ToArray()).ToArray();

                MeshData meshData = new MeshData(subArr);

                int indI = i / chunkSize;
                int indJ = j / chunkSize;
                terrainObjs[indI, indJ].transform.position = new Vector3(-j, 0, -i);
                //terrainObjs[tIndex].transform.rotation = Quaternion.Euler(0,180,0);
                terrainObjs[indI, indJ].GetComponent<MeshFilter>().mesh = meshData.GetMesh();
                terrainObjs[indI, indJ].GetComponent<MeshRenderer>().material.SetTexture("_MainTex", tex);
            }
        }

    }

    void UpdateTexture()
    {
        for (int i = 0; i < Mathf.Sqrt(chunks) * chunkSize; i += chunkSize)
        {
            for (int j = 0; j < Mathf.Sqrt(chunks) * chunkSize; j += chunkSize)
            {
                var tex = heightData.GetChunkOfTexture(i, j, i + chunkSize, j + chunkSize);
                terrainObjs[i / chunkSize, j/ chunkSize].GetComponent<MeshRenderer>().material.SetTexture("_MainTex", tex);

            }
        }
    }





    void Update()
    {
        if(canToggleTexture)
        {

            if (displayRGBTexture != textureState)
            {
                var data = (RGBAHeightData)heightData;
                if (!displayRGBTexture)
                {
                    data.SetAlphaTexture();
                    UpdateTexture();
                    //meshRenderer.material.SetTexture("_MainTex", data.aTexture);
                }
                else
                {
                    data.SetRGBTexture();
                    UpdateTexture();
                    //meshRenderer.material.SetTexture("_MainTex", data.rgbTexture);
                }
                textureState = displayRGBTexture;
            }
        
        }
         

    }



    private GameObject GenerateMeshObject()
    {
        GameObject newMesh = new GameObject();
        newMesh.transform.position = new Vector3(0, 0, 0);
        newMesh.AddComponent<MeshFilter>();
        newMesh.AddComponent<MeshRenderer>();
        newMesh.transform.parent = this.transform;
        //terrainObjs[currentTerrainIdx++] = newMesh;
        return newMesh;
    }

}



public class MeshData
{
    private Vector3[] vertices;
    private int[] triangles;
    private Vector2[] UVs;
    private int nbrVertices;
    private int chunkSize;
    private int height;
    private int width;
    private int currentTriangle;

    public MeshData(float[][] meshArray)
    {
        height = meshArray.GetLength(0);
        width = height;    // meshArray.GetLength(1); -> DOESNT WORK BECAUSE ARRAY IS JAGGED
        nbrVertices = height * width;

        chunkSize = height;
        currentTriangle = 0;


        vertices = new Vector3[height * width];
        triangles = new int[(height - 1) * (width - 1) * 6];
        UVs = new Vector2[width * height];

        SetVerticesAndUVsCentered(0, 0, meshArray);
        SetTriangles();

    }

    public Mesh GetMesh()
    {

        var mesh = new Mesh();
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = UVs;
        mesh.RecalculateNormals();
        return mesh;
    }

    public void SetVerticesAndUVsCentered(int xStart, int zStart, float[][] splitArray)
    {
        float offsetZ = (chunkSize - 1) / 2f;
        float offsetX = (chunkSize - 1) / 2f;
        for (int j = 0; j <  chunkSize; j++)
        {
            for (int i = 0; i <  chunkSize; i++)
            {
                vertices[chunkSize * j + i] = new Vector3(offsetX - i, splitArray[j][i], offsetZ - j);
                UVs[chunkSize * j + i] = new Vector2(i / (float)chunkSize, j / (float)chunkSize);
            }
        }
    }

    public void AddTriangle(int a, int b, int c)
    {
        triangles[currentTriangle] = a;
        triangles[currentTriangle + 1] = b;
        triangles[currentTriangle + 2] = c;
        currentTriangle += 3;
    }

    public void SetTriangles()
    {
        for (int j = 0; j <  chunkSize; j++)
        {
            for (int i = 0; i <  chunkSize; i++)
            {
                if (j < chunkSize - 1 && i <  chunkSize - 1)
                {
                    int currVert = chunkSize * j + i;
                    AddTriangle(currVert, currVert + chunkSize, currVert + chunkSize + 1);
                    AddTriangle(currVert, currVert + chunkSize + 1, currVert + 1);
                }

            }
        }
    }
}