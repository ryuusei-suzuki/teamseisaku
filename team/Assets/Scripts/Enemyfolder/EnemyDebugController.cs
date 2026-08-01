using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyDebugController : MonoBehaviour
{
    [SerializeField] private Enemytester enemy;
    [SerializeField] private int testDamage = 10;

    void Start()
    {
        if (enemy == null)
        {
            enemy = FindFirstObjectByType<Enemytester>();
        }
    }

    void Update()
    {
        if (enemy == null)
            return;

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            enemy.EnemyAttack();
        }

        if (Keyboard.current.aKey.wasPressedThisFrame)
        {
            enemy.TakeDamage(testDamage);
        }

        if (Keyboard.current.hKey.wasPressedThisFrame)
        {
            Debug.Log($"HPäÑçá: {enemy.GetHPRate():P0}");
        }
    }
}