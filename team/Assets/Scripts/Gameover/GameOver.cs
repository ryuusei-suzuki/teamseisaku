using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public void GotoTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }
    public void GotoRetry()
    {
        SceneManager.LoadScene("suzki");
    }
}
