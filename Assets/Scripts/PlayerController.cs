using UnityEngine;
using System.Collections.Generic;
[RequireComponent(typeof(CharacterController))]

public class PlayerController : MonoBehaviour
{

    [System.Serializable]
    public class MovementValues
    {
        public float gravity;

        public float groundSpeed;

        public float groundJumpSpeed;

        public float wallSpeed;

        public float wallJumpSpeed;

        public float airSpeed;

    }

    private enum Orientation
    {
        up = 0,
        right = 90,
        down = 180,
        left = 270
    };

    private enum State
    {
        onGround,
        onWallCeil,
        inAir
    };

    static Dictionary<string, Orientation> orientationsBySurface = new Dictionary<string, Orientation>() {
        { "Ground", Orientation.up },
        { "Left Wall", Orientation.right },
        { "Ceiling", Orientation.down },
        { "Right Wall", Orientation.left }
    };

    static Dictionary<Orientation, string> surfacesByOrientation = new Dictionary<Orientation, string>() {
        { Orientation.up, "Ground" },
        { Orientation.right, "Left Wall" },
        { Orientation.down, "Ceiling" },
        { Orientation.left, "Right Wall" }
    };

    private const int TERRAIN_LAYER = 8;

    private CharacterController controller;
    public MovementValues movement;
    private Orientation orientation;
    private State state;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Rotate Left"))
        {
            Rotate(true);
        }
        if (Input.GetButtonDown("Rotate Right"))
        {
            Rotate(false);
        }
    }

    void FixedUpdate()
    {
        float moveSpeed = 0f;
        float jumpSpeed = 0f;
        switch (state)
        {
            case State.onGround:
                moveSpeed = movement.groundSpeed;
                jumpSpeed = movement.groundJumpSpeed;
                break;
            case State.onWallCeil:
                moveSpeed = movement.wallSpeed;
                jumpSpeed = movement.wallJumpSpeed;
                break;
        }
        Debug.Log(movement.gravity);
        controller.Move(new Vector2(Input.GetAxis("Horizontal") * moveSpeed,
            jumpSpeed - movement.gravity) * Time.fixedDeltaTime);
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == TERRAIN_LAYER)
        {
            string tag = other.gameObject.tag;

            // If we landed on a surface on something other than the wheels...
            if (!tag.Equals(surfacesByOrientation[orientation]))
            {
                Kill(); // Death
            }

            // If we landed on the ground...
            if (tag.Equals("Ground"))
            {
                state = State.onGround; // We're on the ground
            }
            else { // Otherwise...
                state = State.onWallCeil; // We're on a wall or ceiling
            }
        }
    }

    void OnCollisionExit(Collision other)
    {
        if (other.gameObject.layer == TERRAIN_LAYER)
        {
            state = State.inAir;
        }
    }

    private void Rotate(bool cw)
    {
        orientation = orientation + (cw ? 90 : -90) % 360; // Go to next or prev orientation
        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, (float)orientation));
    }

    private void Kill()
    {
        Debug.Log("RIP");
    }
}
