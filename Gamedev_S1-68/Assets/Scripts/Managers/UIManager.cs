using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
  [Header("Referencias")]
  [SerializeField, ReadOnly] internal GameManager gameManager;

  [Header("Valores")]
  [SerializeField] internal TextMeshProUGUI dineroText;
  [SerializeField] internal TextMeshProUGUI moralText;
  [SerializeField] internal TextMeshProUGUI deudaText;
  [SerializeField] internal TextMeshProUGUI eventDescriptionText;

  [Header("Feedback Panel")]
  [SerializeField] internal GameObject feedbackPanel;
  [SerializeField] internal TextMeshProUGUI feedbackText;
  [SerializeField] internal TextMeshProUGUI feedbackStatMoney;
  [SerializeField] internal TextMeshProUGUI feedbackStatMorale;
  [SerializeField] internal TextMeshProUGUI feedbackStatDebt;
  [SerializeField] internal Button continueButton;

  [Header("Botones")]
  [SerializeField] internal Button aceptarButton;
  [SerializeField] internal Button rechazarButton;
  [SerializeField] internal Button negociarButton;

  [Header("Panels")]
  [SerializeField] internal GameObject eventPanel;

  private EventData currentEvent;
  [SerializeField] internal GameObject gameOverPanel;
  [SerializeField] internal TextMeshProUGUI gameOverText;

  [Header("Game Over Controls")]
  [SerializeField] internal Button restartButton;

  public void InitUI()
  {
    UpdateStats();
    HideGameOverPanel();
    HideEventPanel();
    HideFeedbackPanel();
    ShowNoEventMessage();
  }

  // Muestra en el UI el evento actual y las opciones
  public void ShowEvent(EventData eventData)
  {
    ShowEventPanel();
    currentEvent = eventData;
    eventDescriptionText.text = eventData.descripcion;

    EnableAllDecisionButtons();

    // Assigna el texto a los botones dinámicamente
    if (eventData.decisiones.Length > 0)
    {
      SetButton(aceptarButton, eventData.decisiones, DecisionType.Aceptar);
      SetButton(rechazarButton, eventData.decisiones, DecisionType.Rechazar);
      SetButton(negociarButton, eventData.decisiones, DecisionType.Negociar);
    }
  }

  // Función para asignar el texto a los botones dinámicamente
  private void SetButton(Button button, DecisionData[] decisiones, DecisionType type)
  {
    var decision = System.Array.Find(decisiones, d => d.DecisionOption.typeDecision == type);

    if (decision != null)
    {
      button.gameObject.SetActive(true);
      button.GetComponentInChildren<TextMeshProUGUI>().text = decision.DecisionOption.titulo;
      button.onClick.RemoveAllListeners();
      button.onClick.AddListener(() =>
      {
        DisableAllDecisionButtons();
        gameManager.ApplyDecision(decision.DecisionOption);
        UpdateStats();
        ShowFeedback(decision.DecisionOption);
      });
    }
    else
    {
      button.gameObject.SetActive(false);
    }
  }

  // Actualiza los recursos del jugador después de cada decisión
  public void UpdateStats()
  {
    dineroText.text = $"{gameManager.currentMoney}";
    moralText.text = $"{gameManager.currentMoral}";
    deudaText.text = $"{gameManager.currentDebt}";
  }

  // Esconde el panel de eventos
  public void HideEventPanel()
  {
    eventPanel.SetActive(false);
  }

  public void HideFeedbackPanel()
  {
    feedbackPanel.SetActive(false);
  }

  public void HideGameOverPanel()
  {
    gameOverPanel.SetActive(false);
  }

  // Muestra el panel de eventos
  public void ShowEventPanel()
  {
    eventPanel.SetActive(true);
  }

  public void ShowGameOverPanel()
  {
    gameOverPanel.SetActive(true);
  }

  // Muestra el mensaje de "No hay eventos el día de hoy."
  public void ShowNoEventMessage()
  {
    Debug.Log("No hay eventos el día de hoy.");
    eventDescriptionText.text = "No hay eventos el día de hoy.";
  }

  public void ShowFeedback(DecisionOption decision)
  {
    feedbackPanel.SetActive(true);

    feedbackText.text = decision.feedback;

    feedbackStatMoney.text = $"${FormatChange(decision.cambioDinero)}";
    feedbackStatMorale.text = FormatChange(decision.cambioMoral);
    feedbackStatDebt.text = FormatChange(decision.cambioDeuda);

    continueButton.onClick.RemoveAllListeners();
    continueButton.onClick.AddListener(() =>
    {
      feedbackPanel.SetActive(false);
      HideEventPanel();
      gameManager.canProceedToNextDay = true;
    });
  }

  private string FormatChange(int value)
  {
    if (value > 0) return $"+{value}";
    if (value < 0) return $"{value}";
    return "0";
  }


  // Deshabilita los botones de decision
  private void DisableAllDecisionButtons()
  {
    aceptarButton.interactable = false;
    rechazarButton.interactable = false;
    negociarButton.interactable = false;
  }

  // Habilita los botones de decision
  private void EnableAllDecisionButtons()
  {
    aceptarButton.interactable = true;
    rechazarButton.interactable = true;
    negociarButton.interactable = true;
  }

  // Muestra el panel de juego terminado
  public void ShowGameOver(string reason)
  {
    HideEventPanel();
    ShowGameOverPanel();
    gameOverPanel.SetActive(true);
    gameOverText.text = $"Juego Terminado\n{reason}";

    restartButton.onClick.RemoveAllListeners();
    restartButton.onClick.AddListener(() =>
    {
      gameManager.RestartGame();
    });
  }
}
