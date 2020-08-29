using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Station : MonoBehaviour
{
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
    }

    public void PlayerLooking()
    {
        canvas.enabled = true;
    }

    public void PlayerStoppedLooking()
    {
        canvas.enabled = false;
    }
    public void AddIngredient(Ingredient ingredient)
    {
        ingredients.Add(ingredient);
        ingredient.SpriteRenderer.enabled = false;
        ingredient.Collider2D.enabled = false;
    }

    public bool GrabIngredient(out Ingredient ingredient)
    {
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
        
    }
}
