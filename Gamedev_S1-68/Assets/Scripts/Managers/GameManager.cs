using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

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

    [SerializeField, ReadOnly] internal bool canProceedToNextDay = true;
    [SerializeField, ReadOnly] internal bool isGameOver = false;

    // Manager para el UI 
    public UIManager uiManager;

    private void Start()
    {
        //solo se llama si no hay datos creados (futuramente)
        InitPlayer();

        // Crea una lista de eventos
        baseEvents = Resources.LoadAll<EventData>("Events");

        if (baseEvents.Length == 0)
        {
            Debug.LogWarning("No hay eventos en Resources/Events.");
            return;
        }

        GenerateEventsWeek();
        ShowState();

        uiManager.InitUI();

        // Busca el UIManager en la escena
        if (uiManager == null)
        {
            uiManager = FindObjectOfType<UIManager>();
            if (uiManager == null)
            {
                Debug.LogError("UIManager no encontrado en la escena.");
                return;
            }
        }
    }

    private void Update()
    {
        // Método temporal para avanzar al siguiente día
        if (Input.GetKeyDown(KeyCode.Space) && canProceedToNextDay && !isGameOver)
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

        if (debugMode) Debug.Log("Jugador inicializado correctamente.");
    }

    // Genera los eventos de la semana
    private void GenerateEventsWeek()
    {
        weekEvents.Clear();

        // Genera una lista de eventos y excluye los ua utilizados
        List<EventData> availableEvents = baseEvents
        .Where(e => !usedEvents.Contains(e))
        .ToList();

        // Asegura que hayan al menos 3 eventos disponibles o reinicia la lista
        if (availableEvents.Count < 3)
        {
            if (debugMode) Debug.Log("Se han usado todos los eventos, reiniciando lista disponible");
            usedEvents.Clear();
            availableEvents = new List<EventData>(baseEvents);
        }

        List<int> diasDisponibles = new List<int>() { 2, 3, 4, 5, 6, 7 };
        List<EventData> eventosDisponibles = new List<EventData>(availableEvents);

        // Asigna 3 eventos aleatorios a la semana
        // Sugerencia de optimización por ChatGPT
        for (int i = 0; i < EventsPerWeek && eventosDisponibles.Count > 0 && diasDisponibles.Count > 0; i++)
        {
            EventData evento = eventosDisponibles[Random.Range(0, eventosDisponibles.Count)];
            int dia = diasDisponibles[Random.Range(0, diasDisponibles.Count)];

            weekEvents[dia] = evento;
            usedEvents.Add(evento);
            eventosDisponibles.Remove(evento);
            diasDisponibles.Remove(dia);
        }

        // for (int i = 0; i < EventsPerWeek; i++)
        // {
        //     if (eventosDisponibles.Count == 0 || diasDisponibles.Count == 0)
        //         break;

        //     int eventoIndex = Random.Range(0, eventosDisponibles.Count);
        //     int diaIndex = Random.Range(0, diasDisponibles.Count);

        //     EventData evento = eventosDisponibles[eventoIndex];
        //     int dia = diasDisponibles[diaIndex];

        //     weekEvents.Add(dia, evento);
        //     usedEvents.Add(evento);

        //     eventosDisponibles.RemoveAt(eventoIndex);
        //     diasDisponibles.RemoveAt(diaIndex);
        // }

        UpdateEventsDebug();
    }

    // Pasar al siguiente día
    public void NextDay()
    {
        CurrentDay++;

        // Muestra el evento en el UI y en la consola
        if (weekEvents.ContainsKey(CurrentDay))
        {
            var evento = weekEvents[CurrentDay];
            Debug.Log($"Evento de hoy: {evento.descripcion}");

            canProceedToNextDay = false;
            uiManager.ShowEvent(evento);
        }
        else
        {
            Debug.Log("Día tranquilo, sin eventos.");
            uiManager.ShowEventPanel();
            uiManager.ShowNoEventMessage();
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
        CurrentWeek++;

        if (CurrentWeek > 4)
        {
            FinishMonth();
        }

        GenerateEventsWeek();
        Debug.Log($"Semana {CurrentWeek} iniciada con {EventsPerWeek} eventos nuevos.");
    }

    // Muestra el estado actual del jugador en la consola
    private void ShowState()
    {
        if (debugMode) Debug.Log($"Dinero: {CurrentMoney} | Moral: {CurrentMoral} |  Deuda: {CurrentDebt}");
        if (debugMode) Debug.Log($" Mes {CurrentMonth}, Semana {CurrentWeek}, Día {CurrentDay}");
    }

    // Avanza al siguiente mes y el jugador cobra su sueldo
    private void FinishMonth()
    {
        //logica de avanzar a la siguiente zona
        CurrentMoney += CurrentMonthlySalary;
        CurrentWeek = 1;
        CurrentMonth++;
        Debug.Log($" Nuevo mes: {CurrentMonth}");
    }

    // Controla la toma de decisiones del jugador
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

        // Condiciones para terminarl el juego (y mostrar el menú para comenzar nuevamente)
        if (CurrentMoral < 0)
        {
            EndGame("La moral del jugador llegó a cero.");
            return;
        }

        if (CurrentMoney <= 0)
        {
            EndGame("El jugador se quedó sin dinero.");
            return;
        }

        if (CurrentDebt > CurrentMonthlySalary)
        {
            EndGame("La deuda del jugador superó su salario.");
            return;
        }

        canProceedToNextDay = true;

        uiManager.UpdateStats();
        uiManager.HideEventPanel();
    }

    public void UpdateEventsDebug()
    {
        debugEventsWeek.Clear();
        foreach (var kvp in weekEvents)
        {
            debugEventsWeek.Add($"Día {kvp.Key}: {kvp.Value.name}");
        }
    }

    // Termina el juego
    private void EndGame(string reason)
    {
        isGameOver = true;
        canProceedToNextDay = false;
        Debug.Log($"Juego terminado: {reason}");
        uiManager.ShowGameOver(reason);
    }

    public void RestartGame()
    {
        Debug.Log("Reiniciando el juego...");

        isGameOver = false;
        canProceedToNextDay = true;

        InitPlayer();
        CurrentDay = 1;
        GenerateEventsWeek();

        uiManager.HideEventPanel();
        uiManager.gameOverPanel.SetActive(false);
        uiManager.UpdateStats();
        uiManager.ShowNoEventMessage();

        ShowState();
    }
}
