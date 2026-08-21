using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TextWriter : MonoBehaviour
{
    public UIText uitext;
    // Start is called before the first frame update
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
        uitext.DrawText("ナレーションだったらこのまま書けばOK");
        yield return StartCoroutine("Skip");
        uitext.DrawText("名前", "人が話すのならこんな感じ");
        yield return StartCoroutine("Skip");
    }
}