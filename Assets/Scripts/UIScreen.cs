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
    float fadeTime = 1f;
    float opacity = 0.8f;

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

    void OnGameOver () {
        StartCoroutine(Fade());
        UIElements.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<Player>().ObjectDied += OnGameOver;
        UIElements.SetActive(false);
    }

    public void StartNewGame() {
        SceneManager.LoadScene("Game");
    }


}
