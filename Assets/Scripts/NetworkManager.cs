using Ajuna.NetApi.Model.Rpc;
using System;
using Ajuna.NetApi.Model.Extrinsics;
using SubstrateNET.NetApi.Generated;

namespace Assets.Scripts
{
    public delegate void ExtrinsicStateUpdate(string subscriptionId, ExtrinsicStatus extrinsicUpdate);

    public class NetworkManager
    {
        private string _nodeUrl = "ws://127.0.0.1:9944";

        private SubstrateClientExt _client;
        public SubstrateClientExt Client => _client;

        // Start is called before the first frame update
        public void InitializeClient()
        {
            if (_client != null)
            {
                return;
            } 
            
            _client = new SubstrateClientExt(new Uri(_nodeUrl));

        }
    }
}