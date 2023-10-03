using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed = 5.0f;
    public float speedMultiplier = 1.0f;
    public Vector2 initialDirection;
    public LayerMask obstacleLayer;
    
    public Rigidbody2D Rigidbody { get; private set; }
    public Vector2 Direction { get; private set; }
    public Vector2 NextDirection { get; private set; }
    public Vector3 StartingPosition { get; private set; }

    private void Awake()
    {
        this.Rigidbody = GetComponent<Rigidbody2D>();
        this.StartingPosition = this.transform.position;
    }

    private void Start()
    {
        ResetState();
    }

    private void Update()
    {
        if (this.NextDirection != Vector2.zero)
        {
            SetDirection(this.NextDirection);
        }
    }
    private void FixedUpdate()
    {
        Vector2 position = this.Rigidbody.position;
        Vector2 translation = this.Direction * (this.speed * speedMultiplier * Time.fixedDeltaTime);
        
        this.Rigidbody.MovePosition(position + translation);
    }

    public void SetDirection(Vector2 direction, bool forced = false)
    {
        if (!TileOccupied(direction) || forced) // Ou ele vai pra direção desejada
        {
            this.Direction = direction;
            this.NextDirection = Vector2.zero;
        }
        else // Ou coloca no buffer pra quando o movimento puder ser feito
        {
            this.NextDirection = direction;
        }
    }

    public bool TileOccupied(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.BoxCast(this.transform.position, Vector2.one * 0.75f, 0.0f, direction, 1.5f, this.obstacleLayer);
        return hit.collider != null;
    }

    public void ResetState()
    {
        this.speedMultiplier = 1.0f;
        this.Direction = initialDirection;
        this.NextDirection = Vector2.zero;
        this.transform.position = StartingPosition;
        this.Rigidbody.isKinematic = false;
        this.enabled = true;
    }


}
