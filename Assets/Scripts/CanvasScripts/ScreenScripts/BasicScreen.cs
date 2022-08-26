using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasicScreen : MonoBehaviour
{
    void OnEnable()
    {
        if (CanvasManager.GetInstance() == null || !CanvasManager.GetInstance().IsActivated)
        {
            return;
        }

        OnLoadScreen();
    }

    public virtual void OnLoadScreen()
    {
        return;
    }

    void OnDisable()
    {
        if (CanvasManager.GetInstance() == null || !CanvasManager.GetInstance().IsActivated)
        {
            return;
        }

        OnUnloadScreen();
    }

    public virtual void OnUnloadScreen()
    {
        return;
    }
}
