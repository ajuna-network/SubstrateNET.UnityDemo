using Ajuna.NetApi;
using Ajuna.NetApi.Model.Extrinsics;
using Ajuna.NetApi.Model.Rpc;
using Ajuna.NetApi.Model.Types;
using Ajuna.NetApi.Model.Types.Base;
using Ajuna.NetApi.Model.Types.Primitive;
using SubstrateNET.NetApi.Generated.Model.sp_core.crypto;
using SubstrateNET.NetApi.Generated.Model.sp_runtime.multiaddress;
using SubstrateNET.NetApi.Generated.Storage;
using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using StreamJsonRpc;
using TMPro;
using UnityEngine;

public class WalletController : Singleton<WalletController>
{
    public WalletManager manager => WalletManager.GetInstance();

    public bool IsPooling = false;

    public bool IsConnected => manager.Client.IsConnected;

    public TextMeshProUGUI txtBlockNumber, txtPeers, txtName;

    public TextMeshProUGUI txtWalletName, txtWalletAddress, txtWalletBalance, txtWalletState;

    public UnityEngine.UI.Slider slider;

    public UnityEngine.UI.Image imgConnection;

    private Hash _currentBlockHash, _finalBlockHash;

    private BlockData _currentBlockData, _finalBlockData;

    private bool _isPolling = false;

    private static int _counter;

    void Awake()
    {
        base.Awake();
    }

