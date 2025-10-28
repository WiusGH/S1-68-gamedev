using UnityEngine;

[CreateAssetMenu(fileName = "NewAchievement", menuName = "Achievements/Achievement Definition")]
public class AchievementDefinition : ScriptableObject
{
  [Header("Identificación")]
  public string id;

  [Header("Título y Descripción")]
  public string title;
  [TextArea]
  public string description;
  public Sprite icon; // Para enlazar el logro con un ícono
  public int value; // Para ser utilizado para algo como un sistema de puntaje

  [Header("Hidden Settings")]
  public bool hidden; // Opcional para logros ocultos
}
