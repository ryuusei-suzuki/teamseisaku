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

    [Header("サウンド")]
    [SerializeField] private TitleSound TitleSound;

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
        TitleSound.OnClick();
        mainMenu.SetActive(false);
        optionPanel.SetActive(true);
        exitPanel.SetActive(false);
        Blocker.SetActive(true);
        volumeBoard.Show();
    }

    public void HideOption()
    {
        TitleSound.OnClick();
        volumeBoard.Hide();
        Blocker.SetActive(false);

    }

    // 終了確認を表示
    public void ShowExit()
    {
        TitleSound.OnClick();
        mainMenu.SetActive(false);
        optionPanel.SetActive(false);
        exitPanel.SetActive(true);

        exitwindow.Show();

    }

    public void HideExit()
    {
        TitleSound.OnClick();
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