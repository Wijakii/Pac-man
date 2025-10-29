using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PacStudentController : MonoBehaviour
{

    [Header("Data References")]
    public Tweener tweener;
    public Grid grid;
    public Tilemap tilemap;
    public List<TileBase> walkableTiles = new List<TileBase>();
    public List<TileBase> pelletList = new List<TileBase>();
    public TileBase pellet;
    public TileBase powerPellet;
    public TileBase noPellet;
    public Animator animator;
    public AudioSource moveAudio;
    public AudioSource eatAudio;
    public AudioSource wallBumpAudio;
    public ParticleSystem movementParticles;
    public ParticleSystem wallParticles;
    
    [Header("Movement")]
    public float moveSpeed = 0.5f;
    
    [Header("player stuff")]
    public Vector3 startPosition;
    public ParticleSystem deathParticles;

    public Vector3Int gridPosition;
    public Vector2Int currentInput = Vector2Int.zero;
    private Vector2Int lastInput = Vector2Int.zero;
    private Vector3Int lastPosition;
    private bool hitWall = false;
    public PowerPelletManager powerPelletManager;
    private bool isDead = false;
    private int lastDirection = -1;
    
    void Start()
    {
        
        startPosition = transform.position;
        
        gridPosition = grid.WorldToCell(transform.position);
        transform.position = grid.GetCellCenterWorld(gridPosition);
        currentInput = Vector2Int.zero;
        lastInput = Vector2Int.zero;
        lastPosition = gridPosition;
        
        
        animator.SetInteger("Direction", 1);
        
    }

    void Update()
    {
        if (isDead) return;
        if (!GameStart.instance.gameStarted) return;
        
        GetInput();
        if (tweener.HasActiveTween == false)
        {
            move();
            
        }

        if (currentInput != Vector2Int.zero || lastInput != Vector2Int.zero)
        {
            Vector2 dir = (currentInput != Vector2Int.zero) ? currentInput : lastInput;
            AnimationDirection(new Vector3(dir.x, dir.y, 0));
        }
        
        
        eatCheck();
        
        audioManagement();
        
        manageParticles();
        
        
    }

    private void GetInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            lastInput = Vector2Int.up;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            lastInput = Vector2Int.left;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            lastInput = Vector2Int.down;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            lastInput = Vector2Int.right;
        }
    }

    public bool walkable(Vector3Int target)
    {
        TileBase tile = tilemap.GetTile(target);
        

        if  (tile == null) return false;
        
        if (walkableTiles.Contains(tile)) return true;
        return false;
    }

    private void LoseLife()
    {
        if (isDead) return;
        isDead = true;
        
        tweener.ResetTween();
        movementParticles.Stop();
        
        animator.SetTrigger("Dead");
        deathParticles.transform.position = transform.position;
        deathParticles.Play();
        
        
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(3f);
        
        transform.position = startPosition;
        gridPosition = grid.WorldToCell(transform.position);
        tweener.ResetTween();

        foreach (GhostController ghost in PowerPelletManager.instance.ghosts)
        {
            ghost.resetGhost();
        }
        
        isDead = false;
        
    }
    
    private void manageParticles()
    {
        var emmision =  movementParticles.emission;
        
        emmision.enabled = tweener.HasActiveTween;

        if (!movementParticles.isPlaying)
        {
            movementParticles.Play();
        }
        
    }

    public void lerpTo(Vector3Int target)
    {
        Vector3 startPosition = grid.GetCellCenterWorld(gridPosition);
        Vector3 targetPosition = grid.GetCellCenterWorld(target);
        
        tweener.AddTween(transform, startPosition, targetPosition, moveSpeed);
        gridPosition = target;
        AnimationDirection(targetPosition - startPosition);
    }

    public Vector3Int TargetCell(Vector2Int direction)
    {
        return new Vector3Int(gridPosition.x + direction.x, gridPosition.y + direction.y, gridPosition.z);
    }

    private void move()
    {
        if (isDead) return;
        Vector3Int currentTarget = TargetCell(currentInput);
        Vector3Int lastTarget = TargetCell(lastInput);

        if (lastInput != Vector2Int.zero && walkable(lastTarget))
        {
            currentInput = lastInput;
            lerpTo(lastTarget);
            hitWall = false;
        }
        else if (currentInput != Vector2Int.zero && walkable(lastTarget))
        {
            lerpTo(currentTarget);
            hitWall = false;
        }
        else
        {
            if (!hitWall)
            {
                wallParticles.transform.position = transform.position;
                wallParticles.Play();
                wallBumpAudio.PlayOneShot(wallBumpAudio.clip);
                hitWall = true;
            }
            
        }
    }

    private void eatCheck()
    {
        if (gridPosition != lastPosition)
        {
            TileBase tile = tilemap.GetTile(gridPosition);

            if (tile == pellet)
            {
                eatAudio.PlayOneShot(eatAudio.clip);
                
                ScoreManager.instance.AddScore(10);
                tilemap.SetTile(gridPosition, noPellet);
                
            }
            lastPosition = gridPosition;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PowerPellet"))
        {
            eatAudio.PlayOneShot(eatAudio.clip);
            //get pilled
        }

        if (collision.CompareTag("Ghost"))
        {
            GhostController ghost = collision.gameObject.GetComponent<GhostController>();

            if (ghost.currentState == GhostState.Normal)
            {
                LoseLife();
            }
            else if (ghost.currentState == GhostState.Scared || ghost.currentState == GhostState.Recovering)
            {
                ghost.Die();
            }
        }
    }
    
    
    private void audioManagement()
    {
        bool moving = tweener.HasActiveTween;
        bool onPellet = pelletList.Contains(tilemap.GetTile(gridPosition));

        if (moving && !onPellet)
        {
            if(!moveAudio.isPlaying) moveAudio.Play();
        }
        else
        {
            if(!moveAudio.isPlaying) moveAudio.Stop();
        }
    }
    
    private void AnimationDirection(Vector3 direction)
    {
        if (isDead) return;
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
