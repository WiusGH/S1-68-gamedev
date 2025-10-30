using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    [SubHeader("References")]
    [SerializeField, ReadOnly] internal GameManager gameManager;

    [SubHeader("Panels")]
    [SerializeField] internal GameObject eventPanel;
    [SerializeField] internal GameObject feedbackPanel;
    [SerializeField] internal GameObject gameOverPanel;

    [SubHeader("Texts")]
    [SerializeField] internal TextMeshProUGUI eventDescriptionText;

    [SerializeField] internal TextMeshProUGUI feedbackText;
    [SerializeField] internal TextMeshProUGUI feedbackStatMoney;
    [SerializeField] internal TextMeshProUGUI feedbackStatMorale;
    [SerializeField] internal TextMeshProUGUI feedbackStatDebt;

    [SerializeField] internal TextMeshProUGUI moneyStatText;
    [SerializeField] internal TextMeshProUGUI moraleStatText;
    [SerializeField] internal TextMeshProUGUI debtStatText;

    [SerializeField] internal TextMeshProUGUI gameOverText;

    [SerializeField] internal TextMeshProUGUI titleText;
    [SerializeField] internal TextMeshProUGUI dayText;
    [SerializeField] internal TextMeshProUGUI lessonText;

    [SerializeField] internal TextMeshProUGUI neighborhoodText;

    [SubHeader("Buttons")]
    [SerializeField] private Button aceptarButton = default;
    [SerializeField] private Button rechazarButton = default;
    [SerializeField] private Button negociarButton = default;

    [SerializeField] private Button continueButton = default;

    [SerializeField] private Button restartButton = default;

    [SerializeField] internal GenericButton nextDayBtn = default;

    public static readonly Dictionary<Neighborhoods, string> NeighborhoodNames = new()
    {
        { Neighborhoods.Zone_1, "Barrios Endeudados" },
        { Neighborhoods.Zone_2, "Barrios Gastadores" },
        { Neighborhoods.Zone_3, "Los Altos Barrios" }
    };


    private void Awake()
    {
        gameManager = GameMainScene.gameManager;
    }

    public void Start()
    {
        InitButtons();
    }

    public void InitButtons()
    {
        nextDayBtn.InitButton(() => GameMainScene.gameManager.NextDay());
    }

    public void InitUI() 
    {
        UpdateStats();
        HideAllPanels();
        ShowNoEventMessage();

        UpdateNeighborhoodText();
    }

    public void HideAllPanels()
    {
        eventPanel.SetActive(false);
        feedbackPanel.SetActive(false);
        gameOverPanel.SetActive(false);
    }

    public void ShowEvent(EventData eventData)
    {
        ShowPanel(eventPanel);

        eventDescriptionText.text = eventData.descripcion;
        EnableAllDecisionButtons();

        if (eventData.decisiones.Length > 0)
        {
            SetDecisionButton(aceptarButton, eventData.decisiones, DecisionType.Aceptar);
            SetDecisionButton(rechazarButton, eventData.decisiones, DecisionType.Rechazar);
            SetDecisionButton(negociarButton, eventData.decisiones, DecisionType.Negociar);
        }
    }

    public void SetDecisionButton(Button button, DecisionData[] decisiones, DecisionType type)
    {
        var decision = Array.Find(decisiones, d => d.DecisionOption.typeDecision == type);

        bool hasDecision = decision != null;
        button.gameObject.SetActive(hasDecision);
        if (!hasDecision) return;

        var option = decision.DecisionOption;
        button.GetComponentInChildren<TextMeshProUGUI>().text = option.titulo;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => HandleDecision(option));
    }

    public void HandleDecision(DecisionOption decision)
    {
        DisableAllDecisionButtons();
        gameManager.ApplyDecision(decision);
        UpdateStats();
        ShowFeedback(decision);
    }

    public void UpdateStats()
    {
        moneyStatText.text = $"${gameManager.currentMoney}";
        moraleStatText.text = $"%{gameManager.currentMoral}";
        debtStatText.text = $"${gameManager.currentDebt}";
    }

    public void ShowPanel(GameObject panel) => panel.SetActive(true);
    public void HidePanel(GameObject panel) => panel.SetActive(false);

    public void ShowNoEventMessage()
    {
        eventDescriptionText.text = "No hay eventos el día de hoy.";
    }

    public void ShowFeedback(DecisionOption decision)
    {
        ShowPanel(feedbackPanel);

        feedbackText.text = decision.feedback;
        feedbackStatMoney.text = $"${FormatChange(decision.cambioDinero)}";
        feedbackStatMorale.text = FormatChange(decision.cambioMoral);
        feedbackStatDebt.text = FormatChange(decision.cambioDeuda);

        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(() =>
        {
            HidePanel(feedbackPanel);
            HidePanel(eventPanel);
            gameManager.canProceedToNextDay = true;
        });
    }

    public string FormatChange(int value) =>
        value switch
        {
            > 0 => $"+{value}",
            < 0 => value.ToString(),
            _ => "0"
        };

    public void DisableAllDecisionButtons()
    {
        SetButtonsInteractable(false);
    }

    public void EnableAllDecisionButtons()
    {
        SetButtonsInteractable(true);
    }

    public void SetButtonsInteractable(bool state)
    {
        aceptarButton.interactable = state;
        rechazarButton.interactable = state;
        negociarButton.interactable = state;
    }

    public void ShowGameOver(string reason)
    {
        HidePanel(eventPanel);
        ShowPanel(gameOverPanel);

        gameOverText.text = $"Juego Terminado\n{reason}";
        restartButton.onClick.RemoveAllListeners();
        restartButton.onClick.AddListener(gameManager.RestartGame);
    }

    public void UpdateTextsForTime(int week, int day, int totalDays, int currentLesson , string prefix = "Credopolis")
    {
        titleText.text = $"{prefix} - Semana {week}";
        dayText.text = $"Dia {day} de {totalDays}";
        lessonText.text = $"Leccion {currentLesson}";
    }

    public void UpdateNeighborhoodText()
    {
        if (NeighborhoodNames.TryGetValue(GameMainScene.gameManager.Currentneighborhood, out string name))
            neighborhoodText.text = name;
    }
}
