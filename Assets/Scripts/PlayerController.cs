using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{



    Rigidbody2D rb;
    CapsuleCollider2D cc;
    Animator anim;

    Vector2 moveDirection;
    Transform spriteTransform;
    SpriteRenderer spriteRenderer;
    [SerializeField] PhysicsMaterial2D physicsMat;
    [SerializeField] float speed;
    [SerializeField] float jumpPower;
    [SerializeField] float jumpTime;
    [SerializeField] AnimationCurve jumpCurve;

    bool isGrounded;
    bool isHanging;
    bool tryJump;
    bool wasJumping;
    bool isJumping;
    float jumpTimer;
    bool isFacingRight = true;
    bool isEating;
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
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D[] hits = Physics2D.CapsuleCastAll(new Vector2(transform.position.x + cc.offset.x, transform.position.y + cc.offset.y), cc.size + Vector2.one * 0.05f, cc.direction, 0, Vector2.down, 0.01f);

        //List<Vector2> normals = new List<Vector2>();
        //List<Vector2> offsets = new List<Vector2>();

        if (!Mathf.Approximately(moveDirection.x, 0))
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
            groundNormal = Vector2.right;
        }
        else
        {
            groundNormal = Vector2.left;
        }
        bool groundFlag = false;

        isGrounded = false;
        rb.gravityScale = 5;

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].transform.CompareTag("Ground"))
            {
                isGrounded = true;
                rb.gravityScale = 0;

                Vector2 normal = hits[i].normal;


                //normals.Add(hits[i].normal);
                //offsets.Add(hits[i].point - new Vector2(transform.position.x, transform.position.y));
                if (Vector2.Dot(groundNormal, isFacingRight ? Vector2.right : Vector2.left) > Vector2.Dot(normal, isFacingRight ? Vector2.right : Vector2.left))
                {
                    groundNormal = hits[i].normal;
                    groundFlag = true;
                }


                //Debug.DrawRay(hits[i].point, hits[i].normal, Color.red);
            }
        }


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
            spriteTransform.rotation = Quaternion.LookRotation(Vector3.forward, Vector2.up);
        }
    }

    private void FixedUpdate()
    {
        

        if (Mathf.Abs(moveDirection.x) > 0.1f)
        {
            Vector2 dir = new Vector2(moveDirection.x, 0);
            physicsMat.friction = 0;


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

            
            rb.AddForce(dir * speed, ForceMode2D.Impulse);

            if (isGrounded && !isJumping)
            {
                if (rb.velocity.magnitude > speed)
                {
                    rb.velocity = rb.velocity.normalized * speed;
                }
            }
            else
            {
                if (Mathf.Abs(rb.velocity.x) > speed)
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
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }

        if (tryJump && isGrounded)
        {
            if (isHanging)
            {
                rb.velocity = new Vector2(0, -1f);
            }
            else
            {
                isJumping = true;
                if (!wasJumping)
                {
                    jumpTimer = 0;
                    rb.velocity = new Vector2(rb.velocity.x, 0);
                }
            }
            
        }
        if (!tryJump)
        {
            isJumping = false;
            jumpTimer = 0;
        }

        if (isJumping && !isHanging)
        {
            if (jumpTimer < jumpTime)
            {
                rb.AddForce(Vector2.up * jumpPower * jumpCurve.Evaluate(jumpTimer / jumpTime), ForceMode2D.Impulse);
                jumpTimer += 0.0166666f;
            }
            else
            {
                isJumping = false;
            }
        }

        wasJumping = isJumping;


        if (isGrounded)
        {
            anim.SetBool("IsFalling", false);
        }
        else
        {
            anim.SetBool("IsFalling", true);
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
}
