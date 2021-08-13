using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public Image background;

    private void Start()
    {
        Pause();
    }

    public void Play() {
        SceneManager.UnloadSceneAsync(1);
        Resume();
    }

    public void Quit() {
        Application.Quit();
    }


    void Pause() {
        AudioManager.instance.PlayMusic("Menu", 1);
        Time.timeScale = 0;
       // StartCoroutine(Fade(Color.clear, new Color(0, 0, 0, 0.9f), 0.2f));
    }
    void Resume()
    {
        AudioManager.instance.PlayMusic("Main", 1);
        Time.timeScale = 1;
    }

    IEnumerator Fade(Color from, Color to, float time)
    {
        float speed = 1 / time;
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.unscaledDeltaTime * speed;
            background.color = Color.Lerp(from, to, percent);
            yield return null;
        }

    }
}
