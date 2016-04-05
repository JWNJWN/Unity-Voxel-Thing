using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CielaSpike;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]

public class Chunk : MonoBehaviour {

    public Octree<Voxel> voxels;

    public static int ChunkSize = 64;
    int ChunkVolume;

    private MeshFilter filter;
    private MeshCollider coll;

    public bool Dirty = true;

    public World world;
    public WorldPos ChunkPosition;
    
    public bool generated = false;
    public bool rendered = false;
    public Chunk[] SurroundingChunks = new Chunk[6];
    //0-Up 1-Down 2-Left 3-Right 4-Front 5-Back

    public Vector3 voxelCoord = Vector3.zero;
    public bool remove = false;

    public bool Points = false;
    public bool Bounds = false;

    private IRenderer renderer;

    void Awake()
    {
        voxels = new Octree<Voxel>(ChunkSize, new Vector3(ChunkPosition.x + ChunkSize/2, ChunkPosition.y + ChunkSize/2, ChunkPosition.z + ChunkSize/2), 8);
        filter = gameObject.GetComponent<MeshFilter>();
        coll = gameObject.GetComponent<MeshCollider>();
        ChunkVolume = ChunkSize * ChunkSize * ChunkSize;
        renderer = new OctreeBlockRenderer();
        renderer.Initialize();
    }

    void Start()
    {
        //InitSurroundingChunks();
    }

    void InitSurroundingChunks()
    {
        SurroundingChunks[0] = world.GetChunk(ChunkPosition.x, ChunkPosition.y + Chunk.ChunkSize, ChunkPosition.z);
        SurroundingChunks[1] = world.GetChunk(ChunkPosition.x, ChunkPosition.y - Chunk.ChunkSize, ChunkPosition.z);
        SurroundingChunks[2] = world.GetChunk(ChunkPosition.x - Chunk.ChunkSize, ChunkPosition.y, ChunkPosition.z);
        SurroundingChunks[3] = world.GetChunk(ChunkPosition.x + Chunk.ChunkSize, ChunkPosition.y, ChunkPosition.z);
        SurroundingChunks[4] = world.GetChunk(ChunkPosition.x, ChunkPosition.y, ChunkPosition.z - Chunk.ChunkSize);
        SurroundingChunks[5] = world.GetChunk(ChunkPosition.x, ChunkPosition.y, ChunkPosition.z + Chunk.ChunkSize);
    }

    void FixedUpdate()
    {
        if (remove)
        {
            remove = false;
            RemoveVoxel((int)voxelCoord.x, (int)voxelCoord.y, (int)voxelCoord.z);
        }
        if (Dirty)
        {
            Dirty = false;
            rendered = false;
            UpdateSurroundingChunks();
        }
        if (!rendered)
            StartCoroutine(RenderThing());
    }

    public void AddVoxel(int x, int y, int z, Voxel voxel)
    {
        if (InRange(x, y, z))
        {
            voxels.Add(new Voxel(), x, y, z);
            this.Dirty = true;
        }
        else
        {
            world.SetVoxel(x, y, z, voxel);
        }
    }

    public void RemoveVoxel(int x, int y, int z)
    {
        if(InRange(x,y,z))
        {
            voxels.Remove(x, y, z);
            this.Dirty = true;
        }
        else
        {
            world.RemoveVoxel(x, y, z);
        }
    }

    public Voxel GetVoxel(int x, int y, int z)
    {
        if (InRange(x, y, z))
            return voxels.Get(x, y, z);
        return world.GetVoxel(x, y, z);
    }

    private bool InRange(int index)
    {
        if (index < 0 || index >= Chunk.ChunkSize)
            return false;
        return true;
    }

    private bool InRange(int x, int y, int z)
    {
        if (x < ChunkPosition.x || x >= ChunkPosition.x + Chunk.ChunkSize)
            return false;
        if (y < ChunkPosition.y || y >= ChunkPosition.y + Chunk.ChunkSize)
            return false;
        if (z < ChunkPosition.z || z >= ChunkPosition.z + Chunk.ChunkSize)
            return false;
        return true;
    }

    Mesh tempMesh;

    IEnumerator RenderThing()
    {
        Task task;
        if(generated)
        {
            rendered = true;
            this.StartCoroutineAsync(ProcessMesh(), out task);
            yield return StartCoroutine(task.Wait());
            tempMesh = renderer.ToMesh(tempMesh);
            UpdateMesh();
        }
    }

    IEnumerator ProcessMesh()
    {
        renderer.Render(world, this);
        yield break;
    }


    public void UpdateMesh()
    {
        filter.sharedMesh = tempMesh;
        coll.sharedMesh = tempMesh;
    }

    public bool IsSolid
    {
        get
        {
            if (voxels.Count == 0 || voxels.Count == ChunkVolume)
                return true;
            return false;
        }
    }

    void UpdateSurroundingChunks()
    {
        //UpDown
        if (SurroundingChunks[0] != null)
        {
            SurroundingChunks[0].UpdateMesh();
            SurroundingChunks[0].SurroundingChunks[1] = this;
        }
        if (SurroundingChunks[1] != null)
        {
            SurroundingChunks[1].UpdateMesh();
            SurroundingChunks[1].SurroundingChunks[0] = this;
        }
        //LeftRight
        if (SurroundingChunks[2] != null)
        {
            SurroundingChunks[2].UpdateMesh();
            SurroundingChunks[2].SurroundingChunks[3] = this;
        }
        if (SurroundingChunks[3] != null)
        {
            SurroundingChunks[3].UpdateMesh();
            SurroundingChunks[3].SurroundingChunks[2] = this;
        }
        //FrontBack
        if (SurroundingChunks[4] != null)
        {
            SurroundingChunks[4].UpdateMesh();
            SurroundingChunks[4].SurroundingChunks[5] = this;
        }
        if (SurroundingChunks[5] != null)
        {
            SurroundingChunks[5].UpdateMesh();
            SurroundingChunks[5].SurroundingChunks[4] = this;
        }
    }

    public void OnDrawGizmos()
    {
        if(Bounds)
            voxels.DrawAllBounds();
        if (Points)
            voxels.DrawAllPoints();
    }
}
