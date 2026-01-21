using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SizeClass
{
    XSMALL,
    SMALL,
    MEDIUM,
    LARGE,
    XLARGE,
    MASSIVE
}


public class Enemy : Entity
{
    public SizeClass sizeClass;
    public float speed;
    public int damage;

    public bool isCaught;


    public Rigidbody2D rb;
    public Collider2D col;
    //public Transform spriteTransform;
    public Animator anim;

    public Vector2 moveDirection;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }


}
