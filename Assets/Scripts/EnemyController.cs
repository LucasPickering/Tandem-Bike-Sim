using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{

	public float lookRange;
	public float followRange;
	public float stopRange;
	public float xSpeed;
	public float ySpeed;
	public float hurtPause;

	private Transform target;
	private bool facingRight;
	private float timeSinceHurt;

	void Start ()
	{
		target = GameObject.FindWithTag ("Player").transform;
		timeSinceHurt = hurtPause;
	}

	void FixedUpdate ()
	{
		float distance = Vector2.Distance (transform.position, target.position);
		if (distance <= lookRange) {
			bool facingRight = transform.position.x < target.position.x;
			transform.localRotation = Quaternion.Euler (0, facingRight ? 180 : 0, 0);
		}

		if (distance <= followRange) {
			Vector2 diff = target.position - transform.position;
			float xMoveDist = Mathf.Abs (diff.x) > stopRange ? Time.fixedDeltaTime * xSpeed * Mathf.Sign (diff.x) : 0;
			float yMoveDist = Mathf.Abs (diff.y) > stopRange ? Time.fixedDeltaTime * ySpeed * Mathf.Sign (diff.y) : 0;
			transform.position += new Vector3 (xMoveDist, yMoveDist, 0);
		}

		if (timeSinceHurt <= hurtPause) {
			timeSinceHurt += Time.fixedDeltaTime;
		}
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.gameObject.CompareTag ("Player")) {
			other.gameObject.GetComponent<PlayerController> ().hurt (1);
		}
	}
}
