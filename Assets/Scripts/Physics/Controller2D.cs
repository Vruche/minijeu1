using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class Controller2D : RaycastController
{
    private Rigidbody2D rb2d;

    private float maxSlopeAngle = 55;

    public CollisionInfo collisions;
    [HideInInspector]
    public Vector2 input;

    [System.Serializable]
    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public bool climbingSlope;
        public bool descendingSlope;
        public bool slidingDownMaxSlope;

        public bool fallingFromPlatform; // used to fall straight when walking off a platform edge

        public float slopeAngle, prevSlopeAngle;
        public Vector2 slopeNormal;
        public Vector2 prevVelocity;

        public int faceDir;

        public bool fallingThroughPlatform;

        public void Reset() // called at the beginning of each move
        {
            above = below = false;
            left = right = false;
            climbingSlope = false;
            descendingSlope = false;
            slidingDownMaxSlope = false;
            slopeNormal = Vector2.zero;
            prevSlopeAngle = slopeAngle;
            slopeAngle = 0;
        }
    }

    protected override void Start()
    {
        base.Start();
        Init();
        rb2d = GetComponent<Rigidbody2D>();
    }

    public void Init()
    {
        collisions.Reset();
        collisions.faceDir = 1;
        collisions.fallingFromPlatform = false;
        collisions.fallingThroughPlatform = false;
        collisions.prevVelocity = Vector2.zero;
    }

    public void Move(Vector2 deltaMove, bool standingOnPlatform = false)
    {
        Move(deltaMove, Vector2.zero, standingOnPlatform);
    }

    public void Move(Vector2 deltaMove, Vector2 input_, bool standingOnPlatform = false)
    {
        bool prevBelow = collisions.below;

        UpdateRaycastOrigins();

        collisions.Reset();
        collisions.prevVelocity = deltaMove;


        input = input_;
        
        if (deltaMove.y < 0)
        {
            DescendSlope(ref deltaMove);
        }

        if (deltaMove.x != 0) // must be after slope checks
        {
            collisions.faceDir = (int) Mathf.Sign(deltaMove.x);
        }
        if (collisions.fallingFromPlatform) {
            deltaMove.x = 0;
        }

        HorizontalCollisions(ref deltaMove);

        if (deltaMove.y != 0)
        {
            VerticalCollisions(ref deltaMove);
        }
        

        if (prevBelow && !(collisions.below || standingOnPlatform) && deltaMove.y <= 0) {
            collisions.fallingFromPlatform = true;
        }

        rb2d.MovePosition(rb2d.position + deltaMove);

        if (standingOnPlatform)
        {
            collisions.below = true;
        }
        if (collisions.fallingFromPlatform && collisions.below) 
        {
            collisions.fallingFromPlatform = false;
        }
    }

    void HorizontalCollisions(ref Vector2 deltaMove)
    {
        float directionX = collisions.faceDir;
        float rayLength = Mathf.Abs(deltaMove.x) + skinWidth;

        if (Mathf.Abs(deltaMove.x) < skinWidth)
        {
            rayLength = 2 * skinWidth;
        }

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.red);

            if (hit)
            {
                if (hit.collider.tag == "OneWay") {
                    continue; // ignore horizontal collisions on oneway platforms
                }
                if (hit.distance == 0)
                {
                    continue; // next iteration
                }

                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (i == 0 && slopeAngle <= maxSlopeAngle)
                {
                    if (collisions.descendingSlope) // avoid slowing down on transition between descending/climbing slopes
                    {
                        collisions.descendingSlope = false;
                        deltaMove = collisions.prevVelocity;
                    }

                    float distanceToSlopeStart = 0;
                    if (slopeAngle != collisions.prevSlopeAngle)
                    {
                        distanceToSlopeStart = hit.distance - skinWidth;
                        deltaMove.x -= distanceToSlopeStart * directionX;
                    }

                    ClimbSlope(ref deltaMove, slopeAngle, hit.normal);

                    deltaMove.x += distanceToSlopeStart * directionX;
                }

                if (!collisions.climbingSlope || slopeAngle > maxSlopeAngle)
                {
                    deltaMove.x = (hit.distance - skinWidth) * directionX;
                    rayLength = hit.distance; // max horizontal distance adjusted for next iteration

                    if (collisions.climbingSlope)
                    {
                        deltaMove.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(deltaMove.x);
                    }

                    collisions.left = directionX == -1;
                    collisions.right = directionX == 1;
                }
            }
        }
    }

    void VerticalCollisions(ref Vector2 deltaMove)
    {
        float directionY = Mathf.Sign(deltaMove.y);
        float rayLength = Mathf.Abs(deltaMove.y) + skinWidth;

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + deltaMove.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.red);

            if (hit)
            {
                if (hit.collider.tag == "OneWay")
                {
                    if (directionY == 1 || hit.distance == 0)
                    {
                        continue;
                    }
                    //if (collisions.fallingThroughPlatform)
                    //{
                    //    continue;
                    //}
                    //if (input.y == 1)
                    //{
                    //    Debug.Log("yo");
                    //    collisions.fallingThroughPlatform = true;
                    //    Invoke("ResetFallingThroughPlatform", .5f);
                    //    continue;
                    //}
                }

                deltaMove.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance; // max vertical distance adjusted for next iteration

                if (collisions.climbingSlope)
                {
                    deltaMove.x = deltaMove.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(deltaMove.x);
                }

                collisions.below = directionY == -1;
                collisions.above = directionY == 1;
            }
        }

        if (collisions.climbingSlope) // check new slope pb corner case
        {
            float directionX = Mathf.Sign(deltaMove.x);
            rayLength = Mathf.Abs(deltaMove.x) + skinWidth;
            Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * deltaMove.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            if (hit)
            {
                if (hit.collider.tag != "OneWay") { // ignore horizontal collisions on oneway platforms

                    float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                    if (slopeAngle != collisions.slopeAngle) // new slope detected
                    {
                        deltaMove.x = (hit.distance - skinWidth) * directionX;
                        collisions.slopeAngle = slopeAngle;
                    }
                }
            }
        }
    }

    void ClimbSlope(ref Vector2 deltaMove, float slopeAngle, Vector2 slopeNormal)
    {
        float moveDistance = Mathf.Abs(deltaMove.x); // distance on ground (hypothénuse)
        float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

        if (deltaMove.y <= climbVelocityY) // if NOT jumping on slope
        {
            deltaMove.y = climbVelocityY;
            deltaMove.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(deltaMove.x);
            collisions.below = true;
            collisions.climbingSlope = true;
            collisions.slopeAngle = slopeAngle;
            collisions.slopeNormal = slopeNormal;
        }
    }

    void DescendSlope(ref Vector2 deltaMove)
    {
        RaycastHit2D maxSlopeHitLeft = Physics2D.Raycast(raycastOrigins.bottomLeft, Vector2.down, Mathf.Abs(deltaMove.y) + skinWidth, collisionMask);
        RaycastHit2D maxSlopeHitRight = Physics2D.Raycast(raycastOrigins.bottomRight, Vector2.down, Mathf.Abs(deltaMove.y) + skinWidth, collisionMask);
        if (maxSlopeHitLeft ^ maxSlopeHitRight)
        {
            SlideDownMaxSlope(maxSlopeHitLeft, ref deltaMove);
            SlideDownMaxSlope(maxSlopeHitRight, ref deltaMove);
        }

        if (!collisions.slidingDownMaxSlope)
        {
            float directionX = Mathf.Sign(deltaMove.x);
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft; // corner touching the slope
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, Mathf.Infinity, collisionMask);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != 0 && slopeAngle <= maxSlopeAngle)
                {
                    if (Mathf.Sign(hit.normal.x) == directionX) // moving down the slope
                    {
                        if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(deltaMove.x)) // are we close enough to the slope
                        {
                            float moveDistance = Mathf.Abs(deltaMove.x); // distance on ground (hypothénuse)
                            float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                            deltaMove.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(deltaMove.x);
                            deltaMove.y -= descendVelocityY;

                            collisions.slopeAngle = slopeAngle;
                            collisions.slopeNormal = hit.normal;
                            collisions.descendingSlope = true;
                            collisions.below = true;
                        }
                    }
                }
            }
        }
    }

    void SlideDownMaxSlope(RaycastHit2D hit, ref Vector2 deltaMove)
    {
        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

            if (slopeAngle > maxSlopeAngle)
            {
                deltaMove.x = Mathf.Sign(hit.normal.x) * (Mathf.Abs(deltaMove.y) - hit.distance) / Mathf.Tan(slopeAngle * Mathf.Deg2Rad);

                collisions.slopeAngle = slopeAngle;
                collisions.slopeNormal = hit.normal;
                collisions.slidingDownMaxSlope = true;
            }
        }
    }

    void ResetFallingThroughPlatform()
    {
        collisions.fallingThroughPlatform = false;
    }
}

