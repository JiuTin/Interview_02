using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class Widget : MonoBehaviour
{
    private CanvasGroup _canvasGroup;
    //从(0,0)到(1,1)的曲线
    [SerializeField] private AnimationCurve _fadingCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Coroutine _fadeCoroutine;
    public float RenderOpacity
    {
        get => _canvasGroup.alpha;
        set => _canvasGroup.alpha = value;
    }
    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    //在显示对话框时调用，duration<0时直接显示，>0时实现淡入淡出
    public void Fade(float opacity, float duration, Action onFinished)
    {
        if (duration <= 0)
        {
            _canvasGroup.alpha = opacity;
            onFinished?.Invoke();
        }
        else
        {
            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
            }
            _fadeCoroutine = StartCoroutine(Fading(opacity, duration, onFinished));
        }
    }
    //淡入淡出
    private IEnumerator Fading(float opacity, float duration, Action onFinished)
    {
        float timer = 0;
        float start = RenderOpacity;
        while (timer < duration)
        {
            timer = Mathf.Min(duration, timer+Time.unscaledDeltaTime);
            RenderOpacity = Mathf.Lerp(start, opacity, _fadingCurve.Evaluate(timer / duration));
            yield return null; //暂缓1帧
        }
        onFinished?.Invoke();
    }
}
