using Ajuna.NetWallet;
using System.IO;
using UnityEngine;

public class CachingManager : Singleton<CachingManager>
{
    private string _persistentPath;

    void Awake()
    {
        base.Awake();
        _persistentPath = Application.persistentDataPath;
        SystemInteraction.ReadData = f => UnityMainThreadDispatcher.Dispatch(() => Resources.Load<TextAsset>(Path.GetFileNameWithoutExtension(f)).text);
        SystemInteraction.DataExists = f => UnityMainThreadDispatcher.Dispatch(() => Resources.Load(Path.GetFileNameWithoutExtension(f))) != null;
        SystemInteraction.PersistentExists = f => File.Exists(Path.Combine(_persistentPath, f));
        SystemInteraction.ReadPersistent = f => File.ReadAllText(Path.Combine(_persistentPath, f));
        SystemInteraction.Persist = (f, c) => File.WriteAllText(Path.Combine(_persistentPath, f), c);
        Debug.Log(_persistentPath);
    }
}
