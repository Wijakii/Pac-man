using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GhostState {Normal, Scared, Recovering, Dead}

public class GhostController : MonoBehaviour
{

    public Animator animator;
    public GhostState currentState { get; private set; } = GhostState.Normal;

    private Vector3 startPosition;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameStart.instance.gameStarted) return;
    }

    public void SetState(GhostState newState)
    {
        if (currentState == GhostState.Dead && newState != GhostState.Normal) return;

        currentState = newState;

        switch (newState)
        {
            case GhostState.Normal:
                animator.SetTrigger("Normal");
                break;
            case GhostState.Scared:
                animator.SetTrigger("Scared");
                break;
            case GhostState.Recovering:
                animator.SetTrigger("Recovering");
                break;
            case GhostState.Dead:
                animator.SetTrigger("Dead");
                break;
        }
    }

    public void Die()
    {
        if (currentState == GhostState.Recovering || currentState == GhostState.Dead)
        {
            SetState(GhostState.Dead);
            ScoreManager.instance.AddScore(300);
            StartCoroutine(RespawnAfterDelay(3f));

        }
    }

    private IEnumerator RespawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SetState(GhostState.Normal);
    }
    
    public void resetGhost()
    {
        {
            transform.position = startPosition;
            SetState(GhostState.Normal);
        }
    }
}

