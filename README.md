# UnityFractalTerrain

# Introduction

Uses scripting to modify a Unity Terrain. Adds fractal terrain details.

![Image of unblended terrain](https://github.com/johnasharifi/UnityFractalTerrain/blob/master/terrain_fractal_unblended.png)

![Image of blended terrain](https://github.com/johnasharifi/UnityFractalTerrain/blob/master/terrain_fractal_blended.png)

# Implementation details

Phase I: seeds a 2D heightmap with mountain ranges. Each mountain range, when it finishes adding itself to the heightmap, recurses down and appends multiple smaller mountain ranges to itself. This procession of smaller mountain ranges has fractal properties.

Phase II: propagates data to completely fill the 2D heightmap using a flood-fill breadth-first-search algorithm.

# How to use

Create a Terrain gameObject in Unity. Attach "ScripTerrainAuto.cs". Specify a few colors for the terrain to be splat-mapped with. Enter play mode.
