using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public enum CanvasType
{
    StartUI,
    UnlockWalletUI,
    CreateWalletUI,
    ConnectUI,
    WalletUI,
    EndUI
}

public class CanvasManager : Singleton<CanvasManager>
{
    List<CanvasController> canvasControllerList;
    CanvasController lastActiveCanvas;

    public bool IsActivated = false;

    protected override void Awake()
    {
        base.Awake();
        canvasControllerList = GetComponentsInChildren<CanvasController>().ToList();
        canvasControllerList.ForEach(x => x.gameObject.SetActive(false));
        SwitchCanvas(CanvasType.StartUI);
        IsActivated = true;
    }

    public void SwitchCanvas(CanvasType _type)
    {
        if (lastActiveCanvas != null)
        {
            lastActiveCanvas.gameObject.SetActive(false);
        }

        CanvasController desiredCanvas = canvasControllerList.Find(x => x.canvasType == _type);
        if (desiredCanvas != null)
        {
            desiredCanvas.gameObject.SetActive(true);
            lastActiveCanvas = desiredCanvas;
        }
        else { Debug.LogWarning("The desired canvas was not found!"); }
    }
}
