using System;
using System.IO;
using UnityEngine;

public static class AchievementSaveSystem
{
  private static readonly string path = Application.persistentDataPath + "/achievements.json";

  public static void Save(AchievementData data)
  {
    data.lastSaveTime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
    string json = JsonUtility.ToJson(data, true);
    File.WriteAllText(path, json);
    Debug.Log($"[AchievementSaveSystem] Datos guardados en: {path}");
  }

  public static AchievementData Load()
  {
    if (File.Exists(path))
    {
      string json = File.ReadAllText(path);
      AchievementData data = JsonUtility.FromJson<AchievementData>(json);
      Debug.Log("[AchievementSaveSystem] Datos de logros cargados satisfactoriamente");
      return data;
    }
    else
    {
      Debug.Log("[AchievementSaveSystem] No se encontraron datos de logros - Creando nuevos datos.");
      return new AchievementData();
    }
  }

  public static void DeleteSave()
  {
    if (File.Exists(path))
    {
      File.Delete(path);
      Debug.Log("[AchievementSaveSystem] Datos de logros eliminados");
    }
  }
}
