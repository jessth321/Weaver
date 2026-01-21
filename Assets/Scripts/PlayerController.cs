using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    public List<Enemy> caughtEnemies = new List<Enemy>();

    Rigidbody2D rb;
    CapsuleCollider2D cc;
    Animator anim;
    Player player;

    Vector2 moveDirection;
    Transform spriteTransform;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [SerializeField] PhysicsMaterial2D physicsMat;
    [SerializeField] float speed;
    [SerializeField] float jumpPower;
    [SerializeField] float jumpTime;
    [SerializeField] float chargeJumpTime;
    [SerializeField] float chargePerfectWindow;
    [SerializeField] AnimationCurve jumpCurve;
    [SerializeField] AnimationCurve jumpPowerCurve;
    [SerializeField] JumpArrow jumpArrow;
    [SerializeField] GameObject dustExplosionPrefab;

    [SerializeField] Transform caughtTransform;
    [SerializeField] Transform eatingTransform;

    [SerializeField] float jumpAimSpeed;
    [SerializeField] float jumpAimAngle;

    Vector2 mouseDelta;

    bool isGrounded;
    bool wasGrounded;
    bool isHanging;
    bool tryJump;
    bool isChargingJump;
    bool wasChargingJump;
    bool wasJumping;
    bool isJumping;
    Vector2 jumpDir;
    float jumpTimer;
    float chargeJumpTimer;
    bool isFacingRight = true;
    bool isEating;
    [SerializeField] float eatTime;
    float eatTimer;
    bool isAttacking;


    Vector2 groundNormal;



    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cc = GetComponent<CapsuleCollider2D>();

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteTransform = spriteRenderer.transform;
        anim = GetComponentInChildren<Animator>();

        player = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D[] hits = Physics2D.CapsuleCastAll(new Vector2(transform.position.x + cc.offset.x, transform.position.y + cc.offset.y), cc.size + Vector2.one * 0.05f, cc.direction, 0, Vector2.down, 0.01f);

        //List<Vector2> normals = new List<Vector2>();
        //List<Vector2> offsets = new List<Vector2>();

        if (isChargingJump)
        {
            cc.enabled = false;
            rb.isKinematic = true;
        }
        else
        {
            cc.enabled = true;
            rb.isKinematic = false;
        }

        if (!Mathf.Approximately(moveDirection.x, 0) && !isChargingJump)
        {
            if (moveDirection.x > 0)
            {
                isFacingRight = true;
            }
            else
            {
                isFacingRight = false;
            }
        }

        if (isFacingRight)
        {
            groundNormal = spriteTransform.right;
        }
        else
        {
            groundNormal = -spriteTransform.right;
        }
        bool groundFlag = false;

        isGrounded = false;
        rb.gravityScale = 5;

        Vector2 localGroundPoint = Vector2.zero;

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].transform.CompareTag("Ground"))
            {
                isGrounded = true;
                rb.gravityScale = 0;

                Vector2 localPoint = hits[i].point - new Vector2 (transform.position.x, transform.position.y);


                //normals.Add(hits[i].normal);
                //offsets.Add(hits[i].point - new Vector2(transform.position.x, transform.position.y));
                if (!groundFlag || Vector2.Dot(localGroundPoint, isFacingRight ? spriteTransform.right : -spriteTransform.right) < Vector2.Dot(localPoint, isFacingRight ? spriteTransform.right : -spriteTransform.right))
                {
                    groundNormal = hits[i].normal;
                    localGroundPoint = localPoint;
                    groundFlag = true;
                }


                //Debug.DrawRay(hits[i].point, hits[i].normal, Color.red);
            }
        }

        //Debug.DrawRay(transform.position, groundNormal, Color.green);

        if (!groundFlag)
        {
            groundNormal = Vector2.zero;
        }
        //groundNormal = sumNormal.normalized;
        isHanging = false;
        if (groundNormal != Vector2.zero && Vector2.Dot(Vector2.down, groundNormal) > 0f)
        {
            isHanging = true;


        }


        if (isGrounded)
        {
            spriteTransform.rotation = Quaternion.LookRotation(Vector3.forward, groundNormal);
        }
        else
        {

            if (isJumping && jumpTimer < jumpTime)
            {
                spriteTransform.rotation = Quaternion.LookRotation(Vector3.forward, rb.velocity.normalized);
            }
            else
            {
                spriteTransform.rotation = Quaternion.LookRotation(Vector3.forward, Vector2.up);
            }
        }

        if (isGrounded && wasGrounded && caughtEnemies.Count > 0)
        {
            isEating = true;
        }
        else
        {
            isEating = false;
            eatTimer = 0;
        }

        if (isEating)
        {

            for (int i = 0; i < caughtEnemies.Count; i++)
            {
                caughtEnemies[i].transform.position = eatingTransform.position + Quaternion.LookRotation(Vector3.forward, new Vector2((i * (i * 217.4f)), (i * (i * 517.4f)))) * new Vector3(1, 1, 0) * i * 0.1f;
                caughtEnemies[i].transform.rotation = spriteTransform.rotation;
            }
        }

        if (!isGrounded && caughtEnemies.Count > 0)
        {
            for (int i = 0; i < caughtEnemies.Count; i++)
            {
                caughtEnemies[i].transform.position = caughtTransform.position + Quaternion.LookRotation(Vector3.forward, new Vector2((i * (i * 217.4f)), (i * (i * 517.4f)))) * new Vector3(1, 1, 0) * i * 0.1f;
            }
        }

        //Debug.Log(mouseDelta);

        /*Debug.Log("IsGrounded: " + isGrounded.ToString()
            + "\nIsJumping: " + isJumping.ToString()
            + "\nIsCharging: " + isChargingJump.ToString()
            + "\nGroundNormal: " + groundNormal.ToString());*/
    }

    private void FixedUpdate()
    {
        if (isEating)
        {
            rb.velocity = Vector2.zero;



            if (eatTimer < eatTime)
            {
                eatTimer += Time.fixedDeltaTime;
            }
            else
            {
                player.ConsumeCatch();

                isEating = false;
                eatTimer = 0;
            }
        }
        else
        {
            if (isGrounded && tryJump)
            {
                isChargingJump = true;
            }
            else
            {
                isChargingJump = false;
            }

            if (isChargingJump && !wasChargingJump)
            {
                jumpDir = spriteTransform.up;
                chargeJumpTimer = 0;
                jumpArrow.gameObject.SetActive(true);
            }


            if (isChargingJump)
            {
                chargeJumpTimer += Time.fixedDeltaTime;

                jumpArrow.transform.rotation = Quaternion.LookRotation(Vector3.forward, jumpDir);

                jumpArrow.jumpPower01 = Mathf.Clamp01(chargeJumpTimer / chargeJumpTime);

                if (moveDirection.x > 0.1f)
                {
                    jumpDir = Vector2.Lerp(jumpDir, Quaternion.AngleAxis(-jumpAimAngle, Vector3.forward) * spriteTransform.up, jumpAimSpeed * Time.fixedDeltaTime);
                }
                else if (moveDirection.x < -0.1f)
                {

                    jumpDir = Vector2.Lerp(jumpDir, Quaternion.AngleAxis(jumpAimAngle, Vector3.forward) * spriteTransform.up, jumpAimSpeed * Time.fixedDeltaTime);
                }
                else
                {
                    jumpDir = Vector2.Lerp(jumpDir, spriteTransform.up, jumpAimSpeed * Time.fixedDeltaTime);
                }


            }
            else if (wasChargingJump)
            {
                isJumping = true;
                jumpArrow.gameObject.SetActive(false);


                cc.enabled = true;
                rb.isKinematic = false;
            }

            if (isJumping)
            {
                bool perfectJump = chargeJumpTimer - chargeJumpTime > 0 && chargeJumpTimer - chargeJumpTime < chargePerfectWindow;
                if (!wasJumping)
                {
                    jumpTimer = 0;
                    if (perfectJump)
                    {
                        Instantiate(dustExplosionPrefab, transform.position, spriteTransform.rotation);
                    }
                }


                float jumpPowerModifier = jumpPowerCurve.Evaluate(Mathf.Clamp01(chargeJumpTimer / chargeJumpTime));

                if (perfectJump)
                {
                    jumpPowerModifier += 0.2f;

                }

                if (jumpDir.y < -0.1f)
                {
                    jumpPowerModifier *= 0.75f;
                }

                if (jumpTimer < jumpTime)
                {
                    rb.AddForce(jumpDir * jumpPower * jumpPowerModifier * jumpCurve.Evaluate(jumpTimer / jumpTime), ForceMode2D.Impulse);
                    jumpTimer += Time.fixedDeltaTime;
                }

                if ((isGrounded && !wasGrounded) || isGrounded && jumpTimer > Time.fixedDeltaTime * 5)
                {
                    isJumping = false;
                }
            }


            if (!isChargingJump && Mathf.Abs(moveDirection.x) > 0.1f)
            {
                Vector2 dir = new Vector2(moveDirection.x, 0);
                physicsMat.friction = 0;


                float speedModifier = 0.4f;

                if (!isGrounded)
                {
                    speedModifier = 0.1f;
                }
                if (isJumping)
                {
                    speedModifier = 0.03f;
                }


                if (groundNormal.magnitude > 0.1f)
                {
                    if (dir.x > 0)
                    {
                        dir = Quaternion.Euler(0, 0, -90) * groundNormal;
                    }
                    else
                    {
                        dir = Quaternion.Euler(0, 0, 90) * groundNormal;
                    }
                }

                if (rb.velocity.magnitude < 0.1f)
                {
                    transform.position = transform.position + new Vector3(dir.x, dir.y, 0) * Time.fixedDeltaTime;
                }
                rb.AddForce(dir * speed * speedModifier, ForceMode2D.Impulse);

                //Debug.DrawRay(transform.position, dir * speed, Color.blue);

                if (isGrounded)
                {
                    if (rb.velocity.magnitude > speed)
                    {
                        rb.velocity = rb.velocity.normalized * speed;
                    }
                }
                else
                {
                    if (Mathf.Abs(rb.velocity.x) > speed && !isJumping)
                    {

                        if (rb.velocity.x < 0)
                        {
                            rb.velocity = new Vector2(-speed, rb.velocity.y);
                        }
                        else
                        {
                            rb.velocity = new Vector2(speed, rb.velocity.y);
                        }
                    }
                }

            }
            else
            {

                physicsMat.friction = 10;
                if (isGrounded && !isJumping)
                {
                    rb.velocity = Vector2.zero;
                }
                else
                {
                    //rb.velocity = new Vector2(0, rb.velocity.y);
                }
            }
        }
        

        //if (tryJump && isGrounded)
        //{
        //    if (isHanging)
        //    {
        //        rb.velocity = new Vector2(0, -1f);
        //    }
        //    else
        //    {
        //        isJumping = true;
        //        if (!wasJumping)
        //        {
        //            jumpTimer = 0;
        //            rb.velocity = new Vector2(rb.velocity.x, 0);
        //        }
        //    }
        //    
        //}
        //if (!tryJump)
        //{
        //    isJumping = false;
        //    jumpTimer = 0;
        //}
        //
        //if (isJumping && !isHanging)
        //{
        //    if (jumpTimer < jumpTime)
        //    {
        //        rb.AddForce(Vector2.up * jumpPower * jumpCurve.Evaluate(jumpTimer / jumpTime), ForceMode2D.Impulse);
        //        jumpTimer += 0.0166666f;
        //    }
        //    else
        //    {
        //        isJumping = false;
        //    }
        //}

        wasJumping = isJumping;
        wasChargingJump = isChargingJump;
        wasGrounded = isGrounded;
        Debug.Log(isJumping);

        if (isGrounded)
        {
            anim.SetBool("IsFalling", false);
        }
        else
        {
            anim.SetBool("IsFalling", true);
        }

        if (isChargingJump)
        {
            anim.SetBool("IsCrouching", true);
        }
        else
        {
            anim.SetBool("IsCrouching", false);
        }

        if (isJumping && jumpTimer < jumpTime)
        {
            anim.SetBool("IsFlinging", true);
        }
        else
        {
            anim.SetBool("IsFlinging", false);
            //spriteTransform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
        }


        if (!isGrounded && caughtEnemies.Count > 0)
        {
            anim.SetBool("IsCaging", true);

        }
        else
        {
            anim.SetBool("IsCaging", false);
        }

        if (isEating)
        {
            anim.SetBool("IsEating", true);
        }
        else
        {
            anim.SetBool("IsEating", false);
        }


        if (!Mathf.Approximately(moveDirection.x, 0))
        {
            anim.SetBool("IsCrawling", true);
        }
        else
        {
            anim.SetBool("IsCrawling", false);
        }


        spriteRenderer.flipX = isFacingRight;

        if (isFacingRight)
        {
            eatingTransform.localPosition = new Vector3(0.2f, eatingTransform.localPosition.y, 0);
        }
        else
        {
            eatingTransform.localPosition = new Vector3(-0.2f, eatingTransform.localPosition.y, 0);
        }
    }

    public void MultiplyVelocity(Vector2 factor)
    {
        rb.velocity = rb.velocity * factor;
    }



    void OnMove(InputValue val)
    {
        moveDirection = val.Get<Vector2>();
    }

    void OnJump(InputValue val)
    {
        float jumpVal = val.Get<float>();
        tryJump = Mathf.Approximately(jumpVal, 0) ? false : true;
        
    }

    void OnMouseDelta(InputValue val)
    {
        mouseDelta = val.Get<Vector2>();
    }
}
