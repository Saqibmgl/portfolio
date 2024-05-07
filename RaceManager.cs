using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using PowerslideKartPhysics;
public class RaceManager : MonoBehaviour
{
    public GameObject checkPoint;
    public GameObject checkPointHolder;

    public GameObject[] cars;

    public Transform[] checkPointPositions;
    public BasicWaypoint[] waypoints;
    public GameObject[] checkPointPositionsForCar;

    public int totalCars;
    public int totalCheckpoints;

    public float totalDistance;
    public float[] aicarsdistance;

    public static RaceManager instance;

    public CarCpmanager[] carfordistance;
    public CarCpmanager temp;

    public GameManager gm;
    public static int CarIndex;
    
    private GameObject gameobject;
  

    private void Awake()
    {
        instance = this;
        gm = GameManager.instance;
       
       
    }

    // Start is called before the first frame update

    void Start()
    {
        
        totalCars = cars.Length;
        CarIndex = PlayerPrefs.GetInt("SelectedBike") - 1;
        cars[0] = gm.player[CarIndex];
        carfordistance[0] = gm.player[CarIndex].GetComponent<CarCpmanager>();
        //totalCheckpoints = checkPointHolder.transform.childCount;

        //SetCheckPoints();
        //SetCarsPositions();
        InvokeRepeating(nameof(Checker), 0.1f, 0.1f);

        distancetotal();
    }
    

    int k;
    public void Checker()
    {
        print("called");
        for (int i = 0; i < carfordistance.Length - 1; i++)
        {
            // traverse i+1 to array length
            for (int j = i + 1; j < carfordistance.Length; j++)
            {
                if ((carfordistance[i].distance < carfordistance[j].distance)&&(carfordistance[i].laps <= carfordistance[j].laps))
                {
                    //k = i;
                    //carfordistance[i].carPosition = j;
                    //carfordistance[j].carPosition = k;
                    temp = carfordistance[i];
                    carfordistance[i] = carfordistance[j];
                    carfordistance[j] = temp;
                }
            }
        }
        for (int i = 0; i < carfordistance.Length ; i++)
        {
            
            carfordistance[i].carPosition = Array.IndexOf(carfordistance, carfordistance[i])+1;
        }
    }
    public void Cardiszero()
    {

        for (int i = 0; i < carfordistance.Length ; i++)
        {
            carfordistance[i].distance = aicarsdistance[i];  
        }
    }

    public void distancetotal()
    {
        for (int i = 0; i < totalCheckpoints-1; i++)
        {
            totalDistance += Vector3.Distance(checkPointPositions[i].position, checkPointPositions[i + 1].position);
          
        }
        for (int i = 0; i < totalCheckpoints; i++)
        {
            if (i == 0)
            {
                waypoints[i].distance = 0;

            }
            else
            {
                waypoints[i].distance = waypoints[i - 1].distance + Vector3.Distance(waypoints[i].gameObject.transform.position, waypoints[i - 1].gameObject.transform.position);
            }
        }
    }
    public void SetCheckPoints()
    {
        totalCheckpoints = checkPointHolder.transform.childCount;


        checkPointPositions = new Transform[totalCheckpoints];
        waypoints = new BasicWaypoint[totalCheckpoints];

        for (int i = 0; i < totalCheckpoints; i++)
        {
            checkPointPositions[i] = checkPointHolder.transform.GetChild(i).transform;
            waypoints[i] = checkPointHolder.transform.GetChild(i).GetComponent<BasicWaypoint>();
        }


        distancetotal();
        InvokeRepeating(nameof(Checker), 0.1f, 0.1f);
    }

}
