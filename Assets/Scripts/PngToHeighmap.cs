using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
public class PngToHeighmap : MonoBehaviour
{


    //Set read/write in advanced texture properties to true

    /*
     * 
     * Readable textures use twice as much memory as nonreadable textures because they need to have a copy of their pixel data in CPU RAM. 
     *  You should only make a texture readable when you need to, and make them nonreadable when you are done working with the data on the CPU.
    */
    public Texture2D t;


    void Start()
    {
        //t.width
        Color[] colors = t.GetPixels();
        float[][] elev = new float[t.height][];
        for (int i = 0; i < t.height; i++)   //TODO CHECK THE - 1
        {
            elev[i] = new float[t.width];
            elev[i] = colors[(i * t.width)..(((i + 1) * t.width) - 1)].Select(color => color.r).ToArray();
        }
        Debug.Log("Image Processed");
        Debug.Log("Scaling...");
        for (int i = 0; i < elev.GetLength(0); i++)
        {
            for (int j = 0; j < elev[i].Length; j++)
            {
                elev[i][j] = Scale(elev[i][j]);
            }
        }


        //foreach (float[] array in elev)
        //{
        //    foreach (float value in array)
        //    {
        //        Console.WriteLine(value.ToString());
        //    }
        //}
        Debug.Log(elev[0][256]);

    }


    public float Scale(float value)
    {

        //Value will be between 0 and 1
        float max = 8729;
        float min = -415;

        //return Mathf.FloorToInt(value*max);

        //Using  Mathf.Exp((-1 * value) / 0.11f) scaling, the inverse is:
        //Note: result will be [0,1]
        float scaledVal = -0.11f * Mathf.Log(-value + 1);

        float rescaledData = scaledVal * (max - min) + min;
        return rescaledData;
        //float normalizedData = (value - min) / (max - min);
    }

    // Update is called once per frame
    void Update()
    {
        
    }



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
