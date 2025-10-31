using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;

public class DoTweenManager : MonoBehaviour
{
    // --- POPUPS ---
    public void OpenPopup(Transform target, float duration = 0.3f)
    {
        Vector3 originalScale = target.localScale;

        target.localScale = Vector3.zero;
        target.gameObject.SetActive(true);
        target.DOScale(originalScale, duration).SetEase(Ease.OutBack);
    }

    public void ClosePopup(Transform target, float duration = 0.25f)
    {
        Vector3 originalScale = target.localScale;

        target.DOScale(0f, duration).SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                target.gameObject.SetActive(false);
                target.localScale = originalScale;
            });

    }

    public void PlayPopEffect(Transform target)
    {
        if (target == null) return;

        target.DOKill();

        Vector3 originalScale = target.localScale;
        Vector3 reducedScale = originalScale * 0.9f;
        Vector3 popScale = originalScale * 1.05f;

        target.localScale = reducedScale;
        target.gameObject.SetActive(true);

        target.DOScale(popScale, 0.15f)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                target.DOScale(originalScale, 0.1f).SetEase(Ease.InOutSine);
            });
    }

    public void PlayCloseEffect(Transform target)
    {

        if (target == null) return;

        target.DOKill();

        Vector3 originalScale = target.localScale;
        Vector3 shrinkScale = originalScale * 0.85f;

        target.DOScale(shrinkScale, 0.12f)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                target.localScale = originalScale;
                target.gameObject.SetActive(false);
            });
    }

    // --- PANEL TRANSICIONES ---
    public void SlidePanel(RectTransform panel, Vector2 targetPos, float duration = 0.4f)
    {
        panel.DOAnchorPos(targetPos, duration).SetEase(Ease.OutCubic);
    }

    public void SlideBetweenPanels(RectTransform currentPanel, RectTransform nextPanel, bool toRight, float duration = 0.4f)
    {
        float screenWidth = currentPanel.rect.width;
        float direction = toRight ? 1 : -1;

        nextPanel.anchoredPosition = new Vector2(screenWidth * direction, 0);
        nextPanel.gameObject.SetActive(true);

        currentPanel.DOAnchorPos(new Vector2(-screenWidth * direction, 0), duration)
            .SetEase(Ease.OutCubic);

        nextPanel.DOAnchorPos(Vector2.zero, duration)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                currentPanel.gameObject.SetActive(false);
                currentPanel.anchoredPosition = Vector2.zero;
            });
    }

    public void FadeCanvas(CanvasGroup canvasGroup, float targetAlpha, float duration = 0.3f)
    {
        canvasGroup.DOFade(targetAlpha, duration);
        canvasGroup.interactable = targetAlpha > 0.9f;
        canvasGroup.blocksRaycasts = targetAlpha > 0.9f;
    }

    // --- BOTONES Y EFECTOS ---
    public void PunchButton(Transform button, float intensity = 0.1f, float duration = 0.2f)
    {
        button.DOPunchScale(Vector3.one * intensity, duration, 1, 0.5f);
    }

    public void PressButton(Transform button, float intensity = 0.1f, float duration = 0.15f, Action onComplete = null)
    {
        Vector3 pressedScale = Vector3.one * (1f - intensity);

        button.DOScale(pressedScale, duration * 0.5f)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                button.DOScale(Vector3.one, duration * 0.5f).SetEase(Ease.InCubic);
                onComplete?.Invoke();
            });

    }

    public void ColorFlash(Image image, Color flashColor, float duration = 0.3f)
    {
        Color original = image.color;
        image.DOColor(flashColor, duration * 0.5f).OnComplete(() =>
            image.DOColor(original, duration * 0.5f));
    }

    public void RotateIcon(Transform icon, float rotationAmount = 15f, float duration = 0.5f)
    {
        icon.DOPunchRotation(new Vector3(0, 0, rotationAmount), duration);
    }
}
