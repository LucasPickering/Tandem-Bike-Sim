using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Passenger : MonoBehaviour {

	void OnCollisionEnter2D(Collision2D collision) {
		if (collision.gameObject.CompareTag ("Player")) {
			SceneManager.LoadScene("Victory"); // Load the victory screen
		}
	}
}
