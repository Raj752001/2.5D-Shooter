using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public Image fadePlane;
    public GameObject gameOverUI;

    public RectTransform newWaveBanner;
    public Text newWaveTitle;
    public Text newWaveEnemyCount;

    Spawner spawner;

    void Start()
    {
        FindObjectOfType<Player>().OnDeath += OnGameOver;
        
    }

    private void Awake()
    {
        spawner = FindObjectOfType<Spawner>();
        spawner.OnNewWave += OnNewWave;
    }

    void OnNewWave(int newWave) {
        string[] numbers = { "One", "Two", "Three", "Four", "Five" };
        newWaveTitle.text = "- Wave " + numbers[newWave-1] +" -";
        string enemyCountString = ((spawner.waves[newWave - 1].infinite) ? "Infinite" : spawner.waves[newWave - 1].enemyCount + "");
        newWaveEnemyCount.text = "Enemies: " + enemyCountString;

        StopCoroutine("AnimateNewWaveBanner");
        StartCoroutine("AnimateNewWaveBanner");
    }

    IEnumerator AnimateNewWaveBanner() {

        float delayTime = 1.5f;
        float speed = 3f;
        float animatePercent = 0;
        int dir = 1;
        float endDelayTime = Time.time + 1 / speed + delayTime;

        yield return new WaitForSeconds(.5f);
        while (animatePercent >= 0) {
            animatePercent += Time.deltaTime * speed * dir;

            if (animatePercent >= 1) {
                animatePercent = 1;
                if (Time.time > endDelayTime) {
                    dir = -1;
                }
            }

            newWaveBanner.anchoredPosition = Vector2.up * Mathf.Lerp(-250, 0, animatePercent);

            yield return null;
        }
    }

    void OnGameOver() {
        StartCoroutine(Fade(Color.clear, Color.black, 1));
        gameOverUI.SetActive(true);
    }

    IEnumerator Fade(Color from, Color to, float time) {
        float speed = 1 / time;
        float percent = 0;

        while (percent < 1) {
            percent += Time.deltaTime * speed;
            fadePlane.color = Color.Lerp(from, to, percent);
            yield return null;
        }
    }

    public void StartNewGame() {
        Application.LoadLevel("Game");
    }
}
