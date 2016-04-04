using UnityEngine;
using System.Collections.Generic;

public class ThreadedRenderer : ThreadedJob
{
    private World world;
    private Chunk chunk;
    private HashSet<Vector3> voxels;

    public Mesh outMesh;

    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();

    public void Initialize(World world, Chunk chunk)
    {
        this.world = world;
        this.chunk = chunk;
        this.voxels = chunk.voxels.GetPositions();
    }

    protected override void ThreadFunction()
    {
        Debug.Log("Thread Started");

        if (world == null || chunk == null || voxels == null)
            return;

        Debug.Log("Thread Started");
        foreach (Vector3 vox in voxels)
        {
            int x = (int)vox.x, y = (int)vox.y, z = (int)vox.z;

            if (MustFaceBeVisible(x - 1, y, z))
            {
                DrawFace((vox) * Voxel.VoxelSize, (vox + Vector3.forward) * Voxel.VoxelSize, (vox + Vector3.forward + Vector3.up) * Voxel.VoxelSize, (vox + Vector3.up) * Voxel.VoxelSize);
            }
            if (MustFaceBeVisible(x + 1, y, z))
            {
                DrawFace((vox + Vector3.right + Vector3.forward) * Voxel.VoxelSize, (vox + Vector3.right) * Voxel.VoxelSize, (vox + Vector3.right + Vector3.up) * Voxel.VoxelSize, (vox + Vector3.right + Vector3.forward + Vector3.up) * Voxel.VoxelSize);
            }
            if (MustFaceBeVisible(x, y - 1, z))
            {
                DrawFace((vox + Vector3.forward) * Voxel.VoxelSize, (vox) * Voxel.VoxelSize, (vox + Vector3.right) * Voxel.VoxelSize, (vox + Vector3.right + Vector3.forward) * Voxel.VoxelSize);
            }
            if (MustFaceBeVisible(x, y + 1, z))
            {
                DrawFace((vox + Vector3.up) * Voxel.VoxelSize, (vox + Vector3.up + Vector3.forward) * Voxel.VoxelSize, (vox + Vector3.up + Vector3.forward + Vector3.right) * Voxel.VoxelSize, (vox + Vector3.up + Vector3.right) * Voxel.VoxelSize);
            }
            if (MustFaceBeVisible(x, y, z - 1))
            {
                DrawFace((vox + Vector3.right) * Voxel.VoxelSize, (vox) * Voxel.VoxelSize, (vox + Vector3.up) * Voxel.VoxelSize, (vox + Vector3.up + Vector3.right) * Voxel.VoxelSize);
            }
            if (MustFaceBeVisible(x, y, z + 1))
            {
                DrawFace((vox + Vector3.forward) * Voxel.VoxelSize, (vox + Vector3.forward + Vector3.right) * Voxel.VoxelSize, (vox + Vector3.forward + Vector3.right + Vector3.up) * Voxel.VoxelSize, (vox + Vector3.forward + Vector3.up) * Voxel.VoxelSize);
            }
        }
    }

    protected override void OnFinished()
    {
        Debug.Log("Test");
        if (vertices.Count == 0)
        {
            GameObject.Destroy(outMesh);
            return;
        }

        if (outMesh == null)
            outMesh = new Mesh();

        outMesh.Clear();
        outMesh.vertices = vertices.ToArray();
        outMesh.triangles = triangles.ToArray();

        outMesh.Optimize();
        outMesh.RecalculateNormals();
        outMesh.RecalculateBounds();
    }

    private void DrawFace(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
    {
        int index = vertices.Count;

        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        if (v4 != Vector3.zero)
            vertices.Add(v4);

        triangles.Add(index);
        triangles.Add(index + 1);
        triangles.Add(index + 2);
        if (v4 != Vector3.zero)
        {
            triangles.Add(index);
            triangles.Add(index + 2);
            triangles.Add(index + 3);
        }
    }

    private bool MustFaceBeVisible(int x, int y, int z)
    {
        if (world.GetChunk(x, y, z) != null)
        {
            if (voxels.Contains(new Vector3(x, y, z)))
                return false;
            return true;
        }
        return false;
    }
}