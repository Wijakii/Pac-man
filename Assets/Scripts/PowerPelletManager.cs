using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PowerPelletManager : MonoBehaviour
{
    [Header("References")]
    public List<GhostController> ghosts;
    public AudioSource backgroundMusic;
    public AudioSource scaredMusic;
    public TextMeshProUGUI ghostTimer;
    public PacStudentController player;

    [Header("Settings")] 
    public float scaredDuration = 10f;
    public float recovering = 3f;
    
    private Coroutine activeRoutine;
    
    

    public void ActivatePowerPellet()
    {
        
        ScoreManager.instance.AddScore(50);

        foreach (GhostController ghost in ghosts)
        {
            
        }
        
        backgroundMusic.Pause();
        scaredMusic.Play();

        if (activeRoutine != null)
        {
            StopCoroutine(activeRoutine);
            activeRoutine = StartCoroutine(PowerPillTimer());
        }

        IEnumerator PowerPillTimer()
        {
            float timeLeft = scaredDuration;
            ghostTimer.gameObject.SetActive(true);

            while (timeLeft > 0)
            {
                ghostTimer.text = Mathf.CeilToInt(timeLeft).ToString();
                
                timeLeft -= Time.deltaTime;
                yield return null;

            }
            ghostTimer.gameObject.SetActive(false);
            
            scaredMusic.Pause();
            backgroundMusic.Play();
        }
        
    }

}
