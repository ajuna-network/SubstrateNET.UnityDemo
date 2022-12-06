using Ajuna.NetApi;
using Ajuna.NetWallet;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = System.Random;

public class CreateWalletScreen : BasicScreen
{
    public TextMeshProUGUI txtMnemonic;

    public override void OnLoadScreen()
    {
        txtMnemonic.text = $"Please memorize! <#34adfc><size=45>{WalletManager.GetInstance().CreateMnemonicSeed()}";
    }

    public override void OnUnloadScreen()
    {
        txtMnemonic.text = $"Please memorize! <#34adfc><size=45>one two three";
    }
}
