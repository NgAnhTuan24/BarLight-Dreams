using System.IO;
using UnityEngine;

public static class SaveLoadSystem
{
    private static string GetPath(int slot)
    {
        return Path.Combine(Application.persistentDataPath, $"Save Slot {slot}.json");
    }

    public static void SaveGame(GameData data, int slot)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetPath(slot), json);
    }

    public static GameData LoadGame(int slot)
    {
        string path = GetPath(slot);

        if (!File.Exists(path)) return null;

        string json = File.ReadAllText(path);

        return JsonUtility.FromJson<GameData>(json);
    }

    public static void DeleteSave(int slot)
    {
        string path = GetPath(slot);

        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    public static bool HasSave(int slot)
    {
        return File.Exists(GetPath(slot));
    }
}
