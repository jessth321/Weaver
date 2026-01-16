using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public GameObject plusPoints;
    public int points;
    public GameObject deathBits;

    public int maxHealth;
    public int health;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int amount)
    {

    }

    public void Die()
    {
        GameObject spawnedBits = Instantiate(deathBits, transform.position, Quaternion.identity);
        spawnedBits.transform.localScale = transform.localScale;
        Transform plusPointsSpawned = Instantiate(plusPoints, UIManager.UICanvasWS).transform;
        plusPointsSpawned.position = transform.position;
        TempTextFade textFade = plusPointsSpawned.GetComponent<TempTextFade>();
        textFade.textTMP.text = "+" + points.ToString();
        textFade.Fade();
        

        gameObject.SetActive(false);
        Destroy(spawnedBits, 1f);
        Destroy(gameObject, 1.1f);
    }
}