    // Start is called before the first frame update
    void Start()
    {
        txtBlockNumber.text = "0<#557190>/0";
        _currentBlockHash = null;
        _currentBlockData = null;
        _finalBlockHash = null;
        _finalBlockData = null;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void UpdateExtrinsics(string subscriptionId, ExtrinsicStatus extrinsicUpdate)
    {
        Debug.Log($"{subscriptionId} updated to BLK={extrinsicUpdate.InBlock?.Value}|FIN={extrinsicUpdate.Finalized?.Value}[{extrinsicUpdate.ExtrinsicState}]");
    }

    public async void OnButtonMoreClicked()
    {
        if (!manager.Client.IsConnected)
        {
            Debug.Log("Not connected!");
        }

        TransferBalance(WalletManager.GetInstance().Alice, WalletManager.GetInstance().Account, 100000000000000);
    }

    public async void TransferBalance(Account senderAccount,Account recipientAccount,  long amount)
    {
        if (!manager.Client.IsConnected)
        {
            Debug.Log("Not connected!");
        }
        
        var transferRecipient = new AccountId32();
        transferRecipient.Create(recipientAccount.Bytes);
        
        var multiAddress = new EnumMultiAddress();
        multiAddress.Create(MultiAddress.Id, transferRecipient);
        
        var baseCampactU128 = new BaseCom<U128>();
        baseCampactU128.Create(amount);
        
        var transferKeepAlive = BalancesCalls.TransferKeepAlive(multiAddress, baseCampactU128);

        try
        {
            await manager.Client.Author.SubmitAndWatchExtrinsicAsync(
                OnExtrinsicStateUpdateEvent,
                transferKeepAlive,
                senderAccount, new ChargeAssetTxPayment(0, 0), 64, CancellationToken.None).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            var errorMessage = "An error occured";
            
            if (ex is RemoteInvocationException)
            {
                errorMessage = ((RemoteInvocationException) ex).Message;
            }
            
            UnityMainThreadDispatcher.DispatchAsync(() =>
            {
                txtWalletState.text = errorMessage;
                txtWalletState.color = Color.red;
                txtWalletState.fontSize = 40;
            });
        }

    }

    public async Task Connect(bool flag)
    {
        if (flag)
        {
            await manager.Client.ConnectAsync(false, true, CancellationToken.None);
        }
        else
        {
            await manager.Client.CloseAsync(CancellationToken.None);
        }
    }

    public void Pooling(bool flag)
    {
        if (flag)
        {
            IsPooling = flag;
            Invoke("NodeInfoAsync", 0f);
            InvokeRepeating("SystemHealthAsync", 0f, 60f);
            InvokeRepeating("PollAllHeadsAsync", 0f, 0.5f);
        }
        else
        {
            CancelInvoke("SystemHealthAsync");
            CancelInvoke("PollAllHeadsAsync");
        }
    }
    
    public async void NodeInfoAsync()
    {
        var systemProperties = await manager.Client.System.PropertiesAsync(CancellationToken.None);
        if (systemProperties != null)
        {
            // systemProperties {"Ss58Format":0,"TokenDecimals":0,"TokenSymbol":null}
            //Debug.Log($"systemProperties {systemProperties}");
        }
    
        var name = await manager.Client.System.NameAsync(CancellationToken.None);
        if (name != null)
        {
            txtName.text = name;
        }
    }
    
    public async void SystemHealthAsync()
    {
        var systemHealth = await manager.Client.System.HealthAsync(CancellationToken.None);
        if (systemHealth != null)
        {
            txtPeers.text = systemHealth.Peers.ToString();
        }
    }
    
    
    // Gets block info and current wallet account balance 
    public async void PollAllHeadsAsync()
    {

        txtWalletName.text = WalletManager.GetInstance().WalletName;
        txtWalletAddress.text = WalletManager.GetInstance().Account.Value;
        
        if (_isPolling)
        {
            return;
        }

        _isPolling = true;

        if (!IsConnected)
        {
            imgConnection.color = Color.black;
            return;
        }

        imgConnection.color = Color.gray;

        var currentBlockHash = await manager.Client.Chain.GetBlockHashAsync(CancellationToken.None);
        if (currentBlockHash != null && (_currentBlockHash == null || currentBlockHash.Value != _currentBlockHash.Value))
        {
            _currentBlockHash = currentBlockHash;
            _currentBlockData = await manager.Client.Chain.GetBlockAsync(_currentBlockHash, CancellationToken.None);

            var finalBlockHash = await manager.Client.Chain.GetFinalizedHeadAsync(CancellationToken.None);
            if (finalBlockHash != null && (_finalBlockHash == null || finalBlockHash.Value != _finalBlockHash.Value))
            {
                _finalBlockHash = finalBlockHash;
                _finalBlockData = await manager.Client.Chain.GetBlockAsync(_finalBlockHash, CancellationToken.None);
            }

            if (_currentBlockData != null && _finalBlockData != null)
            {
                slider.value = (float) _currentBlockData.Block.Header.Number.Value % 11 / 10;
                txtBlockNumber.text = $"{_finalBlockData.Block.Header.Number.Value}<#C0C0C0>/{_currentBlockData.Block.Header.Number.Value}";

                // since block changed to a balances call
                var accountId32 = new AccountId32();
                accountId32.Create(Utils.GetPublicKeyFrom(WalletManager.GetInstance().Account.Value));
                var accountInfo = await manager.Client.SystemStorage.Account(accountId32, CancellationToken.None);
                
                // AccountInfo.Data will be null before the first transfer
                if (accountInfo != null && accountInfo.Data != null) 
                {
                    txtWalletBalance.text = BigInteger.Divide(accountInfo.Data.Free.Value, BigInteger.Pow(10, 12)).ToString("0.000000");
                }
                else
                {
                    txtWalletBalance.text = "NULL";
                }
            }
        }

        _isPolling = false;
    }
    
    /// <summary>
    /// Callback sent together with the extrinsic
    /// </summary>
    /// <param name="subscriptionId"></param>
    /// <param name="extrinsicStatus"></param>
    private void OnExtrinsicStateUpdateEvent(string subscriptionId, ExtrinsicStatus extrinsicStatus)
    {
        var state = "Unknown";

        switch (extrinsicStatus.ExtrinsicState)
        {
            case ExtrinsicState.None:
                if (extrinsicStatus.InBlock?.Value.Length > 0)
                {
                    state = "InBlock";
                }
                else if (extrinsicStatus.Finalized?.Value.Length > 0)
                {
                    state = "Finalized";
                }
                else
                {
                    state = "None";
                }
                break;

            case ExtrinsicState.Future:
                state = "Future";
                break;

            case ExtrinsicState.Ready:
                state = "Ready";
                break;

            case ExtrinsicState.Dropped:
                state = "Dropped";
                break;

            case ExtrinsicState.Invalid:
                state = "Invalid";
                break;
        }

        UnityMainThreadDispatcher.DispatchAsync(() =>
        {
            txtWalletState.text = state;
            txtWalletState.color = Color.black;
            txtWalletState.fontSize = 40;
            Debug.Log("Font Size: " + txtWalletState.fontSize);
      
        });
    }

}
