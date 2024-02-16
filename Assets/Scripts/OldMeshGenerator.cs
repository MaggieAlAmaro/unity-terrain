using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Jobs;








[RequireComponent(typeof(MeshFilter))]
public class OldMeshGenerator : MonoBehaviour
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


    int currentTriangle;


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
        Mesh mesh = new Mesh();

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
