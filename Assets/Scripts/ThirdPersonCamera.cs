using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float sensitive = 5f;
    float x;
    float y;
    Vector3 offset;

    void Start()
    {
        Vector3 position = new Vector3(target.position.x, target.position.y + 3.5f, target.position.z - 4);
        transform.position = position;
        transform.rotation = Quaternion.identity;
        offset = target.position - transform.position;
    }

    void LateUpdate()
    {
        if (target != null)
        {
            x -= Input.GetAxis("Mouse Y") * sensitive;
            y += Input.GetAxis("Mouse X") * sensitive;
            x = Mathf.Clamp(x, -30, 30);
            Quaternion rotation = Quaternion.Euler(x, y, 0);
            transform.position = target.position - (rotation * offset);
            transform.LookAt(target);
        }
    }

    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }
}
