using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OctreeBlockRenderer : IRenderer
{
    private World world;
    private Chunk chunk;
    private MeshData meshData;
    private HashSet<Vector3> voxels;


    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    public void Initialize()
    {
        meshData = new MeshData();
    }

    public void Render(World world, Chunk chunk)
    {
        this.world = world;
        this.chunk = chunk;
        
        triangles = new List<int>();
        vertices = new List<Vector3>();

        Voxel voxel = new Voxel();
        Vector3 start;
        voxels = new HashSet<Vector3>(chunk.voxels.GetPositions());

        foreach (Vector3 vox in voxels)
        {
            int x = (int)vox.x, y = (int)vox.y, z = (int)vox.z;

            start = new Vector3(vox.x, vox.y, vox.z);
            if (MustFaceBeVisible(x - 1, y, z))
            {
                DrawFace((start) * Voxel.VoxelSize, (start + Vector3.forward) * Voxel.VoxelSize, (start + Vector3.forward + Vector3.up) * Voxel.VoxelSize, (start + Vector3.up) * Voxel.VoxelSize, voxel);
            }
            if (MustFaceBeVisible(x + 1, y, z))
            {
                DrawFace((start + Vector3.right + Vector3.forward) * Voxel.VoxelSize, (start + Vector3.right) * Voxel.VoxelSize, (start + Vector3.right + Vector3.up) * Voxel.VoxelSize, (start + Vector3.right + Vector3.forward + Vector3.up) * Voxel.VoxelSize, voxel);
            }
            if (MustFaceBeVisible(x, y - 1, z))
            {
                DrawFace((start + Vector3.forward) * Voxel.VoxelSize, (start) * Voxel.VoxelSize, (start + Vector3.right) * Voxel.VoxelSize, (start + Vector3.right + Vector3.forward) * Voxel.VoxelSize, voxel);
            }
            if (MustFaceBeVisible(x, y + 1, z))
            {
                DrawFace((start + Vector3.up) * Voxel.VoxelSize, (start + Vector3.up + Vector3.forward) * Voxel.VoxelSize, (start + Vector3.up + Vector3.forward + Vector3.right) * Voxel.VoxelSize, (start + Vector3.up + Vector3.right) * Voxel.VoxelSize, voxel);
            }
            if (MustFaceBeVisible(x, y, z - 1))
            {
                DrawFace((start + Vector3.right) * Voxel.VoxelSize, (start) * Voxel.VoxelSize, (start + Vector3.up) * Voxel.VoxelSize, (start + Vector3.up + Vector3.right) * Voxel.VoxelSize, voxel);
            }
            if (MustFaceBeVisible(x, y, z + 1))
            {
                DrawFace((start + Vector3.forward) * Voxel.VoxelSize, (start + Vector3.forward + Vector3.right) * Voxel.VoxelSize, (start + Vector3.forward + Vector3.right + Vector3.up) * Voxel.VoxelSize, (start + Vector3.forward + Vector3.up) * Voxel.VoxelSize, voxel);
            }
        }
    }

    public Mesh ToMesh(Mesh mesh)
    {
        if (vertices.Count==0)
        {
            GameObject.Destroy(mesh);
            return null;
        }

        if (mesh == null)
            mesh = new Mesh();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.Optimize();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        
        return mesh;
    }

    private void DrawFace(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, Voxel voxel)
    {
        int index = vertices.Count;

        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        if (v4 != Vector3.zero)
            vertices.Add(v4);

        triangles.Add(index);
        triangles.Add(index+1);
        triangles.Add(index+2);
        if (v4 != Vector3.zero)
        {
            triangles.Add(index);
            triangles.Add(index+2);
            triangles.Add(index+3);
        }
    }

    private bool MustFaceBeVisible(int x, int y, int z)
    {
        if (voxels.Contains(new Vector3(x, y, z)))
            return false;
        return true;
    }

}
