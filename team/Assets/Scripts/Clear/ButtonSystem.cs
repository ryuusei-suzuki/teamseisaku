using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonSystem : MonoBehaviour
{
    public void GotoTitle()
    {
        SkillSelectionManager.Instance.ClearSkills();
        SceneManager.LoadScene("TitleScene");
    }
    public void GotoSkillSelection()
    {
        SkillSelectionManager.Instance.ClearSkills();
        SceneManager.LoadScene("Skill selection");
    }
}
