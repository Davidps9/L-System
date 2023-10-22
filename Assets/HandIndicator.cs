using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class HandIndicator : MonoBehaviour
{
    [SerializeField] private Image fill;
    [SerializeField] private float targetAlpha = 0.5f;
    [SerializeField] private float fadeInDelay = 0.5f;
    [SerializeField] private float fadeInTime = 1f;
    //private Vector3 targetPosition = Vector3.zero;
    //private Vector3 currentPosition = Vector3.zero;

    private void Start()
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.DOFade(targetAlpha, fadeInTime).SetDelay(fadeInDelay);
    }

    public void SetSide(bool isLeft)
    {
        transform.localScale = new Vector3(isLeft ? -1 : 1, 1, 1);
        fill.fillClockwise = isLeft;
    }

    public void SetFill(float value)
    {
        fill.fillAmount = value;
    }

    public void EaseFill(float toValue, float duration)
    {
        fill.DOFillAmount(toValue, duration);
    }

    public void SnapTo(Vector3 position)
    {
        transform.position = position;
        //currentPosition = position;
    }

    // TODO: Smooth movement
}
