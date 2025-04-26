using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;

public class ButtonManager : MonoBehaviour
{
    public GameObject lanPanel;
    public Button lanOpen;
    public Button lanClose;
    public Button exitGame;

    private void Start()
    {
        lanOpen.onClick.AddListener(LanOpenHandle);
        lanClose.onClick.AddListener(LanCloseHandle);
        exitGame.onClick.AddListener(ExitGameHandle);
    }

    private async void LanOpenHandle()
    {
        lanPanel.SetActive(true);
        lanOpen.gameObject.SetActive(false);

        List<IPEndPoint> data = await NetworkManager.instance.FindServers();

        foreach (var server in data)
        {
            Debug.Log(server.Address);
        }        
    }

    private void LanCloseHandle()
    {
        lanPanel.SetActive(false);
        lanOpen.gameObject.SetActive(true);
        lanOpen.GetComponent<ButtonScale>().Reset();
    }

    private void ExitGameHandle()
    {
        Application.Quit();
    }
}
