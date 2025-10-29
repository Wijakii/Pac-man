using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameStart : MonoBehaviour
{
    // Start is called before the first frame update
    public static GameStart instance {get; private set;}
    
    [Header("UI")]
    public TMP_Text countdownText;
    public float countdownTime = 3f;
    
    public bool gameStarted = false;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    void Start()
    {
        StartCoroutine(GameStartRoutine());
    }

    private IEnumerator GameStartRoutine()
    {
        Time.timeScale = 0;
        
        countdownText.gameObject.SetActive(true);
        float currentTime = countdownTime;

        while (currentTime > 0f)
        {
            countdownText.text = Mathf.Ceil(currentTime).ToString();
            yield return null;
            currentTime -= Time.unscaledDeltaTime;
        }
        
        countdownText.text = "GO!";
        
        yield return new WaitForSecondsRealtime(1f);
        
        countdownText.gameObject.SetActive(false);
        
        Time.timeScale = 1;
        gameStarted = true;
    }
}
