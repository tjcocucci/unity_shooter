using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class UIScreen : MonoBehaviour
{
    public Image gameOverBackground;
    public GameObject gameOverUI;
    public GameObject inGameUI;
    public Spawner spawner;
    public Transform nextWaveBanner;
    public Text bannerWaveText;
    public Text bannerEnemiesText;

    float fadeTime = 1f;
    float opacity = 0.9f;
    float bannerAnimationTime = 2;

    IEnumerator Fade () {
        Color originalColor = gameOverBackground.color;
        Color fadedColor = Color.black;
        fadedColor.a = opacity;
        float fadeSpeed = 1 / fadeTime;
        float percent = 0;

        while (percent <= 1) {
            gameOverBackground.color = Color.Lerp(originalColor, fadedColor, percent);
            percent += Time.deltaTime * fadeSpeed;
            yield return null;
        }
    }

    void ShowBanner(int waveIndex) {
        bannerWaveText.text = "- Wave Number" + (waveIndex + 1) + " -";
        bannerEnemiesText.text = "Enemies: " + spawner.currentWave.totalEnemies;
        StopCoroutine("BannerAnimation");
        StartCoroutine("BannerAnimation");
    }

    IEnumerator BannerAnimation () {
        float percent = 0;
        float speed = 1 / bannerAnimationTime;
        float dir = 1;
        while (percent >= 0) {
            percent += Time.deltaTime * speed * dir;
            nextWaveBanner.transform.localPosition = Vector3.up * Mathf.Lerp(-200, 110, percent);
            if(percent >= 1) {
                dir = -1;
                yield return new WaitForSeconds(2);
            }
            yield return null;
        }
    }

    void OnGameOver () {
        StartCoroutine(Fade());
        Cursor.visible = true;
        inGameUI.SetActive(false);
        gameOverUI.SetActive(true);
    }

    void Start()
    {
        FindObjectOfType<Player>().ObjectDied += OnGameOver;
        FindObjectOfType<Spawner>().OnNextWaveStart += ShowBanner;
        inGameUI.SetActive(true);
        gameOverUI.SetActive(false);
    }

    public void StartNewGame() {
        SceneManager.LoadScene("Game");
    }

    public void GoToMainMenu () {
        SceneManager.LoadScene("Menu");
    }

}
