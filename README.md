# MarchingCubes
An algorithm that draws polygons around a 3D noise function

## Explanation:

### Generate Chunks
- Run through every chunk
  - A chunk is a mesh which contains every cube vertice and triangle
  - A vertice is a location in space with coordinates X Y Z
  - A triangle is a set of numbers that relate to which order to draw a triangle between vertices like connect the dots. The order a triangle is drawn dictates which way it faces. Ex: vertices (0, 5, 1) (2, 1, 3) (8, 9, 1) and triangles 0 1 2 which would draw a lines between vertice 0 to 1, 1 to 2, 2 to 3 
  - A cube is made of 8 vertices, 12 edges, and up to 5 triangles however not every vertice is recorded, only the ones with triangles related to them are
  - numChunks could be replaced with X Y Z variables for more control over chunk shape

### Generate Cubes
- Run through every cube in a chunk
  - Scale defines how many cubes in a chunk
  - Get surface code, note: in my code, I placed it in the generate cubes function because it was easier to pass offset variables in, however it would make more sense to be in march on a single cube
  - Run marching cubes algorithm on a single cube
  - With generated vertice and triangle lists, update the mesh

#### Get Surface Code
- Surface code represents which vertices are in and out of the surface
- Generate a cube of noise values for each vertex in a cube
  - In the noise function, pass in x y z data to grab the -1 to 1 value of that location
  - Cube is stored in an array of length 8
- Go through each value in the noise cube and test whether the value is below the surface, this defines if a vertex is in or out of the surface which 
  - helps you find which and where to place vertices
- Using bit operators, or the current code with 1 to make all the values 1 and then shift it left the current index adding however many zeroes
  - note: needs to be revised as it doesn't fully explain how this works 
  
### March on a Single Cube
- Runs the algorithm on a single cube and add the generated vertices and triangles to the chunk's lists
- Get the edges the surface intercepts by plugging the surface code into the lookup table: cubeEdgeFlags
  - this value is used to determine which vertices are needed to draw the triangles

#### Get Vertices
- For each of the 12 edges in  a cube
  - Test whether it is intercepted by the surface by grabbing the correct bit value from the generated edge flag code, note: this is necessary as all it does is make sure there are no vertices without triangles. Without it there would be many times the number of vertices however might still work
- For each X Y and Z in a Vector3 vertex
  - Add the current world coordinate offset
  - Add the vertice which is the first vertice in the edge by using lookup tables: vertexOffset and edgeConnection
  - Add offset * value from lookup table: edgeDirection, edgeDirection is which way the edge is pointing so an offset can be added to get the coordinate of the vertex
- Add to list of vertices
- If surface is not intersecting the edge, add an empty Vector3 
- Explanation: This gets the position of a vertex by finding which edge the surface intersected and then adding the amount offset to get where on that edge the surface intersected
- Notes: Offset is the distance between the surface and the vertex. However, it can be replaced by 0.5f like my code which makes the generation blockier and achieves a stylized look

#### Get Triangles
- Run through every possible triangle, there are 5 max possible triangles per cube
  - Test if the current triangle is a triangle in the lookup table: triTable, the table returns -1 if it is not a triangle
  - Inputting surface code into triTable returns which triangle configuration to use, surface code is a binary value which corresponds to an index in tritable, j * 3 gets the beginning of each triangle as a triangle is made of three vertices
  - Get the number of current vertices of the current cube
- Run through every point of a triangle, there are 3 points in a triangle
  - Get which vertice to add to the main list of vertices by inputting the output of triTable into the current list of vertices and add that to the list
  - Add the current number of vertices plus the winding order to the main list of triangles
  - Note: the winding order gets the order which each vertex of a triangle is drawn and adding the last vertex count offsets the drawing order so each triangle point matches up to each vertex

 
  

