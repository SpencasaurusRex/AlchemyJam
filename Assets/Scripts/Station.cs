﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Station : MonoBehaviour
{
    public int PlacedLayer = 2;
    public int TotalProgressNeeded = 5;

    public Ingredient IngredientPrefab;
    public StationType StationType;

    bool looked;

    Recipe _currentRecipe;

    Recipe CurrentRecipe
    {
        get => _currentRecipe;
        set
        {
            if (_currentRecipe == value) return;
            _currentRecipe = value;
            if (value == null)
            {
                stationCanvas.InvalidRecipe();
            }
            else
            {
                stationCanvas.ValidRecipe();
            }
        }
    }

    float _progress = 0;
    float Progress
    {
        get => _progress;
        set
        {
            if (_progress == value) return;
            _progress = value;
            stationCanvas.ProgressChange(value / TotalProgressNeeded);
        }
    }

    public static List<Recipe> recipes;

    StationCanvas stationCanvas;

    List<Ingredient> ingredients = new List<Ingredient>();
    Canvas canvas;

    static Station()
    {
        recipes = new List<Recipe>
        {
            new Recipe(
                StationType.Station1,
                IngredientType.Purple,
                0f,
                IngredientType.Red, IngredientType.Blue),
            new Recipe(
                StationType.Station1,
                IngredientType.Yellow,
                0f,
                IngredientType.Red, IngredientType.Green),
            new Recipe(
                StationType.Station2,
                IngredientType.Orange,
                10f,
                IngredientType.Red, IngredientType.Yellow),
            new Recipe(
                StationType.Station2,
                IngredientType.Cyan,
                15f,
                IngredientType.Blue, IngredientType.Green),
            new Recipe(
                StationType.Station3,
                IngredientType.YellowGreen,
                0f,
                IngredientType.Yellow, IngredientType.Green)
        };
    }

    void Start()
    {
        canvas = GetComponentInChildren<Canvas>();
        canvas.enabled = false;
        stationCanvas = GetComponentInChildren<StationCanvas>();
        stationCanvas.ProgressChange(0);
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

        CheckRecipe();
    }

    void CheckRecipe()
    {
        var ingredientTypes = ingredients.Select(i => i.Type).ToList();
        CurrentRecipe = recipes.FirstOrDefault(x => x.Matches(StationType, ingredientTypes));
    }

    public bool GrabIngredient(out Ingredient ingredient)
    {
        Progress = 0;
        if (ingredients.Any())
        {
            ingredient = ingredients.Last();
            ingredients.RemoveAt(ingredients.Count - 1);
            CheckRecipe();
            return true;
        }

        ingredient = null;
        return false;
    }

    void Update()
    {
        // Mash
        if (StationType == StationType.Station1 && CurrentRecipe != null)
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
                Progress++;
                if (Progress >= TotalProgressNeeded)
                {
                    Complete();
                }
            }
        }

    }

    void Complete()
    {
        Recipe recipe = recipes.First(x => x.Matches(StationType, ingredients.Select(i => i.Type).ToList()));
        var output = recipe.Output;
        foreach (var ingredient in ingredients)
        {
            Destroy(ingredient.gameObject);
        }

        ingredients.Clear();

        var newIngredient = Instantiate(IngredientPrefab, transform.position, Quaternion.identity);
        newIngredient.Type = output;
        newIngredient.Setup();
        newIngredient.SpriteRenderer.color = Color.black;

        Progress = 0;

        AddIngredient(newIngredient);
    }
}

public enum StationType
{
    Station1,
    Station2,
    Station3
}


public class Recipe
{
    public StationType Station;
    public List<IngredientType> Inputs;
    public IngredientType Output;
    public float TimeToProcess;

    public Recipe(StationType station, IngredientType output, float timeToProcess,
        params IngredientType[] inputs)
    {
        Station = station;
        Output = output;
        TimeToProcess = timeToProcess;
        Inputs = inputs.OrderBy(x => x).ToList();
    }

    public bool Matches(StationType station, List<IngredientType> inputs)
    {
        if (Station != station) return false;
        if (Inputs.Count != inputs.Count) return false;

        for (int i = 0; i < Inputs.Count; i++)
        {
            if (Inputs[i] != inputs[i]) return false;
        }

        return true;
    }
}