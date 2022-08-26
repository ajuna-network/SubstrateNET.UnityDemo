using Ajuna.NetApi;
using Ajuna.NetWallet;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : BasicScreen
{
    public enum LoadingState
    {
        None = 0,
        Connect = 1,
        Poll = 2,
        Finish = 3,
    }

    public Slider slider;

    public TextMeshProUGUI txtLoadingInfo;

    public TextMeshProUGUI txtLoadingValue;

    public override void OnLoadScreen()
    {
        slider.value = 0;
        txtLoadingInfo.text = "Initial";
        txtLoadingValue.text = $"Loading... {slider.value}%";

        StartCoroutine(Loading());
    }

    private IEnumerator Loading()
    {
        var loadingState = LoadingState.None;

        while (loadingState != LoadingState.Finish)
        {         

            var loadingStateTask = GetLoadingStateAsync();

            var loadingTask = ExecuteLoading(loadingState);

            yield return new WaitForSeconds(1);

            while (!loadingStateTask.IsCompleted || !loadingTask.IsCompleted)
            {
                yield return new WaitForSeconds(1);

            }

            loadingState = loadingStateTask.Result;

            //UnityMainThreadDispatcher.DispatchAsync(() =>
            //{
                slider.value = (float)loadingState * 20;
                txtLoadingInfo.text = loadingState.ToString();
                txtLoadingValue.text = $"Loading... {slider.value}%";
            //});
        }

        CanvasManager.GetInstance().SwitchCanvas(CanvasType.WalletUI);
    }

    private string GetSeachingText(LoadingState loadingStateTask)
    {
        return loadingStateTask switch
        {
            LoadingState.Connect => "Initial",
            LoadingState.Poll => "Connected",
            LoadingState.Finish => "Finished",
            _ => "Unknow",
        };
    }

    public async Task<LoadingState> GetLoadingStateAsync()
    {
        if (!WalletManager.GetInstance().Client.IsConnected)
        {
            return LoadingState.Connect;
        }

        Debug.Log($"IsConnected = {WalletManager.GetInstance().Client.IsConnected}");

        if (!WalletController.GetInstance().IsPooling)
        {
            return LoadingState.Poll;
        }

        Debug.Log($"IsPooling = {WalletController.GetInstance().IsPooling}");

        return LoadingState.Finish;
    }

    public async Task<bool> ExecuteLoading(LoadingState loadingState)
    {
        switch (loadingState)
        {
            case LoadingState.Connect:
                await WalletController.GetInstance().Connect(true);
                return true;

            case LoadingState.Poll:
                WalletController.GetInstance().Pooling(true);
                return true;

            default:
                return false;
        }
    }

    public override void OnUnloadScreen()
    {
       
    }
}
