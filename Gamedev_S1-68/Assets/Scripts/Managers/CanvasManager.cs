using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class FeedbackUI
{
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI moneyChangeText;
    public TextMeshProUGUI moraleChangeText;
    public TextMeshProUGUI debtChangeText;
}

public class CanvasManager : MonoBehaviour
{
    [SubHeader("References")]
    [SerializeField, ReadOnly] internal GameManager gameManager;
    [SerializeField] internal DoTweenManager doTweenManager;

    [SubHeader("Panels")]
    [SerializeField] internal GameObject feedbackPanel;
    [SerializeField] internal GameObject gameOverPanel;
    [SerializeField] internal GameObject decisionPanel;

    [SubHeader("Texts")]
    [SerializeField] internal TextMeshProUGUI eventDescriptionText;
    [SerializeField] internal TextMeshProUGUI typeEventText;

    [SerializeField] internal TextMeshProUGUI moneyStatText;
    [SerializeField] internal TextMeshProUGUI moraleStatText;
    [SerializeField] internal TextMeshProUGUI debtStatText;

    [SerializeField] internal TextMeshProUGUI gameOverText;

    [SerializeField] internal TextMeshProUGUI titleText;
    [SerializeField] internal TextMeshProUGUI dayText;
    [SerializeField] internal TextMeshProUGUI lessonText;

    [SerializeField] internal TextMeshProUGUI neighborhoodText;
    [SerializeField] internal TextMeshProUGUI nextDayBtnText;

    [SerializeField] internal FeedbackUI feedbackPanelTexts;

    [SubHeader("Buttons")]
    [SerializeField] internal GenericButton acceptBtn = default;
    [SerializeField] internal GenericButton rejectBtn = default;
    [SerializeField] internal GenericButton negotiateBtn = default;

    [SerializeField] internal GenericButton continueButton = default;
    [SerializeField] internal Button restartButton = default;
    [SerializeField] internal GenericButton nextDayBtn = default;

    [SubHeader("Images")]
    [SerializeField] internal Image eventIcon;

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
        feedbackPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        decisionPanel.SetActive(false);
    }

    public void SetDecisionButtons(EventData eventData)
    {
        eventDescriptionText.text = eventData.descripcion;
        typeEventText.text = eventData.eventType;
        eventIcon.sprite = eventData.eventIcon;
        eventIcon.enabled = true;

        nextDayBtnText.text = "Decidir";

        var aceptar = Array.Find(eventData.decisiones, d => 
        d.DecisionOption.typeDecision == DecisionType.Aceptar);

        var rechazar = Array.Find(eventData.decisiones, d => 
        d.DecisionOption.typeDecision == DecisionType.Rechazar);

        var negociar = Array.Find(eventData.decisiones, d =>
        d.DecisionOption.typeDecision == DecisionType.Negociar);

        SetDecisionButton(acceptBtn, aceptar);
        SetDecisionButton(rejectBtn, rechazar);
        SetDecisionButton(negotiateBtn, negociar);

        nextDayBtn.InitButton(() => {
            doTweenManager.OpenPopup(decisionPanel.transform);
        } );
    }


    private void SetDecisionButton(GenericButton button, DecisionData decision)
    {
        bool hasDecision = decision != null;
        if (!hasDecision) return;

        button.gameObject.SetActive(hasDecision);

        var option = decision.DecisionOption;

        List<string> cambios = new();

        if (option.cambioMoral != 0)
            cambios.Add($"Moral {FormatChange(option.cambioMoral)}");

        if (option.cambioDinero != 0)
            cambios.Add($"Dinero {FormatChange(option.cambioDinero)}");

        if (option.cambioDeuda != 0)
            cambios.Add($"Deuda {FormatChange(option.cambioDeuda)}");

        string cambiosTexto = cambios.Count > 0 ? $" ({string.Join(", ", cambios)})" : "";

        button.textBtn.text = $"{option.titulo}{cambiosTexto}";

        button.m_mainBtn.onClick.RemoveAllListeners();
        button.InitButton(() => HandleDecision(option));
    }

    public void HandleDecision(DecisionOption decision)
    {
        gameManager.ApplyDecision(decision);
        UpdateStats();
        HidePanel(decisionPanel);

        ShowFeedback(decision);

        nextDayBtnText.text = "Siguiente dia";
        nextDayBtn.m_mainBtn.onClick.RemoveAllListeners();
        nextDayBtn.InitButton(() => {
            GameMainScene.gameManager.NextDay();
        });
    }

    public void UpdateStats()
    {
        moneyStatText.text = $"${gameManager.currentMoney}";
        moraleStatText.text = $"%{gameManager.currentMoral}";
        debtStatText.text = $"${gameManager.currentDebt}";
    }

    public void ShowPanel(GameObject panel)
    {
        panel.SetActive(true);
        panel.transform.localScale = new Vector3(1, 1, 1);
    }

    public void HidePanel(GameObject panel) => panel.SetActive(false);

    public void ShowNoEventMessage()
    {
        eventDescriptionText.text = "No hay eventos el día de hoy.";
        typeEventText.text = "";
        eventIcon.sprite = null;
        eventIcon.enabled = false;
    }

    public void ShowFeedback(DecisionOption decision)
    {
        doTweenManager.OpenPopup(feedbackPanel.transform);

        feedbackPanelTexts.descriptionText.text = decision.feedback;
        feedbackPanelTexts.moneyChangeText.text = $"${FormatChange(decision.cambioDinero)}";
        feedbackPanelTexts.moraleChangeText.text = $"{FormatChange(decision.cambioMoral)}%";
        feedbackPanelTexts.debtChangeText.text = $"${FormatChange(decision.cambioDeuda)}";

        continueButton.InitButton(() =>
        {
            HidePanel(feedbackPanel);
            gameManager.canProceedToNextDay = true;
        });
    }

    public string FormatChange(int value) =>
        value switch
        {
            > 0 => $"+ {value}",
            < 0 => $"{value}",
            _ => "0"
        };

    public void ShowGameOver(string reason)
    {
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
