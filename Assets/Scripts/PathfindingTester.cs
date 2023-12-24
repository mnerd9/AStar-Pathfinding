using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PathfindingTester : MonoBehaviour
{
    // The A* manager.
    private AStarManager AStarManager = new AStarManager();
    // List of possible waypoints.
    private List<GameObject> Waypoints = new List<GameObject>();
    // List of waypoint map connections. Represents a path.
    private List<Connection> ConnectionArray = new List<Connection>();
    // The start and end nodes.
    [SerializeField]
    private GameObject start;
    [SerializeField]
    private GameObject end;
    // Debug line offset.
    Vector3 OffSet = new Vector3(0, 0.3f, 0);
    // Movement variables.
    [SerializeField] private float currSpeed = 32;
    private int currTarget = 0;
    private Vector3 currTargetPos;
    private int movingDir = 1;
    private bool tempAgent = true;
    private bool hasLogs = false;

    public GameObject collectLogs;
    private GameObject getProp;
    public TextMeshProUGUI storeDistance;
    public TextMeshProUGUI storeTime;
    public TextMeshProUGUI storeSpeed;
    private float newDist;
    private float newTime;
    private float newSpeed;


    // Start is called before the first frame update
    void Start()
    {   
        if (start == null || end == null)
        {
            Debug.Log("No start or end waypoints.");
            return;
        }
        VisGraphWaypointManager tmpWpM = start.GetComponent<VisGraphWaypointManager>();
        if (tmpWpM == null)
        {
            Debug.Log("Start is not a waypoint.");
            return;
        }
        tmpWpM = end.GetComponent<VisGraphWaypointManager>();
        if (tmpWpM == null)
        {
            Debug.Log("End is not a waypoint.");
            return;
        }
        // Find all the waypoints in the level.
        GameObject[] GameObjectsWithWaypointTag;
        GameObjectsWithWaypointTag = GameObject.FindGameObjectsWithTag("Waypoint");
        foreach (GameObject waypoint in GameObjectsWithWaypointTag)
        {
            VisGraphWaypointManager tmpWaypointMan = waypoint.GetComponent<VisGraphWaypointManager>();
            if (tmpWaypointMan)
            {
                Waypoints.Add(waypoint);
            }
        }
        // Go through the waypoints and create connections.
        foreach (GameObject waypoint in Waypoints)
        {
            VisGraphWaypointManager tmpWaypointMan = waypoint.GetComponent<VisGraphWaypointManager>();
            // Loop through a waypoints connections.
            foreach (VisGraphConnection aVisGraphConnection in tmpWaypointMan.Connections)
            {
                if (aVisGraphConnection.ToNode != null)
                {
                    Connection aConnection = new Connection();
                    aConnection.FromNode = waypoint;
                    aConnection.ToNode = aVisGraphConnection.ToNode;
                    AStarManager.AddConnection(aConnection);
                }
                else
                {
                    Debug.Log("Warning, " + waypoint.name + " has a missing to node for a connection!");
                }
            }
        }
        // Run A Star...
        // ConnectionArray stores all the connections in the route to the goal / end node.
        ConnectionArray = AStarManager.PathfindAStar(start, end);
        if (ConnectionArray.Count == 0)
        {
            Debug.Log("Warning, A* did not return a path between the start and end node.");
        }
    }
    // Draws debug objects in the editor and during editor play (if option set).
    void OnDrawGizmos()
    {
        // Draw path.
        foreach (Connection aConnection in ConnectionArray)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine((aConnection.FromNode.transform.position + OffSet), (aConnection.ToNode.transform.position + OffSet));
        }
    }

    void Update() {
        if (tempAgent) {
            if (movingDir > 0) {
                currTargetPos = ConnectionArray[currTarget].ToNode.transform.position;
            } else {
                currTargetPos = ConnectionArray[currTarget].FromNode.transform.position;
            }

            currTargetPos.y = transform.position.y;
            Vector3 direction = currTargetPos - transform.position;

            float agentDist = direction.magnitude;

            direction.y = 0;
            if (direction.magnitude > 0) {
                Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.smoothDeltaTime * 5f);
            }
            Vector3 normDirection = direction.normalized;
            transform.position = transform.position + normDirection * currSpeed * Time.smoothDeltaTime;
            if (agentDist < 1) {
                currTarget = currTarget + movingDir;

                if (currTarget >= ConnectionArray.Count || currTarget < 0) {
                    if (!hasLogs) {
                        attachLogs();
                    }

                    movingDir *= -1;
                    currTarget += movingDir;

                    if (currTarget <= 0) {
                        tempAgent = false;
                        currSpeed = 0f;
                        Debug.Log("The vehicle has returned home with that has logs from the endpoint");
                    }
                }
            }
            float calcDist = currSpeed * Time.smoothDeltaTime;
            newDist = newDist + calcDist;
            newTime = newTime + Time.smoothDeltaTime;

            storeDistance.text = "Distance: " + newDist.ToString("F2") + "";
            storeTime.text = "Time: " + newTime.ToString("F2") + "";

            if (newTime >= 1) {
                newSpeed = newDist / newTime;
                storeSpeed.text = "Speed: " + newSpeed.ToString("F2") + "";
            }
        }
    }

    void attachLogs() {
        hasLogs = true;
        if (collectLogs == null) {
            return;
        }

        getProp = Instantiate(collectLogs, end.transform.position, Quaternion.identity);
        Debug.Log("Attached logs on the vehicle");

        if (getProp != null) {
            getProp.transform.SetParent(transform);
            getProp.transform.localPosition = new Vector3(0.244000003f, 1.58200002f, -3.60500002f);
            getProp.transform.localRotation = Quaternion.Euler(295.442993f,87.2642441f,88.6033249f);
            getProp.transform.localScale = new Vector3(15f, 10.5f, 14.8800001f);
        }
    }
}