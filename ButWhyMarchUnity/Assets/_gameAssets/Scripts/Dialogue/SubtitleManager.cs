using System.Collections;
using UnityEngine;
using TMPro;

// Dil seçeneklerimizi tanýmlýyoruz
public enum GameLanguage
{
    Turkish,
    English
}

public class SubtitleManager : MonoBehaviour
{
    public static SubtitleManager Instance;

    [SerializeField] private TextMeshProUGUI subtitleText;

    // Oyunun þu anki dili (Bunu ileride Ana Menüden deðiþtirebilirsin)
    [Header("Language Settings")]
    public GameLanguage currentLanguage = GameLanguage.Turkish;

    private Coroutine currentCoroutine;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }

        subtitleText.text = "";
        SetTextAlpha(0f);
    }

    public void PlaySequence(StorySequence sequence)
    {
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);

        subtitleText.text = "";
        SetTextAlpha(0f);

        currentCoroutine = StartCoroutine(ShowSubtitles(sequence));
    }

    private IEnumerator ShowSubtitles(StorySequence sequence)
    {
        foreach (DialogueLine line in sequence.lines)
        {
            // Ekrana yazdýrýlacak metni dile göre seçiyoruz!
            string textToShow = "";

            if (currentLanguage == GameLanguage.Turkish)
            {
                textToShow = line.turkishText;
            }
            else if (currentLanguage == GameLanguage.English)
            {
                textToShow = line.englishText;
            }

            // Seçilen metni UI'a ata
            subtitleText.text = textToShow;

            // Fade efektleri ayný kalýyor...
            yield return StartCoroutine(FadeText(0f, 1f, line.fadeInDuration));
            yield return new WaitForSeconds(line.displayDuration);
            yield return StartCoroutine(FadeText(1f, 0f, line.fadeOutDuration));
            yield return new WaitForSeconds(0.2f);
        }

        subtitleText.text = "";
    }

    private IEnumerator FadeText(float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;
        Color textColor = subtitleText.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            textColor.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            subtitleText.color = textColor;
            yield return null;
        }

        textColor.a = endAlpha;
        subtitleText.color = textColor;
    }

    private void SetTextAlpha(float alpha)
    {
        Color color = subtitleText.color;
        color.a = alpha;
        subtitleText.color = color;
    }
}