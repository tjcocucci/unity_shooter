using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    public bool devMode;
    public Enemy enemy;
    public Wave[] waves;

    Player player;
    public Wave currentWave;
    public int currentWaveNumber;
    float timeForNextSpawn;
    int spawnedEnemies;
    int killedEnemies;
    float flashingTime = 2f;
    float flashingSpeed = 5f;

    bool playerCamping;
    float nextCampingTime;
    Vector3 previousPosition;
    float campingCheckTime = 5;
    float campingDistance = 1.5f;

    MapGenerator map;

    bool isDisabled;

    public event System.Action<int> OnNextWaveStart;

    void Start() {
        player = FindObjectOfType<Player>();
        player.ObjectDied += OnPlayerDeath;
        nextCampingTime = Time.time + campingCheckTime;
        playerCamping = false;
        previousPosition = player.transform.position + Vector3.one * campingDistance * 2 ;

        map = FindObjectOfType<MapGenerator>();
        NextWave();
    }

    void NextWave() {
        currentWaveNumber++;
        if (currentWaveNumber <= waves.Length) {
            timeForNextSpawn = Time.time;
            spawnedEnemies = 0;
            killedEnemies = 0;
            playerCamping = false;
            nextCampingTime = Time.time + campingCheckTime;

            currentWave = waves[currentWaveNumber - 1];

            if (OnNextWaveStart != null) {

                OnNextWaveStart(currentWaveNumber - 1);
            }
        }
    }

    void Update() {
        if (!isDisabled) {
            if (Time.time > timeForNextSpawn && spawnedEnemies < currentWave.totalEnemies) {
                spawnedEnemies++;
                timeForNextSpawn = Time.time + currentWave.timeBetweenSpawns;
                StartCoroutine("SpawnEnemy");
            }

            if (Time.time > nextCampingTime) {
                nextCampingTime += campingCheckTime;
                CheckIfCamping();
            }
        }

        if (devMode) {
            if (Input.GetKeyDown(KeyCode.Return)) {
                StopCoroutine("SpawnEnemy");
                foreach (Enemy enemy in FindObjectsOfType<Enemy>()) {
                    GameObject.Destroy(enemy.gameObject);
                }
                Invoke("NextWave", 3);
            }
        }
    }

    void CheckIfCamping () {
        float distance = Vector3.Distance(previousPosition, player.transform.position);
        previousPosition = player.transform.position;
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
            tileTransform = map.TileFromPosition(player.transform.position);
            playerCamping = false;
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

        Enemy spawnedEnemy = Instantiate(enemy, tileTransform.position, Quaternion.identity) as Enemy;
        spawnedEnemy.ObjectDied += OnEnemyDeath;
        if (devMode) {
            currentWave.hitsToKillPlayer = 1000000;
        }

        spawnedEnemy.SetCharacteristics(currentWave.skinColor, currentWave.enemySpeed, currentWave.hitsToKillPlayer, currentWave.health);

    }

    void OnEnemyDeath() {
        killedEnemies++;
        if (killedEnemies == currentWave.totalEnemies && currentWaveNumber < waves.Length) {
            if (currentWaveNumber > 0) {
                AudioManager.instance.PlaySound2D("New Wave");
            }
            Invoke("NextWave", 1);
            Destroy(FindObjectOfType<ParticleSystem>().gameObject, 1);
        }
    }

    void OnPlayerDeath() {
        isDisabled = true;
    }

    [System.Serializable]
    public class Wave {
        public int totalEnemies;
        public float timeBetweenSpawns;
        public Color skinColor;
        public float enemySpeed;
        public int hitsToKillPlayer;
        public float health;
    }

}
