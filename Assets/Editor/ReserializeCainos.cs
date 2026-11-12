using UnityEditor;

public static class ReserializeCainos
{
    [MenuItem("Tools/Reserialize Cainos Assets")]
    static void Run()
    {
        var guids = AssetDatabase.FindAssets("t:Prefab t:Scene", new[] { "Assets/Cainos" });
        var paths = new string[guids.Length];
        for (int i = 0; i < guids.Length; i++)
            paths[i] = AssetDatabase.GUIDToAssetPath(guids[i]);
        AssetDatabase.ForceReserializeAssets(paths);
<<<<<<< HEAD
        UnityEngine.Debug.Log($"Reserialized {paths.Length} Cainos assets.");
    }
}
=======
    }
}
>>>>>>> ba555cc0fa44d6f8d3f9be62cf562b8577e1fd29
