using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public Transform player;

    private bool isFlipped = false;
    private void Start() {
        player = FindObjectOfType<PlayerStats>().transform;
    }

    public void LookAtPlayer()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float dotProduct = Vector3.Dot(directionToPlayer, transform.right);

        if (dotProduct > 0 && isFlipped)
        {
            Flip();
        }
        else if (dotProduct < 0 && !isFlipped)
        {
            Flip();
        }
    }

    private void Flip()
    {
        isFlipped = !isFlipped;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
