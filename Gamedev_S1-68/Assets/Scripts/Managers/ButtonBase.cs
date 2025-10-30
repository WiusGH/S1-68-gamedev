using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[SelectionBase]
public abstract class ButtonBase : MonoBehaviour
{
    [SubHeader("ButtonBase")]
    [SerializeField] internal Button m_mainBtn;

    public void InitButton(UnityAction btnEvent)
    {
        m_mainBtn.onClick.AddListener(btnEvent);
    }

    public void SetColor(Color color)
    {
        m_mainBtn.GetComponent<Image>().color = color;
    }
}
