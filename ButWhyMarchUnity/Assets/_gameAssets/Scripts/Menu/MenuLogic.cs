using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuLogic : MonoBehaviour
{
    [Header("Ayarlar")]
    [SerializeField] private string gameSceneName = "GameScene";
    [SerializeField] private Button continueButton; // Inspector'dan sürükle

    void Start()
    {
        // Eðer daha önce kaydedilmiþ bir bölüm yoksa "Continue" butonunu pasif yap
        if (continueButton != null)
        {
            // "CurrentLevel" anahtarý yoksa butona basýlamasýn
            continueButton.interactable = PlayerPrefs.HasKey("CurrentLevel");
        }
    }

    // YENÝ OYUN: Tüm eski kayýtlarý siler ve baþtan baþlatýr
    public void NewGame()
    {
        // Eski kayýtlarý temizle (isteðe baðlý)
        PlayerPrefs.DeleteKey("CurrentLevel");
        // Veya tüm PlayerPrefs'i sil: PlayerPrefs.DeleteAll();

        // Ýlk sahneyi yükle
        SceneManager.LoadScene(gameSceneName);
    }

    // DEVAM ET: Kayýtlý olan sahneyi veya veriyi yükler
    public void ContinueGame()
    {
        if (PlayerPrefs.HasKey("CurrentLevel"))
        {
            int levelToLoad = PlayerPrefs.GetInt("CurrentLevel");
            SceneManager.LoadScene(levelToLoad);
        }
        else
        {
            Debug.LogWarning("Kayýtlý oyun bulunamadý!");
        }
    }

    // AYARLAR VE ÇIKIÞ (Klasik Mekanikler)
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Oyun Kapatýldý.");
    }
}