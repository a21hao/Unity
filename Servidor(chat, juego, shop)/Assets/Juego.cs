using UnityEngine;
using TMPro;

public class Juego : MonoBehaviour
{
    public GameObject imagePrefab;
    public GameObject specialPrefab;
    public Transform[] spawnPoints;
    public float specialSpawnProbability = 0.01f;
    public float gameTime;
    public TextMeshProUGUI gameText;

    private bool spawningStarted = false;
    private GameObject currentSpawnedObject;

    void Update()
    {
        if (spawningStarted)
        {
            gameTime -= Time.deltaTime;

            if (gameTime > 0)
            {
                if (currentSpawnedObject == null)
                {
                    Spawn();
                }
            }
            else
            {
                spawningStarted = false;
            }

            gameText.text = Mathf.CeilToInt(gameTime).ToString();
        }
    }

    public void StartSpawning()
    {
        spawningStarted = true; 
        gameTime = 60f; 
    }

    public void Spawn()
    {
        float rand = Random.value;
        GameObject prefabToSpawn = rand < specialSpawnProbability ? specialPrefab : imagePrefab;
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        currentSpawnedObject = Instantiate(prefabToSpawn, spawnPoint.position, Quaternion.identity);
    }

    public void OnObjectClicked()
    {
        if (currentSpawnedObject != null)
        {
            Destroy(currentSpawnedObject);
            currentSpawnedObject = null;

            Spawn();
        }
    }
}
