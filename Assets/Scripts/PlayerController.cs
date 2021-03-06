﻿using UnityEngine;
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
		up = 0,
		right = 270,
		left = 90}

	;

	private enum State
	{
		onGround,
		onWall,
		inAir}

	;

	private const int TERRAIN_LAYER = 8;

	public float deathZone;
	public float raycastSourceOffset;
	public float raycastDistance;
	public int maxHp;
	public MovementValues movement;
	[SerializeField]
	public GameObject dustPrefab;

	private Rigidbody2D rigidBody;
	private Animator animator;
	private Orientation orientation;
	private State state = State.inAir;
	private bool jump;
	private int currentHp;

	void Start ()
	{
		rigidBody = GetComponent<Rigidbody2D> ();
		animator = GetComponent<Animator> ();
		currentHp = maxHp;
	}

	void Update ()
	{
		if (Input.GetButtonDown ("Jump")) {
			jump = true;
		}
	}

	private void RotateTo (Orientation or)
	{
		orientation = or;
		transform.rotation = Quaternion.Euler (new Vector3 (0f, 0f, (float)or));
	}

	void FixedUpdate ()
	{
		// Kill if we're down too far
		if (transform.position.y < deathZone) {
			Kill ();
		}

		SetState (); // Set the player's state (on the ground or in the air)

		/*
		// Rotate the player according to the input
		float angularVel = rigidBody.angularVelocity;
		float torque = Input.GetAxisRaw ("Rotation") * movement.rotationSpeed;
		if (Mathf.Abs (angularVel) < movement.maxRotationSpeed || Mathf.Sign (angularVel) != Mathf.Sign (torque)) {
			rigidBody.AddTorque (torque);
		}
		*/

		// Rotate the player
		if (Mathf.Abs(Input.GetAxisRaw ("Rotation")) > 0) {
			rigidBody.MoveRotation (rigidBody.rotation + Input.GetAxisRaw ("Rotation") * movement.rotationSpeed);
		}

		rigidBody.AddRelativeForce (new Vector2 (GetHorizForce (), GetVertForce ()));
		//rigidBody.MovePosition (rigidBody.position + new Vector2 (GetHorizForce (), GetVertForce ()));
		CapVelocity ();
		animator.SetFloat ("Speed", GetPedalSpeed ());
		jump = false; // Reset the jump bool, since GetHorizForce and GetVertForce used it already
	}

	private float GetHorizForce ()
	{
		float input = Input.GetAxisRaw ("Horizontal");
		switch (state) {
		case State.onGround:
			return input * movement.groundSpeed;
		case State.onWall:
                // Reverse the jump force depending on whether we're on a left or right wall
			return jump ? movement.wallJumpSpeed * Mathf.Sign ((float)orientation - 180f) : 0f;
		case State.inAir:
			return input * movement.airSpeed;
		}
		return 0f;
	}

	private float GetVertForce ()
	{
		float input = Input.GetAxisRaw ("Horizontal");
		switch (state) {
		case State.onGround:
			return jump ? movement.groundJumpSpeed : 0f;
		case State.onWall:
			return input * movement.wallSpeed;
		}
		return 0f;
	}

	private void CapVelocity ()
	{
		float cappedX = rigidBody.velocity.x;
		float cappedY = rigidBody.velocity.y;
		switch (state) {
		case State.onGround:
                // Math functions let the cap work in both directions
			if (Mathf.Abs (rigidBody.velocity.x) > movement.maxGroundSpeed) {
				cappedX = Mathf.Sign (rigidBody.velocity.x) * movement.maxGroundSpeed;
			}
			break;
		case State.onWall:
                // Only upward movement is capped
			cappedY = Mathf.Min (cappedY, movement.maxWallSpeed);
			break;
		}
		rigidBody.velocity = new Vector2 (cappedX, cappedY);
	}

	private float GetPedalSpeed ()
	{
		Vector2 velocity = rigidBody.velocity;
		return state == State.onWall ? velocity.y : velocity.x;
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.gameObject.layer == TERRAIN_LAYER) {
			Kill (); // Only the box collider for the rider is a trigger. If it hits anything, the player dies
		}
	}

	// Sets the state to onGround or inAir via raycasting
	void SetState ()
	{
		Vector2 leftVector = RotateAroundPivot (new Vector2 (transform.position.x - raycastSourceOffset, transform.position.y), transform.position, transform.rotation);
		Vector2 rightVector = RotateAroundPivot (new Vector2 (transform.position.x + raycastSourceOffset, transform.position.y), transform.position, transform.rotation);

		RaycastHit2D hitLeft = Physics2D.Raycast (leftVector, transform.rotation * Vector2.down, raycastDistance, 1 << TERRAIN_LAYER);
		RaycastHit2D hitRight = Physics2D.Raycast (rightVector, transform.rotation * Vector2.down, raycastDistance, 1 << TERRAIN_LAYER);
		state = hitLeft.collider != null || hitRight.collider != null ? State.onGround : State.inAir;
	}

	/*
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == TERRAIN_LAYER)
        {
            //state = other.gameObject.CompareTag("Ground") ? State.onGround : State.onWall;
			state = State.onGround;
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
    */

	private void Kill ()
	{
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
	}

	private void MakeDust (Vector2 pos)
	{
		Instantiate (dustPrefab, pos, Quaternion.identity);
	}

	public static Vector2 RotateAroundPivot (Vector3 point, Vector3 pivot, Quaternion rotation)
	{
		return rotation * (point - pivot) + pivot;
	}

	public int getCurrentHp ()
	{
		return currentHp;
	}

	public int getMaxHp ()
	{
		return maxHp;
	}

	public void hurt (int damage)
	{
		currentHp = Mathf.Max (currentHp - damage, 0); // Don't let it go below 0
		if (currentHp <= 0) {
			Kill ();
		}
	}
}
