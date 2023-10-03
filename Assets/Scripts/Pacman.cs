using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pacman : MonoBehaviour
{
    public Movement Movement { get; private set; }
    public bool rotateSprite = false;
    private void Awake()
    {
        Movement = GetComponent<Movement>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
            Movement.SetDirection(Vector2.up);
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            Movement.SetDirection(Vector2.down);
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            Movement.SetDirection(Vector2.left);
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            Movement.SetDirection(Vector2.right);

        float angle = (float)Math.Atan2(this.Movement.Direction.y, this.Movement.Direction.x);
        if (this.rotateSprite)
            this.transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward);
    }

    public void ResetState()
    {
        this.Movement.ResetState();
        this.gameObject.SetActive(true);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Path"))
        {
        }
    }
}
