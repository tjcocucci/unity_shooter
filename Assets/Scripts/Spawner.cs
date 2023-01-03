using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    public Enemy enemy;
    public Wave[] waves;

    Player player;
    Wave currentWave;
    int currentWaveNumber;
    float timeForNextSpawn;
    int spawnedEnemies;
    int killedEnemies;
    float flashingTime = 2f;
    float flashingSpeed = 5f;

    bool playerCamping;
    float nextCampingTime;
    Vector3 previousPosition;
    float campingCheckTime = 2;
    float campingDistance = 1.5f;

    MapGenerator map;

    void Start() {
        player = FindObjectOfType<Player>();
        nextCampingTime = campingCheckTime;
        playerCamping = false;
        previousPosition = player.transform.position + Vector3.one * campingDistance * 2 ;

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

        if (Time.time > nextCampingTime) {
            nextCampingTime += campingCheckTime;
            CheckIfCamping();
        }

    }

    void CheckIfCamping () {
        float distance = Vector3.Distance(previousPosition, transform.position);
        previousPosition = transform.position;
        if (distance < campingDistance) {
            playerCamping = true;
        } else {
            playerCamping = false;
        }
    }


    IEnumerator SpawnEnemy() {

        Transform tileTransform;
        if (!playerCamping){
            tileTransform = map.GetRandomTile(map.shuffledEmptyTiles);
        } else {
            print("Camping Spawn");
            tileTransform = map.TileFromPosition(player.transform.position);
        }
        Material tileMaterial = tileTransform.GetComponent<Renderer> ().material;
        Color originalColor = tileMaterial.color;

        float elapsedTime = 0f;
        while (elapsedTime < flashingTime) {
            float t = Mathf.PingPong(elapsedTime * flashingSpeed, 1f);
            tileMaterial.color = Color.Lerp(originalColor, Color.red, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        tileMaterial.color = originalColor;

        Enemy spawnedEnemy = Instantiate(enemy, tileTransform.position, Quaternion.identity);
        spawnedEnemy.ObjectDied += OnEnemyDeath;

    }

    void OnEnemyDeath() {
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
