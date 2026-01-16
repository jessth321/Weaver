using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    public SizeClass sizeClass;


    public int maxSilk;
    public int silk;



    PlayerController pc;

    // Start is called before the first frame update
    void Start()
    {
        pc = GetComponent<PlayerController>();

        UIManager.SetSilk(silk, maxSilk);
        UIManager.SetHealth(health, maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GainSilk(int amount)
    {
        silk += amount;
        if (silk > maxSilk)
        {
            silk = maxSilk;
        }

        UIManager.SetSilk(silk, maxSilk);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Enemy enemy;

        if (collision.transform.TryGetComponent<Enemy>(out enemy))
        {
            if (enemy.sizeClass < sizeClass)
            {
                GainSilk(enemy.points);

                if (sizeClass - enemy.sizeClass == 1)
                {
                    pc.MultiplyVelocity(Vector2.one * 0.75f);
                }

                enemy.Die();
            }
        }
    }
}
