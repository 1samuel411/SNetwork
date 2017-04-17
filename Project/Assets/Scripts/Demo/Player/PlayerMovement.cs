using System.Collections;
using System.Collections.Generic;
using SNetwork;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float moveSpeed = 20;

    private new Rigidbody rigidbody;
    private NetworkObject networkObject;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        networkObject = GetComponent<NetworkObject>();
    }

    void Update()
    {
        if (networkObject.isMine == false)
        {
            return;
        }
        Vector2 movement = new Vector2();
        movement.x = Input.GetAxis("Horizontal");
        movement.y = Input.GetAxis("Vertical");

        rigidbody.AddForce(new Vector3(movement.x, 0, movement.y) * moveSpeed, ForceMode.VelocityChange);
    }
}
