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
        isGameOver = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}