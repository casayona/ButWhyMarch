using UnityEngine;
using UnityEngine.UI; // UI (Slider) kullanmak için bu satır şart!

public class PenguinHunger : MonoBehaviour
{
    [Header("Açlık Ayarları")]
    public Slider hungerSlider;      // Canvas içindeki Slider'ı buraya sürükleyin
    public float maxHunger = 100f;   // Maksimum açlık
    public float currentHunger;      // Mevcut açlık

    [Header("Azalma Hızları (Saniyede)")]
    public float normalDrainRate = 2f;  // Normal dururken/yürürken
    public float sprintDrainRate = 8f;  // Shift ile koşarken (Daha hızlı)

    void Start()
    {
        // Oyun başında penguen tok başlasın
        currentHunger = maxHunger;

        // Slider'ın en yüksek değerini ayarla
        if (hungerSlider != null)
        {
            hungerSlider.maxValue = maxHunger;
            hungerSlider.value = maxHunger;
        }
    }

    void Update()
    {
        HandleHungerDecay();
    }

    void HandleHungerDecay()
    {
        if (currentHunger > 0)
        {
            float drainAmount = normalDrainRate;

            // 3D HAREKET VE SHIFT KONTROLÜ
            // Sol veya Sağ Shift tuşuna basılıyorsa açlık daha hızlı düşer
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                drainAmount = sprintDrainRate;
            }

            // Açlığı zaman geçtikçe düşür
            currentHunger -= drainAmount * Time.deltaTime;

            // Slider'ı güncelle
            hungerSlider.value = currentHunger;
        }
        else
        {
            currentHunger = 0;
            Debug.Log("Penguen çok acıktı! Hareket edemiyor veya canı gidiyor.");
            // İstersen buraya: SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
            // diyerek bölümü yeniden başlatabilirsin.
        }
    }

    // Yemek Yeme Fonksiyonu
    public void Eat(float energy)
    {
        currentHunger += energy;

        // Maksimumu aşmasın
        if (currentHunger > maxHunger)
        {
            currentHunger = maxHunger;
        }

        hungerSlider.value = currentHunger;
    }

    // 3D ÇARPIŞMA KONTROLÜ
    private void OnTriggerEnter(Collider other)
    {
        // Eğer çarptığın objenin Tag'ı "Food" ise
        if (other.CompareTag("Food"))
        {
            Eat(20f); // 20 birim yemek ekle
            Destroy(other.gameObject); // Sahnedeki balığı yok et
            Debug.Log("Balık yendi, enerji doldu!");
        }
    }
}