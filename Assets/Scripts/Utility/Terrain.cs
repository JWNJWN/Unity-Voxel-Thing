using UnityEngine;
using System.Collections;

public static class Terrain
{
    public static WorldPos GetVoxelPos(Vector3 pos)
    {
        WorldPos voxelPos = new WorldPos(
            Mathf.FloorToInt(pos.x),
            Mathf.FloorToInt(pos.y),
            Mathf.FloorToInt(pos.z)
            );

        Debug.Log(pos.ToString());
        Debug.Log(voxelPos.ToString());
        return voxelPos;
    }

    public static WorldPos GetVoxelPos(RaycastHit hit, bool adjacent = false)
    {
        Debug.Log(hit.point.ToString());
        Vector3 pos = new Vector3(
            MoveWithinVoxel(hit.point.x, hit.normal.x, adjacent),
            MoveWithinVoxel(hit.point.y, hit.normal.y, adjacent),
            MoveWithinVoxel(hit.point.z, hit.normal.z, adjacent)
            );

        return GetVoxelPos(pos);
    }

    static float MoveWithinVoxel(float pos, float norm, bool adjacent = false)
    {
        if (pos - (int)pos == 0.5f || pos - (int)pos == -0.5f)
        {
            if (adjacent)
            {
                pos += (norm / 2);
            }
            else
            {
                pos -= (norm / 2);
            }
        }

        return (float)pos;
    }

    public static bool AddVoxel(RaycastHit hit, Voxel voxel, bool adjacent = false)
    {
        Chunk chunk = hit.collider.GetComponent<Chunk>();
        if (chunk == null)
            return false;

        WorldPos pos = GetVoxelPos(hit, adjacent);

        chunk.AddVoxel(pos.x, pos.y, pos.z, voxel);

        return true;
    }

    public static bool RemoveVoxel(RaycastHit hit, bool adjacent = false)
    {
        Chunk chunk = hit.collider.GetComponent<Chunk>();
        if (chunk == null)
            return false;

        WorldPos pos = GetVoxelPos(hit.point);
            
        chunk.RemoveVoxel(pos.x, pos.y, pos.z);

        return true;
    }

    public static Voxel GetVoxel(RaycastHit hit, bool adjacent = false)
    {
        Chunk chunk = hit.collider.GetComponent<Chunk>();
        if (chunk == null)
            return null;

        WorldPos pos = GetVoxelPos(hit, adjacent);

        Voxel voxel = chunk.world.GetVoxel(pos.x, pos.y, pos.z);

        return voxel;
    }
}