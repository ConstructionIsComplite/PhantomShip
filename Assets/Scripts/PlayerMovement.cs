using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float rotationSpeed = 10f;
    [SerializeField] float acceleration = 15f;
    [SerializeField] float deceleration = 20f;
    [SerializeField] float maxSpeed = 7f;

    [Header("References")]
    [SerializeField] Camera playerCamera;
    [SerializeField] LayerMask groundLayer;

    [Header("Debug")]
    [SerializeField] Vector3 currentVelocity; // ��� ������������ � ����������

    [Header("Animation")]
    [SerializeField] Animator animator;

    public Vector3 CurrentVelocity => rb.velocity;

    Rigidbody rb;
    Vector3 movement;
    Vector3 targetVelocity;
    Plane groundPlane;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        //groundPlane = new Plane(Vector3.up, Vector3.zero);
    }

    void Update()
    {
        HandleRotation();
    }

    void FixedUpdate()
    {
        HandleMovement();
        currentVelocity = rb.velocity; // ��������� ��� �������
    }

    void HandleMovement()
    {
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        // ����������� �������� ������������ ������
        Vector3 cameraForward = playerCamera.transform.forward;
        Vector3 cameraRight = playerCamera.transform.right;
        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraForward.Normalize();
        cameraRight.Normalize();

        // ������������ ������� ��������
        Vector3 desiredDirection = cameraForward * input.z + cameraRight * input.x;
        targetVelocity = Vector3.MoveTowards(
            targetVelocity,
            desiredDirection * maxSpeed,
            (input.magnitude > 0.1f ? acceleration : deceleration) * Time.fixedDeltaTime);

        // ��������� ��������
        rb.velocity = new Vector3(
            targetVelocity.x,
            rb.velocity.y, // ��������� ������������ �������� ��� ����������
            targetVelocity.z
        );

        float speedNormalized = rb.velocity.magnitude / maxSpeed;
        animator.SetFloat("Speed", speedNormalized);
        animator.SetBool("Aiming", speedNormalized < 0.1f); // Aiming = Idle

    }

    void HandleRotation()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayer))
        {
            Vector3 lookDirection = hit.point - transform.position;
            lookDirection.y = 0;

            // ������� ������� � ������������ ������� ��������
            float angleDiff = Vector3.SignedAngle(transform.forward, lookDirection, Vector3.up);
            float rotationStep = Mathf.Sign(angleDiff) * Mathf.Min(
                Mathf.Abs(angleDiff),
                rotationSpeed * Time.deltaTime
            );

            rb.MoveRotation(rb.rotation * Quaternion.Euler(0, rotationStep, 0));
        }
    }
}
