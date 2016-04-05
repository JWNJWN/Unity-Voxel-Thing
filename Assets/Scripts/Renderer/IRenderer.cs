using UnityEngine;

public interface IRenderer
{
    void Initialize();
    void Render(World world, Chunk chunk);

    Mesh ToMesh(Mesh mesh);
}
