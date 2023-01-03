using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    public Enemy enemy;
    public Wave[] waves;

    Wave currentWave;
    int currentWaveNumber;
    float timeForNextSpawn;
    int spawnedEnemies;
    int killedEnemies;
    float flashingTime = 2f;
    float flashingSpeed = 5f;

    MapGenerator map;

    void Start() {
        map = FindObjectOfType<MapGenerator>();
        NextWave();
    }

    void NextWave() {
        timeForNextSpawn = Time.time;
        spawnedEnemies = 0;
        killedEnemies = 0;

        currentWaveNumber++;
        currentWave = waves[currentWaveNumber - 1];
        print("Starting wave: " + currentWaveNumber);
    }

    void Update() {
        if (Time.time > timeForNextSpawn && spawnedEnemies < currentWave.totalEnemies) {
            spawnedEnemies++;
            timeForNextSpawn = Time.time + currentWave.timeBetweenSpawns;
            StartCoroutine(SpawnEnemy());
        }
    }

    IEnumerator SpawnEnemy() {

        Transform tileTransform;
        // if (!camping){
            tileTransform = map.GetRandomTile(map.shuffledEmptyTiles);
        // } else {
        // }
        Material tileMaterial = tileTransform.GetComponent<Renderer> ().material;
        Color originalColor = tileMaterial.color;

        float elapsedTime = 0f;
        while (elapsedTime < flashingTime) {
            float t = Mathf.PingPong(elapsedTime * flashingSpeed, 1f);
            tileMaterial.color = Color.Lerp(originalColor, Color.red, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Enemy spawnedEnemy = Instantiate(enemy, tileTransform.position, Quaternion.identity);
        spawnedEnemy.ObjectDied += OnEnemyDeath;

    }

    void OnEnemyDeath() {
        print("Enemy died");
        killedEnemies++;
        if (killedEnemies == currentWave.totalEnemies && currentWaveNumber < waves.Length) {
            NextWave();
        }
    }

    [System.Serializable]
    public class Wave {
        public int totalEnemies;
        public float timeBetweenSpawns;
    }

}
