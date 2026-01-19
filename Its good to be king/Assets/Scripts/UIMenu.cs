using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMenu : MonoBehaviour
{
    public Transform CameraPos;
    public Vector2 CurrentPos;

    private Vector2 TargetPos;
    private Transform myPivot;

    bool hasStartedTrackingMouse = false;
    bool trackMouse = false;
    bool close = false;
    bool realClose = false;
    float time;
    private Vector2 LastMousePos;
    private Vector2 pivotPos;
    private Vector2 mouseDirection;
    private float mouseSpeed;
    private float speedDecrease = 30;

    private void Start()
    {
        myPivot = gameObject.transform.Find("MenuPivot");
        if(myPivot != null)
        {
            pivotPos = myPivot.GetComponent<RectTransform>().localPosition;
        }
        CurrentPos = transform.position;
        TargetPos = CurrentPos;
        Close(false);
    }

    private void Update()
    {
        time += Time.deltaTime;
        gameObject.GetComponent<RectTransform>().position = Vector2.LerpUnclamped(transform.position, TargetPos, time / 2.0f);
        if(close && Vector2.Distance(transform.position, CurrentPos) < 1.0f)
        {
            gameObject.SetActive(false);
            realClose = true;
        }
        if(Input.GetMouseButtonDown(0))
        {
            trackMouse = true;
            hasStartedTrackingMouse = true;
        }
        if(Input.GetMouseButtonUp(0))
        {
            trackMouse = false;
        }

        if(trackMouse)
        {
            if(hasStartedTrackingMouse)
            {
                LastMousePos = Input.mousePosition;
                hasStartedTrackingMouse = false;
            }
            Vector2 currentMousePos = Input.mousePosition;
            float mouseDistance = Vector2.Distance(currentMousePos, LastMousePos);
            mouseSpeed = mouseDistance;
            mouseDirection = (currentMousePos - LastMousePos).normalized;

            LastMousePos = currentMousePos;
        }
        else 
        {
            if(mouseSpeed > 0)
            {
                mouseSpeed -= speedDecrease * Time.deltaTime;
            }
            else
            {
                mouseSpeed = 0;
            }
        }
        if(myPivot != null)
        {
            myPivot.localPosition = new Vector2(myPivot.transform.localPosition.x, myPivot.transform.localPosition.y + (mouseDirection.y * mouseSpeed));
        }
    }
    public virtual void Open(bool withAnimation)
    {
        if (myPivot != null)
        {
            myPivot.position = CurrentPos;
        }
        TargetPos = CameraPos.position;
        close = false;
        realClose = false;
        if(withAnimation)
        {
            GetComponent<RectTransform>().SetAsLastSibling();
            time = 0;
        }
        else 
        {
            gameObject.GetComponent<RectTransform>().position = TargetPos;
        }
    }
    public virtual void Close(bool withAnimation)
    {
        if (myPivot != null)
        {
            myPivot.position = CameraPos.position;
        }
        TargetPos = CurrentPos;
        close = true;
        if(withAnimation)
        {
            GetComponent<RectTransform>().SetAsLastSibling();
            time = 0;
        }
        else
        {
            gameObject.GetComponent<RectTransform>().position = TargetPos;
        }
    }

    public bool GetIsClosed()
    {
        return realClose;
    }
}
