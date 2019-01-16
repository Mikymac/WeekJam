using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    public Animator _animator;
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
    Vector3 _startPos;

    public bool isGrounded;
    float halfheight = 0.8F;
    RaycastHit ray;
    public Vector2 impulseForce;
    bool isJumping = false;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        _pivot = transform.Find("Pivot");
        _pivot.parent = null;

        _pan = _pivot.Find("Pan");

        _graphics = transform.Find("Graphics");

        _camera = _pan.Find("Camera").GetComponent<Camera>();
        _startPos = transform.position;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
            _animator.SetTrigger("dab");
    }

    private void FixedUpdate()
    {
        Move();
        Jump();

        if (transform.position.y < -99f)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, Mathf.Sqrt(2F * 9.81F * jumpVelocity), _rigidbody.velocity.z);
            isJumping = true;
        }

        if (_rigidbody.velocity.y < 0F) // COLONS ARE FULL OF SHIT
            _rigidbody.mass = 4;
        else
            _rigidbody.mass = 1;

        isGrounded = false;
    }

    private void Move()
    {
        // WASD & left stick input
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0F, Input.GetAxis("Vertical"));
        // arrow keys & right stick input
        Vector3 lookInput = new Vector3(Input.GetAxis("LookX"), 0F, Input.GetAxis("LookY"));

        // moves the player using its rigidbody and input.
        _rigidbody.MovePosition(transform.position + (_pivot.TransformDirection(input.normalized) * speed * (isJumping ? 2F : 1F) * Time.deltaTime));

        // reset the pivot position.
        _pivot.transform.position = transform.position;

        _animator.SetFloat("speed", input.magnitude);

        // turning
        if (input.magnitude != 0)
        {
            lastInput = input;
            transform.eulerAngles = new Vector3(0F, _pivot.eulerAngles.y, 0F);
            _graphics.localRotation = Quaternion.Lerp(_graphics.localRotation, Quaternion.LookRotation(lastInput), Time.deltaTime * rotateSpeed);
        }

        // camera & butt stuff
        cameraAngle += lookInput.x * lookX * Time.deltaTime;

        _pan.localEulerAngles = new Vector3(Mathf.Clamp(_pan.localEulerAngles.x + (-Time.deltaTime * lookInput.z * lookY), minY, maxY), 0F, 0F);
        _pivot.localEulerAngles = new Vector3(0F, cameraAngle, 0F);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.contacts[0].point.y <= transform.position.y)
        {
            isGrounded = true;
        }
    }
}

