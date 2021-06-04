using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public Image fadePlane;
    public GameObject gameOverUI;

    public RectTransform newWaveBanner;
    public Text newWaveTitle;
    public Text newWaveEnemyCount;
    public Text scoreUI;
    public Text gameOverScoreUI;
    public Text highestScoreUI;
    public RectTransform healthBar;


    Spawner spawner;
    Player player;

    void Start()
    {
        player=FindObjectOfType<Player>();
        player.OnDeath += OnGameOver;
        fadePlane.gameObject.SetActive(false);
    }

    private void Awake()
    {
        spawner = FindObjectOfType<Spawner>();
        spawner.OnNewWave += OnNewWave;
    }

    private void Update()
    {
        scoreUI.text = ScoreKeeper.score.ToString("D6");
        float healthPercent = 0;
        if (player != null)
        {
            healthPercent = player.health / player.startingHealth;
        }   
        healthBar.localScale = new Vector3(healthPercent, 1, 1);
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
        fadePlane.gameObject.SetActive(true);
        StartCoroutine(Fade(Color.clear, Color.black, 1));
        int score = ScoreKeeper.score;
        int highestScore = PlayerPrefs.GetInt("Highest Score", score);
        if (score > highestScore)
        {
            highestScore = score;
        }
        PlayerPrefs.SetInt("Highest Score", highestScore);
        highestScoreUI.text = "Highest Score: " + highestScore.ToString("D6");
        gameOverScoreUI.text = "Your Score: " + scoreUI.text;
        healthBar.transform.parent.gameObject.SetActive(false);
        gameOverUI.SetActive(true);
        AudioManager.instance.SetVolume(.3f, AudioManager.AudioChannel.Music);
    }

    IEnumerator Fade(Color from, Color to, float time) {
        float speed = 1 / time;
        float percent = 0;

        yield return new WaitForSeconds(.2f);
        while (percent < 1) {
            percent += Time.deltaTime * speed;
            fadePlane.color = Color.Lerp(from, to, percent);
            yield return null;
        }
    }

    public void StartNewGame() {
        AudioManager.instance.SetVolume(.7f, AudioManager.AudioChannel.Music);
        SceneManager.LoadScene(0);
        //Application.LoadLevel("Game");
    }
}
