using System;
using System.Linq;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
  public static AchievementManager Instance { get; private set; }
  private AchievementData achievementData;

  [Header("References")]
  [SerializeField] private AchievementDatabase database; // Para visualizar en el inspector


  private void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
      DontDestroyOnLoad(gameObject);
    }
    else
    {
      Destroy(gameObject);
      return;
    }

    if (database == null)
    {
      Debug.LogError("[AchievementManager] AchievementDatabase no asignado.");
      return;
    }

    LoadAchievements();
  }
  // Para cargar los logros existente
  private void LoadAchievements()
  {
    achievementData = AchievementSaveSystem.Load();

    foreach (var def in database.achievements)
    {
      if (!achievementData.achievements.Any(a => a.id == def.id))
      {
        achievementData.achievements.Add(new AchievementEntry(def.id));
      }
    }

    AchievementSaveSystem.Save(achievementData);

    Debug.Log($"[AchievementManager] {achievementData.achievements.Count} logros cargados.");
  }

  public void SaveAchievements()
  {
    AchievementSaveSystem.Save(achievementData);
  }

  // Para eliminar todo los logros (quizás haga falta)
  public void DeleteAchievements()
  {
    AchievementSaveSystem.DeleteSave();
    achievementData = new AchievementData();
    Debug.Log("[AchievementManager] Logros reiniciados.");
  }

  public void UnlockAchievement(string id)
  {
    var achievement = achievementData.achievements.Find(a => a.id == id);
    if (achievement == null)
    {
      Debug.LogWarning($"[AchievementManager] No se encontró el logro con ID '{id}'.");
      return;
    }

    if (achievement.unlocked)
    {
      Debug.Log($"[AchievementManager] El logro '{id}' ya estaba desbloqueado.");
      return;
    }

    achievement.unlocked = true;
    achievement.unlockDate = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
    AchievementSaveSystem.Save(achievementData);

    var def = database.achievements.Find(d => d.id == id);
    Debug.Log(def != null
            ? $"🏆 Logro desbloqueado: {def.title}"
            : $"🏆 Logro desbloqueado: {id} (definition not found)");
  }

  // Para verificar si el logro ya fue desbloqueado
  public bool HasAchievement(string id)
  {
    return achievementData.achievements.Any(a => a.id == id && a.unlocked);
  }

  public AchievementDefinition GetDefinition(string id)
  {
    return database.achievements.FirstOrDefault(a => a.id == id);
  }

  // Para guardar al salir de la aplicación
  private void OnApplicationQuit()
  {
    SaveAchievements();
  }
}
