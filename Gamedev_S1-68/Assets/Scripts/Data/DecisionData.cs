using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DecisionData
{
    [SerializeField] internal DecisionOption DecisionOption;
}

[System.Serializable]
public class DecisionOption
{
    public DecisionType typeDecision;

    public int cambioDinero;
    public int cambioMoral;
    public int cambioDeuda;
}

public enum DecisionType
{
    Aceptar,
    Rechazar,
    Negociar,
    Ignorar,
    Otro 
}
