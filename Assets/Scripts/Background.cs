using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    private GameObject player;
    public float speed;
    private Vector2 previousPlayerPosition;
    //private Rigidbody2D rb2D;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        previousPlayerPosition = player.transform.position;
        //rb2D = player.GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        Vector2 currentPlayerPosition = player.transform.position;

        if (currentPlayerPosition != previousPlayerPosition)
        {
            Vector2 movement = currentPlayerPosition - previousPlayerPosition;
            transform.position = new Vector2(transform.position.x + (movement.x * speed * 10f), transform.position.y);
            previousPlayerPosition = currentPlayerPosition;
        }

        //if (Mathf.Abs(rb2D.velocity.x) > 0f)
        //{
        //    transform.position = new Vector2(transform.position.x + (speed * player.transform.localScale.x), transform.position.y);
        //}
    }
}
