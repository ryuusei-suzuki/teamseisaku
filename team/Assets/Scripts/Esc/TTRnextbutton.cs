using UnityEngine;

public class TTRnextbutton : MonoBehaviour
{
    [SerializeField] private float scaleUp = 1.1f;

    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    // êGÇÍÇΩÇ∆Ç´
    public void OnPointerEnter()
    {
        transform.localScale = originalScale * scaleUp;
    }

    // ó£ÇÍÇΩÇ∆Ç´
    public void OnPointerExit()
    {
        transform.localScale = originalScale;
    }
}