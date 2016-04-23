using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TilePRNGMapGenerator : MonoBehaviour {

    public TileMap[] maps;
    public int mapIndex;

    public Transform tilePrefab;
    public Transform columnPrefab;
    public float tileSize;

    [Range(0, 1)]
    public float outlinePercent;

    TileMap currentMap;

    
    public bool autoUpdate;
    public TerrainType[] regions;

    void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        currentMap = maps[mapIndex];
        float[,] noiseMap = Noise.GenerateNoiseMap(currentMap.mapSize.x, currentMap.mapSize.y, currentMap.seed, currentMap.noiseScale, currentMap.octaves, currentMap.persistence, currentMap.lacunarity, currentMap.offset);

        

        //create map holder object
        string holderName = "Generated Map";
        if (transform.FindChild(holderName))
        {
            DestroyImmediate(transform.FindChild(holderName).gameObject);
        }

        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        //spawning columns
        for (int x = 0; x < currentMap.mapSize.x; x++)
        {
            for (int y = 0; y < currentMap.mapSize.y; y++)
            {
                float columnHeight = noiseMap[x,y];
                Vector3 columnPosition = CoordToPosition(x, y);
                Transform newcolumn = Instantiate(columnPrefab, columnPosition + Vector3.up * currentMap.tileHeightCurve.Evaluate(columnHeight) * currentMap.heightMultiplier / 2, Quaternion.identity) as Transform;
                newcolumn.localScale = new Vector3((1 - outlinePercent) * tileSize, currentMap.tileHeightCurve.Evaluate(columnHeight) * currentMap.heightMultiplier, (1 - outlinePercent) * tileSize);
                newcolumn.parent = mapHolder;

                Renderer columnRenderer = newcolumn.GetComponent<Renderer>();
                Material columnMaterial = new Material(columnRenderer.sharedMaterial);

                for (int i = 0; i < regions.Length; i++)
                {
                    if (columnHeight <= regions[i].height)
                    {
                        columnMaterial.color = regions[i].color;
                        break;
                    }
                }
                columnRenderer.sharedMaterial = columnMaterial;
            }
        }
        
    }

    Vector3 CoordToPosition(int x, int y)
    {
        return new Vector3(-currentMap.mapSize.x / 2f + 0.5f + x, 0, -currentMap.mapSize.y / 2f + 0.5f + y) * tileSize;
    }

    [System.Serializable]
    public struct Coord
    {
        public int x;
        public int y;

        public Coord(int _x, int _y)
        {
            x = _x;
            y = _y;
        }
    }

    [System.Serializable]
    public class TileMap
    {
        public Coord mapSize;
        public float noiseScale;
        public int octaves;
        [Range(0, 1)]
        public float persistence;
        public float lacunarity;
        public Vector2 offset;
        public AnimationCurve tileHeightCurve;
        [Range(0, 1)]
        public float columnPercent;
        public int seed;
        public float heightMultiplier;

        public Coord mapCenter
        {
            get
            {
                return new Coord(mapSize.x / 2, mapSize.y / 2);
            }
        }
    }

    void OnValidate()
    {
        if (currentMap.mapSize.x < 1)
        {
            currentMap.mapSize.x = 1;
        }
        if (currentMap.mapSize.y < 1)
        {
            currentMap.mapSize.y = 1;
        }
        if (currentMap.lacunarity < 1)
        {
            currentMap.lacunarity = 1;
        }
        if (currentMap.octaves < 0)
        {
            currentMap.octaves = 0;
        }


    }
    [System.Serializable]
    public struct TerrainType
    {
        public string name;
        public float height;
        public Color color;
    }
}
