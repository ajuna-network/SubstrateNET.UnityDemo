using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BtnUnlockWallet : CanvasSwitcher
{
    public TMP_InputField walletName;

    public TMP_InputField walletPassword;

    public WalletManager Manager => WalletManager.GetInstance();

    public override bool PreButtonClickExecution()
    {
        var flag = Manager.LoadWallet(walletName.text, walletPassword.text);

        Debug.Log($"Load wallet = {flag}");

        return flag;
    }
}
