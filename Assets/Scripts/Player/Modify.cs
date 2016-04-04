using UnityEngine;
using System.Collections;

public class Modify : MonoBehaviour
{

    Vector2 rot;

    public World world;

    public bool WorldBounds = false;
    public bool WorldPoints = false;

    void Update()
    {
        Debug.DrawLine(transform.position, transform.forward * 100);
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, 100))
            {
                Terrain.RemoveVoxel(hit);
            }
            Debug.DrawLine(transform.position, hit.point, Color.red);
        }

        rot = new Vector2(
            rot.x + Input.GetAxis("Mouse X") * 3,
            rot.y + Input.GetAxis("Mouse Y") * 3);

        transform.localRotation = Quaternion.AngleAxis(rot.x, Vector3.up);
        transform.localRotation *= Quaternion.AngleAxis(rot.y, Vector3.left);

        transform.position += transform.forward * 3 * Input.GetAxis("Vertical");
        transform.position += transform.right * 3 * Input.GetAxis("Horizontal");
    }

    void OnDrawGizmos()
    {
        if(WorldBounds)
            world.chunks.DrawAllBounds();
        if(WorldPoints)
            world.chunks.DrawAllPoints();
    }
}