using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Controller2D : MonoBehaviour
{
    Collider2D playerCollider;

    public CollisionInfo collisions;
    int collisionLayer;

    private void Start()
    {
        playerCollider = GetComponent<Collider2D>();
        collisionLayer = LayerMask.GetMask("Collision");
    }

    public void Move(Vector3 velocity)
    {
        collisions.Reset();

        // Get all collidable objects with LayerMask
        Collider2D[] colliders = Physics2D.OverlapBoxAll(gameObject.transform.position, Vector2.one, 0, collisionLayer);

        // Check and resolve collisions
        foreach (Collider2D col in colliders)
        {
            if (AabbCollision(playerCollider, col))
            {
                AabbResolution(playerCollider, col, ref velocity);
            }
        }

        transform.Translate(velocity);
    }

    public bool AabbCollision(Collider2D a, Collider2D b)
    {
        Bounds boundsA = a.bounds;
        Bounds boundsB = b.bounds;

        if (boundsA.min.x < boundsB.max.x && boundsA.max.x > boundsB.min.x &&
            boundsA.min.y < boundsB.max.y && boundsA.max.y > boundsB.min.y)
        {
            return true;
        }

        return false;
    }

    public void AabbResolution(Collider2D a, Collider2D b, ref Vector3 velocity)
    {
        float directionY = Mathf.Sign(velocity.y);

        // ToDo: Optimize later idk
        Bounds boundsA = a.bounds;
        Bounds boundsB = b.bounds;

        // X collision
        float overlapOneX = Mathf.Abs(boundsA.max.x - boundsB.min.x);
        float overlapTwoX = Mathf.Abs(boundsA.min.x - boundsB.max.x);

        float shortestOverlapX = Mathf.Min(overlapOneX, overlapTwoX);

        // Y Collision
        float overlapOneY = Mathf.Abs(boundsA.min.y - boundsB.max.y);
        float overlapTwoY = Mathf.Abs(boundsA.max.y - boundsB.min.y);

        float shortestOverlapY = Mathf.Min(overlapOneY, overlapTwoY);

        // Determine whether the X vector or Y vector is smaller
        // If smaller determine which penetration is smaller to prevent tunneling
        
        if (shortestOverlapX < shortestOverlapY)
        {
            if (overlapOneX < overlapTwoX)
            {
                velocity.x -= shortestOverlapX;
            }
            else
            {
                velocity.x += shortestOverlapX;
            }
        }
        else
        {
            if (overlapOneY < overlapTwoY)
            {
                collisions.below = directionY == -1;
                velocity.y += shortestOverlapY;
            }
            else
            {
                velocity.y -= shortestOverlapY;
            }
        }
    }

    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public void Reset()
        {
            above = below = false;
            left = right = false;
        }
    }
}
