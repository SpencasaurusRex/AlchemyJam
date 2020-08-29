using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float Speed;
    public float DashSpeed;
    public float Threshold;

    public Sprite[] FacingSprites;

    public SpriteRenderer Visuals;

    Rigidbody2D rb;
    Transform grabbable;

    // Gizmos variables
    Vector2 gizmos_position;
    Vector2 gizmos_size;

    int _facing;
    int facing
    {
        get => _facing;
        set
        {
            if (_facing == value) return;
            _facing = value;
            Visuals.sprite = FacingSprites[_facing];
        }
    }

    Vector3 FacingVector
    {
        get
        {
            if (facing == 0) return new Vector2(1, 0);
            if (facing == 1) return new Vector2(0, 1);
            if (facing == 2) return new Vector2(-1, 0);
            if (facing == 3) return new Vector2(0, -1);
            return Vector2.zero;
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        // Determine facing
        {
            float xa = Mathf.Abs(x);
            float ya = Mathf.Abs(y);

            if (xa >= ya)
            {
                if (x > Threshold)
                {
                    facing = 0;
                }
                else if (x < -Threshold)
                {
                    facing = 2;
                }
            }
            else
            {
                if (y > Threshold)
                {
                    facing = 1;
                }
                else if (y < -Threshold)
                {
                    facing = 3;
                }
            }
        }

        // Movement
        {
            Vector2 movement = new Vector2(x, y);
            
            // Dash
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                movement *= DashSpeed;
            }
            else
            {
                movement *= Speed;
            }
            rb.velocity = movement;
        }

        // Grab
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (grabbable)
            {
                Place();
            }
            else
            {
                Vector2 offset = new Vector2();
                if (facing == 0)
                {
                    offset.x = 1;
                }
                else if (facing == 1)
                {
                    offset.y = 1;
                }
                else if (facing == 2)
                {
                    offset.x = -1;
                }
                else if (facing == 3)
                {
                    offset.y = -1;
                }

                offset *= 0.5f;

                gizmos_position = rb.position + offset;
                Vector2 boxSize = new Vector2(0.8f, 0.8f); 
                gizmos_size = boxSize;
            
                Collider2D[] grabbables = Physics2D.OverlapBoxAll(rb.position + offset, boxSize, 0, LayerMask.GetMask("Grabbable"));
                if (grabbables.Any())
                {
                    var closestGrabbable = grabbables.OrderBy(g => (g.transform.position - (Vector3)gizmos_position).sqrMagnitude).First();
                    Grab(closestGrabbable);
                }   
            }
        }

        // Move grabbable
        if (grabbable) {
            Vector2 offset = Vector2.zero;
            switch (facing)
            {
                case 0: offset = new Vector2(0.3f, -0.25f);
                    break;
                case 1: offset = new Vector2(-0.3f, 0.25f);
                    break;
                case 2: offset = new Vector2(-0.3f, -0.25f);
                    break;
                case 3: offset = new Vector2(-0.3f, -0.25f);
                    break;
            }
            grabbable.position = transform.position + (Vector3)offset;
        }
    }

    void Grab(Collider2D collider)
    {
        collider.enabled = false;
        grabbable = collider.transform;
        grabbable.localScale = Vector3.one * 0.5f;
    }

    void Place()
    {
        grabbable.localScale = Vector3.one;
        grabbable.GetComponent<Collider2D>().enabled = true;
        grabbable.position = transform.position + FacingVector;
        grabbable = null;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawCube(gizmos_position, gizmos_size);
    }
}
