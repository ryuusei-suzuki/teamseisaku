using UnityEngine;
using UnityEngine.InputSystem;

public class BattleDebugController : MonoBehaviour
{
    [SerializeField] private BattleManager battleManager;

    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            battleManager.ExecuteTurnFromDebug();
        }
    }
}