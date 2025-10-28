using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tweener : MonoBehaviour
{
    private Tween activeTween;
    public bool HasActiveTween => activeTween != null;

    public void AddTween(Transform targetObject, Vector3 startPos, Vector3 endPos, float duration)
    {
        if (activeTween == null)
        {
            activeTween = new Tween(targetObject, startPos, endPos, Time.time,duration);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (activeTween == null) return;

        float elapsedTime = Time.time - activeTween.startTime;
        float fraction = elapsedTime / activeTween.duration;
        
        activeTween.target.position = Vector3.Lerp(activeTween.startPos, activeTween.endPos, fraction);

        if (fraction >= 1f)
        {
            activeTween.target.position = activeTween.endPos;
            activeTween = null;
        }
    }

    public void ResetTween()
    {
        activeTween = null;
    }
}
