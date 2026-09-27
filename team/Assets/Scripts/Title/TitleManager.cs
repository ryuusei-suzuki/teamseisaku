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

    public void ExitGame()
    {
        Application.Quit();
    }
}