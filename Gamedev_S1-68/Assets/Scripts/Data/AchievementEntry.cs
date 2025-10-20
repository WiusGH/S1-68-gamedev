using System;

[Serializable]
public class AchievementEntry
{
  public string id;
  public bool unlocked;
  public string unlockDate;

  public AchievementEntry(string id)
  {
    this.id = id;
    this.unlocked = false;
    this.unlockDate = "";
  }
}
