using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ACOConnection
{
    private float distance = 0;
    public float Distance
    {
        get { return distance; }
    }
    private float pheromoneLevel;
    public float PheromoneLevel
    {
        set { pheromoneLevel = value; }
        get { return pheromoneLevel; }
    }
    private float pathProbability;
    public float PathProbability
    {
        set { pathProbability = value; }
        get { return pathProbability; }
    }
    private GameObject fromNode;
    public GameObject FromNode
    {
        get { return fromNode; }
    }
    private GameObject toNode;
    public GameObject ToNode
    {
        get { return toNode; }
    }
    // Default constructor.
    public ACOConnection()
    {
    }
    public void SetConnection(GameObject FromNode, GameObject ToNode, float DefaultPheromoneLevel)
    {
        this.fromNode = FromNode;
        this.toNode = ToNode;
        distance = Vector3.Distance(FromNode.transform.position, ToNode.transform.position);
        PheromoneLevel = DefaultPheromoneLevel;
        PathProbability = 0;
    }
}

