using System;
using System.Collections.Generic;

[Serializable]
public class AchievementData
{
  public List<AchievementEntry> achievements = new List<AchievementEntry>();
  public string lastSaveTime;
}
