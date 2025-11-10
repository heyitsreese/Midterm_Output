using System.Collections.Generic;
using UnityEngine;

public class TrashSpawner : MonoBehaviour
{
    [Header("References")]
    public Terrain terrain;
    public GameObject[] trashPrefabs;

    [Header("Spawn Settings")]
    [Range(0f, 1f)] public float trashDensity = 0.05f; // Lower density than trees
    [Range(0f, 1f)] public float mapCoverage = 0.6f;   // 0.5 = half the map, 1.0 = full map
    public bool clearExisting = true;
    public bool randomRotation = true;

    [Header("Avoidance Rules")]
    public float trashRadiusCheck = 1f;
    public LayerMask avoidLayers;
    public GameObject doorObject;
    public float doorExclusionRadius = 10f;
    public List<Transform> exclusionZones; // ponds or no-trash zones

    [Header("Spawn Area Offset")]
    [Tooltip("Offset spawn area from the terrain center (useful if you want trash to spawn only in one side of the map)")]
    public Vector2 areaOffset = Vector2.zero;

    void Start()
    {
        if (terrain == null)
        {
            Debug.LogError("Assign a terrain in the TrashSpawner!");
            return;
        }

        if (trashPrefabs.Length == 0)
        {
            Debug.LogError("Assign at least one trash prefab!");
            return;
        }

        SpawnTrash();
    }

    void SpawnTrash()
    {
        TerrainData data = terrain.terrainData;
        Vector3 terrainPos = terrain.transform.position;

        // Compute effective spawn area
        float spawnWidth = data.size.x * mapCoverage;
        float spawnDepth = data.size.z * mapCoverage;

        float startX = terrainPos.x + (data.size.x - spawnWidth) / 2 + areaOffset.x;
        float startZ = terrainPos.z + (data.size.z - spawnDepth) / 2 + areaOffset.y;

        int trashCount = Mathf.RoundToInt(spawnWidth * spawnDepth * trashDensity);

        if (clearExisting)
        {
            foreach (Transform child in transform)
                Destroy(child.gameObject);
        }

        int totalSpawned = 0;

        for (int i = 0; i < trashCount; i++)
        {
            float randX = Random.Range(startX, startX + spawnWidth);
            float randZ = Random.Range(startZ, startZ + spawnDepth);

            float normalizedX = (randX - terrainPos.x) / data.size.x;
            float normalizedZ = (randZ - terrainPos.z) / data.size.z;
            float height = data.GetInterpolatedHeight(normalizedX, normalizedZ);

            Vector3 worldPos = new Vector3(randX, height, randZ);

            // 🧹 Skip near Door
            if (doorObject != null && Vector3.Distance(worldPos, doorObject.transform.position) < doorExclusionRadius)
                continue;

            // 🧹 Skip in exclusion zones
            if (IsInsideExclusionZone(worldPos))
                continue;

            // 🧹 Skip if collides with other objects
            if (Physics.CheckSphere(worldPos, trashRadiusCheck, avoidLayers))
                continue;

            GameObject prefab = trashPrefabs[Random.Range(0, trashPrefabs.Length)];
            Quaternion rotation = randomRotation ? Quaternion.Euler(0, Random.Range(0f, 360f), 0) : prefab.transform.rotation;

            Instantiate(prefab, worldPos, rotation, transform);
            totalSpawned++;
        }

        Debug.Log($"🗑️ Spawned {totalSpawned} trash items in a {mapCoverage * 100:F0}% area of the map!");
    }

    bool IsInsideExclusionZone(Vector3 pos)
    {
        if (exclusionZones == null || exclusionZones.Count == 0) return false;

        foreach (Transform zone in exclusionZones)
        {
            float radius = 7f;
            Collider col = zone.GetComponent<Collider>();
            if (col != null)
                radius = Mathf.Max(col.bounds.extents.x, col.bounds.extents.z);

            if (Vector3.Distance(pos, zone.position) < radius)
                return true;
        }

        return false;
    }
}
