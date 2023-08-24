using System.Collections;
using System.Collections.Generic;
using Superklub;
using UnityEngine;

/// <summary>
/// A wrapper around SuperklubManager
/// that also :
/// - spawn new nodes in the scene
/// - update existing nodes
/// - destroy nodes of disconnected clients
/// </summary>
public class UnitySuperklub : MonoBehaviour
{
    /// <summary>
    /// To communicate with the server
    /// </summary>
    private SuperklubManager superklubManager = null;

    [SerializeField]
    private string ServerUrl = "http://127.0.0.1:9999";

    [SerializeField]
    private string Channel = "default";

    /// <summary>
    /// Frequency of requests to the server
    /// </summary>
    [SerializeField]
    private float syncFrequency = 10f;

    /// <summary>
    /// Nodes created from distant clients data
    /// </summary>
    private Dictionary<string, SuperklubNode> distantNodes = new Dictionary<string, SuperklubNode>();

    /// <summary>
    /// 
    /// </summary>
    private bool isBeingDestroyed = false;

    /// <summary>
    /// 
    /// </summary>
    void Start()
    {
        var httpClient = new HttpClient();
        var supersynkClient = new SupersynkClient(httpClient);
        superklubManager = new SuperklubManager(supersynkClient);
        superklubManager.ServerUrl = ServerUrl;
        superklubManager.Channel = Channel;

        StartSuperklubLoop();
    }

    /// <summary>
    /// 
    /// </summary>
    public void StartSuperklubLoop()
    {
        float delay = 0.5f;
        float period = 1f / syncFrequency;
        InvokeRepeating("SyncSuperklub", delay, period);
    }

    /// <summary>
    /// 
    /// </summary>
    public async void SyncSuperklub()
    {
        // Network request to server 
        var update = await superklubManager.SynchronizeLocalAndDistantNodes();

        // Handle events since previous update
        foreach (var clientId in update.disconnectedClients)
        {
            Debug.Log("Client " + clientId + " is disconnected");
        }
        
        foreach (var clientId in update.newConnectedClients)
        {
            Debug.Log("Client " + clientId + " is now connected");
        }

        foreach (var node in update.nodesToCreate)
        { 
            SpawnNode(node);
        }
        
        foreach (var node in update.nodesToUpdate)
        {
            UpdateNode(node);
        }
        
        foreach (var node in update.nodesToDelete)
        {
            DestroyNode(node);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void SpawnNode(SuperklubNodeRecord node)
    {
        if(isBeingDestroyed)
        {
            return;
        }

        Debug.Log("Spawning node " + node.Id);
        // Create
        var superklubNode = SuperklubNodeFactory.CreateNode(node);
        // Store (for future update)
        distantNodes.Add(node.Id, superklubNode);
        // Add to the scene
        superklubNode.gameObject.transform.parent = this.transform;
    }

    /// <summary>
    /// 
    /// </summary>
    private void UpdateNode(SuperklubNodeRecord node)
    {
        if(isBeingDestroyed)
        {
            return;
        }

        if (distantNodes.ContainsKey(node.Id))
        {
            // Retrieve node
            var superklubNode = distantNodes[node.Id];
            // Update
            superklubNode.UpdateNode(node);
        }
        else
        {
            Debug.LogError("No node named " + node.Id);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void DestroyNode(SuperklubNodeRecord node)
    {
        if(isBeingDestroyed)
        {
            return;
        }

        Debug.Log("Destroying node " + node.Id);
        if (distantNodes.ContainsKey(node.Id))
        {
            // Retrieve node
            var superklubNode = distantNodes[node.Id];
            // Remove Key
            distantNodes.Remove(node.Id);
            // Destroy
            Destroy(superklubNode.gameObject);
        }
        else
        {
            Debug.LogError("No node named " + node.Id);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnDestroy()
    {
        CancelInvoke();
        distantNodes.Clear();
        isBeingDestroyed = true;
    }
}
