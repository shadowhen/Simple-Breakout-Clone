using DG.Tweening;
using UnityEngine;

public class ScaleUpOneShot : MonoBehaviour
{
    [SerializeField] private Vector3 _initalScale = Vector3.zero;
    [SerializeField] private float _targetScaleScalar = 1f;
    [SerializeField] private float _targetDuration = 0.5f;
    [SerializeField] private Ease _ease = Ease.Linear;

    // Stores the tween in case that we need to kill later
    private Tween _scaleTween;

    // Start is called before the first frame update
    private void Start()
    {
        // Set the inital scale first before we tween the scale
        transform.localScale = _initalScale;

        // Tween towards the target scale during target duration
        _scaleTween = transform.DOScale(_targetScaleScalar, _targetDuration)
            .OnKill(() => Destroy(this)).SetEase(_ease);
    }

    private void OnDestroy()
    {
        // In case the game object is destroyed before the tween is complete,
        // kill the tween to prevent possible issues
        if (_scaleTween.IsActive())
            _scaleTween.Kill();
    }
}
