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
using TMPro;
using UnityEngine;

public class WalletController : Singleton<WalletController>
{
    public WalletManager manager => WalletManager.GetInstance();

    public bool IsPooling = false;

    public bool IsConnected => manager.Client.IsConnected;

    public TextMeshProUGUI txtBlockNumber, txtPeers, txtName;

    public TextMeshProUGUI txtWalletName, txtWalletAddress, txtWalletBalance;

    public UnityEngine.UI.Slider slider;

    public UnityEngine.UI.Image imgConnection;

    private Hash _currentBlockHash, _finalBlockHash;

    private BlockData _currentBlockData, _finalBlockData;

    private bool _isPolling = false;

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

        WalletManager.GetInstance().ExtrinsicStateUpdateEvent += UpdateExtrinsics;
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

        var accountId32 = new AccountId32();
        accountId32.Create(Utils.GetPublicKeyFrom(WalletManager.GetInstance().PublicKey));

        var multiAddress = new EnumMultiAddress();
        multiAddress.Create(MultiAddress.Id, accountId32);

        var baseCampactU128 = new BaseCom<U128>();
        baseCampactU128.Create(100000000000000);

        var transferKeepAlive = BalancesCalls.TransferKeepAlive(multiAddress, baseCampactU128);

        var subscriptionId = await manager.Client.Author.SubmitAndWatchExtrinsicAsync(
               WalletManager.GetInstance().ActionExtrinsicUpdate,
               BalancesCalls.TransferKeepAlive(multiAddress, baseCampactU128),
               WalletManager.GetInstance().Alice, new ChargeAssetTxPayment(0, 0), 64, CancellationToken.None);
    }

    public void OnButtonLessClicked()
    {
        Debug.Log("No need your big bag, take it back!");
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
            InvokeRepeating("PollAllHeadsAsync", 0f, 1f);
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
            // systemHealth {"IsSyncing":false,"Peers":0,"ShouldHavePeers":false}
            //Debug.Log($"systemHealth {systemHealth}");
            txtPeers.text = systemHealth.Peers.ToString();
        }
    }

    public async void PollAllHeadsAsync()
    {

        txtWalletName.text = WalletManager.GetInstance().WalletName;
        txtWalletAddress.text = WalletManager.GetInstance().PublicKey;

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
                accountId32.Create(Utils.GetPublicKeyFrom(WalletManager.GetInstance().PublicKey));
                var accountInfo = await manager.Client.SystemStorage.Account(accountId32, CancellationToken.None);
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

}
