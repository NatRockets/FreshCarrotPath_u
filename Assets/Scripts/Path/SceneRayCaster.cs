using System;
using UnityEngine;

public class SceneRayCaster : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    
    private Func<bool> shiftCheck;
    private Func<bool> lockCheck;

    public void SetCallbacks(Func<bool> sCheck, Func<bool> lCheck)
    {
        shiftCheck = sCheck;
        lockCheck = lCheck;
    }
    
    public void Cast(Vector3 inputPos, bool isSwiped = false, SwipeDirection direction = SwipeDirection.Up)
    {
        Ray ray = mainCamera.ScreenPointToRay(inputPos);
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.TryGetComponent<Slot>(out var slot))
        {
            if (isSwiped)
            {
                if (!shiftCheck())
                {
                    return;
                }
                slot.GetParentLine().ShiftLine(direction);
            }
            else
            {
                if (!lockCheck())
                {
                    return;
                }
                slot.Unlock();
                slot.GetParentLine().ShiftLine(direction);
            }
        }
    }
}
