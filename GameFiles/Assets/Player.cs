using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    // the animator which holds the player animations.
    public Animator _animator;
    [Header("Movement Settings")]
    // the movement speed of the player.
    public float speed;
    // the jump height of the player.
    public float jumpHeight;
    // the rotate speed of the player, used for lerping rotation.
    public float rotateSpeed;
    // player's current movement
    public float currentAcceleration;
    [Space]
    // collision detection stuff
    public Vector3 boxCastCenter;
    public Vector3 boxCastDimensions;

    public bool isGrounded
    {
        get
        {
            return Physics.BoxCastAll(boxCastCenter + transform.position, boxCastDimensions / 2F, Vector3.down, _graphics.rotation, 0.01F).Length != 0;
        }
    }


    [Header("Camera Settings")]
    // X camera sensitivity.
    public float lookX;
    // Y camera sensitivity.
    public float lookY;
    [Space]
    // the minimum Y rotation value of the camera.
    public float minY;
    // the maximum Y rotation value of the camera.
    public float maxY;

    // the angle at which the camera faces on the Y axis.
    private float cameraAngle;

    [Header("Abilities")]
    // whether the player can jump.
    public bool canJump;
    // whether the player can double jump.
    public bool canDoubleJump;
    // temp variable for handling double jumping.
    private bool doubleJump;
    // whether the player can dash.
    public bool canDash;
    // temp variable for handling dashing.
    public bool dash;
    // cooldown on the dash
    private float dashCooldown;

    // the last user input from the WASD buttons or the Left Analog Stick.
    Vector3 lastInput;

    // private variables that store components this object needs.
    // the player's rigidbody.
    Rigidbody _rigidbody;
    // the player camera
    Camera _camera;
    // the pivot for the camera, rotates only on the Y axis.
    Transform _pivot;
    // the panning object for the camera, rotates only on the X axis.
    Transform _pan;
    // the graphics, this is the player's Y rotation.
    Transform _graphics;

    // whether the player is currently jumping.
    bool isJumping = false;
    // whethe rthe player is currently falling.
    bool isFalling = false;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        _pivot = transform.Find("Pivot");
        _pivot.parent = null;

        _pan = _pivot.Find("Pan");

        _graphics = transform.Find("Graphics");

        _camera = _pan.Find("Camera").GetComponent<Camera>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
            _animator.SetTrigger("dab");

        if (Input.GetMouseButton(0))
            _animator.SetBool("punch", true);
        else
            _animator.SetBool("punch", false);

        Move();
        Jump();
        Dash();

        // reduces the dashCooldown over time.
        dashCooldown -= Time.deltaTime;

        if (isGrounded)
        {
            // resets the double jump
            if (!isJumping)
                doubleJump = false;

            // resets the dash.
            if (dashCooldown < 0F)
                dash = false;
        }

        if (transform.position.y < -99f)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void Dash()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) && (canDash && !dash))
        {
            _rigidbody.AddForce((_graphics.forward * 3F + transform.up * 1.5F) * Time.deltaTime * 200F, ForceMode.VelocityChange);
            dash = true;
            dashCooldown = 1F;
        }
    }

    private void Jump()
    {
        isFalling = (isJumping == false && isGrounded == false);

        _animator.SetBool("isJumping", isJumping);
        _animator.SetBool("isFalling", isFalling);

        if (canJump && Input.GetButtonDown("Jump") && (isGrounded || (doubleJump && canDoubleJump)))
        {
            _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, Mathf.Sqrt(2F * 9.81F * jumpHeight), _rigidbody.velocity.z);
            isJumping = true;

            doubleJump = !doubleJump;
        }

        if (_rigidbody.velocity.y < 0F)
        {
            isJumping = false;
            _rigidbody.mass = 4;
        }
        else
        {
            _rigidbody.mass = 1;
        }
    }

    private void Move()
    {
        // WASD & left stick input
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0F, Input.GetAxis("Vertical"));
        // arrow keys & right stick input
        Vector3 lookInput = new Vector3(Input.GetAxis("LookX"), 0F, Input.GetAxis("LookY"));

        // moves the player using its rigidbody and input.
        _rigidbody.MovePosition(transform.position + (_pivot.TransformDirection(input.normalized) * speed * currentAcceleration * (isJumping ? 2F : 1F) * Time.deltaTime));

        // reset the pivot position.
        _pivot.transform.position = transform.position;

        _animator.SetFloat("speed", currentAcceleration);

        // turning
        if (input.magnitude != 0 && dashCooldown < 0F)
        {
            lastInput = input;

            // increases the current acceleration.
            currentAcceleration += Time.deltaTime;

            transform.eulerAngles = new Vector3(0F, _pivot.eulerAngles.y, 0F);
            _graphics.localRotation = Quaternion.Lerp(_graphics.localRotation, Quaternion.LookRotation(lastInput), Time.deltaTime * rotateSpeed);
        }
        else
        {
            // decreases the current acceleration.
            currentAcceleration -= Time.deltaTime * 2F;
        }

        // clamps the current acceleration.
        currentAcceleration = Mathf.Clamp01(currentAcceleration);

        // camera & butt stuff
        cameraAngle += lookInput.x * lookX * Time.deltaTime;

        _pan.localEulerAngles = new Vector3(Mathf.Clamp(_pan.localEulerAngles.x + (-Time.deltaTime * lookInput.z * lookY), minY, maxY), 0F, 0F);
        _pivot.localEulerAngles = new Vector3(0F, cameraAngle, 0F);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.contacts[0].point.y <= transform.position.y)
        {
            //isGrounded = true;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position + boxCastCenter, boxCastDimensions);
    }
}

