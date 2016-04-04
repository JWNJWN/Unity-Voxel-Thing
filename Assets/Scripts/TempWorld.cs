using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TempWorld : MonoBehaviour {

    public Octree<Voxel> voxels;

    public Vector3 chunkGenerationPos;

    public bool generate = true;

    public GameObject chunkPrefab;

    public new IRenderer renderer;
    public bool march = false;

    void Start()
    {
        voxels = new Octree<Voxel>(64, Vector3.zero, 8);
        for (int x = 0; x < 16; x++)
        {
            for (int y = 0; y < 16; y++)
            {
                for (int z = 0; z < 16; z++)
                {
                        voxels.Add(new Voxel(), new Vector3(x, y, z));
                }
            }
        }
        Debug.Log("Generated");
    }

    void Update()
    {
        if (generate)
        {
            generate = false;
            voxels.Remove(chunkGenerationPos);
        }
    }

    void OnDrawGizmos()
    {
        if(voxels != null)
        {
            voxels.DrawAllBounds();
            voxels.DrawAllPoints();
        }
    }

    /*public Voxel GetVoxel(int x, int y, int z)
    {
        Chunk chunk = GetChunk(x, y, z);
        if(chunk != null)
            return chunk.GetVoxel(x, y, z);
        return null;
    }

    public void SetVoxel(int x, int y, int z, Voxel voxel)
    {
        Chunk chunk = GetChunk(x, y, z);
        if(chunk != null)
            chunk.SetVoxel(x, y, z, voxel);
    }*/
}
