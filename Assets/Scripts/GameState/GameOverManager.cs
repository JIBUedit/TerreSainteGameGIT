using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverUi;

    public static GameOverManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Il y a plus d'une instance de GameOverManager dans la scène");
            return;
        }

        instance = this;
    }

    public void OnPlayerDeath()
    {
        gameOverUi.SetActive(true);
    }

    public void RetryButton()
    {
        // Recommencer le niveau
        // Recharge la scène
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        // Replace le joueur au spawn
        // Réactive les mouvements du joueur + qu'on lui rende sa vie
        gameOverUi.SetActive(false);
    }

    public void MainMenuButton()
    {
        // Retour au menu principal
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitButton()
    {
        // Fermer le jeu
        Application.Quit();
    }
}
