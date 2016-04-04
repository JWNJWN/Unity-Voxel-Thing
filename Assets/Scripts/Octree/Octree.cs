using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Octree<T> {

    public int Count { get; private set; }

    OctreeNode<T> RootNode;

    readonly float InitialSize;
    readonly float MinSize;

	public Octree(float InitialSize, Vector3 InitialCenter, float MinSize)
    {
        if(MinSize>InitialSize)
        {
            MinSize = InitialSize;
        }
        Count = 0;
        this.InitialSize = InitialSize;
        this.MinSize = MinSize;
        RootNode = new OctreeNode<T>(InitialSize, InitialCenter, MinSize);
    }

    public T Get(Vector3 position)
    {
        return RootNode.Get(position);
    }
    public T Get(int x, int y, int z)
    {
        return RootNode.Get(new Vector3(x, y, z));
    }

    public HashSet<Vector3> GetPositions()
    {
        return RootNode.GetPositions();
    }

    public void Add(T obj, Vector3 position)
    {
        int count = 0;
        while(!RootNode.Add(obj, position))
        {
            Grow(position - RootNode.Center);
            if(++count > 20)
            {
                return;
            }
        }
        Count++;
    }
    public void Add(T obj, int x, int y, int z)
    {
        int count = 0;
        Vector3 position = new Vector3(x, y, z);
        while (!RootNode.Add(obj, position))
        {
            Grow(position - RootNode.Center);
            if (++count > 20)
            {
                return;
            }
        }
        Count++;
    }

    public bool Remove(Vector3 Position)
    {
        bool removed = RootNode.Remove(Position);

        if (removed)
        {
            Count--;
            Shrink();
        }

        return removed;
    }

    public bool Remove(int x, int y, int z)
    {
        Vector3 position = new Vector3(x, y, z);
        bool removed = RootNode.Remove(position);

        if (removed)
        {
            Count--;
            Shrink();
        }

        return removed;
    }

    public void DrawAllBounds()
    {
        RootNode.DrawAllBounds();
    }

    public void DrawAllPoints()
    {
        RootNode.DrawAllPoints();
    }

    void Grow(Vector3 direction)
    {
        int xDirection = direction.x >= 0 ? 1 : -1;
        int yDirection = direction.y >= 0 ? 1 : -1;
        int zDirection = direction.z >= 0 ? 1 : -1;

        OctreeNode<T> oldRoot = RootNode;
        float half = RootNode.SideLength / 2;
        float NewLength = RootNode.SideLength * 2;
        Vector3 NewCenter = RootNode.Center + new Vector3(xDirection * half, yDirection * half, zDirection * half);

        RootNode = new OctreeNode<T>(NewLength, NewCenter, MinSize);

        int RootPos = GetRootPosIndex(xDirection, yDirection, zDirection);
        OctreeNode<T>[] Children = new OctreeNode<T>[8];
        for (int i = 0; i < 8; i++) 
        {
            if (i == RootPos)
            {
                Children[i] = oldRoot;
            } else
            {
                xDirection = i % 2 == 0 ? -1 : 1;
                yDirection = i > 3 ? -1 : 1;
                zDirection = (i < 2 || (i > 3 && i < 6)) ? -1 : 1;
                Children[i] = new OctreeNode<T>(RootNode.SideLength, NewCenter + new Vector3(xDirection * half, yDirection * half, zDirection * half), MinSize);
            }
        }
        RootNode.SetChildren(Children);
    }

    void Shrink()
    {
        RootNode = RootNode.ShrinkIfPossible(InitialSize);
    }

    static int GetRootPosIndex(int xDir, int yDir, int zDir)
    {
        int result = xDir > 0 ? 1 : 0;
        if (yDir < 0) result += 4;
        if (zDir < 0) result += 2;

        return result;
    }
}
