using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NuevoEvento", menuName = "Game/Evento")]
public class EventData : ScriptableObject
{
    [Header("Descripción del evento")]
    [TextArea] public string descripcion;

    [Header("Descisiones disponibles")]
    public DecisionData[] decisiones;
}