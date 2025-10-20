using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
  public static AchievementManager Instance { get; private set; }

  private AchievementData achievementData;

  // public List<AchievementEntry> GetAllAchievements() => achievementData.achievements;
  // public List<AchievementEntry> GetUnlockedAchievements() => achievementData.achievements.FindAll(a => a.unlocked);
  // public List<AchievementEntry> GetLockedAchievements() => achievementData.achievements.FindAll(a => !a.unlocked);

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

    LoadAchievements();
  }

  // Para cargar los logros existente
  private void LoadAchievements()
  {
    achievementData = AchievementSaveSystem.Load();

    InitializeDefaultAchievements();

    AchievementSaveSystem.Save(achievementData);

    Debug.Log($"[AchievementManager] {achievementData.achievements.Count} logros cargados.");
  }

  // Para agregar logro por defecto
  private void InitializeDefaultAchievements()
  {
    AddAchievementIfNotExists(new AchievementEntry(
        "FirstLogIn",
        "First Log In",
        "Log in to the game for the first time.",
        "Icons/first_login",
        10
    ));
  }

  // Para agregar un logro nuevo siempre y cuando este ya no exista
  private void AddAchievementIfNotExists(AchievementEntry entry)
  {
    if (!achievementData.achievements.Any(a => a.id == entry.id))
      achievementData.achievements.Add(entry);
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

    Debug.Log($"🏆 Logro desbloqueado: {achievement.title}");
  }

  // Para verificar si el logro ya fue desbloqueado
  public bool HasAchievement(string achievementId)
  {
    return achievementData.achievements.Any(a => a.id == achievementId);
  }

  // Para guardar al salir de la aplicación
  private void OnApplicationQuit()
  {
    SaveAchievements();
  }

}
