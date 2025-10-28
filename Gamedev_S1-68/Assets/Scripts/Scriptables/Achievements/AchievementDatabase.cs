using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AchievementDatabase", menuName = "Achievements/Achievement Database")]
public class AchievementDatabase : ScriptableObject
{
  public List<AchievementDefinition> achievements;
}
