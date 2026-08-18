using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("SkillScene");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}