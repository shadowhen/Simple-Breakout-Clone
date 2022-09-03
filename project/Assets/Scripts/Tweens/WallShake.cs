using DG.Tweening;
using UnityEngine;

public class WallShake : MonoBehaviour
{
    [SerializeField] private Vector3 _punch;
    [SerializeField] private float _duration;
    [SerializeField] private AudioSource _wallHitAudio;

    private Vector3 _originalPosition;
    private Vector3 _originalScale;

    private Sequence _shakeSequence;

    private void Start()
    {
        // Store original information since we are going to tween the wall
        _originalPosition = transform.position;
        _originalScale = transform.localScale;
    }

    public void Shake()
    {
        // Kill the shake sequence if the sequence is still playing
        if (_shakeSequence.IsActive())
            _shakeSequence.Kill();

        // Create a new sequence
        _shakeSequence = DOTween.Sequence();

        // Tween the scale and reset the position after the tween is complete
        _shakeSequence.Append(transform.DOPunchScale(_punch, _duration));
        _shakeSequence.OnKill(() =>
        {
            transform.position = _originalPosition;
            transform.localScale = _originalScale;
        });

        // Play audio after playing the sequence
        //GameManager.Instance.SoundManager.PlaySound("wall_hit");
        _wallHitAudio.Play();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // If the game object has a tag "Ball", shake the wall
        if (collision.collider.CompareTag("Ball"))
        {
            Shake();
        }
    }
}
