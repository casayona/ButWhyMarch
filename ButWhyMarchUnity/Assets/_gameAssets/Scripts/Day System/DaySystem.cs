using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using GlobalSnowEffect;

public class DaySystem : MonoBehaviour
{
    [Header("Sistem Baðlantýlarý")]
    public Tenkoku.Core.TenkokuModule tenkoku;
    [Tooltip("Global Snow scriptinin olduðu objeyi (genelde Main Camera) buraya sürükle")]
    public GlobalSnow globalSnow;
    [Tooltip("Rengini deðiþtireceðimiz Iþýk (Directional Light)")]
    public Light hedefIsik;

    [Header("Saat Ayarlarý (24 Saatlik)")]
    public float geceBaslamaSaati = 19f; // Akþam 7
    public float gunduzBaslamaSaati = 6f; // Sabah 6

    [Header("Geçiþ Hýzý")]
    [Tooltip("Deðerlerin ne kadar yavaþ deðiþeceðini ayarlar. Düþük deðer = Daha yavaþ geçiþ")]
    public float gecisHizi = 0.5f;

    // --- GELÝÞMÝÞ HAVA DURUMU SINIFI ---
    [System.Serializable]
    public class HavaAyarlari
    {
        [Header("Temel Ayarlar (Iþýk ve Tenkoku Kodu)")]
        public Color isikRengi;
        public int tenkokuKodu; // 0: Güneþli, 7: Fýrtýna vb.

        [Header("Global Snow 2 Ayarlarý")]
        [Range(0, 1)] public float snowMiktari;  // Yerdeki kar
        [Range(0, 1)] public float frostMiktari; // Kameradaki buzlanma/don

        [Header("Geliþmiþ Tenkoku Detaylarý")]
        [Range(0, 1)] public float bulutKalinligi; // Overcast (Güneþi kapatýr)
        [Range(0, 2)] public float bulutHizi;      // Cloud Speed
        [Range(0, 1)] public float sisMiktari;     // Fog Amount
        [Range(0, 1)] public float ruzgarSiddeti;  // Wind Amount (Uðultu sesini de artýrýr)
        [Range(0, 1)] public float yagmurKar;      // Lapa lapa kar/yaðmur þiddeti
        [Range(0, 1)] public float simsekSiddeti;  // Lightning Amount
        [Range(0, 1)] public float donmaSiddeti;  // Lightning Amount
    }

    [Header("GÜNDÜZ AYARLARI")]
    public HavaAyarlari gunduz;

    [Header("GECE AYARLARI")]
    public HavaAyarlari gece;

    private bool geceModundaMi = false;

    void Start()
    {
        if (tenkoku == null) tenkoku = FindObjectOfType<Tenkoku.Core.TenkokuModule>();

        if (globalSnow == null && Camera.main != null)
        {
            globalSnow = Camera.main.GetComponent<GlobalSnow>();
        }

        // Kendi lerp (yumuþak geçiþ) sistemimizi kullanacaðýmýz için 
        // Tenkoku'nun otomatik hava deðiþimini kapatýyoruz ki bizim kodla çakýþmasýn
        if (tenkoku != null)
        {
            tenkoku.weather_setAuto = false;
        }
    }

    void Update()
    {
        if (tenkoku == null) return;

        // 1. SAAT KONTROLÜ (Gece mi Gündüz mü?)
        float suAnkiSaat = tenkoku.currentHour;
        bool suAnGece = (suAnkiSaat >= geceBaslamaSaati || suAnkiSaat < gunduzBaslamaSaati);

        // Tenkoku'nun ana hava durumunu (Index) sadece saat deðiþtiðinde 1 kere tetikliyoruz
        if (suAnGece && !geceModundaMi)
        {
            geceModundaMi = true;
            tenkoku.weatherTypeIndex = gece.tenkokuKodu;
        }
        else if (!suAnGece && geceModundaMi)
        {
            geceModundaMi = false;
            tenkoku.weatherTypeIndex = gunduz.tenkokuKodu;
        }

        // 2. TÜM DETAYLARI YAVAÞÇA DEÐÝÞTÝRME (LERP SÝSTEMÝ)
        YavascaGecisYap(suAnGece);
    }

    void YavascaGecisYap(bool suAnGece)
    {
        // Hedefimiz Gece mi yoksa Gündüz deðerleri mi?
        HavaAyarlari hedef = suAnGece ? gece : gunduz;

        // Zaman çarpaný (Geçiþin yumuþaklýðýný saðlar)
        float t = Time.deltaTime * gecisHizi;

        // --- IÞIÐI YAVAÞÇA DEÐÝÞTÝR ---
        if (hedefIsik != null)
        {
            hedefIsik.color = Color.Lerp(hedefIsik.color, hedef.isikRengi, t);
        }

        // --- GLOBAL SNOW 2 YAVAÞÇA DEÐÝÞTÝR ---
        if (globalSnow != null)
        {
            // Kar kalýnlýðý
            globalSnow.snowAmount = Mathf.Lerp(globalSnow.snowAmount, hedef.snowMiktari, t);

            // Senin yorum satýrýna aldýðýn kýsýmdaki hata düzeltildi:
            // (Mathf.Lerp içine obje deðil, objenin þu anki deðeri yazýlýr)
            globalSnow.cameraFrostSpread = Mathf.Lerp(globalSnow.cameraFrostIntensity, hedef.frostMiktari, t);
            globalSnow.slopeThreshold = Mathf.Lerp(globalSnow.slopeThreshold, hedef.donmaSiddeti, t);


        }

        // --- TENKOKU ÝNCE AYARLARI (RÜZGAR, SÝS, BULUT) YAVAÞÇA DEÐÝÞTÝR ---
        if (tenkoku != null)
        {
            tenkoku.weather_OvercastAmt = Mathf.Lerp(tenkoku.weather_OvercastAmt, hedef.bulutKalinligi, t);
            tenkoku.weather_cloudSpeed = Mathf.Lerp(tenkoku.weather_cloudSpeed, hedef.bulutHizi, t);
            tenkoku.weather_FogAmt = Mathf.Lerp(tenkoku.weather_FogAmt, hedef.sisMiktari, t);
            tenkoku.weather_WindAmt = Mathf.Lerp(tenkoku.weather_WindAmt, hedef.ruzgarSiddeti, t);
            tenkoku.weather_SnowAmt = Mathf.Lerp(tenkoku.weather_SnowAmt, hedef.yagmurKar, t);
            tenkoku.weather_lightning = Mathf.Lerp(tenkoku.weather_lightning, hedef.simsekSiddeti, t);
        }
    }
}