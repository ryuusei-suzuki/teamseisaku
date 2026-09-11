using UnityEngine;

public class EscExit : MonoBehaviour
{
    [SerializeField] private EscUIManager escUIManager;

    public void ExitGame()
    {
        Application.Quit();
    }
    public void NoExitGame()
    {
        escUIManager.CloseEscPanel();
    }
}
