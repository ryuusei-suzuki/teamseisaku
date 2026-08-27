using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
public class UIText : MonoBehaviour
{
    // talkText:喋っている内容やナレーション
    public TextMeshProUGUI talkText;
   

    public bool playing = false;
    public float textSpeed = 0.1f;
    void Start() { }
    // クリックで次のページを表示させるための関数
    public bool IsClicked()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            return true;

        return false;
    }
    // ナレーション用のテキストを生成する関数
    public void DrawText(string text)
    {
        Debug.Log("DrawText：" + text);
        StartCoroutine(CoDrawText(text));
    }

    // テキストがヌルヌル出てくるためのコルーチン
    IEnumerator CoDrawText(string text)
    {
        playing = true;
        float time = 0;
        while (true)
        {
            yield return 0;
            time += Time.deltaTime;
            // クリックされると一気に表示
            if (IsClicked()) break;
            int len = Mathf.FloorToInt(time / textSpeed);
            if (len > text.Length) break;
            talkText.text = text.Substring(0, len);
        }
        talkText.text = text;
        yield return 0;
        playing = false;
    }
}