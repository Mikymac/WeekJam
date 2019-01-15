using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jump : MonoBehaviour {
    [Header("Movement Settings")]
    public float speed;

    [Header("Camera Settings")]
    public Vector3 cameraOffset;
    public Vector3 cameraRotation;

    Vector3 lastInput = new Vector3(0, 0, 1);

    Rigidbody _rigidbody;
    Camera _camera;

    // Sams variables
    public bool isGrounded;
    public float halfheight;
    RaycastHit ray;
    public Vector2 impulseForce;
    public bool isJumping = false;

    public Transform cube;
    // End sams variables

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        _camera = transform.Find("Camera").GetComponent<Camera>();
        _camera.transform.parent = null;
    }

    private void FixedUpdate()
    {
        Move();

        if (_rigidbody.mass == 20 && isGrounded)
        {
            _rigidbody.mass = 2;
        }

        if (!isGrounded && _rigidbody.velocity.y <= -0.05f)
        {
            isJumping = false;
            _rigidbody.mass = 20;           
        }

        isGrounded = true;
        if (Physics.Raycast(transform.position + (Vector3.up * halfheight),Vector3.down,out ray))
        {
            if (!ray.transform || ray.transform && (ray.transform != transform.GetChild(0) && Vector3.Distance(transform.position, ray.point) > 0.81f))
            {
                isGrounded = false;
                isJumping = false;
            }
        }
    }

    private void Jump()
    {

    }


    private void Move()
    {
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0F, Input.GetAxis("Vertical"));
        _rigidbody.MovePosition(transform.position + (input.normalized * speed * Time.deltaTime));

        _camera.transform.position = transform.position + cameraOffset;
        _camera.transform.eulerAngles = cameraRotation;

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            _rigidbody.AddForce(Vector3.up * impulseForce.y, ForceMode.Impulse);
            _rigidbody.AddForce(transform.forward * impulseForce.x, ForceMode.Impulse);
            isJumping = true;
            _rigidbody.mass = 2;
        }

        if (input.magnitude != 0F)
            lastInput = (input.normalized * 10F);

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lastInput), Time.deltaTime * 5F);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, halfheight / 2);
    }
}
