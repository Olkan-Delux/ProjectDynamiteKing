using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using csDelaunay;

public class Map : MonoBehaviour
{
    public int PixelWidth;
    public int PixelHeight;
    public int PolygonNumber;
    public float scale = 20;
    public float WaterLevel = 0.3f;
    public float MapLockYOffset = 0.0f;
    public float MapLockXOffset = 0.0f;
    public int BorderThickness = 8;
    private Texture2D myTexture;
    private Renderer myRenderer;
    private Voronoi myVoronoi;
    private Rectf myBounds;
    private CellData[] myCells;
    private bool trackMouse = false;
    private bool hasStartedTrackingMouse = false;
    private bool hasMovedNouse = false;
    private Vector2 LastMousePos;
    private Vector2 mouseDirection;
    private float mouseSpeed = 1.0f;
    private int myStartSiteIndex = 0;
    private float cameraLockTop = 0;
    private float cameraLockBottom = 0;
    private float cameraLockLeft = 0;
    private float cameraLockRight = 0;

    public enum LandTypes
    {
        Water,
        Land,
        Beach,
        Mountain
    }

    public struct CellData
    {
        public Land myLand;
        public Color myColor;
        public LandTypes aLandType;
        public int Index;
        public int CordIndex;
        public int distributionIndex;
        public bool isSeen;
        public bool isOwned;
        public List<Vector2> myPixels;
        public Character Owner;
        public int kingdomIndex;
    }


    void Start()
    {
        myTexture = new Texture2D(PixelWidth, PixelHeight);
        GenerateVoronoi();
        CreateGameConditions();
        myRenderer = GetComponent<Renderer>();
        myRenderer.material.mainTexture = myTexture;
        GameHub.Instance.CreateKingdoms();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            trackMouse = true;
            hasStartedTrackingMouse = true;
            hasMovedNouse = false;
        }
        if (Input.GetMouseButtonUp(0))
        {
            trackMouse = false;
            if(hasMovedNouse == false)
            {
                if (UIHub.Instance.GetLandUI().GetMenu().GetComponent<DynamicMenu>().GetComponent<UIMenu>().GetIsClosed() == true)
                {
                    Debug.Log("Opened Menu");
                    OpenMapMenu();
                }
            }
        }
        if (trackMouse)
        {
            if (hasStartedTrackingMouse)
            {
                LastMousePos = Input.mousePosition;
                hasStartedTrackingMouse = false;
            }
            Vector2 currentMousePos = Input.mousePosition;
            float mouseDistance = Vector2.Distance(currentMousePos, LastMousePos);
            mouseSpeed = mouseDistance *  Camera.main.orthographicSize;
            mouseSpeed /= 1080;
            mouseDirection = (currentMousePos - LastMousePos).normalized;
            if(mouseSpeed > 0)
            {
                hasMovedNouse = true;
            }

            LastMousePos = currentMousePos;
        }
        else
        {
            mouseSpeed = 0;
        }
        Camera.main.transform.position = new Vector3(Camera.main.transform.position.x - (mouseDirection.x * mouseSpeed), Camera.main.transform.position.y - (mouseDirection.y * mouseSpeed), Camera.main.transform.position.z);

        if (Camera.main.transform.position.x > (cameraLockRight) && mouseDirection.x < 0)
        {
            Camera.main.transform.position = new Vector3((cameraLockRight), Camera.main.transform.position.y, Camera.main.transform.position.z);
        }
        if(Camera.main.transform.position.x < (cameraLockLeft) && mouseDirection.x > 0)
        {
            Camera.main.transform.position = new Vector3((cameraLockLeft), Camera.main.transform.position.y, Camera.main.transform.position.z);
        }


