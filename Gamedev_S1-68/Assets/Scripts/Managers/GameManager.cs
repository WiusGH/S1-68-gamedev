using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SeparatorUp]
    [HeaderBox("Initial Data Player", 0.4f, 0.7f, 1f, "Datos con los que se configura el inicio del juego")]
    [SeparatorDown]
    [SubHeader("Initial Data Player")]
    [SerializeField] internal int InitialMoney = 1000;
    [SerializeField] internal int InitialDebt = 0;
    [SerializeField] internal int InitialMoral = 50;
    [SerializeField] internal int InitialMonthlySalary = 50;

    [SubHeader("Initial Game Data")]
    [SerializeField] internal int InitialWeek = 1;
    [SerializeField] internal int InitialMonth = 1;

    [SeparatorUp]
    [HeaderBox("Current Dates", 0.6f, 0.2f, 1f, "Datos actuales inGame")]
    [SeparatorDown]
    [SubHeader("Player Stats (Current Stats)")]
    [SerializeField, ReadOnly] internal int CurrentMoney;
    [SerializeField, ReadOnly] internal int CurrentDebt;
    [SerializeField, ReadOnly] internal int CurrentMoral;
    [SerializeField, ReadOnly] internal int CurrentMonthlySalary;
    [SerializeField, ReadOnly] internal int CurrentWeek;
    [SerializeField, ReadOnly] internal int CurrentMonth;

    [SubHeader("debugs")]
    [SerializeField] internal bool debugMode = false;

    private void Start()
    {
        //solo se llama si no hay datos creados (futuramente)
        InitPlayer();
    }

    private void InitPlayer()
    {
        CurrentMoney = InitialMoney;
        CurrentDebt = InitialDebt;
        CurrentMoral = InitialMoral;
        CurrentMonthlySalary = InitialMonthlySalary;

        CurrentWeek = InitialWeek;
        CurrentMonth = InitialMonth;

        if(debugMode) Debug.Log("Jugador inicializado correctamente.");
    }

}
