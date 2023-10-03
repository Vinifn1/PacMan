using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GhostHome : GhostBehavior
{
    public Transform inside;
    public Transform outside;

    private void OnEnable()
    {
        StopAllCoroutines();
    }

    private void OnDisable()
    {
        StartCoroutine(ExitTransition());
    }

    private IEnumerator ExitTransition()
    {
        // Turn off movement while we manually animate the position
        Ghost.Movement.SetDirection(Vector2.up, true);
        Ghost.Movement.Rigidbody.isKinematic = true;
        Ghost.Movement.enabled = false;

        Vector3 position = transform.position;

        float duration = 0.5f;
        float elapsed = 0f;

        // Animate to the starting point
        while (elapsed < duration)
        {
            Ghost.SetPosition(Vector3.Lerp(position, inside.position, elapsed / duration));
            elapsed += Time.deltaTime;
            yield return null;
        }

        elapsed = 0f;

        // Animate exiting the ghost home
        while (elapsed < duration)
        {
            Ghost.SetPosition(Vector3.Lerp(inside.position, outside.position, elapsed / duration));
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Pick a random direction left or right and re-enable movement
        Ghost.Movement.SetDirection(new Vector2(Random.value < 0.5f ? -1f : 1f, 0f), true);
        Ghost.Movement.Rigidbody.isKinematic = false;
        Ghost.Movement.enabled = true;
    }
}