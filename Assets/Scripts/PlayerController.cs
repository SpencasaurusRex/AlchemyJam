using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float Speed;
    public float DashSpeed;
    public float Threshold;

    public Sprite[] FacingSprites;

    public SpriteRenderer Visuals;

    Rigidbody2D rb;

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
        {
            Vector2 boxSize = new Vector2(0.5f, 0.5f); 
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Vector2 offset = new Vector2();
                Physics2D.OverlapBox(rb.position, boxSize, 0);
            }
        }
    }
}
