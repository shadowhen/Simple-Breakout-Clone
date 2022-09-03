using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Brick : MonoBehaviour
{
    [SerializeField] private int _hits;
    [SerializeField] private int _points;
    [SerializeField] Vector3 _rotator;

    [SerializeField] private Material _hitMaterial;
    private Material _origMaterial;
    private Renderer _renderer;
    private Collider _collider;

    //[SerializeField] private AudioSource _destroyAudioSource;
    [SerializeField] private AudioSource _hitAudioSource;

    private Tweener _tweener;

    void Start()
    {
        transform.Rotate(_rotator * (transform.position.x + transform.position.y) * 0.1f);
        _renderer = GetComponent<Renderer>();
        _origMaterial = _renderer.sharedMaterial;
        _collider = GetComponent<Collider>();
    }

    void Update()
    {
        transform.Rotate(_rotator * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        _hits--;
        if (_hits <= 0)
        {
            GameManager.Instance.Score += _points;

            _collider.enabled = false;
            //GameManager.Instance.SoundManager.PlaySound("brick_destroy");
            _tweener = transform.DOScale(0.0f, 0.5f)
                .OnComplete(() => StartCoroutine(DestroyItself()));
        }
        else
        {
            //GameManager.Instance.SoundManager.PlaySound("brick_hit");
        }
        _hitAudioSource?.Play();
        _renderer.sharedMaterial = _hitMaterial;
        Invoke("RestoreMaterial", 0.05f);
    }

    private void RestoreMaterial()
    {
        _renderer.sharedMaterial = _origMaterial;
    }

    private IEnumerator DestroyItself()
    {
        yield return new WaitWhile(() => _hitAudioSource.isPlaying);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        _tweener.Kill();
    }
}
