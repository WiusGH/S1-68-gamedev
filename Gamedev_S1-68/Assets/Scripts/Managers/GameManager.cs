using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public enum Neighborhoods
{
    Zone_1,
    Zone_2,
    Zone_3,
}

public class GameManager : MonoBehaviour
{
    [SeparatorUp]
    [HeaderBox("Initial Player Data", 0.4f, 0.7f, 1f, "Datos con los que se configura el inicio del juego")]
    [SeparatorDown]

    [SubHeader("Initial Player Data")]
    [SerializeField] internal int InitialMoney = 1000;
    [SerializeField] internal int InitialDebt = 0;
    [SerializeField] internal int InitialMoral = 50;
    [SerializeField] internal int InitialMonthlySalary = 50;
    [SerializeField] internal Neighborhoods Startneighborhood = Neighborhoods.Zone_1;

    [SubHeader("Initial Game Data")]
    [SerializeField] internal int InitialWeek = 1;
    [SerializeField] internal int InitialMonth = 1;

    [SeparatorUp]
    [HeaderBox("Current Dates", 0.6f, 0.2f, 1f, "Datos actuales inGame")]
    [SeparatorDown]

    [SubHeader("Time Control")]
    [SerializeField] internal int daysPerWeek = 7;
    [SerializeField] internal int EventsPerWeek = 3;

    [SubHeader("debugs")]
    [SerializeField] internal bool debugMode = false;

    [SeparatorUp]
    [HeaderBox("Events Week", 1f, 1f, 0f, "Datos de los eventos semanales")]
    [SeparatorDown]

    [SubHeader("Eventos de la semana")]
    [SerializeField, ReadOnly] internal EventData[] baseEvents;
    internal Dictionary<int, EventData> weekEvents = new Dictionary<int, EventData>();

    [SubHeader("Events history")]
    [SerializeField, ReadOnly] internal List<EventData> usedEvents = new List<EventData>();

    [SubHeader("Vista Debug - Eventos Semana")]
    [SerializeField, ReadOnly] internal List<string> debugEventsWeek = new List<string>();

    [SerializeField, ReadOnly] internal bool canProceedToNextDay = true;
    [SerializeField, ReadOnly] internal bool isGameOver = false;

    [SubHeader("Progress in Game")]
    [SerializeField, ReadOnly] internal int currentMoney;
    [SerializeField, ReadOnly] internal int currentDebt;
    [SerializeField, ReadOnly] internal int currentMoral;
    [SerializeField, ReadOnly] internal int currentMonthlySalary;

    [SerializeField, ReadOnly] internal int currentWeek;
    [SerializeField, ReadOnly] internal int currentMonth;
    [SerializeField, ReadOnly] internal int currentDay = 0;

    [SerializeField, ReadOnly] internal int currentLesson = 0;

    [SerializeField, ReadOnly] internal Neighborhoods Currentneighborhood;

    private void Start()
    {
        InitPlayer();

        baseEvents = Resources.LoadAll<EventData>("Events");
        if (baseEvents.Length == 0)
        {
            Debug.LogWarning("No hay eventos en Resources/Events.");
            return;
        }

        GenerateEventsWeek();

        GameMainScene.canvasManager.InitUI();
        GameMainScene.canvasManager.UpdateTextsForTime(currentWeek, currentDay, daysPerWeek, currentLesson);
    }

    private void InitPlayer()
    {
        currentMoney = InitialMoney;
        currentDebt = InitialDebt;
        currentMoral = InitialMoral;
        currentMonthlySalary = InitialMonthlySalary;
        currentWeek = InitialWeek;
        currentMonth = InitialMonth;
        Currentneighborhood = Startneighborhood;

    }

