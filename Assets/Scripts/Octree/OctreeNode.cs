using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OctreeNode<T>
{

    Dictionary<Vector3, T> objects = new Dictionary<Vector3, T>();

    OctreeNode<T>[] Children = null;

    OctreeNode<T> RootNode;

    Bounds Bounds = default(Bounds);
    Bounds[] ChildBounds;
    Vector3 ActualBoundSize;

    public float MinSize;
    public Vector3 Center { get; private set; }
    public float SideLength { get; private set; }

    int AllowedObjects = 8;

    public OctreeNode(float InitialSize, Vector3 InitialCenter, float MinSize)
    {
        SetValues(InitialSize, InitialCenter, MinSize);
    }

    public T Get(Vector3 position)
    {
        T tempVox = default(T);
        if (objects.ContainsKey(position))
            objects.TryGetValue(position, out tempVox);

        if (tempVox == null && Children != null)
        {
            int bestFitChild = BestFitChild(position);
            tempVox = Children[bestFitChild].Get(position);
        }

        return tempVox;
    }

    public HashSet<Vector3> GetPositions()
    {
        HashSet<Vector3> positions = new HashSet<Vector3>(objects.Keys);

        if(Children!=null)
        {
            foreach(OctreeNode<T> Child in Children)
            {
                positions.UnionWith(Child.GetPositions());
            }
        }
        return positions;
    }

    public bool Add(T voxel, Vector3 position)
    {
        if (!Encapsulates(Bounds, position))
            return false;
        SubAdd(voxel, position);
        return true;
    }

    void SubAdd(T voxel, Vector3 position)
    {
        if (objects.Count < AllowedObjects || (SideLength / 2) < MinSize)
        {
            objects.Add(position, voxel);
        }
        else
        {
            int bestFitChild;
            if(Children == null)
            {

                Split();
                if (Children == null)
                    return;
                
                Dictionary<Vector3, T> tempobjects = new Dictionary<Vector3, T>(objects);

                foreach(KeyValuePair<Vector3, T> kvp in tempobjects)
                {
                    bestFitChild = BestFitChild(kvp.Key);
                    Children[bestFitChild].SubAdd(kvp.Value, kvp.Key);
                    objects.Remove(kvp.Key);
                }
                
            }
            bestFitChild = BestFitChild(position);
            Children[bestFitChild].SubAdd(voxel, position);
        }
    }

    static bool Encapsulates(Bounds outerBounds, Vector3 point)
    {
        return outerBounds.Contains(point);
    }

    public bool Remove(Vector3 position)
    {
        bool removed = false;
        if(objects.ContainsKey(position))
            removed = objects.Remove(position);

        if(!removed && Children != null)
        {
            foreach(OctreeNode<T> Child in Children)
            {
                removed = Child.Remove(position);
                if(removed)
                    break;
            }
        }

        if(removed && Children != null)
        {
            if(ShouldMerge())
            {
                Merge();
            }
        }

        return removed;
    }

    public void SetChildren(OctreeNode<T>[] children)
    {
        if (children.Length != 8)
            return;
        Children = children;
    }

    public OctreeNode<T> ShrinkIfPossible(float minLength)
    {
        if (SideLength < (2 * minLength))
            return this;
        if (objects.Count == 0 && Children.Length == 0)
            return this;

        int bestFit = -1;
        int i = 0;
        foreach (Vector3 position in objects.Keys)
        {
            int newBestFit = BestFitChild(position);
            if(i==0||newBestFit == bestFit)
            {
                if (bestFit < 0)
                    bestFit = newBestFit;
                i++;
            }
            else
            {
                i++;
                return this;
            }
        }

        if(Children != null)
        {
            bool childHadContent = false;
            int j = 0;
            foreach(OctreeNode<T> Child in Children)
            {
                if(Child.HasAnyobjects())
                {
                    if (childHadContent)
                    {
                        return this;
                    }
                    if (bestFit >= 0 && bestFit != j)
                    {
                        return this;
                    }
                    childHadContent = true;
                    bestFit = j;
                    j++;
                }
            }
        }
        else
        {
            SetValues(SideLength / 2, ChildBounds[bestFit].center, MinSize);
            return this;
        }

        return Children[bestFit];

    }

    void SetValues(float baseLength, Vector3 center, float minSize)
    {
        SideLength = baseLength;
        MinSize = minSize;
        Center = center;

        ActualBoundSize = new Vector3(SideLength, SideLength, SideLength);
        Bounds = new Bounds(Center, ActualBoundSize);

        float quarter = SideLength / 4;
        float childActualLength = SideLength / 2;
        Vector3 childActualSize = new Vector3(childActualLength, childActualLength, childActualLength);

        ChildBounds = new Bounds[8];
        ChildBounds[0] = new Bounds(Center + new Vector3(-quarter, quarter, -quarter), childActualSize);
        ChildBounds[1] = new Bounds(Center + new Vector3(quarter, quarter, -quarter), childActualSize);
        ChildBounds[2] = new Bounds(Center + new Vector3(-quarter, quarter, quarter), childActualSize);
        ChildBounds[3] = new Bounds(Center + new Vector3(quarter, quarter, quarter), childActualSize);
        ChildBounds[4] = new Bounds(Center + new Vector3(-quarter, -quarter, -quarter), childActualSize);
        ChildBounds[5] = new Bounds(Center + new Vector3(quarter, -quarter, -quarter), childActualSize);
        ChildBounds[6] = new Bounds(Center + new Vector3(-quarter, -quarter, quarter), childActualSize);
        ChildBounds[7] = new Bounds(Center + new Vector3(quarter, -quarter, quarter), childActualSize);
    }

    void Split()
    {
        float quarter = SideLength / 4;
        float newLength = SideLength / 2;

        Children = new OctreeNode<T>[8];
        Children[0] = new OctreeNode<T>(newLength, Center + new Vector3(-quarter, quarter, -quarter), MinSize);
        Children[1] = new OctreeNode<T>(newLength, Center + new Vector3(quarter, quarter, -quarter), MinSize);
        Children[2] = new OctreeNode<T>(newLength, Center + new Vector3(-quarter, quarter, quarter), MinSize);
        Children[3] = new OctreeNode<T>(newLength, Center + new Vector3(quarter, quarter, quarter), MinSize);
        Children[4] = new OctreeNode<T>(newLength, Center + new Vector3(-quarter, -quarter, -quarter), MinSize);
        Children[5] = new OctreeNode<T>(newLength, Center + new Vector3(quarter, -quarter, -quarter), MinSize);
        Children[6] = new OctreeNode<T>(newLength, Center + new Vector3(-quarter, -quarter, quarter), MinSize);
        Children[7] = new OctreeNode<T>(newLength, Center + new Vector3(quarter, -quarter, quarter), MinSize);
    }

    void Merge()
    {
        foreach(OctreeNode<T> child in Children)
        {
            foreach (KeyValuePair<Vector3, T> knv in child.objects)
                objects.Add(knv.Key, knv.Value);
        }
        Children = null;
    }

    bool ShouldMerge()
    {
        int totalobjects = objects.Count;
        if (Children != null)
        {
            foreach (OctreeNode<T> child in Children)
            {
                if (child.Children != null)
                {
                    return false;
                }
                totalobjects += child.objects.Count;
            }
        }
        return totalobjects <= AllowedObjects;
    }

    int BestFitChild(Vector3 objPos)
    {
        return (objPos.x <= Center.x ? 0 : 1) + (objPos.y >= Center.y ? 0 : 4) + (objPos.z <= Center.z ? 0 : 2);
    }

    bool HasAnyobjects()
    {
        if (objects.Count > 0) return true;

        if(Children != null)
        {
            foreach(OctreeNode<T> Child in Children)
            {
                if (Child.HasAnyobjects())
                    return true;
            }
        }
        return false;
    }


    public void DrawAllBounds(float depth = 0)
    {
        float tintVal = depth / 7; 
        Gizmos.color = new Color(tintVal, 0, 1.0f - tintVal);

        if (HasAnyobjects())
        {
            Bounds thisBounds = new Bounds(Center, new Vector3(SideLength, SideLength, SideLength));
            Gizmos.DrawWireCube(thisBounds.center, thisBounds.size);
        }

        if (Children != null)
        {
            depth++;
            foreach (OctreeNode<T> Child in Children)
            {
                Child.DrawAllBounds();
            }

        }
        Gizmos.color = Color.white;
    }

    public void DrawAllPoints()
    {
        float TintVal = SideLength / 10;
        Gizmos.color = new Color(TintVal, 0, 1.0f - TintVal);

        foreach (Vector3 position in objects.Keys)
            Gizmos.DrawCube(position, new Vector3(.75f, .75f, .75f));

        if (Children != null)
        {
            foreach(OctreeNode<T> Child in Children)
            {
                Child.DrawAllPoints();
            }
        }
        Gizmos.color = Color.white;
    }
}

