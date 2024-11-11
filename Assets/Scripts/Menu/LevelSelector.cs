using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    public GameObject levelSelector;
    public void LoadLevelPassed(string levelName)
    {
        SceneManager.LoadScene(levelName);
        Time.timeScale = 1;
    }
    public void CloseLevelSelector()
    {
        levelSelector.SetActive(false);
        Time.timeScale = 1;
    }
}
