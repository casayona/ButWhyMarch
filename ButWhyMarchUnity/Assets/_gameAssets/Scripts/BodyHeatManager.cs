using GlobalSnowEffect; // Kütüphane ismine dikkat, kýrmýz olursa GlobalSnow yapglo
using UnityEngine;

public class BodyHeatManager : MonoBehaviour
{
    [Header("Isý Ayarlarý")]
    public float currentHeat = 100f;
    public float maxHeat = 100f;
    public float heatLossSpeed = 1.5f;

    [Header("GlobalSnow Entegrasyonu")]
    public GlobalSnow snowEffect;

    [Header("Frost Eþiði (Threshold) Ayarlarý")]
    public float maxThreshold = 1.5f; // Sýcakken (Buz dýþarýda)
    public float minThreshold = 0.3f; // Donarken (Buz merkezde)

    [Header("Frost Yayýlma (Spread) Ayarlarý")]
    public float minSpread = 1.0f;
    public float maxSpread = 5.0f;

    void Update()
    {
        currentHeat -= heatLossSpeed * Time.deltaTime;
        currentHeat = Mathf.Clamp(currentHeat, 0, maxHeat);

        if (snowEffect != null)
        {
            float heatRatio = currentHeat / maxHeat;

            // 1. Intensity (Belirginlik)
            snowEffect.cameraFrostIntensity = 1 - heatRatio;

            // 2. DOÐRU DEÐÝÞKEN BURASI: cameraFrostThreshold
            // Eskiden slopeThreshold yazmýþtýn, o yüzden çalýþmýyordu.
            snowEffect.slopeThreshold = Mathf.Lerp(minThreshold, maxThreshold, heatRatio);

            // 3. Spread (Yayýlma)
            snowEffect.cameraFrostSpread = Mathf.Lerp(maxSpread, minSpread, heatRatio);

            // Opsiyonel: Kar yaðýþý
            snowEffect.snowfallIntensity = (1 - heatRatio) * 0.5f; // Çok aþýrý olmasýn diye 0.5 ile çarptým
        }

        if (currentHeat <= 0)
        {
            Debug.Log("Penguen tamamen dondu...");
        }
    }
}