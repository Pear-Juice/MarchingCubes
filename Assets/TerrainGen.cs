using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGen : MonoBehaviour
{
    SimplexNoiseGenerator noise = new SimplexNoiseGenerator("2");
    Lookup lookup = new Lookup();

    public float surfaceLevel = 0; //level that dictates where the suface is drawn relative to the surface, if noise value is less than surface, its inside else outisde

    public int numChunks = 5;

    int scale = 25; //The width / height / length of a chunk

    [Space (20)]
    [Header("Noise Settings")]
    public int octaves = 1;
    public int multiplier = 25;
    public float amplitude = 0.5f;
    public float lacunarity = 2f;
    public float persistance = 0.9f;

    public Material material;


    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();

    private void Start()
    {
        for (int i = 0; i < numChunks * scale; i+= scale)
        {
            for (int j = 0; j < numChunks * scale; j+= scale)
            {
                for (int k = 0; k < numChunks * scale; k+= scale)
                {
                    generateChunk(i, j, k);
                }
            }
        }
    }

    public void generateChunk(int xOffset, int yOffset, int zOffset)
    {
        GameObject gameObject = new GameObject("Chunk: " + xOffset + " " + yOffset + " " + zOffset);
        MeshFilter  meshFilter = gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();

        Mesh mesh = new Mesh();

        meshRenderer.material = material;
        meshFilter.mesh = mesh;

        lookup.setWindingOrder(surfaceLevel); //to set the order unity draws triangles based on which vertices to draw from, a triangle drawn in backwards order faces the opposite direction

        for (int x = xOffset; x < scale + xOffset; x++)
        {
            for (int y = yOffset; y < scale + yOffset; y++)
            {
                for (int z = zOffset; z < scale + zOffset; z++)
                {
                    int surfaceCode = getSurface(x, y, z); //get code which is which vertices are inside or outside surface

                    generateVerticesAndTriangles(x, y, z, surfaceCode); //calculate where every vertice is and add it to the lists
                }
            }
        }


        transform.GetComponent<MeshFilter>().mesh = mesh;

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        vertices.Clear();
        triangles.Clear();
    }

    void generateVerticesAndTriangles(int x, int y, int z, int surfaceCode)
    {
        int edgeFlags = lookup.cubeEdgeFlags[surfaceCode]; //get list of edges intercepted or not
        int currentTrianglesToAdd;
        List<Vector3> currentVertices = new List<Vector3>();


        //If the cube is entirely inside or outside of the surface, then there will be no intersections
        if (edgeFlags == 0)
            return;

        for (int i = 0; i < 12; i++) //run through every edge
        {
            if ((edgeFlags & (1 << i)) != 0) //test if current edge is intercepted
            {
                Vector3 vertice = new Vector3(
                    x + lookup.vertexOffset[lookup.edgeConnection[i, 0], 0] + 0.5f * lookup.edgeDirection[i, 0], //generates where the surface intersects as a vertice and 
                    y + lookup.vertexOffset[lookup.edgeConnection[i, 0], 1] + 0.5f * lookup.edgeDirection[i, 1],
                    z + lookup.vertexOffset[lookup.edgeConnection[i, 0], 2] + 0.5f * lookup.edgeDirection[i, 2]
                    );

                currentVertices.Add(vertice);
            }
            else
            {
                currentVertices.Add(new Vector3());
            }
        }


        for (int j = 0; j < 5; j++) //run through every triangle (triangles have three indexes)
        {
            if (lookup.triTable[surfaceCode, 3 * j] < 0) //if asked for triangle is -1 / null, don't add the vertices to the list
                break;

            int lastNumVertices = vertices.Count;

            for (int k = 0; k < 3; k++) //run through every triangle index
            {
                currentTrianglesToAdd = lookup.triTable[surfaceCode, 3 * j + k]; //3 * i + j means grab the beginning of a triangle (3 * i) then add j to get which index in the triangle
                vertices.Add(currentVertices[currentTrianglesToAdd]); //add the vertices based on which triangle index
                triangles.Add(lastNumVertices + lookup.windingOrder[k]); //list of triangles is the order to draw triangles, last num vertices is the index of the last section of triangles added, so by adding winding order you are saying add which index of triangle to draw first plus the offset in the list
            }
        }
    }

    int getSurface(int x, int y, int z)
    {
        int flagIndex = 0;

        float[] noiseCube = generateNoiseCube(x, y, z);

        //generate binary code relating to which vertices are inside the surface
        for (int i = 0; i < 8; i++) //go though every noise value for each vertex location in a cube
            if (noiseCube[i] <= surfaceLevel) //test if noise value is below the surface
                flagIndex |= 1 << i; //make value 1 and then push binary value left one

        return flagIndex;
    }

    float[] generateNoiseCube(int x, int y, int z)
    {
        float[] cube = new float[8];
        int ix = 0;
        int iy = 0;
        int iz = 0;

        for (int i = 0; i < 8; i++)
        {
            //look up value in vertexOffset table relating to which vertice in a cube and add x y or z offset to that to get the world position of the cube
            ix = x + lookup.vertexOffset[i, 0];
            iy = y + lookup.vertexOffset[i, 1];
            iz = z + lookup.vertexOffset[i, 2];

            //set value of cube to generated noise value
            cube[i] = noise.coherentNoise(ix, iy, iz, octaves, multiplier, amplitude, lacunarity, persistance);
        }

        return cube;
    }
}

/*
 * Sources:
 * http://paulbourke.net/geometry/polygonise/ : General description of algorithm, tri and edge tables
 * https://github.com/Scrawk/Marching-Cubes : Someone elses implimentation of Marching Cubes
 * https://www.youtube.com/watch?v=vTMEdHcKgM4 (Sebastian Lague)
 * https://www.youtube.com/watch?v=M3iI2l0ltbE (Sebastian Lague)
 */
