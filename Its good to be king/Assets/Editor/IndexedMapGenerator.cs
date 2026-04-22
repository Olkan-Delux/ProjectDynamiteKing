#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class IndexedMapGenerator : MonoBehaviour
{

    [MenuItem("Assets/Map Tools/Create Province Index Map")]
    public static void GenerateIndexMap()
    {
        MapTilesScriptableObject map = AssetDatabase.LoadAssetAtPath<MapTilesScriptableObject>("Assets/Scriptable Objects/Provinces.asset");
        // Get selected texture
        Texture2D regionTex = Selection.activeObject as Texture2D;

        if (regionTex == null)
        {
            Debug.LogError("Select a Region Texture first!");
            return;
        }

        string path = AssetDatabase.GetAssetPath(regionTex);
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

        if (importer != null)
        {
            importer.isReadable = true;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.filterMode = FilterMode.Point;
            importer.SaveAndReimport();
        }

        Color32[] pixels = regionTex.GetPixels32();

        int width = regionTex.width;
        int height = regionTex.height;

        Dictionary<Color32, int> provinceToIndex = new Dictionary<Color32, int>();
        List<Color32> indexToProvince = new List<Color32>();

        Texture2D indexTex = new Texture2D(width, height, TextureFormat.RFloat, false);

        //int nextIndex = 0;
        Dictionary<Color, Province> myProvincesByColor = new Dictionary<Color, Province>();
        //Dictionary<int, Province> myProvincesByIndex = new Dictionary<int, Province>();
        for (int i = 0; i < map.Provinces.Count; i++)
        {
            //myProvincesByIndex.Add(map.Provinces[i].id, map.Provinces[i]);
            myProvincesByColor.Add(Normalize(map.Provinces[i].color), map.Provinces[i]);
            Debug.Log(map.Provinces[i].color);
        }


        for (int i = 0; i < pixels.Length; i++)
        {


            Color32 c = pixels[i];

            int x = i % width;
            int y = i / width;
            if (myProvincesByColor.TryGetValue(Normalize(c), out Province province))
            {
                
                indexTex.SetPixel(x, y, new Color(province.id / 255f, 0, 0, 1));
                //if(myProvincesByIndex.TryGetValue(province.id, out Province idProvince))
                //{
                //    indexTex.SetPixel(x, y, new Color(idProvince.color.r, idProvince.color.g, idProvince.color.b, 1));
                //}
            }
            else
            {

                indexTex.SetPixel(x, y, new Color(0, 0, 0, 0));
            }

        }

        indexTex.Apply();

        // Save as asset next to original texture
        string directory = Path.GetDirectoryName(path);
        string newPath = directory + "/" + regionTex.name + "_Index.png";

        byte[] png = indexTex.EncodeToPNG();
        File.WriteAllBytes(newPath, png);

        AssetDatabase.Refresh();

        Debug.Log("Province index map created at: " + newPath);
    }

    // Make it appear only when right-clicking textures
    [MenuItem("Assets/Map Tools/Create Province Index Map", true)]
    private static bool Validate()
    {
        return Selection.activeObject is Texture2D;
    }
    static Color Normalize(Color c)
    {
        return new Color(
            Mathf.Round(c.r * 255) / 255f,
            Mathf.Round(c.g * 255) / 255f,
            Mathf.Round(c.b * 255) / 255f,
            Mathf.Round(c.a * 255) / 255f
        );
    }
}
