using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Collections.AllocatorManager;
public class TextWriter : MonoBehaviour
{
    [SerializeField] private TutorialPanel tutorialPanel;
    [SerializeField] private TrialBattleManager trialBattleManager; // ttrmanagerをInspectorで割り当てる

    public UIText uitext;
    public GameObject TutorialPanel;

    void Awake()
    {
        TutorialPanel.SetActive(false);
    }
    void Start()
    {
        StartCoroutine("Cotest");
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
        uitext.DrawText("「あなたの実力はいかほどか…まずはこのモンスターを倒してみてください」");
        yield return StartCoroutine("Skip");
        TutorialPanel.SetActive(true);
        tutorialPanel.StartTutorial();

        yield return new WaitUntil(() => tutorialPanel.IsTutorialFinished);

        uitext.DrawText("使用するスキルを選ぼう");
        yield return StartCoroutine("Skip");

        trialBattleManager.StartBattle();
        
    }
}