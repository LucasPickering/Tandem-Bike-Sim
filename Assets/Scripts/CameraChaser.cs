using UnityEngine;
using System.Collections;

public class CameraChaser : MonoBehaviour
{

    [SerializeField]
    public GameObject target;

    void Update()
    {
        transform.position = target.transform.position - Vector3.forward;
    }
}
