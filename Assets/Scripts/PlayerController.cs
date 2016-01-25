using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    private enum Rotation
    {
        up = 0, right = 90, down = 180, left = 270
    };

    public int moveSpeed;
    public int jumpSpeed;

    private Rigidbody2D rigidBody;
    private Rotation rotation;
    private float distanceToGround;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        distanceToGround = GetComponent<Collider2D>().bounds.extents.y;
    }

    void FixedUpdate()
    {
        Vector2 force = new Vector2(0f, 0f);
        if (CanJump())
        {
            force.x = Input.GetAxis("Horizontal") * moveSpeed;
            force.y = Input.GetButton("Vertical") ? jumpSpeed : 0f;
        }

        force = Quaternion.Euler(new Vector3(0f, 0f, (float)rotation)) * force;
        rigidBody.AddForce(force);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        switch (other.gameObject.tag)
        {
            case "Ground":
                rotation = Rotation.up;
                break;
            case "Right Wall":
                rotation = Rotation.right;
                break;
            case "Ceiling":
                rotation = Rotation.down;
                break;
            case "Left Wall":
                rotation = Rotation.left;
                break;
            default:
                return;
        }

        // We know a wall, ground, or ceiling was hit by now
        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, (float)rotation));
    }

    private bool CanJump()
    {
        return Physics2D.Raycast(transform.position, transform.rotation * Vector3.down, distanceToGround + 0.1f);
    }
}
