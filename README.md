# MarchingCubes
An algorithm that draws polygons around a 3D noise function

Explination:


### Generate Chunks
- Run through every cube in a chunk xyz
  - A cube is made of 8 virtices and 12 edges
  - Scale defines how many cubes in a chunk
  - Get the surface
  - run marching cubes algorithm on single cube
  - with generated vertice and triangle lists, update the mesh
  
### March on a Single Cube


