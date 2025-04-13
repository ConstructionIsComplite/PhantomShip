using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Over Settings")]
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] TMP_Text gameOverText;

    private bool isGameOver = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Уничтожаем дубликаты при перезагрузке
            Destroy(gameObject);
        }
    }

    public void TriggerGameOver(string reason)
    {
        if (isGameOver) return;
        isGameOver = true;

        // Остановка времени через TimeManager
        TimeManager.Instance.StopTime(); 

        Cursor.lockState = CursorLockMode.None;

        // Показать UI
        gameOverPanel.SetActive(true);
        gameOverText.text = $"GAME OVER\n{reason}";
    }

    public void RestartLevel()
    {
        // Удаляем DontDestroyOnLoad для текущего GameManager
        if (Instance == this)
        {
            Instance = null;
            Destroy(gameObject);
        }

        // Сброс параметров времени
        Time.timeScale = 1f;
        TimeManager.Instance?.ResetTimeScale();

        // Перезагрузка сцены
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}