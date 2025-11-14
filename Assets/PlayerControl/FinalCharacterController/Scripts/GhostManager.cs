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

    [Header("Day/Night Control")]
    public bool isDaytime = true;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SpawnInitialGhosts(2);
        SetDaytime(isDaytime);
        // 
        if (!isDaytime)
        {
            EnemyFollow follow = ghostPrefab.GetComponent<EnemyFollow>();
        }
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

        RegisterGhost(ghost);
    }

    public void RegisterGhost(GameObject ghost)
    {
        if (ghost == null) return;

        if (!activeGhosts.Contains(ghost))
            activeGhosts.Add(ghost);

        EnemyFollow follow = ghost.GetComponent<EnemyFollow>();
        if (follow != null)
        {
            follow.player = player != null ? player.transform : null;
            follow.moveSpeed = baseMoveSpeed;
            follow.isAggressive = false;
            follow.isDaytime = isDaytime;
        }
        else
        {
            Debug.LogWarning($"⚠️ Ghost '{ghost.name}' has no EnemyFollow component!");
        }
    }

    public void SetDaytime(bool isDay)
    {
        isDaytime = isDay;

        foreach (GameObject g in activeGhosts)
        {
            if (g == null) continue;

            EnemyFollow follow = g.GetComponent<EnemyFollow>();
            if (follow != null)
                follow.isDaytime = isDay;

            g.SetActive(!isDay); // deactivate during day, activate at night
        }

        if (!isDay)
        {
            Debug.Log("🌙 Nighttime started — ghosts are waking up!");
            // ensure they know who to follow
            foreach (GameObject g in activeGhosts)
            {
                if (g == null) continue;
                EnemyFollow follow = g.GetComponent<EnemyFollow>();
                if (follow != null && player != null)
                    follow.player = player.transform;
            }
        }
        else
        {
            Debug.Log("☀️ Daytime started — ghosts are hiding!");
        }
    }

    public void ActivateGhostAggression()
    {
        Debug.Log("Aggression activated!!");
        if (player == null)
        {
            Debug.LogWarning("⚠️ GhostManager: Player is null, cannot activate aggression!");
            return;
        }

        bool foundAggressive = false;

        foreach (GameObject ghost in activeGhosts)
        {
            if (ghost == null || !ghost.activeInHierarchy) continue;

            EnemyFollow follow = ghost.GetComponent<EnemyFollow>();
            if (follow == null) continue;

            float dist = Vector3.Distance(ghost.transform.position, player.transform.position);

            // Ghosts always can chase at night
            if (!isDaytime && dist <= 30f)
            {
                follow.isAggressive = true;
                follow.moveSpeed = baseMoveSpeed * aggressionMultiplier;

                if (!foundAggressive)
                    Debug.Log("👻 Ghosts are now chasing the player!");

                foundAggressive = true;

                StartCoroutine(StopAggressionAfterTime(follow, 8f));
            }
        }

        if (!foundAggressive)
            Debug.Log("😶 No nearby ghosts found to become aggressive.");
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
        Debug.Log($"💢 Ghosts are more aggressive! Multiplier: {aggressionMultiplier}");
        ActivateGhostAggression();
    }
}
