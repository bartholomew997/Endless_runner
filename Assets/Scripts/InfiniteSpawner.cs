using UnityEngine;
using System.Collections.Generic;

public class InfiniteSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject coinPrefab;
    public GameObject firePrefab;
    public GameObject barrelPrefab;

    [Header("Spawn Settings")]
    [Range(0.1f, 0.9f)] public float coinSpawnChance = 0.7f;
    public float spawnAheadDistance = 25f;
    public float coinHeightMin = 1f;
    public float coinHeightMax = 3f;
    public int maxActiveObjects = 50;
    public float tileWidth = 2.56f;

    [Header("Burst Settings")]
    public int coinsPerBurst = 10;
    public int burstLengthInTiles = 2;
    public int gapBetweenBursts = 4;

    [Header("Obstacle Settings")]
    [Range(0, 1)] public float fireSpawnChance = 0.15f;
    [Range(0, 1)] public float barrelSpawnChance = 0.1f;
    public float minObstacleGap = 5f;

    [Header("Sorting Layers")]
    public string coinSortingLayer = "Player";
    public string obstacleSortingLayer = "Player";

    public LayerMask groundLayer;

    private Transform player;
    private List<GameObject> activeObjects = new List<GameObject>();
    private float nextTileToSpawnX;
    private Queue<float> burstCoinPositions;
    private float lastObstacleX = Mathf.NegativeInfinity;

    private int tileCounter = 0;
    private bool inCoinBurst = true;
    private int burstTilesRemaining;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        nextTileToSpawnX = player.position.x;
        burstTilesRemaining = burstLengthInTiles;
        PrewarmObjects();
    }

    void Update()
    {
        float spawnThreshold = player.position.x + spawnAheadDistance;

        while (nextTileToSpawnX < spawnThreshold && activeObjects.Count < maxActiveObjects)
        {
            SpawnOnTile(nextTileToSpawnX);
            nextTileToSpawnX += tileWidth;
            tileCounter++;

            burstTilesRemaining--;
            if (burstTilesRemaining <= 0)
            {
                inCoinBurst = !inCoinBurst;
                burstTilesRemaining = inCoinBurst ? burstLengthInTiles : gapBetweenBursts;
                if (inCoinBurst) GenerateBurstCoinPositions();
            }
        }

        CleanupObjects();
    }

    void GenerateBurstCoinPositions()
    {
        burstCoinPositions = new Queue<float>();
        float startX = nextTileToSpawnX - tileWidth / 2f;
        float totalBurstLength = burstLengthInTiles * tileWidth;
        float spacing = totalBurstLength / (coinsPerBurst - 1);

        for (int i = 0; i < coinsPerBurst; i++)
        {
            burstCoinPositions.Enqueue(startX + i * spacing);
        }
    }

    void SpawnOnTile(float tileCenterX)
    {
        // Spawn coins in burst
        if (inCoinBurst && burstCoinPositions != null)
        {
            float tileLeft = tileCenterX - tileWidth / 2f;
            float tileRight = tileCenterX + tileWidth / 2f;

            while (burstCoinPositions.Count > 0)
            {
                float coinX = burstCoinPositions.Peek();
                if (coinX >= tileLeft && coinX <= tileRight)
                {
                    burstCoinPositions.Dequeue();
                    Vector2 coinPos = GetSpawnPosition(coinX, true);
                    if (coinPos != Vector2.zero)
                    {
                        GameObject coin = Instantiate(coinPrefab, coinPos, Quaternion.identity);
                        SetSortingLayer(coin, coinSortingLayer);
                        activeObjects.Add(coin);
                    }
                }
                else if (coinX > tileRight) break;
                else burstCoinPositions.Dequeue();
            }
        }

        // Spawn obstacles
        SpawnObstacle(firePrefab, fireSpawnChance, tileCenterX);
        SpawnObstacle(barrelPrefab, barrelSpawnChance, tileCenterX);
    }

    void SpawnObstacle(GameObject prefab, float spawnChance, float tileCenterX)
    {
        if (Random.value < spawnChance)
        {
            float xObstacle = tileCenterX + Random.Range(-tileWidth / 2f, tileWidth / 2f);
            
            if (Mathf.Abs(xObstacle - lastObstacleX) >= minObstacleGap)
            {
                Vector2 obstaclePos = GetSpawnPosition(xObstacle, false);
                if (obstaclePos != Vector2.zero)
                {
                    GameObject obstacle = Instantiate(prefab, obstaclePos, Quaternion.identity);
                    SetSortingLayer(obstacle, obstacleSortingLayer);
                    activeObjects.Add(obstacle);
                    lastObstacleX = xObstacle;
                }
            }
        }
    }

    Vector2 GetSpawnPosition(float xPos, bool isCoin)
    {
        Vector2 rayOrigin = new Vector2(xPos, player.position.y + 10f);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, 20f, groundLayer);

        if (hit.collider != null)
        {
            return isCoin ? 
                new Vector2(xPos, hit.point.y + Random.Range(coinHeightMin, coinHeightMax)) :
                new Vector2(xPos, hit.point.y);
        }
        return Vector2.zero;
    }

    void SetSortingLayer(GameObject obj, string layerName)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null) renderer.sortingLayerName = layerName;
    }

    void CleanupObjects()
    {
        for (int i = activeObjects.Count - 1; i >= 0; i--)
        {
            if (activeObjects[i] == null || 
                activeObjects[i].transform.position.x < player.position.x - 15f)
            {
                if (activeObjects[i] != null) Destroy(activeObjects[i]);
                activeObjects.RemoveAt(i);
            }
        }
    }

    void PrewarmObjects()
    {
        float prewarmDistance = player.position.x + spawnAheadDistance;
        while (nextTileToSpawnX < prewarmDistance)
        {
            SpawnOnTile(nextTileToSpawnX);
            nextTileToSpawnX += tileWidth;
        }
    }
}