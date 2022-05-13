using UnityEngine;

public class PlayerController : MonoBehaviour
{

    CharacterController controller;
    Camera playerCamera;
    public float speed = 10f;
    public float mouseSensitivity = 10f;
    [Range(0, 90)]
    public float turnLimit;
    private float xrotation = 0, yrotation = 0;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
    }

    private void OnEnable() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    private void OnDisable() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        xrotation = Mathf.Clamp(xrotation-Input.GetAxis("Mouse Y")*mouseSensitivity, -turnLimit, turnLimit);
        yrotation += Input.GetAxis("Mouse X")*mouseSensitivity;
        Vector3 move = transform.right * x + transform.forward * z;
        controller.SimpleMove(move*speed);

        transform.Rotate(0, Input.GetAxis("Mouse X")*mouseSensitivity, 0);
        playerCamera.transform.rotation = Quaternion.Euler(xrotation, yrotation, 0);

    }
}
