using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class BattleBoard : MonoBehaviour
{
    [SerializeField] private GameObject battleBoard;
    [SerializeField] private GameObject YesButton;
    [SerializeField] private GameObject NoButton;
    [SerializeField] private GameObject NigeruButton;
    [SerializeField] private GameObject Block;
    [SerializeField] private NIGERUeffect nIGERUeffect;

    void Start()
    {
        // 初期状態ではバトルボードとボタンを非表示にする
        battleBoard.SetActive(false);
        YesButton.SetActive(false);
        NoButton.SetActive(false);
        Block.SetActive(false);
        NigeruButton.SetActive(true);
    }

    public void ShowBattleBoard()
    {
        if (nIGERUeffect.isTransitioning) return; // すでに遷移中なら何もしない
        // バトルボードとボタンを表示する
        battleBoard.SetActive(true);
        YesButton.SetActive(true);
        NoButton.SetActive(true);
        Block.SetActive(true);
        NigeruButton.SetActive(false);
    }

    public void HideBattleBoard()
    {
        if (nIGERUeffect.isTransitioning) return; // すでに遷移中なら何もしない
        // バトルボードとボタンを非表示にする
        battleBoard.SetActive(false);
        YesButton.SetActive(false);
        NoButton.SetActive(false);
        Block.SetActive(false);
        NigeruButton.SetActive(true);
    }
}
