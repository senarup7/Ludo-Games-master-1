using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class TokenWaypoints 
{
    HomeBaseManager hm;

    public List<Transform> Waypoints {
		get; private set;
	}


    public int HomebasePointIndex
    {
        get; private set;
    }

    public Transform GetBaseIndex(int index, List<Transform> homebaseTransform)
    {
        return homebaseTransform[index];
    }
    public Transform GetWayPointIndex(int index)
    {
        return Waypoints[index].transform;
    }


    public Transform HomeBase {
		get; private set;
	}

	public Transform CurrentWaypoint {
		get; set;
	}
    public int CurrentIndex
    {
        get; set;
    }

    public Transform LastWaypoint {
		get; private set;
	}

	public Transform StartingWaypoint {
		get { return Waypoints[0]; }
	}

	public int CurrentWaypointIndex {
		get; private set;
	}

	private int _destWaypointIdx;
	public int DestWaypointIndex {
		get { return _destWaypointIdx; } 
		set {
			_destWaypointIdx = Mathf.Min (Waypoints.Count - 1, Mathf.Max (value, 0));
		}
	}

	public float DistFromPrevWaypoint {
		get; private set;
	}

	public TokenWaypoints (List<Transform> waypoints, Transform homeBase, int index) 
	{
		Waypoints = waypoints;
		HomeBase = homeBase;
        LastWaypoint = HomeBase;
        HomebasePointIndex = index;
		CurrentWaypointIndex = 0;
		DestWaypointIndex = 0;
		CurrentWaypoint = Waypoints[CurrentWaypointIndex];

		DistFromPrevWaypoint = Vector3.Distance (HomeBase.position, CurrentWaypoint.position);
	}

	public bool IsArrive ()
	{
		return CurrentWaypointIndex == DestWaypointIndex;
	}

	public bool IsFinishPoint ()
	{
		return CurrentWaypointIndex == Waypoints.Count;
	}

	public bool IsInsideHomeBase ()
	{
		return HomeBase == CurrentWaypoint;
	}


	public void GoToNextWaypoint () 
	{

       // Debug.Log(".......................................................");
		LastWaypoint = CurrentWaypoint;
		CurrentWaypointIndex = Mathf.Min (CurrentWaypointIndex + 1, DestWaypointIndex);
		CurrentWaypoint = Waypoints[CurrentWaypointIndex];
		DistFromPrevWaypoint = Vector3.Distance (LastWaypoint.position, CurrentWaypoint.position);	
	}


/// <summary>
/// 
/// </summary>
    
    public void GoToPreviousWaypoint()
    {
        
        LastWaypoint = CurrentWaypoint;
        if (CurrentWaypointIndex - 1 < 0)
        {
            Debug.Log("Home Base " + HomeBase.transform.gameObject.name);
           CurrentWaypoint = HomeBase;



        }
        else
        {

            CurrentWaypoint = Waypoints[--CurrentWaypointIndex];
        }

        DistFromPrevWaypoint = Vector3.Distance(LastWaypoint.position, CurrentWaypoint.position);
    }
}