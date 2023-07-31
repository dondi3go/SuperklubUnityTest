using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Superklub;

/// <summary>
/// Create a Unity GameObject
/// from a SuperklubNodeRecord
/// </summary>
public class SuperklubNodeFactory
{
    /// <summary>
    /// Returns a GameObject having a SuperklubNode 
    /// matching the SuperklubNodeRecord parameter
    /// </summary>
    public static SuperklubNode CreateNode(SuperklubNodeRecord node)
    {
        // Create a Game Object with a Mesh depending on node.Shape ("box", "ball", "pill")
        GameObject gameObject = CreateGameObject(node.Shape);

        // Material depends on node.Color ("red", "green", "blue")
        Material mat = CreateMaterial(node.Color);
        NodeBuilder.SetMaterial(gameObject, mat);

        // Id
        gameObject.name = node.Id;

        // Position
        gameObject.transform.position = new Vector3(
            node.Position.x, node.Position.y, node.Position.y);

        // Rotation
        gameObject.transform.rotation = new Quaternion(
            node.Rotation.w, node.Rotation.x, node.Rotation.y, node.Rotation.y);

        // Add SuperklubNode (for further update)
        var superklubNode = gameObject.AddComponent<SuperklubNode>();

        return superklubNode;
    }

    /// <summary>
    /// 
    /// </summary>
    private static GameObject CreateGameObject(string shapeName)
    {
        Dictionary<string, PrimitiveType> map = new Dictionary<string, PrimitiveType>();
        map.Add("ball", PrimitiveType.Sphere);
        map.Add("pill", PrimitiveType.Capsule);
        map.Add("box", PrimitiveType.Cube);

        // The default primitive
        PrimitiveType primitive = PrimitiveType.Cylinder;

        if (map.ContainsKey(shapeName))
        {
            primitive = map[shapeName];
        }

        return GameObject.CreatePrimitive(primitive);
    }

    /// <summary>
    /// 
    /// </summary>
    private static Material CreateMaterial(string colorName)
    {
        Dictionary<string, Color> map = new Dictionary<string, Color>();
        map.Add("red", new Color(1f, 0f, 0f));
        map.Add("green", new Color(0f, 1f, 0f));
        map.Add("blue", new Color(0f, 0f, 1f));

        // The default color
        Color color = new Color(0.5f, 0.5f, 0.5f);

        if (map.ContainsKey(colorName))
        {
            color = map[colorName];
        }

        Material mat = new Material(Shader.Find("Standard"));
        mat.color = color;

        return mat;
    }
}
