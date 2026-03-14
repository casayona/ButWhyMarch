using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class IntroManager : MonoBehaviour
{
    [Header("UI Ayarlarý")]
    public Image blackScreen;

    [Header("Kameralar")]
    public GameObject introCamera;
    public GameObject mainPlayableCamera;

    [Header("Ýntro Süreleri")]
    public float wakeUpTime = 3f; // Ýlk uyanýþ süresi
    public float blinkTime = 0.5f; // Kamera geçerkenki göz kýrpma (kararma) süresi

    [Header("Bakýnma Ayarlarý")]
    public float lookAngle = 35f;
    public float lookSpeed = 1.5f;

    [Header("Karakter Kontrolü")]
    public MonoBehaviour playerMovementScript;

    private void Start()
    {
        StartCoroutine(PlayIntroSequence());
    }

    private IEnumerator PlayIntroSequence()
    {
        // 1. HAZIRLIK
        if (playerMovementScript != null)
            playerMovementScript.enabled = false;

        if (mainPlayableCamera != null) mainPlayableCamera.SetActive(false);
        introCamera.SetActive(true);
        blackScreen.gameObject.SetActive(true);

        SetAlpha(1f); // Ekran tam siyah baþlýyor
        yield return new WaitForSeconds(1f);

        // 2. ÝLK UYANIÞ (Gözler yavaþça açýlýr)
        yield return StartCoroutine(FadeBlackScreen(1f, 0f, wakeUpTime));

        // 3. NORMAL KAMERAYI SAÐA SOLA ÇEVÝR
        Transform camTransform = introCamera.transform;
        Quaternion baslangicAcisi = camTransform.rotation;

        Quaternion solaBakis = baslangicAcisi * Quaternion.Euler(0, -lookAngle, 0);
        Quaternion sagaBakis = baslangicAcisi * Quaternion.Euler(0, lookAngle, 0);

        // Sola Dön
        yield return StartCoroutine(YumusakKameraDonusu(camTransform, solaBakis, lookSpeed));
        yield return new WaitForSeconds(0.3f);

        // Saða Dön
        yield return StartCoroutine(YumusakKameraDonusu(camTransform, sagaBakis, lookSpeed * 1.5f));
        yield return new WaitForSeconds(0.3f);

        // Ortaya Dön
        yield return StartCoroutine(YumusakKameraDonusu(camTransform, baslangicAcisi, lookSpeed));
        yield return new WaitForSeconds(0.5f);

        // ==========================================
        // 4. KAMERA GEÇÝÞÝ ÝÇÝN GÖZLERÝ KAPAT (Blink)
        // ==========================================

        // Ekraný hýzlýca karart (Gözler kapanýyor)
        yield return StartCoroutine(FadeBlackScreen(0f, 1f, blinkTime));

        // EKRAN TAM SÝYAHKEN KAMERALARI DEÐÝÞTÝR! (Oyuncu kesilmeyi görmeyecek)
        introCamera.SetActive(false);
        if (mainPlayableCamera != null)
            mainPlayableCamera.SetActive(true);

        // Oyuncuya kontrolü ver
        if (playerMovementScript != null)
            playerMovementScript.enabled = true;

        // Biraz karanlýkta kal (Daha dramatik olur, istersen bu süreyi kýsaltabilirsin)
        yield return new WaitForSeconds(0.2f);

        // 5. GÖZLERÝ 3. ÞAHIS KAMERASINDA TEKRAR AÇ
        yield return StartCoroutine(FadeBlackScreen(1f, 0f, blinkTime));

        // Ýþimiz bitti, siyah ekraný tamamen kapat
        blackScreen.gameObject.SetActive(false);
    }

    private IEnumerator YumusakKameraDonusu(Transform camTransform, Quaternion hedefAci, float sure)
    {
        Quaternion baslangic = camTransform.rotation;
        float gecenZaman = 0f;

        while (gecenZaman < sure)
        {
            gecenZaman += Time.deltaTime;

            float t = gecenZaman / sure;
            t = t * t * (3f - 2f * t);

            camTransform.rotation = Quaternion.Slerp(baslangic, hedefAci, t);
            yield return null;
        }

        camTransform.rotation = hedefAci;
    }

    private IEnumerator FadeBlackScreen(float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;
        Color color = blackScreen.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            blackScreen.color = color;
            yield return null;
        }

        color.a = endAlpha;
        blackScreen.color = color;
    }

    private void SetAlpha(float alpha)
    {
        Color c = blackScreen.color;
        c.a = alpha;
        blackScreen.color = c;
    }
}