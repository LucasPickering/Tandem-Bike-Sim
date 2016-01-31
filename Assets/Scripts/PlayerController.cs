using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{

    private enum Orientation
    {
        up = 0, right = 90, down = 180, left = 270
    };

    private enum State
    {
        onGround, onWallCeil, inAir
    };

    static Dictionary<string, Orientation> orientationsBySurface = new Dictionary<string, Orientation>()
    {
        {"Ground", Orientation.up},
        {"Left Wall", Orientation.right},
        {"Ceiling", Orientation.down},
        {"Right Wall", Orientation.left}
    };

    static Dictionary<Orientation, string> surfacesByOrientation = new Dictionary<Orientation, string>()
    {
        {Orientation.up, "Ground"},
        {Orientation.right, "Left Wall"},
        {Orientation.down, "Ceiling"},
        {Orientation.left, "Right Wall"}
    };

    private const int TERRAIN_LAYER = 8;

    public float groundMoveSpeed;
    public float groundJumpSpeed;
    public float wallCeilMoveSpeed;
    public float wallCeilJumpSpeed;

    private Rigidbody2D rigidBody;
    private Orientation orientation;
    private State state;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        float moveSpeed = 0f;
        float jumpSpeed = 0f;
        switch (state)
        {
            case State.onGround:
                moveSpeed = groundMoveSpeed;
                jumpSpeed = groundJumpSpeed;
                break;
            case State.onWallCeil:
                moveSpeed = wallCeilMoveSpeed;
                jumpSpeed = wallCeilJumpSpeed;
                break;
        }
        rigidBody.AddRelativeForce(new Vector2(Input.GetAxis("Horizontal") * groundMoveSpeed,
            Input.GetButton("Vertical") ? jumpSpeed : 0f));
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == TERRAIN_LAYER)
        {
            string tag = other.gameObject.tag;
            if (tag.Equals("Ground"))
            {
                state = State.onGround;
            }
            else
            {
                state = State.onWallCeil;
            }
            orientation = orientationsBySurface[other.gameObject.tag];
            transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, (float)orientation));
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.layer == TERRAIN_LAYER)
        {
            state = State.inAir;
        }
    }
}
