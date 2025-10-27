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
    public List<TileBase> walls = new List<TileBase>();
    public List<TileBase> pellets = new List<TileBase>();
    public Animator animator;
    public AudioSource moveAudio;
    public AudioSource eatAudio;
    
    [Header("Movement")]
    public float moveSpeed = 0.5f;
    
    private Vector3Int gridPosition;
    private Vector2Int currentInput = Vector2Int.zero;
    private Vector2Int lastInput = Vector2Int.zero;
    
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        gridPosition = grid.WorldToCell(transform.position);
        transform.position = grid.GetCellCenterWorld(gridPosition);
        currentInput = Vector2Int.right;
        lastInput = Vector2Int.zero;
        
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();

        if (tweener.HasActiveTween == false)
        {
            move();
        }
        
        bool moving = tweener.HasActiveTween;
        bool eatable = eatCheck();
        
        audioManagement(moving, eatable);
        
        
    }

    private void GetInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            currentInput = Vector2Int.up;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            currentInput = Vector2Int.left;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            currentInput = Vector2Int.down;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            currentInput = Vector2Int.right;
        }
    }

    private bool walkable(Vector3Int target)
    {
        TileBase tile = tilemap.GetTile(target);

        if (walls.Contains(tile))
        {
            return false;
        }
        return true;
    }

    private void lerpTo(Vector3Int target)
    {
        Vector3 startPosition = grid.GetCellCenterWorld(gridPosition);
        Vector3 targetPosition = grid.GetCellCenterWorld(target);
        
        tweener.AddTween(transform, startPosition, targetPosition, moveSpeed);
        gridPosition = target;
        AnimationDirection(targetPosition - startPosition);
    }

    private void move()
    {
        if (lastInput != Vector2Int.zero && walkable(gridPosition + (Vector3Int)lastInput))
        {
            currentInput = lastInput;
            lerpTo(gridPosition + (Vector3Int)lastInput);
            
        }
        else if (currentInput != Vector2Int.zero && walkable(gridPosition + (Vector3Int)currentInput))
        {
            lerpTo(gridPosition + (Vector3Int)currentInput);
        }
    }

    private bool eatCheck()
    { 
        TileBase tile = tilemap.GetTile(gridPosition);
        return  pellets.Contains(tile);
    }

    private void audioManagement(bool moving, bool isEatable)
    {
        if (moving)
        {
            if (isEatable)
            {
                if(!eatAudio.isPlaying) eatAudio.Play();
                if(moveAudio.isPlaying) moveAudio.Stop();
            }
            else
            {
                if(eatAudio.isPlaying) eatAudio.Stop();
                if(moveAudio.isPlaying) moveAudio.Play();
            }
        }
        else
        {
            if(eatAudio.isPlaying) eatAudio.Stop();
            if(moveAudio.isPlaying) moveAudio.Stop();
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