        if (Camera.main.transform.position.y > (cameraLockTop) && mouseDirection.y < 0)
        {
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, (cameraLockTop), Camera.main.transform.position.z);
        }
        if(Camera.main.transform.position.y < (cameraLockBottom) && mouseDirection.y > 0)
        {
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, (cameraLockBottom), Camera.main.transform.position.z);
        }
    }

    public CellData[] GetCells()
    {
        return myCells;
    }

    public Voronoi GetVoronoi()
    {
        return myVoronoi;
    }

    public void CreateGameConditions()
    {
        do
        {
            myStartSiteIndex = Random.Range(0, PolygonNumber);
        }
        while (myCells[myStartSiteIndex].aLandType == LandTypes.Water);
        
        Site spawnSite = myVoronoi.SitesIndexedByLocation[myVoronoi.SiteCoords()[myStartSiteIndex]];
        SeeCell(spawnSite.SiteIndex);
        foreach(Site site in spawnSite.NeighborSites())
        {
            SeeCell(site.SiteIndex);
        }
        Vector2 spawnSiteRealPos = new Vector2((transform.position.x - (transform.localScale.x * 0.5f)) + (transform.localScale.x * (myVoronoi.SiteCoords()[spawnSite.SiteIndex].x / PixelWidth)), 
                                               (transform.position.y - (transform.localScale.y * 0.5f)) + (transform.localScale.y * (myVoronoi.SiteCoords()[spawnSite.SiteIndex].y / PixelHeight)));

        Camera.main.transform.position = new Vector3(spawnSiteRealPos.x, spawnSiteRealPos.y, Camera.main.transform.position.z);
        CalculateScreenSize();
    }

    public void CreateDebugObjectOnCell(int cellIndex, Color aColor, string aName)
    {
        Vector2 spawnSiteRealPos = new Vector2((transform.position.x - (transform.localScale.x * 0.5f)) + (transform.localScale.x * (myVoronoi.SiteCoords()[cellIndex].x / PixelWidth)),
                                   (transform.position.y - (transform.localScale.y * 0.5f)) + (transform.localScale.y * (myVoronoi.SiteCoords()[cellIndex].y / PixelHeight)));

        UIHub.Instance.CreateDebugObject(spawnSiteRealPos, aColor, aName);
    }

    public void DrawBorders(Kingdom aKingdom)
    {
        foreach (CellData cell in aKingdom.myLands)
        {
            CreateDebugObjectOnCell(cell.CordIndex, aKingdom.myColor, myCells[cell.Index].Owner.myName + " " + cell.Index);


            int index = 0;
            foreach(Site site in myVoronoi.SitesIndexedByLocation[myVoronoi.SiteCoords()[cell.CordIndex]].NeighborSites())
            {
                if(myCells[site.SiteIndex].isOwned)
                {
                    if(myCells[site.SiteIndex].Owner != aKingdom.myOwner)
                    {
                        if (index < myVoronoi.SitesIndexedByLocation[myVoronoi.SiteCoords()[cell.CordIndex]].Edges.Count)
                        {
                            if(myVoronoi.SitesIndexedByLocation[myVoronoi.SiteCoords()[cell.CordIndex]].Edges[index].ClippedEnds != null)
                            {
                                DrawLine(myVoronoi.SitesIndexedByLocation[myVoronoi.SiteCoords()[cell.CordIndex]].Edges[index].ClippedEnds[LR.LEFT], myVoronoi.SitesIndexedByLocation[myVoronoi.SiteCoords()[cell.CordIndex]].Edges[index].ClippedEnds[LR.RIGHT]);
                            }
                        }
                    }
                }
                index++;
            }
        }
        myTexture.Apply();
    }

    private int DrawLine(Vector2f p0, Vector2f p1)
    {
        int x0 = (int)p0.x;
        int y0 = (int)p0.y;
        int x1 = (int)p1.x;
        int y1 = (int)p1.y;

        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        int pixelAmount = 0;

        while (true)
        {
            for (int i = 0; i < BorderThickness; i++)
            {
                myTexture.SetPixel(x0 + i, y0, Color.black);
            }

            if (x0 == x1 && y0 == y1) break;
            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
            pixelAmount++;
        }
        return pixelAmount;
    }

    private void CalculateScreenSize()
    {
        if (Camera.main.orthographicSize < 6)
        {
            Vector2f firstIndexPos = myVoronoi.SiteCoords()[myStartSiteIndex];
            Vector2f EasternMostIndex = firstIndexPos;
            Vector2f WesterMostIndex = firstIndexPos;
            Vector2f NortherMostIndex = firstIndexPos;
            Vector2f SouthernMostIndex = firstIndexPos;
            foreach(KeyValuePair<Vector2f, Site> kv in myVoronoi.SitesIndexedByLocation)
            {
                if(myCells[kv.Value.SiteIndex].isSeen)
                {
                    if(EasternMostIndex.x < kv.Key.x)
                    {
                        EasternMostIndex = kv.Key;
                    }
                    if (WesterMostIndex.x > kv.Key.x)
                    {
                        WesterMostIndex = kv.Key;
                    }
                    if (NortherMostIndex.y > kv.Key.y)
                    {
                        NortherMostIndex = kv.Key;
                    }
                    if (SouthernMostIndex.y < kv.Key.y)
                    {
                        SouthernMostIndex = kv.Key;
                    }
                }
            }
            Vector2 uEasternMostIndex = new Vector2(EasternMostIndex.x, EasternMostIndex.y);
            Vector2 uWesterMostIndex = new Vector2(WesterMostIndex.x, WesterMostIndex.y);
            Vector2 uNortherMostIndex = new Vector2(NortherMostIndex.x, NortherMostIndex.y);
            Vector2 uSouthernMostIndex = new Vector2(SouthernMostIndex.x, SouthernMostIndex.y);

            float weDist = Vector2.Distance(uEasternMostIndex, uWesterMostIndex) * 1.5f;
            float nsDist = Vector2.Distance(uNortherMostIndex, uSouthernMostIndex) * 1.5f;
            if (weDist > nsDist)
            {
                Camera.main.orthographicSize = transform.localScale.x * (weDist / PixelWidth);
            }
            else
            {
                Camera.main.orthographicSize = transform.localScale.y * (nsDist / PixelHeight);
            }

            if(Camera.main.orthographicSize > 6)
            {
                Camera.main.orthographicSize = 6;
                
            }

            cameraLockRight = new Vector2((transform.position.x - (transform.localScale.x * 0.5f)) + (transform.localScale.x * (uEasternMostIndex.x / PixelWidth)),
                                               (transform.position.y - (transform.localScale.y * 0.5f)) + (transform.localScale.y * (uEasternMostIndex.y / PixelHeight))).x;

            cameraLockLeft = new Vector2((transform.position.x - (transform.localScale.x * 0.5f)) + (transform.localScale.x * (uWesterMostIndex.x / PixelWidth)),
                                   (transform.position.y - (transform.localScale.y * 0.5f)) + (transform.localScale.y * (uWesterMostIndex.y / PixelHeight))).x;

            cameraLockTop = new Vector2((transform.position.x - (transform.localScale.x * 0.5f)) + (transform.localScale.x * (uSouthernMostIndex.x / PixelWidth)),
                                   (transform.position.y - (transform.localScale.y * 0.5f)) + (transform.localScale.y * (uSouthernMostIndex.y / PixelHeight))).y;

            cameraLockBottom = new Vector2((transform.position.x - (transform.localScale.x * 0.5f)) + (transform.localScale.x * (uNortherMostIndex.x / PixelWidth)),
                                   (transform.position.y - (transform.localScale.y * 0.5f)) + (transform.localScale.y * (uNortherMostIndex.y / PixelHeight))).y;


            if (cameraLockRight > Camera.main.transform.position.x + (transform.localScale.x * 0.5f) - (Camera.main.orthographicSize * 0.5f))
            {
                cameraLockRight = Camera.main.transform.position.x + (transform.localScale.x * 0.5f) - (Camera.main.orthographicSize * 0.5f);
            }
            if (cameraLockLeft < Camera.main.transform.position.x - (transform.localScale.x * 0.5f) + (Camera.main.orthographicSize * 0.5f))
            {
                cameraLockLeft = Camera.main.transform.position.x - (transform.localScale.x * 0.5f) + (Camera.main.orthographicSize * 0.5f);
            }
            if (cameraLockTop > Camera.main.transform.position.y + (transform.localScale.y * 0.5f) - (Camera.main.orthographicSize * 0.5f))
            {
                cameraLockTop = Camera.main.transform.position.y + (transform.localScale.y * 0.5f) - (Camera.main.orthographicSize * 0.5f);
            }
            if (cameraLockRight < Camera.main.transform.position.y - (transform.localScale.y * 0.5f) + (Camera.main.orthographicSize * 0.5f))
            {
                cameraLockRight = Camera.main.transform.position.y - (transform.localScale.y * 0.5f) + (Camera.main.orthographicSize * 0.5f);
            }
        }
    }

    private void GenerateVoronoi()
    {
        List<Vector2f> randomPoints = CreateRandomPoint();

        myBounds = new Rectf(0, 0, PixelWidth, PixelHeight);

        myVoronoi = new Voronoi(randomPoints, myBounds, 5);
        myCells = new CellData[myVoronoi.SiteCoords().Count];
        for(int i = 0; i < myCells.Length; i++)
        {
            myCells[i].myPixels = new List<Vector2>();
            myCells[i].isOwned = false;
        }
        
        GenerateMapCells();
        DisplayVoronoiDiagram();
    }

    public Land GetLandFromIndex(int anIndex)
    {
        return myCells[anIndex].myLand;
    }

    private List<Vector2f> CreateRandomPoint()
    {
        List<Vector2f> points = new List<Vector2f>();
        for (int i = 0; i < PolygonNumber; i++)
        {
            points.Add(new Vector2f(Random.Range(0, PixelWidth), Random.Range(0, PixelHeight)));
        }

        return points;
    }

    private void DisplayVoronoiDiagram()
    {
        foreach (KeyValuePair<Vector2f, Site> kv in myVoronoi.SitesIndexedByLocation)
        {
            List<Vector2f> edges = new List<Vector2f>();
            foreach(Edge edge in kv.Value.Edges)
            {
                if(edge.ClippedEnds != null)
                {
                    edges.Add(edge.ClippedEnds[LR.LEFT]);
                    edges.Add(edge.ClippedEnds[LR.RIGHT]);
                }
            }

            Vector2f highestPoint = edges[0];
            foreach (Vector2f edge in edges)
            {
                if(edge.y > highestPoint.y)
                {
                    highestPoint = edge;
                }
            }
            
            CheckLowerDecks(highestPoint, kv.Value.SiteIndex, CreateConvexHull(edges));
        }

        myTexture.Apply();
    }

    private int CheckLowerDecks(Vector2f pos, int siteIndex, List<Vector2f> edges)
    {
        bool hasStopped = false;
        float xPos = pos.x;
        int yPos = 0;
        int tries = 0;
        while (!hasStopped)
        {
            tries++;
            hasStopped = !GetPixelsFromSite(edges, new Vector2f(xPos, pos.y + yPos));
            if(hasStopped)
            {
                Vector2f lowestPoint = edges[0];
                foreach(Vector2f edge in edges)
                {
                    if(lowestPoint.y > edge.y)
                    {
                        lowestPoint = edge;
                    }
                }

                xPos = lowestPoint.x;
                hasStopped = !GetPixelsFromSite(edges, new Vector2f(xPos, pos.y + yPos));
            }
            myTexture.SetPixel((int)xPos, (int)pos.y + yPos, myCells[siteIndex].myColor);
            myCells[siteIndex].myPixels.Add(new Vector2((int)xPos, (int)pos.y + yPos));

            CheckNeighbour(LR.RIGHT, new Vector2f(xPos + 1, pos.y + yPos), siteIndex, edges);
            CheckNeighbour(LR.LEFT, new Vector2f(xPos - 1, pos.y + yPos), siteIndex, edges);
            if (tries > 300)
            {
                break;
            }
            yPos--;
        }
        return 0;
    }

    private void CheckNeighbour(LR direction, Vector2f pos, int siteIndex, List<Vector2f> edges)
    {
        bool hasStopped = false;
        int xPos = 0;
        int tries = 0;
        while(!hasStopped)
        {
            tries++;
            hasStopped = !GetPixelsFromSite(edges, new Vector2f(pos.x + xPos, pos.y));
            if(((PixelWidth * (int)pos.y) + (int)(pos.x + xPos)) < (PixelHeight * PixelWidth) - 1 && (int)((PixelWidth * (int)pos.y) + (int)(pos.x + xPos)) > 0)
            {
                myTexture.SetPixel((int)pos.x + xPos, (int)pos.y, myCells[siteIndex].myColor);
                myCells[siteIndex].myPixels.Add(new Vector2((int)pos.x + xPos, (int)pos.y));
            }
            if(tries > 150)
            {
                break;
            }
            if(direction == LR.RIGHT)
            {
                xPos++;
            }
            else
            {
                xPos--;
            }
        }
    }

    private bool GetPixelsFromSite(List<Vector2f> Edges, Vector2f Pos)
    {
        var pos = 0;
        var neg = 0;

        for(int i = 0; i < Edges.Count; i++)
        {
            Vector2f newPos = Edges[i];
            if (newPos == Pos)
            {
                return true;
            }

            var x1 = newPos.x;
            var y1 = newPos.y;

            var i2 = (i + 1) % Edges.Count;
            Vector2f i2Pos = Edges[i2];

            var x2 = i2Pos.x;
            var y2 = i2Pos.y;

            var x = Pos.x;
            var y = Pos.y;

            var d = (x - x1) * (y2 - y1) - (y - y1) * (x2 - x1);

            if (d > 0) pos++;
            if (d < 0) neg++;

            if (pos > 0 && neg > 0)
            {
                return false;
            }
        }
        return true;
    }

    private List<Vector2f> CreateConvexHull(List<Vector2f> edges)
    {
        if (edges.Count < 3)
        {
            return edges;
        }
        List<Vector2f> SortedHull = new List<Vector2f>();
        Vector2f LeftEdge = edges[0];
        foreach (Vector2f edge in edges)
        {
            if (LeftEdge.x < edge.x)
            {
                LeftEdge = edge;
            }
        }

        Vector2f LastEdge = edges[0];

        do
        {
            SortedHull.Add(LeftEdge);
            LastEdge = edges[0];
            for (int i = 1; i < edges.Count; i++)
            {
                if ((LeftEdge == LastEdge)
                    || (Orientation(LeftEdge, LastEdge, edges[i]) == -1))
                {
                    LastEdge = edges[i];
                }
            }
            LeftEdge = LastEdge;
        }
        while (LastEdge != SortedHull[0]);

        return SortedHull;
    }

    private static int Orientation(Vector2f a, Vector2f b, Vector2f c)
    {
        float Orin = (b.x - a.x) * (c.y - a.y) - (c.x - a.x) * (b.y - a.y);

        if (Orin > 0)
            return -1;
        if (Orin < 0)
            return 1;

        return 0;
    }

    private void GenerateMapCells()
    {
        float randomizedPosX = Random.Range(0.0f, 100000.0f);
        float randomizedPosY = Random.Range(0.0f, 100000.0f);
        int i = 0;
        foreach(Vector2f pos in myVoronoi.SiteCoords())
        {
            Site site = myVoronoi.SitesIndexedByLocation[pos];

            float nX = site.x / PixelWidth * scale + randomizedPosX;
            float nY = site.y / PixelHeight * scale + randomizedPosY;
            float sample = Mathf.PerlinNoise(nX, nY);
            if (sample < 0.1f)
            {
                myCells[site.SiteIndex].myColor = new Color(1 - sample, 1 - sample, 1 - sample);
                myCells[site.SiteIndex].aLandType = LandTypes.Mountain;
            }
            else if(sample < WaterLevel)
            {
                myCells[site.SiteIndex].myColor = new Color(0, (1 - sample), 0);
                myCells[site.SiteIndex].aLandType = LandTypes.Land;
            }
            else
            {
                myCells[site.SiteIndex].myColor = new Color(0, 0, WaterLevel / sample);
                myCells[site.SiteIndex].aLandType = LandTypes.Water;
            }
            myCells[site.SiteIndex].Index = site.SiteIndex;
            myCells[site.SiteIndex].CordIndex = i;
            myCells[site.SiteIndex].isSeen = false;
            i++;
        }
        foreach (KeyValuePair<Vector2f, Site> kv in myVoronoi.SitesIndexedByLocation)
        {
            foreach (Site site in kv.Value.NeighborSites())
            {
                if (myCells[site.SiteIndex].aLandType == LandTypes.Water && myCells[kv.Value.SiteIndex].aLandType != LandTypes.Water)
                {
                    myCells[kv.Value.SiteIndex].aLandType = LandTypes.Beach;
                    myCells[kv.Value.SiteIndex].myColor = new Color(0.75f, 0.7f, 0.5f);
                }
            }
            Color color = myCells[kv.Value.SiteIndex].myColor;
            Color colör = new Color(color.r, color.g, color.b, 1);
            myCells[kv.Value.SiteIndex].myColor = colör;
        }
    }

    public void SeeCell(int siteIndex)
    {
        myCells[siteIndex].myColor = new Color(myCells[siteIndex].myColor.r, myCells[siteIndex].myColor.g, myCells[siteIndex].myColor.b, 1.0f);
        foreach(Vector2 pixel in myCells[siteIndex].myPixels)
        {
            myTexture.SetPixel((int)pixel.x, (int)pixel.y, myCells[siteIndex].myColor);
            myCells[siteIndex].isSeen = true;
        }
        myTexture.Apply();
    }

    private void OpenMapMenu()
    {
        float closestDistance = float.MaxValue;
        int index = 0;
        foreach(KeyValuePair<Vector2f, Site> kv in myVoronoi.SitesIndexedByLocation)
        {
            if (myCells[index].isSeen)
            {

                Vector2f newSitePos = new Vector2f((transform.position.x - (transform.localScale.x * 0.5f)) + (kv.Key.x / (transform.localScale.y * 0.64)), (transform.position.y - (transform.localScale.y * 0.5f)) + (kv.Key.y / (transform.localScale.x * 0.64)));
                Vector3 mousePosInWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                float distance = Vector2f.DistanceSquare(new Vector2f(mousePosInWorld.x, mousePosInWorld.y), newSitePos);
                closestDistance = distance;
                index = kv.Value.SiteIndex;
            }
        }
        UIHub.Instance.GetLandUI().OpenMenu(myCells[index]);
        //UIHub.Instance.GetLandUI().CloseMenu();
    }
}
