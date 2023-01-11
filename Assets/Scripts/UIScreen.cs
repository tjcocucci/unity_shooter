using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class UIScreen : MonoBehaviour
{
    public Image background;
    public GameObject UIElements;
    public Button playAgainButton;
    public Spawner spawner;
    public Transform nextWaveBanner;
    public Text bannerWaveText;
    public Text bannerEnemiesText;

    float fadeTime = 1f;
    float opacity = 0.8f;
    float bannerAnimationTime = 2;

    IEnumerator Fade () {
        Color originalColor = background.color;
        Color fadedColor = Color.black;
        fadedColor.a = opacity;
        float fadeSpeed = 1 / fadeTime;
        float percent = 0;

        while (percent <= 1) {
            background.color = Color.Lerp(originalColor, fadedColor, percent);
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
            nextWaveBanner.transform.localPosition = Vector3.up * Mathf.Lerp(-500, -240, percent);
            if(percent >= 1) {
                dir = -1;
                yield return new WaitForSeconds(2);
            }
            yield return null;
        }
    }

    void OnGameOver () {
        StartCoroutine(Fade());
        UIElements.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<Player>().ObjectDied += OnGameOver;
        FindObjectOfType<Spawner>().OnNextWaveStart += ShowBanner;
        UIElements.SetActive(false);
    }

    public void StartNewGame() {
        SceneManager.LoadScene("Game");
    }


}
