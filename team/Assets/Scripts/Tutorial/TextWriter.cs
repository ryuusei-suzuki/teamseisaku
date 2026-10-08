using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Collections.AllocatorManager;
public class TextWriter : MonoBehaviour
{
    [SerializeField] private TutorialPanel tutorialPanel;
    [SerializeField] private TrialBattleManager trialBattleManager;
    public GameObject texbox;
    public UIText uitext;
    public GameObject TutorialPanel;

    void Awake()
    {
        TutorialPanel.SetActive(false);
        texbox.SetActive(false);
    }
    public void effectfin()
    {
        texbox.SetActive(true);
        StartCoroutine(Cotest());
    }
    // クリック待ちのコルーチン
    IEnumerator Skip()
    {
        while (uitext.playing) yield return 0;
        while (!uitext.IsClicked()) yield return 0;
    }
    // 文章を表示させるコルーチン
    IEnumerator Cotest()
    {
        uitext.DrawText("「試練の間に挑戦されるんですね！」");
        yield return StartCoroutine("Skip");
        uitext.DrawText("「あなたの実力はいかほどか…まずはこのモンスターをたおしてみてください」");
        yield return StartCoroutine("Skip");
        TutorialPanel.SetActive(true);
        tutorialPanel.StartTutorial();

        yield return new WaitUntil(() => tutorialPanel.IsTutorialFinished);
        uitext.DrawText("「ルールはわかりましたか？ESCをおすといつでもルールが見れますよ」");
        yield return StartCoroutine("Skip");
        uitext.DrawText("「ではバトルスタート!」");
        yield return StartCoroutine("Skip");
        uitext.DrawText("「スキルアイコンを押してこうげきしよう！」");
        if (trialBattleManager != null)
        {
            trialBattleManager.StartBattle();
        }
    }
}