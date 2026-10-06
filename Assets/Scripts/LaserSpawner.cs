using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSpawner : MonoBehaviour
{
    [Header("Prefabs (assign all 4 patterns here)")]
    public List<GameObject> laserPrefabs = new List<GameObject>();

    public Transform spawnPoint;

    [Header("Spawn Timing")]
    public float spawnInterval = 3f;

    [Header("Laser Movement")]
    public float laserSpeed = 6f;

    [Header("Safety cap")]
    public int maxActiveLasers = 15;

    private bool spawning;
    private Coroutine spawnCoroutine;

    public void StartSpawning()
    {
        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);

        spawning = true;
        spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    public void StopSpawning()
    {
        spawning = false;

        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    private IEnumerator SpawnRoutine()
    {
        while (spawning)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (!spawning)
                yield break;

            GameObject prefab = PickPrefab();
            if (prefab == null)
            {
                Debug.LogWarning("[LaserSpawner] No laser prefab assigned.", this);
                continue;
            }

            if (CountActiveLasers() >= maxActiveLasers)
                continue;
            
            Transform t = spawnPoint != null ? spawnPoint : transform;
            GameObject laser = Instantiate(prefab, t.position, t.rotation * prefab.transform.rotation);

            MoveToStart movement = laser.GetComponent<MoveToStart>();
            if (movement == null)
                movement = laser.GetComponentInChildren<MoveToStart>();

            if (movement != null)
                movement.speed = laserSpeed;
            else
                Debug.LogWarning("[LaserSpawner] Spawned laser has no MoveToStart.", laser);
        }
    }

    private GameObject PickPrefab()
    {
        if (laserPrefabs is { Count: > 0 })
            return laserPrefabs[Random.Range(0, laserPrefabs.Count)];

        return null;
    }

    private int CountActiveLasers()
    {
        return FindObjectsByType<MoveToStart>(FindObjectsInactive.Exclude).Length;
    }
}
