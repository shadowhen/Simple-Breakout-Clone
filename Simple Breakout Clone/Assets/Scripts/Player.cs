using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody _rigidbody;

    [SerializeField] private AudioSource _ballCollidedAudioSource;
    [SerializeField] private PaddleShake _shake;

    void Start()
    {
        // Get rigid body component so we can interact with the rigid body for movement
        _rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // We need to get mouse position first
        Vector3 mousePosition = Input.mousePosition;

        // We also want to set the z as the distance from the camera plane
        mousePosition.z = 50;

        // We want to get position in world space where the mouse cursor is on the screen
        Vector3 mouseWorldSpacePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        // Create a new position using the position from the screen to world
        Vector3 newPosition = new Vector3(mouseWorldSpacePosition.x, -17, 0);

        // Move to a new position by moving the rigid body's position
        _rigidbody.MovePosition(newPosition);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Should the collider have a tag "Ball", play audio and shake the paddle
        if (collision.collider.CompareTag("Ball"))
        {
            _ballCollidedAudioSource?.Play();
            _shake.Shake();
        }
    }
}
