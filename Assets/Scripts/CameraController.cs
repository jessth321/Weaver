using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    [SerializeField]
    Transform player;
    [SerializeField]
    Vector2 softBoundary;
    [SerializeField]
    Vector2 hardBoundary;
    [SerializeField]
    float softTightness;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 difference = player.position - transform.position;



        if (Mathf.Abs(difference.x) > softBoundary.x)
        {
            transform.position = new Vector3(Mathf.Lerp(transform.position.x, player.position.x - (difference.x < 0f ? -softBoundary.x : softBoundary.x), Time.deltaTime * softTightness), transform.position.y, transform.position.z);
        }

        if (Mathf.Abs(difference.y) > softBoundary.y)
        {
            transform.position = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, player.position.y - (difference.y < 0f ? -softBoundary.y : softBoundary.y), Time.deltaTime * softTightness), transform.position.z);
        }

        if (Mathf.Abs(difference.x) > hardBoundary.x)
        {
            transform.position = new Vector3(player.position.x - (difference.x < 0f ? -hardBoundary.x : hardBoundary.x), transform.position.y, transform.position.z);
        }

        if (Mathf.Abs(difference.y) > hardBoundary.y)
        {
            transform.position = new Vector3(transform.position.x, player.position.y - (difference.y < 0f ? -hardBoundary.y : hardBoundary.y), transform.position.z);
        }
    }
}
