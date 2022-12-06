using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BtnCreateWallet : CanvasSwitcher
{
    public TMP_InputField walletName;

    public TMP_InputField walletPassword;

    public WalletManager Manager => WalletManager.GetInstance();

    public override bool PreButtonClickExecution()
    {
        var flag = Manager.CreateWallet(walletName.text, walletPassword.text);

        Debug.Log($"Created wallet = {flag}");

        return flag;
    }
}
