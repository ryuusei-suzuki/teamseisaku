using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class SkillHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("説明文を表示するPanel")]
    public GameObject descriptionPanel;

    [Header("説明文")]
    [TextArea(2, 5)]
    public string description;

    [Header("説明文のText")]
    public TMP_Text descriptionText;


    // カーソルがスキルアイコンに入ったとき
    public void OnPointerEnter(PointerEventData eventData)
    {
        descriptionPanel.SetActive(true);

        descriptionText.text = description;
    }


    // カーソルがスキルアイコンから出たとき
    public void OnPointerExit(PointerEventData eventData)
    {
        descriptionPanel.SetActive(false);
    }
}