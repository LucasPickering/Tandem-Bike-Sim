using UnityEngine;
using System.Collections;

public class CameraChaser : MonoBehaviour
{

    [SerializeField]
    public GameObject target;

    void Update()
    {
		float x = target.transform.position.x;
		float y = target.transform.position.y;
		float z = transform.position.z;
		transform.position = new Vector3 (x, y, z);
    }
}
