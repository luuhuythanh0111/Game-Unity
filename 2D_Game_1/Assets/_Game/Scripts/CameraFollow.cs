using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform Target;
    public Vector3 Offset;
    public float speed = 20;

    // Start is called before the first frame update
    void Start()
    {
        Target = FindObjectOfType<Player>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(Target.position, Target.position + Offset, Time.deltaTime*speed);
    }
}
