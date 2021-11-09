using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Generates a noise map that updates every second, useful for testing what combinations of noise look like
public class NoiseMap : MonoBehaviour
{
    public bool update;

    public Material material;
    public int scale;
    
    SimplexNoiseGenerator noise = new SimplexNoiseGenerator("1");
    Texture2D texture;

    public float darkness;

    [Header("Noise Settings")]
    public int octaves = 1;
    public int multiplier = 25;
    public float amplitude = 0.5f;
    public float lacunarity = 2f;
    public float persistance = 0.9f;

    private void Start()
    {
        InvokeRepeating("SlowUpdate", 0.0f, 1);
    }
    void SlowUpdate()
    {
        if (update)
        {
            texture = new Texture2D(scale, scale, TextureFormat.RGB24, true);
            float stepSize = 1f / scale;

            for (int y = 0; y < scale; y++)
            {
                for (int x = 0; x < scale; x++)
                {
                    float noiseVal = (noise.coherentNoise(x, y, 0, octaves, multiplier, amplitude, lacunarity) * darkness + 1) / 2;
                    texture.SetPixel(x, y, Color.white * noiseVal);
                    texture.Apply();

                    material.mainTexture = texture;
                }
            }
        }
    }
}
