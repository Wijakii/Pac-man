using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tween
{
    public Transform target {get; private set;}
    public Vector3 startPos {get; private set;}
    public Vector3 endPos {get; private set;}
    public float duration {get; private set;}
    public float startTime { get; private set; }

    public Tween(Transform target, Vector3 startPos, Vector3 endPos, float startTime, float duration)
    {
        this.target = target;
        this.startPos = startPos;
        this.endPos = endPos;
        this.duration = duration;
        this.startTime = startTime;
    }
}
