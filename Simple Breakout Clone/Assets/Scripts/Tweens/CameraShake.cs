using DG.Tweening;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private float _duration;
    [SerializeField] private Vector3 _strength;

    private Vector3 _resetPosition;

    private void Start()
    {
        _resetPosition = transform.position;
    }

    public void ShakeCamera()
    {
        transform.DOShakePosition(_duration, _strength).OnComplete(() => transform.position = _resetPosition);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShakeCamera();
        }
    }
}
