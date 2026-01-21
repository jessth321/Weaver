using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpArrow : MonoBehaviour
{
    [SerializeField] int numDots;
    [SerializeField] GameObject dotPrefab;
    [SerializeField] float spacing;
    [SerializeField] float curveFalloff;

    List<Transform> dots = new List<Transform>();

    [HideInInspector] public float jumpPower01;



    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < numDots; i++)
        {
            dots.Add(Instantiate(dotPrefab, transform).transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeInHierarchy)
        {
            for (int i = 0; i < numDots; i++)
            {
                dots[i].localPosition = Vector3.up * i * spacing * jumpPower01;
                dots[i].position -= Vector3.up * Vector3.Magnitude(dots[i].position - transform.position) * Vector3.Magnitude(dots[i].position - transform.position) * curveFalloff * (0.5f + (0.5f - jumpPower01*0.5f));

                dots[i].GetChild(0).gameObject.SetActive(false);
                if ((float)i / (float)(numDots - 1) <= jumpPower01)
                {
                    dots[i].GetChild(0).gameObject.SetActive(true);
                }
            }
        }
    }
}
