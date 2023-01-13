using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{

    public float timeToLoseStreak = 2;

    public Text scoreText;
    public Text streakText;
    int score;
    float streakExpireTime;
    float currentStreak;


    void Start()
    {
        Enemy.EnemyDeath += UpdateScore;
        FindObjectOfType<Player> ().ObjectDied += OnPlayerDeath;
        streakExpireTime = Time.time + timeToLoseStreak;
    }

    void UpdateScore () {
        if (Time.time > streakExpireTime) {
            currentStreak = 1;
        } else {
            currentStreak++;
        }
        streakExpireTime = Time.time + timeToLoseStreak;

        streakText.text = "Streak: " + currentStreak.ToString();
        score += (int) Mathf.Pow(1.2f, currentStreak) * 5;
        scoreText.text = score.ToString("D6");
    }

    void OnPlayerDeath () {
        Enemy.EnemyDeath -= UpdateScore;
    }

    void Update () {
        if (Time.time > streakExpireTime) {
            streakText.text = "Streak: 0";
        }
    }

}
