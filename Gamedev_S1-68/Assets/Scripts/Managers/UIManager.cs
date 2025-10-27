using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
  [Header("Referencias")]
  public GameManager gameManager;

  [Header("Valores")]
  public TextMeshProUGUI dineroText;
  public TextMeshProUGUI moralText;
  public TextMeshProUGUI deudaText;
  public TextMeshProUGUI eventDescriptionText;

  [Header("Botones")]
  public Button aceptarButton;
  public Button rechazarButton;
  public Button negociarButton;

  [Header("Panels")]
  public GameObject eventPanel;

  private EventData currentEvent;
  public GameObject gameOverPanel;
  public TextMeshProUGUI gameOverText;

  [Header("Game Over Controls")]
  public Button restartButton;

  void Start()
  {

  }

  public void InitUI()
  {
    UpdateStats();
    HideEventPanel();
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
    dineroText.text = $"{gameManager.CurrentMoney}";
    moralText.text = $"{gameManager.CurrentMoral}";
    deudaText.text = $"{gameManager.CurrentDebt}";
  }

  // Esconde el panel de eventos
  public void HideEventPanel()
  {
    eventPanel.SetActive(false);
  }

  // Muestra el panel de eventos
  public void ShowEventPanel()
  {
    eventPanel.SetActive(true);
  }

  // Muestra el mensaje de "No hay eventos el día de hoy."
  public void ShowNoEventMessage()
  {
    Debug.Log("No hay eventos el día de hoy.");
    eventDescriptionText.text = "No hay eventos el día de hoy.";
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
    gameOverPanel.SetActive(true);
    gameOverText.text = $"Juego Terminado\n{reason}";

    restartButton.onClick.RemoveAllListeners();
    restartButton.onClick.AddListener(() =>
    {
      gameManager.RestartGame();
    });
  }
}
