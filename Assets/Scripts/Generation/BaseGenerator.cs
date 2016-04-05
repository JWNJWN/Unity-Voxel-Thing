using UnityEngine;

public class BaseGenerator
{
    public World world;
    public Chunk chunk;

    public virtual void FetchChunk(out Chunk chunk)
    {
        chunk = this.chunk;
    }

    public virtual void Generate(World world, Chunk chunk)
    {
        this.world = world;
        this.chunk = chunk;
        GenerateChunk();
    }

    public virtual void GenerateChunk()
    {

    }
}