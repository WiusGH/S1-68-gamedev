using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameMainScene : DekuSingletonBase<GameMainScene>
{
    protected GameMainScene() { }

    #region main References
    private CanvasManager p_canvasManager;
    public static CanvasManager CanvasManager => Instance.p_canvasManager ? Instance.p_canvasManager : Instance.GetCanvasManager();
    private CanvasManager GetCanvasManager()
    {
        CanvasManager canvasManager = FindObjectOfType<CanvasManager>();
        if (canvasManager)
        {
            Instance.p_canvasManager = canvasManager;
            return Instance.p_canvasManager;
        }
        else
        {
            Debug.LogError("No se encontró el objeto CanvasManager en la escena.");
            return null;
        }
    }

     private GameManager p_gameManager;
    public static GameManager GameManager => Instance.p_gameManager ? Instance.p_gameManager : Instance.GetGameManager();
    private GameManager GetGameManager()
    { 
        GameManager gameManager = FindObjectOfType<GameManager>();

        if (gameManager) 
        {
            Instance.p_gameManager = gameManager;
            return Instance.p_gameManager;
        }
        else 
        {
            Debug.LogError("No se encontro el objeto GameManager en la escena");
            return null;
        }
    }

    #endregion
}
