using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollisionEmitter : MonoBehaviour
{
    public UnityEvent OnCollidedLeft, OnCollidedRight, OnCollidedAbove, OnCollidedBelow;

    public UnityEvent RaphaelEvent;
}
