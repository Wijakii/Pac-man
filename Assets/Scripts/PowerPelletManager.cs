using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PowerPelletManager : MonoBehaviour
{
    
    public static PowerPelletManager instance {get; private set;}
     
    [Header("References")]
    public List<GhostController> ghosts;
    public AudioSource backgroundMusic;
    public AudioSource scaredMusic;
    public TextMeshProUGUI ghostTimer;
    

    [Header("Settings")] 
    public float scaredDuration = 10f;
    public float recovering = 3f;
    
    
    private Coroutine activeRoutine;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void ActivatePowerPellet()
    {

        ScoreManager.instance.AddScore(50);

        if (activeRoutine != null)
        {
            StopCoroutine(activeRoutine);
        }

        foreach (GhostController ghost in ghosts)
        {
            
            ghost.SetState(GhostState.Scared);
            Debug.Log("Setting ghost" + ghost.name + "to scared");
        }

        backgroundMusic.Stop();
        scaredMusic.Play();

        activeRoutine = StartCoroutine(PowerPillTimer());
    }

    IEnumerator PowerPillTimer()
        {
            float timeLeft = scaredDuration;
            ghostTimer.gameObject.SetActive(true);
            bool recoveringTriggered = false;

            while (timeLeft > 0)
            {
                ghostTimer.text = Mathf.CeilToInt(timeLeft).ToString();

                if (!recoveringTriggered && timeLeft <= recovering)
                {
                    recoveringTriggered = true;
                    foreach (GhostController ghost in ghosts)
                    {
                        if (ghost.currentState == GhostState.Scared)
                        {
                            ghost.SetState(GhostState.Recovering);
                        }
                    }
                }
                
                timeLeft -= Time.deltaTime;
                yield return null;

            }
            foreach (GhostController ghost in ghosts)
            {
                if (ghost.currentState != GhostState.Dead)
                {
                    ghost.SetState(GhostState.Normal);
                }
            }
            ghostTimer.gameObject.SetActive(false);
            
            scaredMusic.Stop();
            backgroundMusic.Play();
            
            activeRoutine = null;
        }
        
    }


