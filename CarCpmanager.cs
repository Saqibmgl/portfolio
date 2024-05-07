using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PowerslideKartPhysics;
using DG.Tweening;

public class CarCpmanager : MonoBehaviour
{
    public int carNumber;
    public int cpCrossed;
    public int carPosition;

    public Transform respawn;

    public RaceManager rM;



   
    public enum type { player,enemy};
    public type _type;
    
   
    public Text posText;
    public Text LapText;


    [Header("Distance")]
    public float distance;
   
    public Vector3 previosPos;

    public bool aiDistance;
    public bool oncelap;


    public int laps = 0;
    public GameManager gm;
    public bool player;
    private int star = 0;
    private int score;
    public static int playerIndex;
    public BasicWaypointFollowerPlayer basic;
    public GameObject bike;

    public Kart kart;
    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.instance;
       
    }

    // Update is called once per frame
    void Update()
    {
        playerIndex = PlayerPrefs.GetInt("SelectedCar") - 1;
        distance += Vector3.Distance(previosPos, transform.position);
        previosPos = transform.position;

           if(_type==type.player)
                posText.text = carPosition.ToString();

          
       
         
     }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("checkpoint"))
        {
            distance = other.GetComponent<BasicWaypoint>().distance;
           
            respawn = other.gameObject.transform;


        }
        if (other.gameObject.CompareTag("Finish")&&!oncelap)
        {
           
            if(!player)
            {
                laps += 1;

            }
               

            if (player && basic.wrong == false)
            {
                laps += 1;
                LapText.text = laps.ToString();
                if(laps <= 2)
                {
                    gm.LapCompletedText.text = "LAP " + laps.ToString();
                    gm.Lapcompleted.SetActive(true);
                }
               
            }
            
            oncelap = true;
            StartCoroutine(laponce());
            if(laps == 3 && !player)
            {
                StartCoroutine(FinishStopWait());
            }
            if (laps == 3 && player)
            {
                star = 0;
                score = 100; 
                if (carPosition < 4)
                {
                    for (int i = 3; i >= carPosition; i--)
                    {
                        AudioManager.instance.Stars();
                        gm.Levelcompleted();
                        gm.stars[star].SetActive(true);
                        PlayerPrefs.SetInt("TotalEarning", PlayerPrefs.GetInt("TotalEarning") + 100);
                        gm.LevelCompletedscore.text = score.ToString();
                        star = star + 1;


                        score = score + 100;
                    }
                    PlayerPrefs.SetInt("level" + PlayerPrefs.GetInt("level"), star);

                }

                

                else
                {
                    gm.Levelcompleted();
                    gm.LevelCompletedscore.text = 000.ToString();
                }
                   

            }
        }

       
       
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log(other.gameObject.name + " this");
            //Respawn();
      
            StartCoroutine(SpawnWait());
        }
    }

    IEnumerator laponce()
    {
        yield return new WaitForSeconds(2);
        oncelap = false;
    }
    public void Respawn()
    {
        //StartCoroutine(SpawnWait());
        this.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        transform.position = respawn.transform.position;
        transform.rotation = respawn.transform.rotation;
        this.gameObject.transform.GetChild(0).transform.localRotation = Quaternion.Euler(0, 0, 0);
    
        
    }

    IEnumerator SpawnWait()
    {
        yield return new WaitForSeconds(0.7f);
        Respawn();
        this.gameObject.GetComponent<Rigidbody>().isKinematic = true;

        bike.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        bike.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        bike.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        bike.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        bike.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        bike.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        bike.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        bike.SetActive(true);
       


        yield return new WaitForSeconds(0.4f);
     
        this.gameObject.GetComponent<Rigidbody>().isKinematic = false;

       
    }

    IEnumerator FinishStopWait()
    {
        yield return new WaitForSeconds(0.7f);
        this.gameObject.GetComponent<Rigidbody>().isKinematic = true;

    }
}
