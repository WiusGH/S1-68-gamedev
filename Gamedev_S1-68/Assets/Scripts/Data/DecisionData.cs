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

    public string titulo;

    public int cambioDinero;
    public int cambioDeuda;
    public int cambioMoral;

}

public enum DecisionType
{
    Aceptar,
    Rechazar,
    Negociar,
    Ignorar,
    Otro
}
