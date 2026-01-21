using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    public SizeClass sizeClass;


    public float healthTickRate;
    float healthTickTimer;

    public int maxSilk;
    public int silk;

    public int maxCatch;
    public List<Enemy> caughtEnemies = new List<Enemy>();


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
        if (healthTickTimer > healthTickRate)
        {
            health -= 1;
            UIManager.SetHealth(health, maxHealth);

            healthTickTimer = 0;
        }

        healthTickTimer += Time.deltaTime;

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

    void GainHealth(int amount)
    {
        health += amount;
        if (health > maxHealth)
        {
            health = maxHealth;
        }

        UIManager.SetHealth(health, maxHealth);
    }

    public void ConsumeCatch()
    {
        for (int i =0; i < caughtEnemies.Count; i++)
        {
            int healthDiff = maxHealth - health;
            if (healthDiff > 0)
            {
                if (healthDiff > caughtEnemies[i].points)
                {
                    GainHealth(caughtEnemies[i].points);
                }
                else
                {
                    GainHealth(healthDiff);
                    GainSilk(caughtEnemies[i].points - healthDiff);
                }
            }
            else
            {
                GainSilk(caughtEnemies[i].points);
            }

            if (sizeClass - caughtEnemies[i].sizeClass == 1)
            {
                //pc.MultiplyVelocity(Vector2.one * 0.75f);
            }

            caughtEnemies[i].Die();
        }

        caughtEnemies = new List<Enemy>();
        pc.caughtEnemies = caughtEnemies;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Enemy enemy;

        
        if (collision.transform.TryGetComponent<Enemy>(out enemy))
        {
            if (enemy.sizeClass < sizeClass)
            {
                if (caughtEnemies.Count < maxCatch)
                {
                    enemy.isCaught = true;
                    caughtEnemies.Add(enemy);
                    pc.caughtEnemies = caughtEnemies;


                }
            }
        }
    }
}
