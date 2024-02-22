using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Juego : MonoBehaviour
{
    public GameObject imagePrefab;
    public Transform[] spawnPoints;
    //public TextMeshProUGUI scoreText;

    void Start()
    {
        Spawn();
    }

    void Update()
    {
        
    }

    public void Spawn()
    {
        GameObject imagen = Instantiate(imagePrefab) as GameObject;
        imagen.transform.position = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
    }
}
