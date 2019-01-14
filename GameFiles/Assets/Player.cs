using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed;

    [Header("Camera Settings")]
    public Vector3 cameraOffset;
    public Vector3 cameraRotation;

    Vector3 lastInput = new Vector3(0, 0, 1);

    Rigidbody _rigidbody;
    Camera _camera;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        _camera = transform.Find("Camera").GetComponent<Camera>();
        _camera.transform.parent = null;
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0F, Input.GetAxis("Vertical"));
        _rigidbody.MovePosition(transform.position + (input.normalized * speed * Time.deltaTime));

        _camera.transform.position = transform.position + cameraOffset;
        _camera.transform.eulerAngles = cameraRotation;

        if (Input.GetKeyDown(KeyCode.Space))
            _rigidbody.AddForce(Vector3.up * 14F, ForceMode.Impulse);

        if (input.magnitude != 0F)
            lastInput = (input.normalized * 10F);

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lastInput), Time.deltaTime * 5F);
    }
}
