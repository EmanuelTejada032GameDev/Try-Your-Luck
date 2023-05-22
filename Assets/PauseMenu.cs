using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool gameIsPaused = false;
    [SerializeField] private GameObject PauseMenuPanel;
    [SerializeField] private GameObject StartMenuPanel;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameIsPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void ResumeGame()
    {
        PauseMenuPanel.SetActive(false);
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
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        StartMenuPanel.SetActive(false);
        UIManager.Instance.GameStartupConfig();
    }


    public void PauseGame()
    {
        PauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        gameIsPaused = true;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
