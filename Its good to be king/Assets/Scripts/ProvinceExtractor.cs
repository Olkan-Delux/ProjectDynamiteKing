using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProvinceExtractor : MonoBehaviour
{
    public MapTilesScriptableObject myProvicnes;
    private Dictionary<Color, Province> myProvincesByColor = new Dictionary<Color, Province>();
    public Camera cam;
    public Texture2D regionTexture1;
    public Texture2D regionTexture2;
    public Texture2D regionTexture3;
    public Texture2D regionTexture4;
    public GameObject map1;
    public GameObject map2;
    public GameObject map3;
    public GameObject map4;
    private int maxMapStates = 1;
    private int mapState = 0;
    private void Start()
    {
        for(int i = 0;  i < myProvicnes.Provinces.Count; i++)
        {
            myProvincesByColor.Add(Normalize(myProvicnes.Provinces[i].color), myProvicnes.Provinces[i]);
        }
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            mapState++;
            if(mapState > maxMapStates)
            {
                mapState = 0;
            }

            SetMapState(mapState);
        }

        //if (Input.GetMouseButton(0)) 
        {
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

            if (hit.collider != null)
            {
                SpriteRenderer sr = hit.collider.GetComponent<SpriteRenderer>();
                ColorTextureContainer ctc = hit.collider.GetComponent<ColorTextureContainer>();
                if (ctc != null)
                {
                    //Material mat = sr.material;
                    Texture2D tex = ctc.GetColorCodedTexture();


                    Vector2 localPos = hit.transform.InverseTransformPoint(hit.point);

                    Sprite sprite = sr.sprite;

                    Rect rect = sprite.textureRect;

                    float pixelsPerUnit = sprite.pixelsPerUnit;

                    // Convert to texture coordinates
                    float x = localPos.x * pixelsPerUnit + rect.width / 2;
                    float y = localPos.y * pixelsPerUnit + rect.height / 2;

                    int texX = Mathf.FloorToInt(rect.x + x);
                    int texY = Mathf.FloorToInt(rect.y + y);

                    Color color = tex.GetPixel(texX, texY);

                    SpriteRenderer map1sr = map1.GetComponent<SpriteRenderer>();
                    SpriteRenderer map2sr = map2.GetComponent<SpriteRenderer>();
                    SpriteRenderer map3sr = map3.GetComponent<SpriteRenderer>();
                    SpriteRenderer map4sr = map4.GetComponent<SpriteRenderer>();
                    map1sr.material.SetColor("_SelectedRegion", color);
                    map2sr.material.SetColor("_SelectedRegion", color);
                    map3sr.material.SetColor("_SelectedRegion", color);
                    map4sr.material.SetColor("_SelectedRegion", color);

                    //Province SelectedProvince = null;

                    //KingdomClass kingdom = transform.gameObject.GetComponent<ProvinceManager>().GetKingdomFromColor(color);

                    //if(kingdom != null)
                    //{
                    //    Debug.Log(kingdom.kingdomId);
                        
                    //}

                    //Debug.Log(color);
                }
            }
        }
    }

    public void SetMapState(int mapState)
    {
        SpriteRenderer map1sr = map1.GetComponent<SpriteRenderer>();
        SpriteRenderer map2sr = map2.GetComponent<SpriteRenderer>();
        SpriteRenderer map3sr = map3.GetComponent<SpriteRenderer>();
        SpriteRenderer map4sr = map4.GetComponent<SpriteRenderer>();
        map1sr.material.SetFloat("_MapState", mapState);
        map2sr.material.SetFloat("_MapState", mapState);
        map3sr.material.SetFloat("_MapState", mapState);
        map4sr.material.SetFloat("_MapState", mapState);
    }

    public void SetProvinceCount(float provinceCount)
    {
        SpriteRenderer map1sr = map1.GetComponent<SpriteRenderer>();
        SpriteRenderer map2sr = map2.GetComponent<SpriteRenderer>();
        SpriteRenderer map3sr = map3.GetComponent<SpriteRenderer>();
        SpriteRenderer map4sr = map4.GetComponent<SpriteRenderer>();
        map1sr.material.SetFloat("_ProvinceCount", provinceCount);
        map2sr.material.SetFloat("_ProvinceCount", provinceCount);
        map3sr.material.SetFloat("_ProvinceCount", provinceCount);
        map4sr.material.SetFloat("_ProvinceCount", provinceCount);
    }

    public void SetLookUpTexture(Texture2D aTexture)
    {
        SpriteRenderer map1sr = map1.GetComponent<SpriteRenderer>();
        SpriteRenderer map2sr = map2.GetComponent<SpriteRenderer>();
        SpriteRenderer map3sr = map3.GetComponent<SpriteRenderer>();
        SpriteRenderer map4sr = map4.GetComponent<SpriteRenderer>();
        map1sr.material.SetTexture("_ProvinceToKingdomTex", aTexture);
        map2sr.material.SetTexture("_ProvinceToKingdomTex", aTexture);
        map3sr.material.SetTexture("_ProvinceToKingdomTex", aTexture);
        map4sr.material.SetTexture("_ProvinceToKingdomTex", aTexture);
    }

    public void SetKingdomSize(float aSize)
    {
        SpriteRenderer map1sr = map1.GetComponent<SpriteRenderer>();
        SpriteRenderer map2sr = map2.GetComponent<SpriteRenderer>();
        SpriteRenderer map3sr = map3.GetComponent<SpriteRenderer>();
        SpriteRenderer map4sr = map4.GetComponent<SpriteRenderer>();
        map1sr.material.SetFloat("_KingdomCount", aSize);
        map2sr.material.SetFloat("_KingdomCount", aSize);
        map3sr.material.SetFloat("_KingdomCount", aSize);
        map4sr.material.SetFloat("_KingdomCount", aSize);
    }

    public static Color Normalize(Color c)
    {
        return new Color(
            Mathf.Round(c.r * 255) / 255f,
            Mathf.Round(c.g * 255) / 255f,
            Mathf.Round(c.b * 255) / 255f,
            Mathf.Round(c.a * 255) / 255f
        );
    }
}
