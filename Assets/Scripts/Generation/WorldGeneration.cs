using UnityEngine;
using System.Collections;
using SimplexNoise;

public class WorldGeneration {

	public Chunk GenerateChunk(World world, Chunk chunk)
    {
        for(int x = chunk.ChunkPosition.x; x < chunk.ChunkPosition.x + Chunk.ChunkSize; x++) 
        {
            for (int z = chunk.ChunkPosition.z; z < chunk.ChunkPosition.z + Chunk.ChunkSize; z++)
            {
                chunk = ChunkColumnGen(chunk, x, z);
            }
        }
        chunk.generated = true;
        return chunk;
    }

    float stoneBaseHeight = -24;
    float stoneBaseNoise = 0.05f;
    float stoneBaseNoiseHeight = 4;

    float stoneMountainHeight = 48;
    float stoneMountainFrequency = 0.008f;
    float stoneMinHeight = -12;

    public Chunk ChunkColumnGen(Chunk chunk, int x, int z)
    {
        int stoneHeight = Mathf.FloorToInt(stoneBaseHeight);
        stoneHeight += GetNoise(x, 0, z, stoneMountainFrequency, Mathf.FloorToInt(stoneMountainHeight));

        if (stoneHeight < stoneMinHeight)
            stoneHeight = Mathf.FloorToInt(stoneMinHeight);

        stoneHeight += GetNoise(x, 0, z, stoneBaseNoise, Mathf.FloorToInt(stoneBaseNoiseHeight));

        for (int y = chunk.ChunkPosition.y; y < chunk.ChunkPosition.y + Chunk.ChunkSize; y++)
        {
            if (y <= stoneHeight)
            {
                chunk.AddVoxel(x, y, z, new Voxel());
            }

        }

        return chunk;
    }

    public static int GetNoise(int x, int y, int z, float scale, int max)
    {
        return Mathf.FloorToInt((Noise.Generate(x * scale, y * scale, z * scale) + 1f) * (max / 2f));
    }
}
