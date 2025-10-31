using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DecisionData
{
    [SerializeField] public DecisionOption DecisionOption;
}

[System.Serializable]
public class DecisionOption
{
    public DecisionType typeDecision;

    public string titulo;

    public int cambioDinero;
    public int cambioMoral;
    public int cambioDeuda;
    [TextArea(2, 5)]
    public string feedback;
}

public enum DecisionType
{
    Aceptar,
    Rechazar,
    Negociar,
    Ignorar,
    Otro
}
