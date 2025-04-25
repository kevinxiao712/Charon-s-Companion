using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;     

public class SceneTriggerLoader : MonoBehaviour
{
    [Header("Fade settings")]
    [Tooltip("Full-screen UI Image whose color is already pure blue (A = 0)")]
    [SerializeField] private Image fadeImage;     // overlay on a Canvas
    [Tooltip("Seconds it takes to fade from transparent to opaque")]
    [SerializeField] private float fadeDuration = 1f;

    private void Awake()
    {
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;                   // start fully transparent
            fadeImage.color = c;
            fadeImage.gameObject.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            StartCoroutine(FadeThenLoad());
    }

    private IEnumerator FadeThenLoad()
    {
        if (fadeImage != null)
        {
            float t = 0f;
            Color c = fadeImage.color;

            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                c.a = Mathf.Clamp01(t / fadeDuration);
                fadeImage.color = c;
                yield return null;
            }
        }

        int current = SceneManager.GetActiveScene().buildIndex;
        int next = current + 1;

        if (next < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(next);
        else
            Debug.Log("No more scenes to load.");
    }
}
