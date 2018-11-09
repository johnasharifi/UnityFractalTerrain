using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptTerrainAuto : MonoBehaviour {
	public Color[] color_lut;

	private SplatPrototype[] splats;

	private int twidth = 512;

	private Terrain t;
	private TerrainData tdata;

	// Use this for initialization
	void Start () {
		Boot ();
		}
	
	// Update is called once per frame
	void Update () {
		
		}

	void OnApplicationQuit() {
		tdata.alphamapResolution = tdata.heightmapResolution = 32;
		}

	void Boot() {
		t = GetComponent<Terrain> ();
		tdata = t.terrainData;
		tdata.splatPrototypes = SplatsFromColors (color_lut);
		tdata.alphamapResolution = tdata.heightmapResolution = twidth;

		float[,] heightmap = new float[twidth, twidth];
		SeedHeightmap (ref heightmap);

		for (int i = 0; i < 5; i++)
		FloatPropagate.Blur (ref heightmap);

		tdata.SetHeights (0, 0, heightmap);
		tdata.SetAlphamaps (0, 0, SplatValuesFromTdata(ref tdata));
		}

	void SeedHeightmap(ref float[,] heightmap) {
		int xdim = heightmap.GetLength (0);
		int ydim = heightmap.GetLength (1);
		int seed_count = 3;

		for (int dum = 0; dum < seed_count; dum++) {
			int xpos = Random.Range (0, xdim);
			int ypos = Random.Range (0, ydim);

			SpinDown (ref heightmap, xpos, ypos, 1f * dum / seed_count, 5, Random.Range (0f, Mathf.PI * 2f), xdim);
			}

		FloatPropagate.ForcePropagate (ref heightmap, offset: 0.05f);
		for (int i = 0; i < 5; i++) {
			FloatPropagate.Blur (ref heightmap);
			}
		}

	void SpinDown(ref float[,] heightmap, int pos_x, int pos_y, float initial_value, int n, float theta, float r) {
		if (n <= 1)
			return;

		int dim = heightmap.GetLength (0);

		float inst_theta = theta;
		float inst_value = initial_value;

		for (int q = 3; q < n; q++) {
			inst_theta = inst_theta + Random.Range (-0.2f, 0.2f);
			inst_value = inst_value * 1f;

			pos_x = (int) ((pos_x + r / n * Mathf.Cos (inst_theta) + dim) % dim);
			pos_y = (int) ((pos_y + r / n * Mathf.Sin (inst_theta) + dim) % dim);

			SpinDown (ref heightmap, pos_x, pos_y, inst_value, n - 1, inst_theta + Mathf.PI / 10, r * 0.5f);
			SpinDown (ref heightmap, pos_x, pos_y, inst_value, n - 1, inst_theta - Mathf.PI / 10, r * 0.5f);

			heightmap [pos_x, pos_y] = inst_value;
			}

		}

	private SplatPrototype[] SplatsFromColors(Color[] colors) {
		SplatPrototype[] splat_arr = new SplatPrototype[colors.Length];

		for (int i = 0; i < colors.Length; i++) {
			splat_arr [i] = new SplatPrototype ();
			Texture2D tex = new Texture2D (1, 1);
			tex.SetPixel (0, 0, colors [i]);
			tex.Apply ();

			splat_arr [i].texture = tex;
			}

		return(splat_arr);
		}

	private float[,,] SplatValuesFromTdata(ref TerrainData td) {
		int dim = td.heightmapWidth - 1;
		float[,] heightmap = td.GetHeights (0, 0, dim, dim);
		float[,,] alpha_values = new float[dim, dim, td.alphamapLayers];

		float denom = 1f / td.alphamapLayers;

		float perl_coeff = 5f;

		for (int i = 0; i < dim - 1; i++) {
			for (int j = 0; j < dim - 1; j++) {
				float a = Mathf.Clamp(heightmap [i, j] * td.alphamapLayers, 0, td.alphamapLayers);

				int color_index = Mathf.Clamp ((int)a, 0, td.alphamapLayers - 1);
				int color_index2 = Mathf.Clamp ((int)(a + 1), 0, td.alphamapLayers - 1);

				float lerp_alpha = Mathf.Max(0.6f, a - Mathf.Floor (a));

				alpha_values [i, j, color_index] = 1 - lerp_alpha;
				alpha_values [i, j, color_index2] = lerp_alpha;

				}
			}

		return(alpha_values);
		}

	
	}
