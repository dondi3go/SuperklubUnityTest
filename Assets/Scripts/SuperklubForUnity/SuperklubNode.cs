using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Superklub;

/// <summary>
/// A MonoBehaviour able to handle updates from Superklub
/// Will be used by UnitySuperklub 
/// </summary>
public class SuperklubNode : MonoBehaviour
{
    public void UpdateNode(SuperklubNodeRecord node)
    {
        // Position
        transform.position = new Vector3(
            node.Position.x, node.Position.y, node.Position.z);

        // Rotation
        transform.rotation = new Quaternion(
            node.Rotation.w, node.Rotation.x, node.Rotation.y, node.Rotation.y);

        // No update of 'Shape' of 'Color' for the time being
    }
}
