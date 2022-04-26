using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    CharacterController controller;
    Camera playerCamera;
    public float speed = 10f;
    public float mouseSensitivity = 10f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;   
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move*speed*Time.deltaTime);
        transform.Rotate(0, Input.GetAxis("Mouse X")*mouseSensitivity, 0);
        playerCamera.transform.Rotate(
            -Input.GetAxis("Mouse Y")*mouseSensitivity, 
            0, 0);

    }
}
