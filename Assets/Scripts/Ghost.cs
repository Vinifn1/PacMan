using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public Movement Movement { get; private set; }
    public GhostHome Home { get; private set; }
    public GhostScatter Scatter { get; private set; }
    public GhostChase Chase { get; private set; }
    public GhostFrightened Frightened { get; private set; }

    public Transform target;
    public GhostBehavior initialBehavior;
    public int points = 200;
    public LayerMask obstacleLayer;
    public GameObject redMarker;
    public GameObject yellowMarker;

    public void Awake()
    {
        this.Movement = GetComponent<Movement>();
        this.Home = GetComponent<GhostHome>();
        this.Scatter = GetComponent<GhostScatter>();
        this.Chase = GetComponent<GhostChase>();
        this.Frightened = GetComponent<GhostFrightened>();
        
    }

    public void Start()
    {
        ResetState();
    }

    public void ResetState()
    {
        this.gameObject.SetActive(true);
        this.Movement.ResetState();
        
        this.Chase.Disable();
        this.Frightened.Disable();
        this.Scatter.Enable();

        if (this.Home != initialBehavior)
        {
            this.Home.Disable();
        }

        if (this.initialBehavior != null)
        {
            this.initialBehavior.Enable();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Pacman"))
        {
            Debug.Log("pacman");
            if (this.Frightened.enabled)
            {
                FindObjectOfType<GameManager>().GhostEaten(this);
            }
            else
            {
                FindObjectOfType<GameManager>().PacmanEaten();
            }
        }
    }
    
    public void SetPosition(Vector3 position)
    {
        // Keep the z-position the same since it determines draw depth
        position.z = transform.position.z;
        transform.position = position;
    }
}


