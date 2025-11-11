using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GhostManager : MonoBehaviour
{
    public static GhostManager Instance;

    [Header("Spawn Settings")]
    public GameObject ghostPrefab;
    public Transform[] spawnPoints;
    public GameObject player;

    [Header("Ghost Control")]
    public List<GameObject> activeGhosts = new List<GameObject>();

    [Header("Aggression Settings")]
    public float baseMoveSpeed = 3f;
    public float aggressionMultiplier = 1f;
    public float aggressionIncrease = 0.2f;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SpawnInitialGhosts(2); // initial ghosts (hidden Day 1)
    }

    public void SpawnInitialGhosts(int count)
    {
        for (int i = 0; i < count; i++)
            SpawnGhost();
    }

    public void SpawnGhost()
    {
        if (ghostPrefab == null || spawnPoints.Length == 0) return;

        Transform spawnPos = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject ghost = Instantiate(ghostPrefab, spawnPos.position, Quaternion.identity);
        ghost.SetActive(true);
        activeGhosts.Add(ghost);

        EnemyFollow follow = ghost.GetComponent<EnemyFollow>();
        if (follow != null && player != null)
        {
            follow.player = player.transform;
            follow.moveSpeed = baseMoveSpeed;
            follow.isAggressive = false;
        }
    }

    public void ActivateGhostAggression()
    {
        if (player == null) return;

        // Find nearest ghost that is not already aggressive
        GameObject nearest = null;
        float nearestDist = float.MaxValue;

        foreach (GameObject g in activeGhosts)
        {
            if (g == null) continue;
            EnemyFollow follow = g.GetComponent<EnemyFollow>();
            if (follow == null || follow.isAggressive) continue;

            float dist = Vector3.Distance(g.transform.position, player.transform.position);
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearest = g;
            }
        }

        if (nearest != null)
        {
            EnemyFollow follow = nearest.GetComponent<EnemyFollow>();
            if (follow != null)
            {
                follow.player = player.transform; // Make sure player is assigned
                follow.isAggressive = true;
                follow.moveSpeed = baseMoveSpeed * aggressionMultiplier;

                Debug.Log("👻 Ghost is now chasing the player!");

                // Stop chasing after 5 seconds if player escapes
                StartCoroutine(StopAggressionAfterTime(follow, 5f));
            }
        }
    }


    private IEnumerator StopAggressionAfterTime(EnemyFollow follow, float time)
    {
        yield return new WaitForSeconds(time);
        if (follow != null)
        {
            follow.isAggressive = false;
            follow.moveSpeed = baseMoveSpeed;
            Debug.Log("👻 Ghost stopped chasing the player.");
        }
    }

    public void AddCorruption()
    {
        aggressionMultiplier += aggressionIncrease;
        Debug.Log($"👻 Ghosts are more aggressive! Multiplier: {aggressionMultiplier}");
        ActivateGhostAggression();
    }
}
