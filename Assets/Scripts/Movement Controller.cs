using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    [Header("Tweener")]
    [SerializeField] private Tweener tweener;

    [Header("waypoints ")] [SerializeField]
    private Transform[] waypoints;
    
    [Header("Animation")]
    [SerializeField] private Animator animator;
    
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    
    private float moveDuration = 1f;
    
    private int i = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        transform.position = waypoints[0].position;
        Debug.Log("pac man starting at "+transform.position);
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (!HasActiveTween())
        {
            MoveToNextWaypoint();
        }
    }

    private void MoveToNextWaypoint()
    {
        int nextIndex = (i + 1) % waypoints.Length;
        Vector3 startPos = transform.position;
        Vector3 endPos = waypoints[nextIndex].position;
        
        AnimationDirection(endPos - startPos);

        tweener.AddTween(transform, startPos, endPos, moveDuration);
        i = nextIndex;
    }
    private bool HasActiveTween()
    {
       return tweener && tweener.HasActiveTween;
    }

    private void AnimationDirection(Vector3 direction)
    {
        Vector2 dir = new Vector2(direction.x, direction.y);
        
        dir.Normalize();

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            if (dir.x > 0)
                animator.SetInteger("Direction", 1);
            else
                animator.SetInteger("Direction", 3);
        }
        else
        {
            if (dir.y > 0)
                animator.SetInteger("Direction", 0);
            else
                animator.SetInteger("Direction", 2);
        }
    }
}
