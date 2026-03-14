using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [Header("UI Paneli")]
    [SerializeField] private GameObject pauseMenuPanel; // Inspector'dan Pause panelini buraya sürükle

    private bool isPaused = false;

    void Update()
    {
        // 1. ESC tuþuna basýlýp basýlmadýðýný kontrol et
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        pauseMenuPanel.SetActive(true); // Paneli aç
        Time.timeScale = 0f;           // Oyunu durdur (Fizik ve zaman durur)
        isPaused = true;

        // Mouse imlecini görünür yap ve serbest býrak
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false); // Paneli kapat
        Time.timeScale = 1f;            // Oyunu devam ettir
        isPaused = false;

        // Mouse imlecini tekrar gizle (Eðer FPS oyunuysa gerekir)
        // Cursor.visible = false;
        // Cursor.lockState = CursorLockMode.Locked;
    }
}