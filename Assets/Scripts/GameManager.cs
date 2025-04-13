using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Player Settings")]
    [SerializeField] private PlayerMovement playerController;

    [Header("UI Settings")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text gameOverText;

    private bool isGameOver;

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

    public void PausePlayerControls(bool pause)
    {
        if (playerController != null)
        {
            playerController.enabled = !pause;
        }
    }

    public void TriggerGameOver(string reason)
    {
        if (isGameOver) return;

        isGameOver = true;
        TimeManager.Instance.StopTime();
        Cursor.lockState = CursorLockMode.None;
        gameOverPanel.SetActive(true);
        gameOverText.text = $"GAME OVER\n{reason}";
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}