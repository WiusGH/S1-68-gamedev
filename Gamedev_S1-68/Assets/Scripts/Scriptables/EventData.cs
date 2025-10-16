using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NuevoEvento", menuName = "Game/Evento")]
public class EventData : ScriptableObject
{
    [TextArea] public string descripcion;
    public DecisionData[] decisiones;
}