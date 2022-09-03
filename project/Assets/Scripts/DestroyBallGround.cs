using UnityEngine;

public class DestroyBallGround : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Ball ball = other.GetComponent<Ball>();
        if (ball != null)
        {
            GameManager.Instance.Balls--;
            ball.Kill();
        }
    }
}
