using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Station : MonoBehaviour
{
    public int PlacedLayer = 2;
    public int TotalProgressNeeded = 5;

    public Ingredient IngredientPrefab;

    bool looked = false;
    int progress = 0;
    StationCanvas stationCanvas;

    public enum InputType
    {
        Mash,
        Hold
    }

    List<Ingredient> ingredients = new List<Ingredient>();
    Canvas canvas;

    void Start()
    {
        canvas = GetComponentInChildren<Canvas>();
        canvas.enabled = false;
        stationCanvas = GetComponentInChildren<StationCanvas>();
    }

    public void PlayerLooking()
    {
        canvas.enabled = true;
        looked = true;
    }

    public void PlayerStoppedLooking()
    {
        canvas.enabled = false;
        looked = false;
    }
    public void AddIngredient(Ingredient ingredient)
    {
        ingredients.Add(ingredient);
        ingredient.Collider2D.enabled = false;

        int index = ingredients.Count - 1;
        Vector3 offset = Vector3.zero;
        if (index == 0) offset = new Vector3(-1, 1, 0);
        if (index == 1) offset = new Vector3(1, 1, 0);
        if (index == 2) offset = new Vector3(-1, -1, 0);
        if (index == 3) offset = new Vector3(1, -1, 0);

        ingredient.transform.position = transform.position + offset * 0.25f;

        ingredient.transform.localScale = Vector3.one * 0.5f;
        ingredient.SpriteRenderer.sortingOrder = PlacedLayer;
    }

    public bool GrabIngredient(out Ingredient ingredient)
    {
        progress = 0;
        if (ingredients.Any())
        {
            ingredient = ingredients.Last();
            ingredients.RemoveAt(ingredients.Count - 1);
            return true;
        }

        ingredient = null;
        return false;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.E))
        {
            stationCanvas.KeyPressed();
        }
        else
        {
            stationCanvas.KeyUnpressed();
        }

        if (looked && ingredients.Any() && Input.GetKeyDown(KeyCode.E))
        {
            progress++;
            if (progress >= TotalProgressNeeded)
            {
                Complete();
            }
        }
    }

    void Complete()
    {
        foreach (var ingredient in ingredients)
        {
            Destroy(ingredient.gameObject);
        }

        ingredients.Clear();

        var newIngredient = Instantiate(IngredientPrefab, transform.position, Quaternion.identity);
        newIngredient.Setup();
        newIngredient.SpriteRenderer.color = Color.black;

        progress = 0;

        AddIngredient(newIngredient);
    }
}
