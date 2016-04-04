using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CielaSpike;

public class World : MonoBehaviour {
    
    public Octree<Chunk> chunks;

    public int worldSize;
    public int minSize = 256;

    public Vector3 chunkGenerationPos;
    OctreeWorldGeneration generator;
    public bool generate = true;

    public GameObject chunkPrefab;

    public bool rendered = false;

    public new IRenderer renderer;
    public bool march = false;

    void Awake()
    {
        renderer = new OctreeBlockRenderer();
        generator = new OctreeWorldGeneration();
        chunks = new Octree<Chunk>(worldSize, Vector3.zero, minSize);
    }

    void Start()
    {
    }

    void Update()
    {
        /*if(march)
        {
            renderer = new OctreeMarchRenderer();
        }
        else
        {
            renderer = new OctreeBlockRenderer();
        }*/

        renderer.Initialize();
        if(generate)
            StartCoroutine(CreateChunk());
    }

    public Voxel GetVoxel(int x, int y, int z)
    {
        Chunk tempChunk = GetChunk(x, y, z);
        if (tempChunk != null)
            return tempChunk.GetVoxel(x, y, z);
        return null;
    }

    public void SetVoxel(int x, int y, int z, Voxel voxel)
    {
        Chunk tempChunk = GetChunk(x, y, z);
        if (tempChunk != null)
            tempChunk.AddVoxel(x, y, z, voxel);
    }

    public void RemoveVoxel(int x, int y, int z)
    {
        Chunk tempChunk = GetChunk(x, y, z);
        if (tempChunk != null)
            tempChunk.RemoveVoxel(x, y, z);
    }

    public Chunk GetChunk(int x, int y, int z)
    {
        WorldPos pos = new WorldPos();
        float multiple = Chunk.ChunkSize;
        pos.x = Mathf.FloorToInt(x / multiple) * Chunk.ChunkSize;
        pos.y = Mathf.FloorToInt(y / multiple) * Chunk.ChunkSize;
        pos.z = Mathf.FloorToInt(z / multiple) * Chunk.ChunkSize;
        
        return chunks.Get(pos.x, pos.y, pos.z);
    }

    IEnumerator CreateChunk()
    {
        Task GenerationTask;
        if (generate)
        {
            generate = false;
            WorldPos TempPos = new WorldPos((int)chunkGenerationPos.x * Chunk.ChunkSize, (int)chunkGenerationPos.y * Chunk.ChunkSize, (int)chunkGenerationPos.z * Chunk.ChunkSize);
            yield return Ninja.JumpToUnity;

            GameObject TempChunkObject = Instantiate(chunkPrefab) as GameObject;
            TempChunkObject.transform.SetParent(this.transform);
            TempChunkObject.name = TempPos.ToString();
            Chunk TempChunkScript = TempChunkObject.GetComponent<Chunk>();

            yield return Ninja.JumpBack;
            TempChunkScript.world = this;
            this.StartCoroutineAsync(GenerateChunk(TempChunkScript), out GenerationTask);
            yield return StartCoroutine(GenerationTask.Wait());
            Logger.Log(this, "Fetching Started");
            TempChunkScript = generator.FetchChunk();
            Logger.Log(this, "Fetching Finished");

            chunks.Add(TempChunkScript, TempPos.ToVector3());
        }
    }

    IEnumerator GenerateChunk(Chunk chunk)
    {
        Logger.LogAsync("Generation Started");
        generator.Generate(this, chunk);
        Logger.LogAsync("Generation Ended");
        yield break;
    }

    public void DestroyChunk(int x, int y, int z)
    {
        chunks.Remove(new Vector3(x, y, z));
    }
}
