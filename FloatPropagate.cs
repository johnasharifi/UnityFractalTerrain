using UnityEngine;
using System.Collections;
using System.Linq;
using System.Threading;

public static class FloatPropagate {
	public static void ForcePropagate(ref float[,] map, float offset = 0.1f) {
		int dim = map.GetLength (0);

		bool repeat = true;
		while (repeat) {
			repeat = false;
			for (int i = 0; i < dim; i++) {
				for (int j = 0; j < dim; j++) {
					if (map [i, j] == 0f) {
						repeat = true;
						int adj_i = (i + Random.Range (-1, 2) + dim) % dim;
						int adj_j = (j + Random.Range (-1, 2) + dim) % dim;

						map [i, j] = Mathf.Clamp01 (map [adj_i, adj_j] * Random.Range (1 - offset, 1 + offset));
						}
					}
				}

			}

		}


	public static void Blur(ref float[,] map) {
		int dim = map.GetLength (0);

		for (int i = 0; i < dim; i++) {
			for (int j = 0; j < dim; j++) {
				int adj_i = (i + Random.Range (-1, 2) + dim) % dim;
				int adj_j = (j + Random.Range (-1, 2) + dim) % dim;

				map [i, j] = (map [i, j] + map [adj_i, adj_j]) / 2f;
				}
			}
		}

	public static void Amplify(ref float[,] map) {
		int dim = map.GetLength (0);
		float[,] map_2x = new float[dim * 2, dim * 2];

		for (int i = 0; i < dim; i++) {
			for (int j = 0; j < dim; j++) {
				map_2x [i * 2, j * 2] = map [i, j];
				}
			}
		map = map_2x;
		}

	public static Texture2D HeightToTexture(ref float[,] map, Color[] color_lut) {
		int dim = map.GetLength (0);
		int hcount = color_lut.Length - 2;

		Texture2D tex = new Texture2D (dim, dim);

		for (int i = 0; i < dim; i++) {
			for (int j = 0; j < dim; j++) {
				int prev_index = (int) (map[i,j] * hcount);
				int next_index = prev_index + 1;
				Color c1_color = Color.Lerp(color_lut[prev_index], color_lut[next_index], (map[i,j] % (1f / hcount)) * hcount);

				tex.SetPixel (i, j, c1_color);
				}
			}

		tex.Apply ();
		return(tex);
		}

	public static Texture2D TextureToNormal(ref Texture2D tex, float strength = 1f) {
		Texture2D normalTexture;
		float xLeft;
		float xRight;
		float yUp;
		float yDown;
		float yDelta;
		float xDelta;

		normalTexture = new Texture2D (tex.width, tex.height, TextureFormat.ARGB32, true);

		for (int y=0; y<normalTexture.height; y++) 
		{
			for (int x=0; x<normalTexture.width; x++) 
			{
				xLeft = tex.GetPixel(x-1,y).grayscale*strength;
				xRight = tex.GetPixel(x+1,y).grayscale*strength;
				yUp = tex.GetPixel(x,y-1).grayscale*strength;
				yDown = tex.GetPixel(x,y+1).grayscale*strength;
				xDelta = ((xLeft-xRight)+1)*0.5f;
				yDelta = ((yUp-yDown)+1)*0.5f;
				normalTexture.SetPixel(x,y,new Color(xDelta,yDelta,1.0f,yDelta));
				}
			}
		normalTexture.Apply();

		//Code for exporting the image to assets folder
		return normalTexture;
		}

	}
