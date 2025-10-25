using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI_GameOver : MonoBehaviour
{
    [SerializeField] Button restartButton;
    [SerializeField] Button quitButton;

    void Start()
    {
        if (restartButton)
            restartButton.onClick.AddListener(RestartGame);

        if (quitButton)
            quitButton.onClick.AddListener(QuitGame);
    }

    void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void QuitGame()
    {
        Application.Quit();
    }
}
