using UnityEngine;
using System.Collections; // Bu satır Coroutine (zamanlayıcı) için ŞARTTIR

public class ObjeAktifEt : MonoBehaviour
{
    public GameObject gosterilecekObje;  // E'ye basınca açılacak büyük resim/panel
    public GameObject etkilesimYazisi;    // Yaklaşınca çıkacak olan "E'ye Bas" yazısı

    private bool oyuncuYakininda = false;
    private Coroutine kapamaZamanlayici; // Çalışan sayacı kontrol etmek için

    void Start()
    {
        gosterilecekObje.SetActive(false);
        etkilesimYazisi.SetActive(false);
    }

    void Update()
    {
        // Oyuncu alandaysa ve E tuşuna basarsa
        if (oyuncuYakininda && Input.GetKeyDown(KeyCode.E))
        {
            // Eğer resim zaten açıksa ve tekrar E'ye basılırsa eski sayacı durdur
            if (kapamaZamanlayici != null)
                StopCoroutine(kapamaZamanlayici);

            // Resmi aç, ipucu yazısını kapat
            gosterilecekObje.SetActive(true);
            etkilesimYazisi.SetActive(false);

            // 10 saniye saymaya başla
            kapamaZamanlayici = StartCoroutine(OnSaniyeBekleVeKapat());
        }
    }

    // 10 Saniye Sayacak Olan Fonksiyon
    IEnumerator OnSaniyeBekleVeKapat()
    {
        yield return new WaitForSeconds(10f); // 10 saniye bekle

        gosterilecekObje.SetActive(false); // Resmi kapat

        // Eğer oyuncu hala objenin yanındaysa "E'ye bas" yazısını geri getir
        if (oyuncuYakininda)
        {
            etkilesimYazisi.SetActive(true);
        }

        kapamaZamanlayici = null; // İşlem bittiği için referansı temizle
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            oyuncuYakininda = true;
            // Eğer o sırada resim açık değilse yazıyı göster
            if (!gosterilecekObje.activeSelf)
            {
                etkilesimYazisi.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            oyuncuYakininda = false;
            etkilesimYazisi.SetActive(false);
            gosterilecekObje.SetActive(false);

            // Alan dışına çıkarsa çalışan bir sayaç varsa durdur
            if (kapamaZamanlayici != null)
            {
                StopCoroutine(kapamaZamanlayici);
                kapamaZamanlayici = null;
            }
        }
    }
}