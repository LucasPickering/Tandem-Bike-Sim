using UnityEngine;
using System.Collections;

public class HealthBarHandler : MonoBehaviour
{

	[SerializeField]
	public GameObject player;
	private float hpToPixelRatio;

	private PlayerController playerController;

	// Use this for initialization
	void Start ()
	{
		playerController = player.GetComponent<PlayerController> ();
		hpToPixelRatio = ((RectTransform)transform).rect.width;
	}
	
	// Update is called once per frame
	void Update ()
	{
		RectTransform rectTrans = (RectTransform)transform;
		float height = rectTrans.rect.height;
		rectTrans.sizeDelta = new Vector2 (playerController.getCurrentHp () / playerController.getMaxHp () * hpToPixelRatio, height);
	}
}
