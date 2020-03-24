using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public Transform self, player;
    public PlayerController controller;
    public float cameraSpeed = 0.3f;
    public float cameraBackSpeed = 0.3f;
    public float maxDelta = 1;
    float targetPosition;
    float leftDelta;

    void Start()
    {
        leftDelta = 0;
    }

    void Update()
    {
        if (controller.movementVector.x > 0)
            leftDelta += Mathf.Abs(controller.movementVector.x) * cameraSpeed;
        
        else if (controller.movementVector.x < 0)
            leftDelta -= Time.deltaTime * cameraBackSpeed;

        leftDelta = Mathf.Clamp(leftDelta, 0, maxDelta);

        targetPosition = player.position.x + leftDelta;

       
        self.position = new Vector3(targetPosition, self.position.y, self.position.z);
    }
}
