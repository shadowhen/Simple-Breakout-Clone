using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private float _speed = 20f;
    [SerializeField] private float _maxSpeed = 30f;
    [SerializeField] private float _speedIncrement = 0.5f;

    private Rigidbody _rigidbody;
    private Vector3 _velocity;

    [SerializeField] private float _launchTime;
    [SerializeField, Range(0f, 90f)] private float _minYAngle;

    private Sequence _scaleSequence;

    [SerializeField] private AudioSource _destroyAudio;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        Invoke("Launch", _launchTime);
    }

    private void Launch()
    {
        _rigidbody.velocity = Vector3.up * _speed;
    }

    void FixedUpdate()
    {
        _rigidbody.velocity = _rigidbody.velocity.normalized * _speed;
        _velocity = _rigidbody.velocity;   

        // Disabled since it is better to use colliders rather checking if it is visible
        /*
        if (!_renderer.isVisible)
        {
            GameManager.Instance.Balls--;
            Destroy(gameObject);
        }
        */
    }

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 newVelocity = Vector3.Reflect(_velocity, collision.contacts[0].normal);

        // Normalize the velocity to get the direction
        Vector3 direction = newVelocity.normalized;

        // Find the radians using sine and cosine functions
        float xRadian = Mathf.Acos(direction.x);
        float yRadian = Mathf.Asin(direction.y);

        // Convert radians to angles
        float xAngle = xRadian * Mathf.Rad2Deg; // Note: Does not have any use yet
        float yAngle = yRadian * Mathf.Rad2Deg;

        // Maxmize the y angle since we want the ball not to move "horizontally"
        float newYAngle = Mathf.Max(_minYAngle, Mathf.Abs(yAngle));

        // Convert the y angle into radians
        float newYRadian = newYAngle * Mathf.Deg2Rad;

        // Set new directions using trig functions
        direction.x = Mathf.Cos(newYRadian) * Mathf.Sign(direction.x);
        direction.y = Mathf.Sin(newYRadian) * Mathf.Sign(direction.y);

        // Make sure that the direction moves upward when collided with the player
        if (collision.collider.CompareTag("Player"))
        {
            // Since up is always positive, we would take the absolute value
            direction.y = Mathf.Abs(direction.y);
        }

        // Increment and clamp the speed
        _speed = Mathf.Min(_speed + _speedIncrement, _maxSpeed);

        // Set the velcoity using the direction and speed
        _rigidbody.velocity = direction * _speed;

        // Tween the ball's scale
        if (_scaleSequence.IsActive())
            _scaleSequence.Kill();
        _scaleSequence = DOTween.Sequence();
        _scaleSequence.Append(transform.DOScale(0.75f, 0.2f));
        _scaleSequence.Append(transform.DOScale(0.5f, 0.2f));
    }

    // Destroys the ball while also enabling the sound
    public void Kill(bool playAudio = true)
    {
        if (playAudio)
        {
            _destroyAudio.Play();
        }
        StartCoroutine(KillBall());
    }

    // Coroutine method to handle destroying the ball
    private IEnumerator KillBall()
    {
        // This would prevent the destroying the game object prematurely while
        // the destroy audio is playing
        // Note: The only problem is that the update call can 
        yield return new WaitWhile(() => _destroyAudio.isPlaying);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        //GameManager.Instance.SoundManager.PlaySound("lost_ball");
        if (_scaleSequence.IsActive())
            _scaleSequence.Kill();
    }
}
