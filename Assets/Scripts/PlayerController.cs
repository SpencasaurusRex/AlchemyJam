using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float Speed;
    public float DashSpeed;
    public float Threshold;
    public float DashDuration;
    public float DoneDashDuration;
    public float DoneDashSpeed;

    public int GrabbedLayer;
    public int PlacedLayer;

    public Sprite[] FacingSprites;

    public SpriteRenderer Visuals;

    Rigidbody2D rb;
    Transform grabbable;
    MovementState state = MovementState.Moving;
    Vector2 dashDirection;
    float stateTimeRemaining;

    Station _lookingStation;

    Station LookingStation
    {
        get => _lookingStation;
        set
        {
            if (_lookingStation == value) return;
            if (_lookingStation)
                _lookingStation.PlayerStoppedLooking();
            _lookingStation = value;
            if (value != null)
                _lookingStation.PlayerLooking();
        }
    }

    // Gizmos variables
    Vector2 gizmos_position;
    Vector2 gizmos_size;

    int _facing;
    int Facing
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
            if (Facing == 0) return new Vector2(1, 0);
            if (Facing == 1) return new Vector2(0, 1);
            if (Facing == 2) return new Vector2(-1, 0);
            if (Facing == 3) return new Vector2(0, -1);
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
        float xa = Mathf.Abs(x);
        float ya = Mathf.Abs(y);
        
        // Determine facing
        {
            if (xa >= ya)
            {
                if (x > Threshold)
                {
                    Facing = 0;
                }
                else if (x < -Threshold)
                {
                    Facing = 2;
                }
            }
            else
            {
                if (y > Threshold)
                {
                    Facing = 1;
                }
                else if (y < -Threshold)
                {
                    Facing = 3;
                }
            }
        }

        // Movement
        {
            Vector2 movement = new Vector2(x, y);
            
            // Dash
            if (state == MovementState.Moving &&
                Input.GetKeyDown(KeyCode.LeftShift) && 
                (xa > Threshold || ya > Threshold))
            {
                state = MovementState.Dashing;
                movement *= DashSpeed;
                dashDirection = new Vector2(x, y);
                stateTimeRemaining = DashDuration;
            }
            
            if (state == MovementState.Moving)
            {
                movement *= Speed;
            }
            else if (state == MovementState.DoneDash)
            {
                movement *= DoneDashSpeed;
                stateTimeRemaining -= Time.deltaTime;
                if (stateTimeRemaining <= 0)
                {
                    state = MovementState.Moving;
                }
            }
            else if (state == MovementState.Dashing)
            {
                movement = dashDirection * DashSpeed;

                // Dash is over
                stateTimeRemaining -= Time.deltaTime;
                if (stateTimeRemaining <= 0)
                {
                    state = MovementState.DoneDash;
                    stateTimeRemaining = DoneDashDuration;
                }
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
                Grab();
            }
        }

        // Move grabbable
        if (grabbable) {
            Vector2 offset = Vector2.zero;
            switch (Facing)
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

        // Look at stations
        {
            Vector2 position = transform.position + FacingVector * 0.5f;
            Vector2 size = Vector2.one * 0.8f;
            var stations = Physics2D.OverlapBoxAll(position, size, 0, LayerMask.GetMask("Station"));
            if (GetClosestOverlapBox(position, size, LayerMask.GetMask("Station"), transform.position, out var stationCollider))
            {
                var station = stationCollider.GetComponent<Station>();
                LookingStation = station;
            }
            else if (LookingStation)
            {
                LookingStation = null;
            }
        }
    }

    bool GetClosestOverlapBox(Vector2 position, Vector2 size, int mask, Vector3 comparePosition, out Collider2D result)
    {
        var objects = Physics2D.OverlapBoxAll(position, size, 0, mask);
        if (objects.Any())
        {
            result = objects.OrderBy(o => (o.transform.position - comparePosition).sqrMagnitude).First();
            return true;
        }

        result = null;
        return false;
    }

    void Grab()
    {
        bool pickedUp = false;
        if (LookingStation)
        {
            // Grab from station
            if (LookingStation.GrabIngredient(out var ingredient))
            {
                ingredient.Collider2D.enabled = false;
                grabbable = ingredient.transform;
                grabbable.localScale = Vector3.one * 0.5f;
                ingredient.SpriteRenderer.sortingOrder = GrabbedLayer;
                ingredient.SpriteRenderer.enabled = true;
                pickedUp = true;
            }
        }
        if (!pickedUp)
        {
            // Grab from ground
            Vector2 offset = FacingVector * 0.5f;

            gizmos_position = rb.position + offset;
            Vector2 boxSize = new Vector2(0.8f, 0.8f); 
            gizmos_size = boxSize;

            var position = rb.position + offset;
            int mask = LayerMask.GetMask("Grabbable");
            if (GetClosestOverlapBox(rb.position + offset, boxSize, mask, transform.position, out var itemCollider))
            {
                itemCollider.enabled = false;
                grabbable = itemCollider.transform;
                grabbable.localScale = Vector3.one * 0.5f;
                grabbable.GetComponent<SpriteRenderer>().sortingOrder = GrabbedLayer;
            }
        }
    }

    void Place()
    {
        if (LookingStation)
        {
            // Place in station
            grabbable.localScale = Vector3.one;
            // NOTE: This will fail when we can carry more than just ingredients
            var ingredient = grabbable.GetComponent<Ingredient>();
            LookingStation.AddIngredient(ingredient);
            grabbable = null;
        }
        else
        {
            // Place on ground
            grabbable.localScale = Vector3.one;
            grabbable.GetComponent<Collider2D>().enabled = true;
            grabbable.position = transform.position + FacingVector;
            grabbable.GetComponent<SpriteRenderer>().sortingOrder = PlacedLayer;
            grabbable = null;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawCube(gizmos_position, gizmos_size);
    }

    public enum MovementState
    {
        Moving,
        Dashing,
        DoneDash
    }
}
