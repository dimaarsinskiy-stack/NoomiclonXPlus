using UnityEngine;
using System.Reflection;
using System.IO;

public class ModLoader : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        var go = new GameObject("ModLoader");
        go.AddComponent<ModLoader>();
        DontDestroyOnLoad(go);
    }

    void Awake()
    {
        string modPath = Path.Combine(Application.persistentDataPath, "mods");
        if (!Directory.Exists(modPath)) Directory.CreateDirectory(modPath);
        foreach (var dll in Directory.GetFiles(modPath, "*.dll"))
        {
            try
            {
                var asm = Assembly.LoadFrom(dll);
                foreach (var type in asm.GetTypes())
                {
                    var m = type.GetMethod("Init", BindingFlags.Public | BindingFlags.Static);
                    m?.Invoke(null, null);
                }
                Debug.Log($"[ModLoader] OK: {dll}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[ModLoader] FAIL {dll}: {e}");
            }
        }
    }
}