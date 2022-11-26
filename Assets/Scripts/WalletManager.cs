using Ajuna.NetApi;
using Ajuna.NetApi.Model.Types;
using Ajuna.NetWallet;
using Schnorrkel.Keys;
using SubstrateNET.NetApi.Generated;
using SubstrateNET.RestClient;
using System;
using System.Net.Http;
using System.Net.WebSockets;
using UnityEngine;
using Random = System.Random;

public class WalletManager : Singleton<WalletManager>
{
    public MiniSecret MiniSecretAlice => new MiniSecret(Utils.HexToByteArray("0xe5be9a5092b81bca64be81d212e7f2f9eba183bb7a90954f7b76361f6edb5c0a"), ExpandMode.Ed25519);
    public Account Alice => Account.Build(KeyType.Sr25519, MiniSecretAlice.ExpandToSecret().ToBytes(), MiniSecretAlice.GetPair().Public.Key);

    [SerializeField]
    public string WalletName = "wallet";

    [SerializeField]
    public string NodeUrl = "ws://127.0.0.1:9944";
    public Account Account => _wallet.Account;

    private Wallet _wallet;

    private SubstrateClientExt _client;

    public SubstrateClientExt Client => _client;

    private Random _random;

    private HttpClient _httpClient;

    private Client _serviceClient;

    private string _mnemonicSeed;

    void Awake()
    {
        base.Awake();
    }

    // Start is called before the first frame update
    void Start()
    {
        _random = new Random();

        _client = new SubstrateClientExt(new Uri(NodeUrl));
    }

    internal string CreateMnemonicSeed()
    {
        var randomBytes = new byte[16];
        _random.NextBytes(randomBytes);
        _mnemonicSeed = string.Join(' ', Mnemonic.MnemonicFromEntropy(randomBytes, Mnemonic.BIP39Wordlist.English));
        return _mnemonicSeed;
    }

    internal bool LoadWallet(string name, string password)
    {
        // on creation we don't maintain instance, to have wallet locked.
        _wallet = new Wallet();
        
        if (!_wallet.Load(name))
        {
            return false;
        }

        if (!_wallet.Unlock(password))
        {
            return false;
        }

        WalletName = name;

        return true;
    }

    internal bool CreateWallet(string name, string password)
    {
        if (_mnemonicSeed == null) 
        { 
            return false;
        }

        // on creation we don't maintain instance, to have wallet locked.
        var wallet = new Wallet();
        if (!wallet.Create(password, _mnemonicSeed, KeyType.Sr25519,
            Mnemonic.BIP39Wordlist.English, name))
        {
            return false;
        }

        WalletName = name;

        return true;
    }
}
