using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance;

    [Header("UI")]
    public Image fadeImage;
    public float fadeDuration = 1f; // 페이드 시간

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(FadeAndLoad(sceneName));
    }

    private IEnumerator FadeAndLoad(string sceneName)
    {
        yield return StartCoroutine(Fade(1f));

        Camera mainCam = Camera.main;
        if (mainCam != null) mainCam.enabled = false;

        yield return SceneManager.LoadSceneAsync(sceneName);

        yield return StartCoroutine(Fade(0f));

        mainCam = Camera.main;
        if (mainCam != null) mainCam.enabled = true;

        // 게임 재개
        Time.timeScale = 1f;
    }

    private IEnumerator Fade(float targetAlpha)
    {
        Color color = fadeImage.color;
        float startAlpha = color.a;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime; 
            float t = time / fadeDuration;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, t);
            fadeImage.color = color;
            yield return null;
        }

        color.a = targetAlpha;
        fadeImage.color = color;
    }
}
