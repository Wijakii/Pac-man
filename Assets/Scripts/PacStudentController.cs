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
    public List<TileBase> pellets = new List<TileBase>();
    public Animator animator;
    public AudioSource moveAudio;
    public AudioSource eatAudio;
    public ParticleSystem particles;
    
    [Header("Movement")]
    public float moveSpeed = 0.5f;
    
    private Vector3Int gridPosition;
    private Vector2Int currentInput = Vector2Int.zero;
    private Vector2Int lastInput = Vector2Int.zero;
    private Vector3Int lastPosition;
    
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        gridPosition = grid.WorldToCell(transform.position);
        transform.position = grid.GetCellCenterWorld(gridPosition);
        currentInput = Vector2Int.zero;
        lastInput = Vector2Int.zero;
        lastPosition = gridPosition;
        
        animator.SetInteger("Direction", 1);
        
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();

        if (tweener.HasActiveTween == false)
        {
            move();
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

    private bool walkable(Vector3Int target)
    {
        TileBase tile = tilemap.GetTile(target);
        Debug.Log(tile);

        if  (tile == null) return false;
        
        if (walkableTiles.Contains(tile)) return true;
        return false;
    }

    private void manageParticles()
    {
        var emmision =  particles.emission;
        
        emmision.enabled = tweener.HasActiveTween;

        if (!particles.isPlaying)
        {
            particles.Play();
        }
        
    }
    
    private void lerpTo(Vector3Int target)
    {
        Vector3 startPosition = grid.GetCellCenterWorld(gridPosition);
        Vector3 targetPosition = grid.GetCellCenterWorld(target);
        
        tweener.AddTween(transform, startPosition, targetPosition, moveSpeed);
        gridPosition = target;
        AnimationDirection(targetPosition - startPosition);
    }

    private Vector3Int TargetCell(Vector2Int direction)
    {
        return new Vector3Int(gridPosition.x + direction.x, gridPosition.y + direction.y, gridPosition.z);
    }

    private void move()
    {
        Vector3Int currentTarget = TargetCell(currentInput);
        Vector3Int lastTarget = TargetCell(lastInput);

        if (lastInput != Vector2Int.zero && walkable(lastTarget))
        {
            currentInput = lastInput;
            lerpTo(lastTarget);
        }
        else if (currentInput != Vector2Int.zero && walkable(lastTarget))
        {
            lerpTo(currentTarget);
        }
    }

    private void eatCheck()
    {
        if (gridPosition != lastPosition)
        {
            TileBase tile = tilemap.GetTile(gridPosition);
            if (pellets.Contains(tile)) eatAudio.PlayOneShot(eatAudio.clip);
            lastPosition = gridPosition;
        }
    }
    
    
    
    private void audioManagement()
    {
        bool moving = tweener.HasActiveTween;
        bool onPellet = pellets.Contains(tilemap.GetTile(gridPosition));

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
