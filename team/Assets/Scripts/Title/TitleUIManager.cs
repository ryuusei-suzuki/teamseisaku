using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class TitleUIManager : MonoBehaviour
{
    [Header("タイトル画面")]
    [SerializeField] private GameObject mainMenu;

    [Header("オプション画面")]
    [SerializeField] private GameObject optionPanel;
    [SerializeField] private TitleVolumeBoard volumeBoard;

    [Header("終了確認画面")]
    [SerializeField] private GameObject exitPanel;
    [SerializeField] private EXITWindow exitwindow;
    [SerializeField] private GameObject Blocker;


    private void Start()
    {
        ShowMainMenu();
    }

    // メインメニューを表示
    public void ShowMainMenu()
    {
        mainMenu.SetActive(true);
        optionPanel.SetActive(false);
        exitPanel.SetActive(false);
        Blocker.SetActive(false);
    }

    // オプションを表示
    public void ShowOption()
    {
        mainMenu.SetActive(false);
        optionPanel.SetActive(true);
        exitPanel.SetActive(false);
        Blocker.SetActive(true);
        volumeBoard.Show();
    }

    public void HideOption()
    {
        Blocker.SetActive(false);
        volumeBoard.Hide();

    }

    // 終了確認を表示
    public void ShowExit()
    {
        mainMenu.SetActive(false);
        optionPanel.SetActive(false);
        exitPanel.SetActive(true);

        exitwindow.Show();

    }

    public void HideExit()
    {
        exitwindow.Hide();
    }

    public void BackHome()
    {
        mainMenu.SetActive(false);
        optionPanel.SetActive(false);
        exitPanel.SetActive(false);
    }
    //一旦これ
}