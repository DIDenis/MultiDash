using UnityEngine;
using Mirror;
using System;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] ThirdPersonCamera playerCamera;
    [SerializeField] Collider dash;
    [SerializeField] float speed = 5f; 
    [SerializeField] float dashDistance = 10f;
    [SerializeField] float dashImpulse = 50f;
    CharacterController characterController;
    CapsuleCollider capsuleCollider;
    Animator animator;
    Action action;
    float dashTimer;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        dash.enabled = false;
        characterController.enabled = false;
        capsuleCollider.enabled = true;
        playerCamera.SetActive(false);
    }

    public override void OnStartLocalPlayer()
    {
        characterController.enabled = true;
        capsuleCollider.enabled = false;
        playerCamera.SetActive(true);
        action = Move;
    }
    
    [ClientRpc]
    public void RpcStopMatch()
    {
        if (isLocalPlayer)
        {
            characterController.enabled = false;
            this.enabled = false;
            animator.SetBool("dash", false);
        }
    }

    [ClientRpc]
    public void RpcRestartPlayer(Vector3 position)
    {
        if (isLocalPlayer)
        {
            transform.position = position;
            transform.rotation = Quaternion.identity;
            characterController.enabled = true;
            this.enabled = true;
        }
    }
    
    [ClientCallback]
    void Update()
    {
        if (isLocalPlayer)
            action();
    }

    void Move()
    {
        Vector3 move = Vector3.zero;
        Vector3 right = transform.right * Input.GetAxis("Horizontal");
        Vector3 forward = transform.forward * Input.GetAxis("Vertical");
        move = (right + forward) * speed;
        move = Vector3.ClampMagnitude(move, speed);
        if (move.magnitude > 0)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation, TargetRotation(), Time.smoothDeltaTime * 15f
            );
        }
        move *= Time.smoothDeltaTime;
        characterController.Move(move);

        animator.SetFloat("x", Input.GetAxis("Horizontal"));
        animator.SetFloat("y", Input.GetAxis("Vertical"));

        if (Input.GetMouseButtonDown(0))
        {
            action = StartDash;
            animator.SetFloat("x", 0);
            animator.SetFloat("y", 0);
        }
    }

    void StartDash()
    {
        if (isServer)
            SetDash(true);
        else
            CmdSetDash(true);
        animator.SetBool("dash", true);
        dashTimer = Time.time;
        transform.rotation = TargetRotation();
        action = Dash;
    }
    void Dash()
    {
        if (dashTimer + dashDistance / dashImpulse > Time.time)
        {
            Vector3 dash = transform.forward * dashImpulse;
            dash *= Time.smoothDeltaTime;
            characterController.Move(dash);
        }
        else
            action = EndDash;
    }
    void EndDash()
    {
        if (isServer)
            SetDash(false);
        else
            CmdSetDash(false);
        animator.SetBool("dash", false);
        action = Move;
    }

    [Server] 
    public void SetDash(bool newValue)
    {
        dash.enabled = newValue;
    }
    [Command]
    public void CmdSetDash(bool newValue)
    {
        SetDash(newValue);
    }

    Quaternion TargetRotation()
    {
        Quaternion rotation = Quaternion.LookRotation(playerCamera.transform.forward);
        rotation.x = 0;
        rotation.z = 0;
        return rotation;
    }
}
