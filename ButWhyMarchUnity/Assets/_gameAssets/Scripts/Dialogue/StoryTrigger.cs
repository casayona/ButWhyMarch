using UnityEngine;

public class StoryTrigger : MonoBehaviour
{
    public StorySequence sequenceToPlay; // Bu bölgeye girilince çalacak hikaye verisi
    private bool hasPlayed = false; // Oyuncu geri dönerse tekrar çalmasýný engellemek için

    private void OnTriggerEnter(Collider other)
    {
        // Çarpan obje "Player" etiketine sahipse ve bu hikaye daha önce çalmadýysa
        if (other.CompareTag("Player") && !hasPlayed)
        {
            hasPlayed = true; // Tekrar çalmasýný engelle
            SubtitleManager.Instance.PlaySequence(sequenceToPlay); // Altyazýyý baþlat
        }
    }
}