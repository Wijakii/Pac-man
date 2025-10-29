using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CherryController : MonoBehaviour
{

    [Header("References")] 
    public Tweener tweener;
    public GameObject cherryPrefab;

    public SpriteMask levelMask;

    [Header("Settings")] 
    public float spawnDelay = 5f;
    public float moveDuration = 5f;

    private Vector3 levelCenter;
    private Vector3 levelMin;
    private Vector3 levelMax;
    // Start is called before the first frame update
    void Start()
    {
        determineBounds();
        StartCoroutine(SpawnCherryRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameStart.instance.gameStarted) return;
    }

    private void determineBounds()
    {
        SpriteRenderer maskRenderer = levelMask.GetComponent<SpriteRenderer>();
        
        Bounds bounds = levelMask.bounds;
        levelMin = bounds.min;
        levelMax = bounds.max;
        levelCenter = bounds.center;
        Debug.Log(levelCenter);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator SpawnCherryRoutine()
    {
        Debug.Log("routine started");
        yield return new WaitForSeconds(spawnDelay);

        while (true)
        {
            Debug.Log("spawning cherry");
            
            GameObject cherry = spawnCherry();
            yield return new WaitUntil(() => cherry == null);
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private GameObject spawnCherry()
    {
        Vector3 spawnPos = Vector3.zero;
        Vector3 exitPos = Vector3.zero;
        
        int side = Random.Range(0, 4);

        switch (side)
        {
            case 0:
                spawnPos = new Vector3(levelMin.x - 1f, Random.Range(levelMin.y, levelMax.y), 0);
                float yExit = levelCenter.y + (levelCenter.y - spawnPos.y);
                exitPos = new Vector3(levelMax.x + 1f, yExit, 0);
                break;
            case 1:
                spawnPos = new Vector3(levelMax.x + 1f, Random.Range(levelMin.y, levelMax.y), 0);
                yExit = levelCenter.y + (levelCenter.y - spawnPos.y);
                exitPos = new Vector3(levelMin.x -1f, yExit, 0);
                break;
            case 2:
                spawnPos = new Vector3(Random.Range(levelMin.x, levelMax.x), levelMax.y +1f, 0);
                float xExit =  levelCenter.x + (levelCenter.x - spawnPos.x);
                exitPos = new Vector3(xExit, levelMin.y -1f, 0);
                break;
            case 3:
                spawnPos = new Vector3(Random.Range(levelMin.x, levelMax.x), levelMin.y -1f, 0);
                xExit = levelCenter.x + (levelCenter.x - spawnPos.x);
                exitPos = new Vector3(xExit, levelMax.y +1f, 0);
                break;
                
        }
        
        
        GameObject cherry = Instantiate(cherryPrefab, spawnPos, Quaternion.identity);
        
        
        var spriteRenderer = cherry.GetComponent<SpriteRenderer>();
        spriteRenderer.maskInteraction = SpriteMaskInteraction.None;
        spriteRenderer.sortingLayerName = "foreground";
        spriteRenderer .sortingOrder = 1;

        StartCoroutine(MoveCherry(cherry.transform, spawnPos, exitPos));
        return cherry;



    }

    private IEnumerator MoveCherry(Transform cherry, Vector3 start, Vector3 end)
    {
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            cherry.position = Vector3.Lerp(start, end, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        cherry.position = end;
        Destroy(cherry.gameObject);
    }
}
