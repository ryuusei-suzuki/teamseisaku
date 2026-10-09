using UnityEngine;


[CreateAssetMenu(fileName = "SkillData", menuName = "Scriptable Objects/SkillData")]
public class SkillData : ScriptableObject
{
    public string SkillName;
    public AttributeType attribute;
    public DistanceType distance;
    public float Power;
    public int AP;
    public Sprite iconImage;
    public SkillType skillType;

    public AudioClip skillSE;

    [TextArea(3, 5)] public string Description;
}
