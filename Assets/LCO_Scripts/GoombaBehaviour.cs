using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoombaBehaviour : MonoBehaviour
{
    public bool goesRight = false;
    public PlayerController controller;

    public void StartGoingLeft()
    {
        goesRight = false;
        Debug.Log("goes left");
    }

    public void RaphaelMethod()
    {
        Debug.Log("Hello Raphael....");
    }

    public void StartGoingRight()
    {
        goesRight = true;
        Debug.Log("goes right");
    } 

    void Update()
    {
        controller.leftKey = !goesRight;
        controller.rightKey = goesRight;
    }
}
