using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public Transform topPoint;      // Top point position
    public Transform bottomPoint;   // Bottom point position
    private float moveSpeed = 3.5f;  // Movement speed
    private bool goingLeft = true;    // Whether the wall is currently moving left
    private Vector3 targetPosition; // Next target position
    // Start is called before the first frame update
    void Start()
    {
        targetPosition = goingLeft ? topPoint.position : bottomPoint.position;
    }

    // Update is called once per frame
    void Update()
    {
        MoveAtkWall();
    }
    void MoveAtkWall()
    {

        // Move the wall towards the target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // If the wall reaches the target position, change direction
        if (transform.position == targetPosition)
        {
            goingLeft = !goingLeft;
            targetPosition = goingLeft ? topPoint.position : bottomPoint.position;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            LevelManager.Instance.Loss();
        }
    }
}
