using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed;
    public float jumpVelocity;
    public float rotateSpeed;
    [Header("Camera Settings")]
    public float lookX;
    public float lookY;
    [Space]
    public float minY;
    public float maxY;

    private float cameraAngle;

    Vector3 lastInput;

    Rigidbody _rigidbody;
    Camera _camera;
    Transform _pivot;
    Transform _pan;
    Transform _graphics;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        _pivot = transform.Find("Pivot");
        _pivot.parent = null;

        _pan = _pivot.Find("Pan");

        _graphics = transform.Find("Graphics");

        _camera = _pan.Find("Camera").GetComponent<Camera>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        // WASD & left stick input
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0F, Input.GetAxis("Vertical"));
        // arrow keys & right stick input
        Vector3 lookInput = new Vector3(Input.GetAxis("LookX"), 0F, Input.GetAxis("LookY"));

        // moves the player using its rigidbody and input.
        _rigidbody.MovePosition(transform.position + (_pivot.TransformDirection(input.normalized) * speed * Time.deltaTime));

        // reset the pivot position.
        _pivot.transform.position = transform.position;

        // jumping
        if (Input.GetButtonDown("Jump"))
            _rigidbody.AddForce(Vector3.up * jumpVelocity, ForceMode.Impulse);

        // turning
        if (input.magnitude != 0)
        {
            lastInput = input;
            transform.eulerAngles = new Vector3(0F, _pivot.eulerAngles.y, 0F);
            _graphics.localRotation = Quaternion.Lerp(_graphics.localRotation, Quaternion.LookRotation(lastInput), Time.deltaTime * rotateSpeed);
        }

        // camera stuff
        cameraAngle += lookInput.x * lookX * Time.deltaTime;

        _pan.localEulerAngles = new Vector3(Mathf.Clamp(_pan.localEulerAngles.x + (-Time.deltaTime * lookInput.z * lookY), minY, maxY), 0F, 0F);
        _pivot.localEulerAngles = new Vector3(0F, cameraAngle, 0F);
    }
}
