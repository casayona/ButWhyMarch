using UnityEngine;
using UnityEngine.AI; // NavMeshAgent bileþenini kullanmak için gerekli kütüphane
using System.Collections; // Coroutine'ler (IEnumerator) için gerekli kütüphane

public class RandomPenguinWalker : MonoBehaviour
{
    // --- INSPECTOR'DAN AYARLANABÝLÝR PARAMETRELER ---

    [Header("Hareket Ayarlarý")]
    [SerializeField] private float walkSpeed = 1.5f; // Penguenin yürüme hýzý
    [SerializeField] private float patrolRadius = 20f; // Penguenin baþlangýç noktasý etrafýnda gezinebileceði maksimum yarýçap
    [SerializeField] private float minWaitTime = 2f; // Hedefe ulaþtýktan sonra minimum bekleme süresi
    [SerializeField] private float maxWaitTime = 5f; // Hedefe ulaþtýktan sonra maksimum bekleme süresi
    [SerializeField] private float destinationThreshold = 0.5f; // Penguenin bir hedefe ne kadar yaklaþtýðýnda "ulaþtý" sayýlacaðýný belirler

    [Header("Animasyon Ayarlarý")]
    [SerializeField] private Animator penguinAnimator; // Penguenin Animator bileþeni
    [SerializeField] private string walkAnimParam = "isWalking"; // Yürüme animasyonunu kontrol eden bool parametresinin adý

    // --- ÖZEL DEÐÝÞKENLER ---

    private NavMeshAgent agent; // Penguenin NavMesh üzerinde hareket etmesini saðlayan bileþen
    private Vector3 startPosition; // Penguenin baþlangýçtaki global pozisyonu (gezinti alaný için referans noktasý)
    private bool isWaiting = false; // Penguenin þu anda bekleyip beklemediðini tutan bayrak

    // --- UNITY YAÞAM DÖNGÜSÜ METOTLARI ---

    // Oyun objesi ilk yüklendiðinde bir kere çalýþýr (Start'tan bile önce)
    void Awake()
    {
        // NavMeshAgent bileþenini al
        agent = GetComponent<NavMeshAgent>();
        if (agent == null) // Eðer NavMeshAgent bulunamazsa hata mesajý ver ve script'i devre dýþý býrak
        {
            Debug.LogError("NavMeshAgent bulunamadý! Lütfen penguen GameObject'ine NavMeshAgent ekleyin.");
            enabled = false; // Bu script'in çalýþmasýný durdur
            return;
        }

        // Animator bileþenini al (çocuk objelerinde de arayabiliriz)
        if (penguinAnimator == null)
        {
            penguinAnimator = GetComponentInChildren<Animator>();
            if (penguinAnimator == null) // Eðer Animator bulunamazsa uyarý ver
            {
                Debug.LogWarning("Animator bulunamadý! Animasyonlar oynatýlmayacak.");
            }
        }

        startPosition = transform.position; // Penguenin baþlangýç pozisyonunu kaydet
        agent.speed = walkSpeed; // NavMeshAgent'ýn yürüme hýzýný ayarla

        // Penguenin devriye gezme rutini coroutine'ini baþlat
        StartCoroutine(PatrolRoutine());
    }

    // Her karede bir kere çalýþýr
    void Update()
    {
        // Eðer Animator atanmýþsa, yürüme animasyonu parametresini güncelle
        if (penguinAnimator != null)
        {
            // Penguenin hýzý belirli bir eþiðin üzerindeyse (yani hareket ediyorsa), isWalking parametresini true yap
            // Aksi takdirde (duruyorsa) false yap
            penguinAnimator.SetBool(walkAnimParam, agent.velocity.magnitude > 0.1f);
        }
    }

    // --- COROUTINE METOTLARI ---

    // Penguenin rastgele gezinti ve bekleme rutinini yöneten coroutine
    IEnumerator PatrolRoutine()
    {
        // Bu rutin oyun boyunca sürekli çalýþacak
        while (true)
        {
            // Eðer penguen beklemiyorsa VE hedefine yeterince yaklaþtýysa VE yeni bir yol hesaplamýyorsa
            if (!isWaiting && agent.remainingDistance < destinationThreshold && !agent.pathPending)
            {
                // Rastgele bir bekleme süresi belirle
                float waitTime = Random.Range(minWaitTime, maxWaitTime);
                isWaiting = true; // Penguenin beklediðini iþaretle

                // Eðer Animator atanmýþsa, yürüme animasyonunu kapat (idle animasyonuna geçiþ yapar)
                if (penguinAnimator != null) penguinAnimator.SetBool(walkAnimParam, false);

                yield return new WaitForSeconds(waitTime); // Belirlenen süre boyunca bekle

                isWaiting = false; // Bekleme süresi bitti, penguen artýk beklemiyor
                SetRandomDestination(); // Yeni bir rastgele hedef belirle
            }
            yield return null; // Bir sonraki kareye kadar bu coroutine'i duraklat
        }
    }

    // --- YARDIMCI METOTLAR ---

    // Penguen için NavMesh üzerinde rastgele bir hedef belirler
    void SetRandomDestination()
    {
        // Baþlangýç noktasý etrafýnda rastgele bir yön belirle
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += startPosition; // Bu yönü baþlangýç pozisyonuna göre ayarla

        NavMeshHit hit;
        // Belirlenen rastgele noktanýn NavMesh üzerinde geçerli olup olmadýðýný kontrol et
        // NavMesh.SamplePosition, rastgele noktanýn en yakýnýndaki NavMesh noktasýný bulur
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position); // NavMeshAgent'ýn yeni hedefini ayarla
        }
        // Eðer NavMesh üzerinde geçerli bir nokta bulunamazsa, bu döngüde bir þey yapmayýz.
        // PatrolRoutine bir sonraki döngüsünde tekrar yeni bir hedef aramaya çalýþýr.
    }

    // --- EDITOR GÖRSELLEÞTÝRMELERÝ ---

    // Sadece Editörde GameObject seçiliyken çalýþan bir metot (debug amaçlý)
    void OnDrawGizmosSelected()
    {
        // Penguenin baþlangýç noktasý etrafýndaki devriye gezme alanýný görselleþtirmek için bir küre çizer
        Gizmos.color = Color.cyan; // Çizimin rengi
        Gizmos.DrawWireSphere(startPosition, patrolRadius); // Küreyi çiz
    }
}