    private void GenerateEventsWeek()
    {
        weekEvents.Clear();

        List<EventData> availableEvents = baseEvents
            .Where(e => !usedEvents.Contains(e))
            .ToList();

        if (availableEvents.Count < 3)
        {
            usedEvents.Clear();
            availableEvents = new List<EventData>(baseEvents);
        }

        List<int> diasDisponibles = new List<int>() { 2, 3, 4, 5, 6, 7 };
        List<EventData> eventosDisponibles = new List<EventData>(availableEvents);

        for (int i = 0; i < EventsPerWeek && eventosDisponibles.Count > 0 && diasDisponibles.Count > 0; i++)
        {
            EventData evento = eventosDisponibles[Random.Range(0, eventosDisponibles.Count)];
            int dia = diasDisponibles[Random.Range(0, diasDisponibles.Count)];

            weekEvents[dia] = evento;
            usedEvents.Add(evento);
            eventosDisponibles.Remove(evento);
            diasDisponibles.Remove(dia);
        }

        UpdateEventsDebug();
    }

    public void NextDay()
    {
        if (!canProceedToNextDay || isGameOver) return;

        currentDay++;

        if (currentDay > daysPerWeek)
        {
            NextWeek();
        }

        if (weekEvents.ContainsKey(currentDay))
        {
            var evento = weekEvents[currentDay];

            canProceedToNextDay = false;
            GameMainScene.canvasManager.SetDecisionButtons(evento);

            currentLesson++;

            GameMainScene.canvasManager.UpdateTextsForTime(currentWeek, currentDay, daysPerWeek, currentLesson);

        }
        else
        {
            GameMainScene.canvasManager.ShowNoEventMessage();
            GameMainScene.canvasManager.UpdateTextsForTime(currentWeek, currentDay, daysPerWeek, currentLesson);
        }
    }

    private void NextWeek()
    {
        currentWeek++;
        currentLesson = 0;
        currentDay = 1;

        if (currentWeek > 4)
        {
            FinishMonth();
        }

        GenerateEventsWeek();
        GameMainScene.canvasManager.UpdateTextsForTime(currentWeek, currentDay, daysPerWeek, currentLesson);

    }

    private void FinishMonth()
    {
        currentMoney += currentMonthlySalary;
        currentWeek = 1;
        currentMonth++;
        Debug.Log($" Nuevo mes: {currentMonth}");
    }

    public void ApplyDecision(DecisionOption decision)
    {
        currentMoney += decision.cambioDinero;
        currentMoral += decision.cambioMoral;
        currentDebt += decision.cambioDeuda;

        switch (decision.typeDecision)
        {
            case DecisionType.Aceptar:
                break;
            case DecisionType.Rechazar:
                break;
            case DecisionType.Negociar:
                break;
        }

        if (currentMoral < 0)
        {
            EndGame("La moral del jugador llegó a cero.");
            return;
        }

        if (currentMoney <= 0)
        {
            EndGame("El jugador se quedó sin dinero.");
            return;
        }

        if (currentDebt > currentMonthlySalary)
        {
            EndGame("La deuda del jugador superó su salario.");
            return;
        }

        canProceedToNextDay = true;

        GameMainScene.canvasManager.UpdateStats();
        GameMainScene.canvasManager.HidePanel(GameMainScene.canvasManager.decisionPanel);
    }

    public void UpdateEventsDebug()
    {
        debugEventsWeek.Clear();
        foreach (var kvp in weekEvents)
        {
            debugEventsWeek.Add($"Día {kvp.Key}: {kvp.Value.name}");
        }
    }

    private void EndGame(string reason)
    {
        isGameOver = true;
        canProceedToNextDay = false;
        GameMainScene.canvasManager.ShowGameOver(reason);
    }

    public void RestartGame()
    {
        isGameOver = false;
        canProceedToNextDay = true;

        InitPlayer();
        currentDay = 1;
        GenerateEventsWeek();

        GameMainScene.canvasManager.HideAllPanels(); 
        GameMainScene.canvasManager.UpdateStats();
        GameMainScene.canvasManager.ShowNoEventMessage();
    }
}
