using System.Collections.Generic;
using UnityEngine;

public class TreeSpawner : MonoBehaviour
{
    [Header("References")]
    public Terrain terrain;
    public GameObject[] treePrefabs;

    [Header("Options")]
    [Range(0f, 1f)] public float treeDensity = 0.1f;
    public float minHeight = 0f;
    public float maxHeight = 1f;
    public bool addCollidersOnly = false;
    public float colliderScale = 1.2f;
    public bool randomRotation = true;
    public bool clearExisting = true;

    [Header("Spawn Rules")]
    public float treeRadiusCheck = 2f;
    public LayerMask avoidLayers;

    [Header("Exclusion Settings")]
    public GameObject doorObject;
    public float doorExclusionRadius = 10f;

    [Tooltip("Add ponds, buildings, or any object you want to avoid here")]
    public List<Transform> exclusionZones;

    void Start()
    {
        if (terrain == null)
        {
            Debug.LogError("Assign a terrain in the TreeSpawner script.");
            return;
        }

        if (treePrefabs.Length == 0)
        {
            Debug.LogError("Assign at least one tree prefab!");
            return;
        }

        SpawnProceduralTrees();
    }

    void SpawnProceduralTrees()
    {
        TerrainData data = terrain.terrainData;
        Vector3 terrainPos = terrain.transform.position;
        int treeCount = Mathf.RoundToInt(data.size.x * data.size.z * treeDensity);

        if (clearExisting)
        {
            foreach (Transform child in transform)
                Destroy(child.gameObject);
        }

        int totalSpawned = 0;

        for (int i = 0; i < treeCount; i++)
        {
            float randX = Random.Range(0f, data.size.x);
            float randZ = Random.Range(0f, data.size.z);
            float normalizedX = randX / data.size.x;
            float normalizedZ = randZ / data.size.z;
            float height = data.GetInterpolatedHeight(normalizedX, normalizedZ) / data.size.y;

            if (height < minHeight || height > maxHeight) continue;

            Vector3 worldPos = new Vector3(randX, data.GetInterpolatedHeight(normalizedX, normalizedZ), randZ) + terrainPos;

            // 🛑 Skip near Door
            if (doorObject != null && Vector3.Distance(worldPos, doorObject.transform.position) < doorExclusionRadius)
                continue;

            // 🛑 Skip inside user-defined exclusion zones (like ponds)
            if (IsInsideExclusionZone(worldPos))
                continue;

            // 🛑 Skip if collides with existing objects
            if (Physics.CheckSphere(worldPos, treeRadiusCheck, avoidLayers))
                continue;

            GameObject prefab = treePrefabs[Random.Range(0, treePrefabs.Length)];
            Quaternion rotation = randomRotation ? Quaternion.Euler(0, Random.Range(0f, 360f), 0) : prefab.transform.rotation;

            if (addCollidersOnly)
            {
                GameObject colObj = new GameObject("TreeCollider");
                colObj.transform.position = worldPos;
                CapsuleCollider cc = colObj.AddComponent<CapsuleCollider>();
                cc.radius = 0.5f * colliderScale;
                cc.height = 3f * colliderScale;
                cc.center = Vector3.up * 1.5f;
            }
            else
            {
                Instantiate(prefab, worldPos, rotation, transform);
            }

            totalSpawned++;
        }

        Debug.Log($"🌲 Spawned {totalSpawned} procedural trees successfully!");
    }

    bool IsInsideExclusionZone(Vector3 pos)
    {
        if (exclusionZones == null || exclusionZones.Count == 0) return false;

        foreach (Transform zone in exclusionZones)
        {
            float radius = 7f; // Adjust based on your pond size
            Collider col = zone.GetComponent<Collider>();
            if (col != null)
                radius = Mathf.Max(col.bounds.extents.x, col.bounds.extents.z);

            if (Vector3.Distance(pos, zone.position) < radius)
                return true;
        }

        return false;
    }
}
