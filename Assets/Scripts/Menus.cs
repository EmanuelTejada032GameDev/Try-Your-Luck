
using System.Collections;
using TMPro;
using UnityEngine;

public class Menus : MonoBehaviour
{
    public static Menus Instance;

    public static bool gameIsPaused = false;
    [SerializeField] private GameObject PauseMenuPanel;
    [SerializeField] private GameObject StartMenuPanel;
    [SerializeField] private GameObject GameOverMenuPanel;


    [SerializeField] private TextMeshProUGUI _finalScoreTMP;
    [SerializeField] private TextMeshProUGUI _currencySpentTMP;
    [SerializeField] private TextMeshProUGUI _commonPacksTMP;
    [SerializeField] private TextMeshProUGUI _plusPacksTMP;

    [SerializeField] private AudioClip _gameOverClip;

    private bool _isGameOver = false;
    private bool _isStartScreen = true;

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape)))
        {
            if (!_isGameOver && !_isStartScreen)
            {
                if (gameIsPaused)
                    ResumeGame();
                else
                    PauseGame();
            }
        }
    }

    public void ResumeGame()
    {
        PauseMenuPanel.SetActive(false);
        UIManager.Instance.PauseGameLoopMusic(false);
        Time.timeScale = 1f;
        gameIsPaused = false;
    }

    public void MainMenu()
    {
        PauseMenuPanel.SetActive(false);
        StartMenuPanel.SetActive(true);
        UIManager.Instance.ResetGame();
        Time.timeScale = 0f;
        gameIsPaused = false;
        _isStartScreen = true;
        _isGameOver = false;
    }

    public void MainMenuFromGameOver()
    {
        GameOverMenuPanel.SetActive(false);
        StartMenuPanel.SetActive(true);
        UIManager.Instance.ResetGame();
        Time.timeScale = 0f;
        gameIsPaused = false;
        _isStartScreen = true;
        _isGameOver = false;
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        StartMenuPanel.SetActive(false);
        UIManager.Instance.GameStartupConfig();
        _isStartScreen = false;
    }

    public void GameOver()
    {
        Debug.Log($"Game Over, Score: {UIManager.Instance.PlayerScore}");
        StartCoroutine("SetGameOver");
    }

    IEnumerator SetGameOver()
    {
        yield return new WaitForSeconds(1);
        UIManager.Instance.PauseGameLoopMusic(true);
        UIManager.Instance.PlayOneShotClip(_gameOverClip);
        _finalScoreTMP.text = UIManager.Instance.PlayerScore.ToString();
        _currencySpentTMP.text = UIManager.Instance.CurrencySpent.ToString();
        _commonPacksTMP.text = UIManager.Instance.CommonPacksBougths.ToString();
        _plusPacksTMP.text = UIManager.Instance.PlusPacksBougths.ToString();
        GameOverMenuPanel.SetActive(true);
        _isGameOver = true;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        GameOverMenuPanel.SetActive(false);
        UIManager.Instance.ResetGame();
        UIManager.Instance.PauseGameLoopMusic(false);
        UIManager.Instance.GameStartupConfig();
        _isGameOver = false;


    }

    public void PauseGame()
    {
        PauseMenuPanel.SetActive(true);
        UIManager.Instance.PauseGameLoopMusic(true);
        Time.timeScale = 0f;
        gameIsPaused = true;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}




