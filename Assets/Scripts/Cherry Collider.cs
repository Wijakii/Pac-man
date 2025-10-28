using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CherryCollider : MonoBehaviour
{
    public int scoreValue = 100;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            ScoreManager.instance.AddScore(scoreValue);
            
            Destroy(gameObject);
        }
    }
}
