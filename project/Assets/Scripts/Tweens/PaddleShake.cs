using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaddleShake : MonoBehaviour
{
    [SerializeField] Vector3 _punch;
    [SerializeField] float _duration;
    [SerializeField] Ease _ease = Ease.Linear;

    private Tween _shakeTween;

    public void Shake()
    {
        // Restart the tween if active so we can tween the paddle
        if (_shakeTween.IsActive())
        {
            _shakeTween.Restart();
        }
        else
        {
            // Relatively shake the paddle
            _shakeTween = transform.DOPunchPosition(_punch, _duration)
                .SetEase(_ease)
                .SetRelative();
        }
        
    }

    private void OnDestroy()
    {
        // Kill the tween if active to prevent future problems
        if (_shakeTween.IsActive())
            _shakeTween.Kill();
    }
}
