using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/EnemyElement")]
public class Element : ScriptableObject
{

    public enum EnemyElement
    {
        fire,
        water,
        wind
    }


}
