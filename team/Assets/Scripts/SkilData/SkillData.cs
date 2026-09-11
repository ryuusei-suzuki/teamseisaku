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
    public SkillType skillType; // 近距離弱攻撃・ガード判定に使用(CloseWeak/Guard等)

}
