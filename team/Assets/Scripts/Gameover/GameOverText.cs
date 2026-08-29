using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class GameOverText : MonoBehaviour
{
    public TextMeshProUGUI GameOverTalkText;

    public bool playing = false;
    public float textSpeed = 0.1f;

    private Coroutine currentCoroutine;

    public bool IsClicked()
    {
        if (Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            return true;
        }

        return false;
    }

    public void DrawText(string text)
    {
        // 現在表示中の文章を止める
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        // 新しい文章を表示
        currentCoroutine = StartCoroutine(CoDrawText(text));
    }

    private IEnumerator CoDrawText(string text)
    {
        playing = true;

        float time = 0f;

        while (true)
        {
            yield return null;

            time += Time.deltaTime;

            // クリックされたら一気に表示
            if (IsClicked())
            {
                break;
            }

            int len = Mathf.FloorToInt(time / textSpeed);

            if (len >= text.Length)
            {
                break;
            }

            GameOverTalkText.text = text.Substring(0, len);
        }

        // 最後は全文表示
        GameOverTalkText.text = text;

        playing = false;
        currentCoroutine = null;
    }
}
