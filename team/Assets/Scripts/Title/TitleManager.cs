using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{

    public void StartGame()
    {
        SceneManager.LoadScene("Tutorial Scene");
    }

    public void BattleGame()
    {
        SceneManager.LoadScene("suzki");
    }

    public void SelectSkill()
    {
        SceneManager.LoadScene("Skill selection");
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}