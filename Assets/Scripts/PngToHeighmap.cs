using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


class ImageScaler 
{
    //Value will be between 0 and 1
    static public float Scale(float value)
    {
        //Downsizing by x30
        float max = 291; // max is 8729;
        float min = -14;  // min is -415;

        //Note: No need to normalize, result will be [0,1], if not then normalizedData = (value - min) / (max - min);
        float lambda = 0.11f;
        float scaledVal = InverseCumulativeExponential(value, lambda);

        float rescaledData = scaledVal * (max - min) + min;
        return rescaledData;
    }

    static public float InverseCumulativeExponential(float value, float lambda)
    {
        //Using  Mathf.Exp((-1 * value) / 0.11f) scaling, the inverse is:
        return -lambda * Mathf.Log(-value + 1);

    }
}

interface IHeightData
{
    public float[][] elevation { get; set; }
    public Texture2D texture { get; set; }
    public Texture2D GetChunkOfTexture(int startX, int startY, int endX, int endY);

}


//Uses a 1 channel heighmap texture and translates into a jagged array: float[][] elevation
//Then scales the height of the data, given the fact that each point in the image has a 30km distance, and given the fact it was scaled previously.
class HeightData : IHeightData
{
    public Texture2D texture { get; set; }
    private Color[] aColors;
    public int size;

    public float[][] elevation { get; set; }

    public HeightData(Texture2D heightmap)
    {
        size = heightmap.height;
        Color[] colors = heightmap.GetPixels();
        elevation = new float[heightmap.height][];
        for (int i = 0; i < heightmap.height; i++)
        {
            elevation[i] = new float[heightmap.width];
            elevation[i] = colors[(i * heightmap.width)..(((i + 1) * heightmap.width))].Select(color => color.r).ToArray();
        }

        Debug.Log("Image Processed");
        Debug.Log("Scaling...");

        for (int i = 0; i < elevation.GetLength(0); i++)
        {
            for (int j = 0; j < elevation[i].Length; j++)
            {
                elevation[i][j] = ImageScaler.Scale(elevation[i][j]);
            }
            
        }

        aColors = heightmap.GetPixels();
        for (int i = 0; i < aColors.Length; i++)
        {
            aColors[i].r = aColors[i].r;
            aColors[i].g = aColors[i].r;
            aColors[i].b = aColors[i].r;
            aColors[i].a = 1;
        }

        texture = new Texture2D(heightmap.width, heightmap.height);

        for (int h = 0; h < heightmap.height; h++)
            for (int w = 0; w < heightmap.width; w++)
                texture.SetPixel(w, h, aColors[h * heightmap.width + w]);
        texture.Apply();

    }
    public HeightData(float[][] _elevation, Texture2D _texture)
    {
        elevation = _elevation;
        texture = _texture;
    }

    public Texture2D GetChunkOfTexture(int startX, int startY, int endX, int endY)
    {        
        //Assume squared
        var chunkSize = endX - startX;
        Debug.Log("size" + chunkSize);
        Texture2D chunkTexture = new Texture2D(chunkSize, chunkSize);


        for (int w = startX; w < endX; w++)
        {
            for (int h = startY; h < endY; h++)
            {
                var a = aColors[w * size + h];
                chunkTexture.SetPixel(h, w, a);
            }
        }
        chunkTexture.Apply();
        return chunkTexture;
    }
}


public class RGBAHeightData : IHeightData
{
    public float[][] elevation { get; set; }
    public int size;
    public Texture2D texture { get; set; }
    public Color[] textureArray;
    public Color[] rgb;
    public Color[] a;
    public Texture2D rgbTexture;
    public Texture2D aTexture;

    public RGBAHeightData(Texture2D heightmap) 
    {
        Color[] colors = heightmap.GetPixels();
        size = heightmap.height;

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
                elevation[i][j] = ImageScaler.Scale(elevation[i][j]);
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

        aTexture = new Texture2D(heightmap.width, heightmap.height);

        for (int h = 0; h < heightmap.height; h++)
            for (int w = 0; w < heightmap.width; w++)
                aTexture.SetPixel(w, h, aColors[h * heightmap.width + w]);
        aTexture.Apply();
        texture = aTexture;

        for (int i = 0; i < colors.Length; i++)
        {
            colors[i].a = 1;
        }
        rgb = colors;
        a = aColors;
        //colors.Select(color => color.a = 1);

        rgbTexture = new Texture2D(heightmap.width, heightmap.height);

        for (int h = 0; h < heightmap.height; h++)
            for (int w = 0; w < heightmap.width; w++)
                rgbTexture.SetPixel(w, h, colors[h * heightmap.width + w]);
        rgbTexture.Apply();

        SetRGBTexture();
    }

    public Texture2D GetChunkOfTexture(int startX, int startY, int endX, int endY)
    {
        //Assume squared
        var chunkSize = endX - startX;
        Debug.Log("size" + chunkSize);
        Texture2D chunkTexture= new Texture2D(chunkSize, chunkSize);


        for (int w = startX; w < endX; w++)
        {
            for (int h = startY; h < endY; h++)
            {
                var a = textureArray[w * size + h];
                chunkTexture.SetPixel(h, w, a);
            }
        }
        chunkTexture.Apply();
        return chunkTexture;
    }

    public void SetAlphaTexture()
    {
        textureArray = a;
        texture = aTexture;
    }
    public void SetRGBTexture()
    {
        textureArray = rgb;
        texture = rgbTexture;
    }

}




