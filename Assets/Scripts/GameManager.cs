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
            // ���������� ��������� ��� ������������
            Destroy(gameObject);
        }
    }

    public void TriggerGameOver(string reason)
    {
        if (isGameOver) return;
        isGameOver = true;

        // ��������� ������� ����� TimeManager
        TimeManager.Instance.StopTime(); 

        Cursor.lockState = CursorLockMode.None;

        // �������� UI
        gameOverPanel.SetActive(true);
        gameOverText.text = $"GAME OVER\n{reason}";
    }

    public void RestartLevel()
    {
        // ������� DontDestroyOnLoad ��� �������� GameManager
        if (Instance == this)
        {
            Instance = null;
            Destroy(gameObject);
        }

        // ����� ���������� �������
        Time.timeScale = 1f;
        TimeManager.Instance?.ResetTimeScale();

        // ������������ �����
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}