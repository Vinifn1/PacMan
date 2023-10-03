using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GhostScatter : GhostBehavior
{
    private void OnDisable()
    {
        this.Ghost.Chase.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Node node = other.GetComponent<Node>();
        // Scatter Behavior = Movimentos aleatorios
        if (node != null && this.enabled)
        {
            int index = Random.Range(0, node.AvailableDirections.Count);

            // Regra para o fantasma nao ficar indo pra frente e pra tras, alternando entre duas direções.
            if (node.AvailableDirections[index] == -this.Ghost.Movement.Direction && node.AvailableDirections.Count > 1)
            {
                index++;

                if (index >= node.AvailableDirections.Count)
                {
                    index = 0;
                }
            }
            
            this.Ghost.Movement.SetDirection(node.AvailableDirections[index]);
        }
    }
}
