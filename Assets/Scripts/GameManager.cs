using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class GameManager : MonoBehaviour
{
    //public GameObject cube;
    public void ImportTerrain()
    {
        string path = EditorUtility.OpenFilePanel("Import terrain Heightmap", "", "png");
        if (path.Length != 0)
        {
            var fileContent = File.ReadAllBytes(path);
            string now = Time.time.ToString();
            File.WriteAllBytes(Application.dataPath +"\\Temp\\"+ now+".png", fileContent);

            /* Loads png Into Texture
             * 
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(fileContent);
                cube.GetComponent<Renderer>().material.mainTexture = tex;


            //encoding to png
            var png = tex.EncodeToPNG();
            */
        }
    }
    // Start is called before the first frame update
    void Start()
    {
		// To open File Explorrere
		//string path = EditorUtility.OpenFilePanel("Import terrain Heightmap", "", "png");
		TerrainData terrainData = new TerrainData();


    }









	/*
	@MenuItem("Terrain/Heightmap From Texture")
	static void ApplyHeightmap()
	{
		Texture2D heightmap  = Selection.activeObject as Texture2D;
		if (heightmap == null)
		{
			EditorUtility.DisplayDialog("No texture selected", "Please select a texture.", "Cancel");
			return;
		}
		Undo.RegisterUndo(Terrain.activeTerrain.terrainData, "Heightmap From Texture");

		var terrain = Terrain.activeTerrain.terrainData;
		var w = heightmap.width;
		var h = heightmap.height;
		var w2 = terrain.heightmapWidth;
		var heightmapData = terrain.GetHeights(0, 0, w2, w2);
		var mapColors = heightmap.GetPixels();
		var map = new Color[w2 * w2];

		if (w2 != w || h != w)
		{
			// Resize using nearest-neighbor scaling if texture has no filtering
			if (heightmap.filterMode == FilterMode.Point)
			{
				float dx = parseFloat(w) / w2;
				float dy = parseFloat(h) / w2;
				for (y = 0; y < w2; y++)
				{
					if (y % 20 == 0)
					{
						EditorUtility.DisplayProgressBar("Resize", "Calculating texture", Mathf.InverseLerp(0.0, w2, y));
					}
					var thisY = parseInt(dy * y) * w;
					var yw = y * w2;
					for (x = 0; x < w2; x++)
					{
						map[yw + x] = mapColors[thisY + dx * x];
					}
				}
			}
			// Otherwise resize using bilinear filtering
			else
			{
				var ratioX = 1.0 / (parseFloat(w2) / (w - 1));
				var ratioY = 1.0 / (parseFloat(w2) / (h - 1));
				for (y = 0; y < w2; y++)
				{
					if (y % 20 == 0)
					{
						EditorUtility.DisplayProgressBar("Resize", "Calculating texture", Mathf.InverseLerp(0.0, w2, y));
					}
					var yy = Mathf.Floor(y * ratioY);
					var y1 = yy * w;
					var y2 = (yy + 1) * w;
					yw = y * w2;
					for (x = 0; x < w2; x++)
					{
						var xx = Mathf.Floor(x * ratioX);

						var bl = mapColors[y1 + xx];
						var br = mapColors[y1 + xx + 1];
						var tl = mapColors[y2 + xx];
						var tr = mapColors[y2 + xx + 1];

						var xLerp = x * ratioX - xx;
						map[yw + x] = Color.Lerp(Color.Lerp(bl, br, xLerp), Color.Lerp(tl, tr, xLerp), y * ratioY - yy);
					}
				}
			}
			EditorUtility.ClearProgressBar();
		}
		else
		{
			// Use original if no resize is needed
			map = mapColors;
		}

		// Assign texture data to heightmap
		for (y = 0; y < w2; y++)
		{
			for (x = 0; x < w2; x++)
			{
				heightmapData[y, x] = map[y * w2 + x].grayscale;
			}
		}
		terrain.SetHeights(0, 0, heightmapData);
	}
	*/

	// Update is called once per frame
	void Update()
    {
        
    }
}
