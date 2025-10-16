using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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

    [SubHeader("Time Control")]
    [SerializeField, ReadOnly] internal int CurrentDay = 1;
    [SerializeField] internal int DaysPerWeek = 7;
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



    private void Start()
    {
        //solo se llama si no hay datos creados (futuramente)
        InitPlayer();

        baseEvents = Resources.LoadAll<EventData>("Events");

        if (baseEvents.Length == 0)
        {
            Debug.LogWarning("No hay eventos en Resources/Events.");
            return;
        }

        GenerateEventsWeek();
        ShowState(); //optional initial debug
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            NextDay();
        }
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

    private void GenerateEventsWeek()
    {
        weekEvents.Clear();

        List<EventData> availableEvents = baseEvents
        .Where(e => !usedEvents.Contains(e))
        .ToList();

        if (availableEvents.Count < 3)
        {
            if (debugMode) Debug.Log("Se han usado todos los eventos, reiniciando lista disponible");
            usedEvents.Clear();
            availableEvents = new List<EventData>(baseEvents);
        }

        List<int> diasDisponibles = new List<int>() {2, 3, 4, 5, 6, 7 };
        List<EventData> eventosDisponibles = new List<EventData>(availableEvents);

        for (int i = 0; i < EventsPerWeek; i++)
        {
            if (eventosDisponibles.Count == 0 || diasDisponibles.Count == 0)
                break;

            int eventoIndex = Random.Range(0, eventosDisponibles.Count);
            int diaIndex = Random.Range(0, diasDisponibles.Count);

            EventData evento = eventosDisponibles[eventoIndex];
            int dia = diasDisponibles[diaIndex];

            weekEvents.Add(dia, evento);
            usedEvents.Add(evento);

            eventosDisponibles.RemoveAt(eventoIndex);
            diasDisponibles.RemoveAt(diaIndex);
        }

        UpdateEventsDebug();
    }

    public void NextDay()
    {
        // Pasar al siguiente día
        CurrentDay++;

        if (weekEvents.ContainsKey(CurrentDay))
        {
            var evento = weekEvents[CurrentDay];
            Debug.Log($"Evento de hoy: {evento.descripcion}");

            // Aquí luego vas a mostrarlo por UI
        }
        else
        {
            Debug.Log("Día tranquilo, sin eventos.");
        }

        // Si se pasa de 7 → nueva semana
        if (CurrentDay > DaysPerWeek)
        {
            NextWeek();
            CurrentDay = 1;
        }

        ShowState();
    }

    private void NextWeek()
    {
        CurrentMoney += CurrentMonthlySalary;
        CurrentWeek++;

        if (CurrentWeek > 4)
        {
            FinishMonth();
        }

        GenerateEventsWeek();
        Debug.Log($"Semana {CurrentWeek} iniciada con {EventsPerWeek} eventos nuevos.");
    }

    private void ShowState()
    {
        if (debugMode) Debug.Log($"Dinero: {CurrentMoney} | Moral: {CurrentMoral} |  Deuda: {CurrentDebt}");
        if (debugMode) Debug.Log($" Mes {CurrentMonth}, Semana {CurrentWeek}, Día {CurrentDay}");
    }

    private void FinishMonth()
    {
        //logica de avanzar a la siguiente zona

        CurrentWeek = 1;
        CurrentMonth++;
        Debug.Log($" Nuevo mes: {CurrentMonth}");
    }

    public void ApplyDecision(DecisionOption decision)
    {
        CurrentMoney += decision.cambioDinero;
        CurrentMoral += decision.cambioMoral;
        CurrentDebt += decision.cambioDeuda;

        switch (decision.typeDecision)
        {
            case DecisionType.Aceptar:
                Debug.Log("El jugador aceptó la propuesta.");
                break;
            case DecisionType.Rechazar:
                Debug.Log("El jugador rechazó la propuesta.");
                break;
            case DecisionType.Negociar:
                Debug.Log("El jugador intentó negociar.");
                break;
        }

        Debug.Log($"Nuevo estado → Dinero: {CurrentMoney}, Moral: {CurrentMoral}, Deuda: {CurrentDebt}");
    }

    public void UpdateEventsDebug()
    {
        debugEventsWeek.Clear();
        foreach (var kvp in weekEvents)
        {
            debugEventsWeek.Add($"Día {kvp.Key}: {kvp.Value.name}");
        }
    }
}
