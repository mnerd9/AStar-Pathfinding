using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class ACOCON {
    private float defaultPheromone = 1.0f;
    public float DefaultPheromone {
        get {
            return defaultPheromone;
        }
    }

    public float Alpha { get; set; }
    public float Beta { get; set; }
    public float EvaporationFactor { get; set; }
    public float Q { get; set; }
    // Ants of agents moving through the graph. This class stores properties: Total distance and connections used.
    private List < ACOAnt > Ants = new List < ACOAnt > ();
    // The generated route.
    private List < ACOConnection > MyRoute = new List < ACOConnection > ();
    public ACOCON() {
        Alpha = 1.0f;
        Beta = 0.0001f;
        EvaporationFactor = 0.5f;
        Q = 0.0006f;
    }
    /* IterationThreshold = Max number of iterations.
    TotalNumAnts = Total number of ants in the simulation.
    Connections = Connections between nodes.
    WaypointNodes = All the waypoint nodes in the waypoint graph used by the ACO algorithm.
    */
    public List < ACOConnection > ACO(int IterationThreshold, int TotalNumAnts,
        GameObject[] WaypointNodes, List < ACOConnection > Connections,
        GameObject StartNode, int MaxPathLength) {
        if (StartNode == null) {
            Debug.Log("No Start node.");
            return null;
        }
        // The node the ant is currently at.
        GameObject currentNode;
        // A list of all visited nodes.
        List < GameObject > VisitedNodes = new List < GameObject > ();
        for (int i = 0; i < IterationThreshold; i++) {
            // Clear ants from previous iterations.
            Ants.Clear();
            for (int i2 = 0; i2 < TotalNumAnts; i2++) {
                ACOAnt aAnt = new ACOAnt();
                // Randomly choose start node.
                currentNode = WaypointNodes[Random.Range(0, WaypointNodes.Length)];
                aAnt.StartNode = currentNode;
                VisitedNodes.Clear();
                // Keep moving through the nodes until visited them all.
                // Keep looping until the number of nodes visited equals the number of nodes.
                while (VisitedNodes.Count < WaypointNodes.Length) {
                    // Get all connections from node.
                    List < ACOConnection > ConnectionsFromNodeAndNotVisited =
                        AllConnectionsFromNodeAndNotVisited(currentNode, Connections, VisitedNodes);
                    // Sum the product of the pheromone level and the visibility
                    // factor on all allowed paths.
                    float TotalPheromoneAndVisibility = CalculateTotalPheromoneAndVisibility(ConnectionsFromNodeAndNotVisited);
                    // Calculate the product of the pheromone level and the visibility
                    // factor of the proposed path.
                    
                    // Loop through the paths and check if visited destination already.
                    foreach(ACOConnection aConnection in ConnectionsFromNodeAndNotVisited) {
                        // Not visited the path before.
                        float PathProbability = (Mathf.Pow(aConnection.PheromoneLevel, Alpha) *
                            Mathf.Pow((1 / aConnection.Distance), Beta));
                        PathProbability = PathProbability / TotalPheromoneAndVisibility;
                        // Set path probability. Path probability is reset
                        // to zero at the end of each run.
                        aConnection.PathProbability = PathProbability;
                    }
                    // Travel down the path with the largest probability - or have a random
                    // choice if there are paths with equal probabilities.
                    // Loop through the paths and check if visited destination already.
                    ACOConnection largestProbability = null;
                    if (ConnectionsFromNodeAndNotVisited.Count > 0) {
                        largestProbability = ConnectionsFromNodeAndNotVisited[0];
                        for (int i3 = 1; i3 < ConnectionsFromNodeAndNotVisited.Count; i3++) {
                            if (ConnectionsFromNodeAndNotVisited[i3].PathProbability >
                                largestProbability.PathProbability) {
                                largestProbability = ConnectionsFromNodeAndNotVisited[i3];
                            } else if (ConnectionsFromNodeAndNotVisited[i3].PathProbability ==
                                largestProbability.PathProbability) {
                                // Currently, 100% of the time chooses shortest connection if probabilities are the same.
                                if(ConnectionsFromNodeAndNotVisited[i3].Distance <
                                    largestProbability.Distance) {
                                    largestProbability = ConnectionsFromNodeAndNotVisited[i3];
                                }
                            }
                        }
                    }
                    // largestProbability contains the path to move down.
                    VisitedNodes.Add(currentNode);
                    if (largestProbability != null) {
                        currentNode = largestProbability.ToNode;
                        aAnt.AddTravelledConnection(largestProbability);
                        aAnt.AddAntTourLength(largestProbability.Distance);
                    }
                } //~END: While loop.
                // Add a connection from the current node back to the start node for this tour.
                foreach(ACOConnection aConnection in Connections) {
                    if (aConnection.FromNode.Equals(currentNode)) {
                        if (aConnection.ToNode.Equals(aAnt.StartNode)) {
                            aAnt.AddTravelledConnection(aConnection);
                            aAnt.AddAntTourLength(aConnection.Distance);
                        }
                    }
                }
                Ants.Add(aAnt);
            }


            // Find the best ants.
            List<ACOAnt> SortedAnts = Ants.OrderBy(x => x.AntTourLength).ToList();
            List<ACOAnt> BestAnts = SortedAnts.GetRange(0, 3);
            
            // Update pheromone by formula Delta_Tau_ij.
            // Loop through the paths and check if visited destination already.
            foreach(ACOConnection aConnection in Connections) {
                float Sum = 0;
                foreach(ACOAnt TmpAnt in BestAnts) {
                    List < ACOConnection > TmpAntConnections = TmpAnt.AntTravelledConnections;
                    foreach(ACOConnection tmpConnection in TmpAntConnections) {
                        if (aConnection.Equals(tmpConnection)) {
                            Sum += Q / TmpAnt.AntTourLength;
                        }
                    }
                }
                float NewPheromoneLevel = (1 - EvaporationFactor) * aConnection.PheromoneLevel + Sum;
                aConnection.PheromoneLevel = NewPheromoneLevel;
                // Reset path probability.
                aConnection.PathProbability = 0;
            }
        }
        // Output connections and Pheromone to the log.
        LogAnts();
        LogRoute(StartNode, MaxPathLength, WaypointNodes, Connections);
        LogConnections(Connections);
        MyRoute = GenerateRoute(StartNode, MaxPathLength, Connections);
        return MyRoute;
    }
    // Return all Connections from a node.
    private List < ACOConnection > AllConnectionsFromNode(GameObject FromNode, List < ACOConnection >
        Connections) {
        List < ACOConnection > ConnectionsFromNode = new List < ACOConnection > ();
        foreach(ACOConnection aConnection in Connections) {
            if (aConnection.FromNode == FromNode) {
                ConnectionsFromNode.Add(aConnection);
            }
        }
        return ConnectionsFromNode;
    }
    // Return all Connections from a node that have not been visited.
    private List < ACOConnection > AllConnectionsFromNodeAndNotVisited(GameObject FromNode, List < ACOConnection > Connections, List < GameObject > VisitedList) {
        List < ACOConnection > ConnectionsFromNode = new List < ACOConnection > ();
        foreach(ACOConnection aConnection in Connections) {
            if (aConnection.FromNode == FromNode) {
                if (!VisitedList.Contains(aConnection.ToNode)) {
                    ConnectionsFromNode.Add(aConnection);
                }
            }
        }
        return ConnectionsFromNode;
    }
    // Sum the product of the pheromone level and the visibility factor on all allowed paths.
    private float CalculateTotalPheromoneAndVisibility(List < ACOConnection >
        ConnectionsFromNodeAndNotVisited) {
        float TotalPheromoneAndVisibility = 0;
        // Loop through the paths not visited and calculate total pheromone & visibility.
        foreach(ACOConnection aConnection in ConnectionsFromNodeAndNotVisited) {
            TotalPheromoneAndVisibility +=
                (Mathf.Pow(aConnection.PheromoneLevel, Alpha) * Mathf.Pow((1 / aConnection.Distance),
                    Beta));
        }
        return TotalPheromoneAndVisibility;
    }
    public List < ACOConnection > GenerateRoute(GameObject StartNode, int MaxPath, List < ACOConnection >
        Connections) {
        GameObject CurrentNode = StartNode;
        List < ACOConnection > Route = new List < ACOConnection > ();
        ACOConnection HighestPheromoneConnection = null;
        int PathCount = 1;
        while (CurrentNode != null) {
            List < ACOConnection > AllFromConnections = AllConnectionsFromNode(CurrentNode, Connections);
            if (AllFromConnections.Count > 0) {
                HighestPheromoneConnection = AllFromConnections[0];
                foreach(ACOConnection aConnection in AllFromConnections) {
                    if (aConnection.PheromoneLevel > HighestPheromoneConnection.PheromoneLevel) {
                        HighestPheromoneConnection = aConnection;
                    }
                }
                Route.Add(HighestPheromoneConnection);
                CurrentNode = HighestPheromoneConnection.ToNode;
            } else {
                CurrentNode = null;
            }
            // If the current node is the start node at this point then we have looped through the path and should stop.
            if(CurrentNode.Equals(StartNode)) {
                CurrentNode = null;
            }
            // If the path count is greater than a max we should stop.
            if (PathCount > MaxPath) {
                CurrentNode = null;
            }
            PathCount++;
        }
        return Route;
    }
    // Log Connections.
    private void LogConnections(List < ACOConnection > Connections) {
        foreach(ACOConnection aConnection in Connections) {
            // Debug.Log(">" + aConnection.FromNode.name + " | ---> " +
            //     aConnection.ToNode.name + " = " + aConnection.PheromoneLevel);
        }
    }
    // Log Route
    private void LogRoute(GameObject StartNode, int MaxPath,
        GameObject[] WaypointNodes, List < ACOConnection > Connections) {
        GameObject CurrentNode = null;
        foreach(GameObject GameObjectNode in WaypointNodes) {
            if (GameObjectNode.Equals(StartNode)) {
                CurrentNode = GameObjectNode;
            }
        }
        ACOConnection HighestPheromoneConnection = null;
        string Output = "Route (Q: " + Q + ", Alpha: " + Alpha + ", Beta: " +
            Beta + ", EvaporationFactor: " +
            EvaporationFactor + ", DefaultPheromone: " + DefaultPheromone + "):\n";
        int PathCount = 1;
        while (CurrentNode != null) {
            List < ACOConnection > AllFromConnections = AllConnectionsFromNode(CurrentNode, Connections);
            if (AllFromConnections.Count > 0) {
                HighestPheromoneConnection = AllFromConnections[0];
                foreach(ACOConnection aConnection in AllFromConnections) {
                    if (aConnection.PheromoneLevel > HighestPheromoneConnection.PheromoneLevel) {
                        HighestPheromoneConnection = aConnection;
                    }
                }
                CurrentNode = HighestPheromoneConnection.ToNode;
                Output += "| FROM: " + HighestPheromoneConnection.FromNode.name + ", TO: " +
                    HighestPheromoneConnection.ToNode.name +
                    " (Pheromone Level: " + HighestPheromoneConnection.PheromoneLevel + ") | \n";
            } else {
                CurrentNode = null;
            }
            // If the current node is the start node at this point then we have looped
            // through the path and should stop.
            if (CurrentNode.Equals(StartNode)) {
                CurrentNode = null;
                Output += "HOME (Total Nodes:" + WaypointNodes.Length +
                    ", Nodes in Route: " + PathCount + ").\n";
            }
            // If the path count is greater than a max we should stop.
            if (PathCount > MaxPath) {
                CurrentNode = null;
                Output += "MAX PATH (Total Nodes:" + WaypointNodes.Length +
                    ", Nodes in Route: " + PathCount + ").\n";
            }
            PathCount++;
        }
        // Debug.Log(Output);
    }
    // Log Route
    private void LogAnts() {
        string Output = " Last Ant Tour Info (Q: " + Q + ", Alpha: " + Alpha + ", Beta: " + Beta + ", EvaporationFactor: " +
        EvaporationFactor + ", DefaultPheromone: " + DefaultPheromone +
            "):\n";
        for (int i = 0; i < Ants.Count; i++) {
            Output += "Ant " + i + " - Start Node: " + Ants[i].StartNode.name +
                " | Tour Length: " + Ants[i].AntTourLength + "\n";
        }
        // Debug.Log(Output);
    }
}