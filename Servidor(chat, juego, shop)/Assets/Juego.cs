using UnityEngine;

public class Juego : MonoBehaviour
{
    public GameObject imagePrefab;
    public GameObject specialPrefab;
    public Transform[] spawnPoints;
    public float specialSpawnProbability = 0.01f;

    void Start()
    {
        Spawn();
    }

    public void Spawn()
    {
        float rand = Random.value;

        GameObject prefabToSpawn = rand < specialSpawnProbability ? specialPrefab : imagePrefab;

        GameObject imagen = Instantiate(prefabToSpawn, spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);
    }
}
