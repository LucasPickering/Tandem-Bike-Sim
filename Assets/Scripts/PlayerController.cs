using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{

    [System.Serializable]
    public class MovementValues
    {
        public float gravity;

        public float groundSpeed;
        public float groundJumpSpeed;
        public float maxGroundSpeed;

        public float wallSpeed;
        public float wallJumpSpeed;
        public float maxWallSpeed;

        public float airSpeed;

        [System.NonSerialized]
        public Vector2 velocity;
    }

    private enum Orientation
    {
        up = 0, right = 90, down = 180, left = 270
    };

    private enum State
    {
        onGround, onWall, onCeiling, inAir
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
    private Rigidbody2D rigidBody;
    private Orientation orientation;
    private State state = State.inAir;
    private bool jump;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }
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
        rigidBody.AddForce(new Vector2(GetHorizForce(), GetVertForce()));
        CapVelocity();
        jump = false; // Reset the jump bool, since GetHorizForce and GetVertForce used it already
    }

    private float GetHorizForce()
    {
        float inputH = Input.GetAxis("Horizontal");
        switch (state)
        {
            case State.onGround:
                return inputH * movement.groundSpeed;
            case State.onWall:
                // Reverse the jump force depending on whether we're on a left or right wall
                return jump ? movement.wallJumpSpeed * Mathf.Sign((float)orientation - 180f) : 0f;
            case State.onCeiling:
                return inputH * movement.wallSpeed;
            case State.inAir:
                return inputH * movement.airSpeed;
        }
        return 0f;
    }

    private float GetVertForce()
    {
        float inputV = Input.GetAxis("Vertical");
        switch (state)
        {
            case State.onGround:
                return jump ? movement.wallJumpSpeed : 0f;
            case State.onWall:
                return inputV * movement.groundSpeed;
            case State.onCeiling:
                return jump ? -movement.wallJumpSpeed : 0f;
        }
        return 0f;
    }

    private void CapVelocity()
    {
        float cappedX = rigidBody.velocity.x;
        float cappedY = rigidBody.velocity.y;
        switch (state)
        {
            case State.onGround:
                // Math functions let the cap work in both directions
                if (Mathf.Abs(rigidBody.velocity.x) > movement.maxGroundSpeed)
                {
                    cappedX = Mathf.Sign(rigidBody.velocity.x) * movement.maxGroundSpeed;
                }
                break;
            case State.onWall:
                // Only upward movement is capped
                cappedY = Mathf.Min(cappedY, movement.maxWallSpeed);
                break;
            case State.onCeiling:
                // Math functions let the cap work in both directions
                if (Mathf.Abs(rigidBody.velocity.x) > movement.maxWallSpeed)
                {
                    cappedX = Mathf.Sign(rigidBody.velocity.x) * movement.maxWallSpeed;
                }
                break;
        }
        rigidBody.velocity = new Vector2(cappedX, cappedY);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == TERRAIN_LAYER)
        {
            string tag = other.gameObject.tag;

            // If we landed on a surface on something other than the wheels...
            if (!tag.Equals(surfacesByOrientation[orientation]))
            {
                Kill(); // Death
            }

            if (tag.Equals("Ground"))
                state = State.onGround; // We're on the ground
            else if (tag.Equals("Ceiling"))
                state = State.onCeiling; // We're on the ceiling
            else
                state = State.onWall; // We're on a wall
        }
    }

    void OnCollisionExit2D(Collision2D other)
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
