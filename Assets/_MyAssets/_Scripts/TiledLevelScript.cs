using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TiledLevelScript : MonoBehaviour
{
    [SerializeField] private Tilemap[] tileMaps;
    [SerializeField] private TileBase[] tileBases; // Now, loading this will be dynamic.
    [SerializeField] private char[] tileKeys;
    [SerializeField] private char[] tileObstacles;
    private int rows = 24; // Y-axis.
    private int cols = 32; // X-axis.

    [SerializeField] private Color targetColor;
    [SerializeField] private Color[] targetColors;
    [SerializeField] private float transitionDuration;
    [SerializeField] private float pauseDuration;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        StartCoroutine(ChangeBackgroundColor());
        LoadLevel();
    }

    IEnumerator ChangeBackgroundColor()
    {
        while(true) 
        {
            // Starting our transition to a new color.
            Color startColor = cam.backgroundColor;

            // Set a target color from the presets.
            do
            {
                targetColor = targetColors[UnityEngine.Random.Range(0, targetColors.Length)];
            } while (targetColor == startColor);
            // Calculate the color increment for each channel.
            float rIncrement = (targetColor.r - startColor.r) / transitionDuration;
            float gIncrement = (targetColor.g - startColor.g) / transitionDuration;
            float bIncrement = (targetColor.b - startColor.b) / transitionDuration;

            // Transition to the target color.
            float elapsedTime = 0f;
            while(elapsedTime < transitionDuration)
            {
                Color newColor = new Color(
                    startColor.r + rIncrement * elapsedTime,
                    startColor.g + gIncrement * elapsedTime,
                    startColor.b + bIncrement * elapsedTime,
                    cam.backgroundColor.a); // or 1f for alpha.
                cam.backgroundColor = newColor;
                elapsedTime += Time.deltaTime;
                yield return null; // No waiting, just pass control back.
            }
            cam.backgroundColor = targetColor;
            // Wait for a pause before starting a new loop.
            yield return new WaitForSeconds(pauseDuration);

        }
    }

    private void LoadLevel()
    {
        try
        {
            LoadAndSortTileBases();
            // Load tile data first.
            using (StreamReader reader = new StreamReader("Assets/TileData.txt"))
            {
                // Read all tile chars and create an array from it.
                string line = reader.ReadLine();
                tileKeys = line.ToCharArray();
                // Next is the obstacle tiles.
                line = reader.ReadLine();
                tileObstacles = line.ToCharArray();
                // We can also do the hazards. Next time.
            }
            // Then load level data.
            using (StreamReader reader = new StreamReader("Assets/Level1.txt"))
            {
                GetRowsAndColumns();
                string line;
                for (int row = 1; row < rows+1; row++)
                {
                    line = reader.ReadLine();
                    for (int col = 0; col < cols; col++)
                    {
                        char c = line[col];
                        if (c == '*') continue; // Skip if sky tile.

                        int charIndex = Array.IndexOf(tileKeys, c);
                        if (charIndex == -1) throw new Exception("Index not found.");
                        // Check if tile is obstacle or normal.
                        if (Array.IndexOf(tileObstacles, c) > -1) // Tile is obstacle.
                        {
                            SetTile(0, charIndex, col, row);
                        }
                        else // Tile is normal.
                        {
                            SetTile(1, charIndex, col, row);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    private void GetRowsAndColumns()
    {
        // Read all lines from the text file.
        string[] lines = File.ReadAllLines("Assets/Level1.txt");

        // Check if any lines were read.
        if (lines.Length == 0) return;  // Early-exit if file is empty
        rows = lines.Length; // Count how many elements in array - number of lines.
        cols = lines[0].Length; // Count how many chars in a line.
    }

    private void LoadAndSortTileBases()
    {
        // Load all TileBases into array.
        tileBases = Resources.LoadAll<TileBase>("TileBases");
        // Now sort the tiles.
        // Short version
        Array.Sort(tileBases, CompareTileNames);
    }

    private int CompareTileNames(TileBase x, TileBase y)
    {
        int numberX = ExtractNumber(x.name); // Tiles_0 -> 0
        int numberY = ExtractNumber(y.name);
        return numberX.CompareTo(numberY);
    }

    private int ExtractNumber(string name)
    {
        string numberString = ""; // A temp field that holds just the number in the name.
        foreach (char c in name) 
        {
            if(Char.IsDigit(c))
            {
                numberString += c; 
            }
        }
        return Int32.Parse(numberString); // Convert the new string of just digits, to an int.
    }

    private void SetTile(int tileMapIndex, int charIndex, int col, int row)
    {
        // Check all tilemaps to see if there's a manually-painted tile there.
        foreach (Tilemap tilemap in tileMaps)
        {
            if (tilemap.HasTile(new Vector3Int(col, -row, 0))) return;
        }
        // If no tile, then set the tile in the desired tilemap.
        tileMaps[tileMapIndex].SetTile(new Vector3Int(col, -row, 0), tileBases[charIndex]);
    }
}
