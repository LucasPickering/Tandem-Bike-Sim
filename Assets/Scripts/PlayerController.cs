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

        public float rotationSpeed;
        public float maxRotationSpeed;
    }

    private enum Orientation
    {
        up = 0, right = 270, left = 90
    };

    private enum State
    {
        onGround, onWall, inAir
    };

    private const int TERRAIN_LAYER = 8;

    public MovementValues movement;
    private Rigidbody2D rigidBody;
    private Animator animator;

    [SerializeField]
    public GameObject dustPrefab;
    private Vector3 dustPos1 = new Vector3(-0.235f, -0.23f, 0f);
    private Vector3 dustPos2 = new Vector3(0.235f, -0.23f, 0f);

    private Orientation orientation;
    private State state = State.inAir;
    private bool jump;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }
    }

    private void RotateTo(Orientation or)
    {
        orientation = or;
        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, (float)or));
    }

    void FixedUpdate()
    {
        //Debug.Log("State: " + state + ", " + new Vector2(GetHorizForce(), GetVertForce()));
        // Rotate the player according to the input
        if(rigidBody.angularVelocity < movement.maxRotationSpeed) {
            rigidBody.AddTorque(Input.GetAxisRaw("Rotation") * movement.rotationSpeed);
        }

        rigidBody.AddRelativeForce(new Vector2(GetHorizForce(), GetVertForce()));
        CapVelocity();
        animator.SetFloat("Speed", GetPedalSpeed());
        jump = false; // Reset the jump bool, since GetHorizForce and GetVertForce used it already
    }
    private float GetHorizForce()
    {
        float input = Input.GetAxisRaw("Horizontal");
        switch (state)
        {
            case State.onGround:
                return input * movement.groundSpeed;
            case State.onWall:
                // Reverse the jump force depending on whether we're on a left or right wall
                return jump ? movement.wallJumpSpeed * Mathf.Sign((float)orientation - 180f) : 0f;
            case State.inAir:
                return input * movement.airSpeed;
        }
        return 0f;
    }

    private float GetVertForce()
    {
        float input = Input.GetAxisRaw("Horizontal");
        switch (state)
        {
            case State.onGround:
                return jump ? movement.groundJumpSpeed : 0f;
            case State.onWall:
                return input * movement.wallSpeed;
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
        }
        rigidBody.velocity = new Vector2(cappedX, cappedY);
    }

    private float GetPedalSpeed()
    {
        Vector2 velocity = rigidBody.velocity;
        return state == State.onWall ? velocity.y : velocity.x;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Kill(); // Only the box collider is a trigger. If it hits anything, the player dies
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == TERRAIN_LAYER)
        {
            state = other.gameObject.CompareTag("Ground") ? State.onGround : State.onWall;
            MakeDust(other.contacts[0].point); // Make dust particles
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.layer == TERRAIN_LAYER)
        {
            state = State.inAir;
        }
    }

    private void Kill()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void MakeDust(Vector2 pos)
    {
        Instantiate(dustPrefab, pos, Quaternion.identity);
    }
}
