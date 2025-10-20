using System;
using UnityEngine;

[Serializable]
public class AchievementEntry
{
  public string id;
  public string title;
  public string description;
  public string iconPath; // Para enlazar el logro con un ícono
  public int value; // Para ser utilizado para algo como un sistema de puntaje
  public bool unlocked;
  public string unlockDate;

  public AchievementEntry(string id, string title, string description, string iconPath = "", int value = 0)
  {
    this.id = id;
    this.title = title;
    this.description = description;
    this.iconPath = iconPath;
    this.value = value;
    this.unlocked = false;
    this.unlockDate = "";
  }
}
