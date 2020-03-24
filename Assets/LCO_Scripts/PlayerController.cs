using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Ground Movement")]
    public float speed = 5f;
    public float runMultiplier = 2f;
    public float accelerationTime = 0.5f;
    public AnimationCurve acceleration = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("Vertical Movement")]    
    public float gravity = 9.81f;
    public AnimationCurve jumpCurve = AnimationCurve.Constant(0,1,1);
    public float jumpReleaseMultiplier = 3f;
    public int maxJumpsAllowed = 1;

    [Header("Physics")]
    public float movementThreshold = 0.0015f;

    [Header("References")]
    public Transform self;
    public CharacterRaycaster raycaster;
    public Transform graphicsTransform;
    public SpriteRenderer sprite;
    public Animator animator;

    [System.NonSerialized]
    public bool leftKeyDown, leftKey, leftKeyUp,
        rightKeyDown, rightKey, rightKeyUp,
        jumpKeyUp, jumpKey, jumpKeyDown,
        actionKeyUp, actionKey, actionKeyDown;

    [Header("Debug")]
    public Transform fakeGroundLevel;
    public bool debugMode;

    // Getters
    [System.NonSerialized] public Vector2 movementVector;
    public bool isGrounded { get { return raycaster.flags.below; } }

    bool isJumping;
    int jumpsAllowedLeft;
    float timeSinceJumped, timeSinceAccelerated;

    void Start()
    {
        timeSinceJumped = 10f;
        jumpsAllowedLeft = maxJumpsAllowed;
    }

    void Update()
    {
        InputUpdate();
        JumpUpdate();
        MovementUpdate();
        PostMovementJumpUpdate();
        DebugUpdate();
        AnimationUpdate();
    }

    void InputUpdate()
    {
        //Reset movement
        movementVector = Vector2.zero;

        //Check if player make horizontal movement
        if (leftKey)
            movementVector.x--;
        else if (rightKey)
            movementVector.x++;

        //Jump detection
        if (jumpKeyDown)
            TryJump();
    }

    void JumpUpdate()
    {
        // par défaut, la gravité s'applique simplement
        movementVector.y = gravity * -1f;

        // la suite ne concerne que le saut
        if (!isJumping)
            return;

        // si on release la touche, le saut se finira plus vite
        float releaseMultiplier = jumpKey ? 1 : jumpReleaseMultiplier;

        // tenir trace de "depuis combien de temps le saut a commencé"
        timeSinceJumped += Time.deltaTime * releaseMultiplier;

        // mesurer le modificateur de gravité, et l'appliquer
        float gravityMultiplier = jumpCurve.Evaluate(timeSinceJumped);
        movementVector.y *= gravityMultiplier * -1;
        Debug.Log("MY : " + movementVector.y);

        // à la fin de la courbe, le saut est fini
        if (timeSinceJumped > jumpCurve.keys[jumpCurve.keys.Length-1].time)
            isJumping = false;
    }

    void PostMovementJumpUpdate()
    {
        // si on touche le sol, le saut est fini aussi
        if (isGrounded)
        {
            isJumping = false;
            jumpsAllowedLeft = maxJumpsAllowed;
        }
    }

    void MovementUpdate()
    {
        if (movementVector.x == 0)
            timeSinceAccelerated = 0;
        else
            timeSinceAccelerated += Time.deltaTime;

        float accelerationMultiplier = 1;
        if (accelerationTime > 0)
            accelerationMultiplier = acceleration.Evaluate(timeSinceAccelerated/accelerationTime);
        float currentRunMultiplier = actionKey ? runMultiplier : 1;

        float usedSpeed = speed * accelerationMultiplier * currentRunMultiplier;
        movementVector.x *= usedSpeed;

        Vector3 finalVector = Time.deltaTime * movementVector;
        
        Move(finalVector);
    }

    void DebugUpdate()
    {
        if (!debugMode) return;

        if (self.position.y <= fakeGroundLevel.position.y)
        {
            //isGrounded = true;
            jumpsAllowedLeft = maxJumpsAllowed;
            self.position = new Vector3(self.position.x, fakeGroundLevel.position.y, self.position.z);
        }
    }

    void AnimationUpdate()
    {
        animator.SetBool("IsMoving", movementVector.x != 0);
        animator.SetBool("IsGrounded", isGrounded);

        if (movementVector.x != 0)
            graphicsTransform.localScale = new Vector3(
                Mathf.Abs(graphicsTransform.transform.localScale.x) * Mathf.Sign(movementVector.x),
                graphicsTransform.transform.localScale.y,
                graphicsTransform.transform.localScale.z
            );
    }

    void TryJump()
    {
        if (!isGrounded)
            return;
        if (jumpsAllowedLeft == 0)
            return;
        jumpsAllowedLeft--;
        StartJump();
    }

    void StartJump()
    {
        isJumping = true;
        //isGrounded = false;
        timeSinceJumped = 0f;
        animator.SetTrigger("Jump");

    }

    void Move(Vector3 movement)
    {
        if (movement.x != 0)
            movement.x = raycaster.CastBoxHorizontal(movement.x);
        if (Mathf.Abs(movement.x) < movementThreshold)
            movement.x = 0;

        if (movement.y != 0)
            movement.y = raycaster.CastBoxVertical(movement.y);
        if (Mathf.Abs(movement.y) < movementThreshold)
            movement.y = 0;

        if (movement.x > 0) raycaster.flags.left = false;
        if (movement.x < 0) raycaster.flags.right = false;
        if (movement.y > 0) raycaster.flags.below = false;
        if (movement.y < 0) raycaster.flags.above = false;

        self.Translate(movement);
    }
}
