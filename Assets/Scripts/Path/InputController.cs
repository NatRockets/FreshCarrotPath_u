using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SwipeDirection
{
    Left,
    Right,
    Up,
    Down
}

public class InputController : MonoBehaviour
{
    [SerializeField] private MonoEvent updateHandler;
    
    private Vector3 firstTouch;
    private Vector3 lastTouch;
    private float dragDistance; //minimum distance for a swipe to be registered

    private bool isActive;
    private bool swipeEnabled;

    private System.Action<SwipeDirection, Vector3> SwipeCallback;
    private System.Action<Vector3> TouchCallback;

    private void Start()
    {
        dragDistance = Screen.width * 0.2f;
    }

    private void OnDisable()
    {
        ActivateSwiping(false);
    }

    private void HandleSwipe()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                //Debug.Log("began");
                isActive = true;
                firstTouch = touch.position;
                lastTouch = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended && isActive)
            {
                //Debug.Log("ended");
                lastTouch = touch.position;

                float horizDist = Mathf.Abs(lastTouch.x - firstTouch.x);
                float vertDist = Mathf.Abs(lastTouch.y - firstTouch.y);

                SwipeDirection positiveValue;
                SwipeDirection negativeValue;
                float targetDist;
                float initParam, finishParam;
                if (horizDist > vertDist)
                {
                    targetDist = horizDist;
                    positiveValue = SwipeDirection.Right;
                    negativeValue = SwipeDirection.Left;
                    initParam = firstTouch.x;
                    finishParam = lastTouch.x;
                }
                else
                {
                    targetDist = vertDist;
                    positiveValue = SwipeDirection.Up;
                    negativeValue = SwipeDirection.Down;
                    initParam = firstTouch.y;
                    finishParam = lastTouch.y;
                }

                if (targetDist > dragDistance)
                {
                    if (finishParam > initParam)
                    {
                        SwipeCallback?.Invoke(positiveValue, firstTouch);
                    }
                    else
                    {
                        SwipeCallback?.Invoke(negativeValue, firstTouch);
                    }
                }
                else //not swipe but touch
                {
                    TouchCallback?.Invoke(firstTouch);
                }

                isActive = false;
            }
        }
    }

    public void ActivateSwiping(bool activate)
    {
        if (swipeEnabled == activate)
        {
            return;
        }

        if (activate)
        {
            updateHandler.UpdateEvent += HandleSwipe;
        }
        else
        {
            updateHandler.UpdateEvent -= HandleSwipe;
        }

        swipeEnabled = activate;
        //Debug.Log($"swipe {activate}");
    }

    public void SetupSwipe(System.Action<SwipeDirection, Vector3> callback)
    {
        SwipeCallback = callback;
    }

    public void SetupTouch(System.Action<Vector3> callback)
    {
        TouchCallback = callback;
    }

    public void ResetSwipe()
    {
        isActive = false;
        firstTouch = lastTouch = Vector3.zero;
    }
}
