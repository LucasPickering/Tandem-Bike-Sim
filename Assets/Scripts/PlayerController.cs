using UnityEngine;
using System.Collections.Generic;

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

    public MovementValues movement;
    private Orientation orientation;
    private State state = State.inAir;

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
        float vx = 0f;
        float vy = 0f;
        switch (state)
        {
            case State.onGround:
                vx = movement.groundSpeed;
                vy = movement.groundJumpSpeed;
                break;
            case State.onWallCeil:
                vx = movement.wallSpeed;
                vy = movement.wallJumpSpeed;
                break;
            case State.inAir:
                vx = movement.airSpeed;
                vy = movement.gravity;
                break;
        }
        transform.Translate(new Vector2(Input.GetAxis("Horizontal") * vx,
            vy) * Time.fixedDeltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
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

    void OnTriggerExit2D(Collider2D other)
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
