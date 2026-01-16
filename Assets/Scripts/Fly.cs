using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Fly : Enemy
{
    enum MoveState
    {
        WANDER,
        RUN
    }
    [SerializeField]
    MoveState moveState;
    Vector2 spawnPos;

    float rotDelta;
    float timer;
    float moveDirTime = 1;


    // Start is called before the first frame update
    void Awake()
    {
        spawnPos = transform.position;

        moveDirection = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.forward) * Vector2.up;
    }

    // Update is called once per frame
    void Update()
    {
        if (moveState == MoveState.WANDER)
        {
            //rb.velocity = Vector2.zero;
            rb.AddForce(moveDirection * speed * Time.deltaTime);

            moveDirection = Quaternion.AngleAxis(rotDelta * Time.deltaTime, Vector3.forward) * moveDirection;

            if (timer > moveDirTime)
            {
                rotDelta = Random.Range(-180, 180);

                timer = 0;
                moveDirTime = Random.Range(0.5f, 2f);
            }

            timer += Time.deltaTime;
        }
    }


}
