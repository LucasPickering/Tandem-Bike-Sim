using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    public int moveSpeed;
    public int jumpSpeed;

    private Rigidbody2D rigidBody;
    private bool onGround = true;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        float lateralForce = Input.GetAxis("Horizontal") * moveSpeed;
        float verticalForce = Input.GetButton("Vertical") && onGround ? jumpSpeed : 0f;
        rigidBody.AddForce(new Vector2(lateralForce, verticalForce));
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            onGround = true;
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            onGround = false;
        }
    }
}
