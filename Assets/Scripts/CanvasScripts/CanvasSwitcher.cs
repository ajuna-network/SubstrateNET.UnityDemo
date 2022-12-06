using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CanvasSwitcher : MonoBehaviour
{
    public CanvasType desiredCanvasType;

    private CanvasManager _canvasManager;
    private Button _menuButton;

    private void Start()
    {
        _menuButton = GetComponent<Button>();
        _menuButton.onClick.AddListener(OnButtonClicked);
        _canvasManager = CanvasManager.GetInstance();
    }

    public virtual bool PreButtonClickExecution()
    {
        return true;
    }

    void OnButtonClicked()
    {
        if (PreButtonClickExecution())
        {
            _canvasManager.SwitchCanvas(desiredCanvasType);
        }
    }
}