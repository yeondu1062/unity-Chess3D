using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using TMPro;

public class UiManager : MonoBehaviour
{
    public Button lan;
    public Button setting;
    public Button dimension;
    public Button exitGame;

    public GameObject lanDropDown;
    public TMP_Dropdown lanDropdown;

    public GameObject speedSlider;
    public Slider dragSpeedSlider;
    public Slider zoomSpeedSlider;

    public static bool IsDraggingUI = false;

    private void Start()
    {
        lan.onClick.AddListener(LanHandle);
        setting.onClick.AddListener(SettingHandle);
        dimension.onClick.AddListener(DimensionHandle);
        exitGame.onClick.AddListener(ExitGameHandle);

        dragSpeedSlider.onValueChanged.AddListener(DragSpeedChangedHandle);
        zoomSpeedSlider.onValueChanged.AddListener(ZoomSpeedChangedHandle);

        AddCameraDragBlock(dragSpeedSlider);
        AddCameraDragBlock(zoomSpeedSlider);
    }

    private async void LanHandle()
    {
        bool isActive = lanDropDown.activeSelf;
        lanDropDown.SetActive(!isActive);
        speedSlider.SetActive(false);

        if (isActive) return;

        List<IPEndPoint> servers = await NetworkManager.instance.FindServers();
        List<string> options = new List<string>();

        foreach (var server in servers) options.Add(server.Address.ToString());

        lanDropdown.ClearOptions();
        lanDropdown.AddOptions(options);
    }

    private void SettingHandle()
    {
        speedSlider.SetActive(!speedSlider.activeSelf);
        lanDropDown.SetActive(false);
    }

    private void DimensionHandle()
    {
        var cameraDrag = FindFirstObjectByType<CameraDrag>();
        cameraDrag.y = 90;
        cameraDrag.x = (ChessManager.instance.trun % 2 == 0) ? 0 : 180;
        cameraDrag.distance = 8;
    }

    private void ExitGameHandle()
    {
        Application.Quit();
    }

    private void DragSpeedChangedHandle(float value)
    {
        CameraDrag.xSpeed = value;
        CameraDrag.ySpeed = value;
    }

    private void ZoomSpeedChangedHandle(float value)
    {
        CameraDrag.zoomSpeed = value;
    }

    private void AddCameraDragBlock(Slider slider)
    {
        EventTrigger trigger = slider.gameObject.AddComponent<EventTrigger>();

        var entryDown = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
        entryDown.callback.AddListener(_ => IsDraggingUI = true);
        trigger.triggers.Add(entryDown);

        var entryUp = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        entryUp.callback.AddListener(_ => IsDraggingUI = false);
        trigger.triggers.Add(entryUp);
    }
}
