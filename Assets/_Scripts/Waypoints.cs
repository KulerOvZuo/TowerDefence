using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

#pragma warning disable 0649

public class Waypoints : MonoBehaviour {

    [SerializeField] public Transform startPoint;
    [SerializeField] public Transform endPoint;
    [SerializeField] public Transform[] wayPoints;	
	// Use this for self-initialization
    void Awake(){
        Assert.IsNotNull(startPoint);
        Assert.IsNotNull(endPoint);
        Assert.IsNotNull(wayPoints);
    }
}
