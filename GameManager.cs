using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using PowerslideKartPhysics;
using DG.Tweening;

using MTAssets.EasyMinimapSystem;
public class GameManager : MonoBehaviour
{



    public static GameManager instance;
    public Level_Info[] levels;
    public int currentLevel;

    public GameObject[] player;
    public GameObject[] Ai;
    public GameObject lvlCompleted;
    public GameObject lvlFailed, pausePnl;


    public static int CarIndex;

    public Slider loadingBar;
    public GameObject LoadingPnl;
    private Level_Info levelInfo;

    public GameObject[] stars;
    public Text LevelCompletedscore;

    public KartCamera kc;
    public UIControl UI;

    private BasicWaypoint waypoint;

    public RaceManager rm;

    public GameObject bg;
    //public GameObject[] AiAudio;
    public GameObject[] PlayerAudio;
    public GameObject canvas;
    public GameObject level1;
    public GameObject level2;
    public GameObject level3;
    public GameObject level4;
    public MinimapRenderer minimaprenderer;
    public GameObject Lapcompleted;
    public Text LapCompletedText;

    private void Awake()
    {

        instance = this;


    }

    void Start()
    {
        AdsPluginScript.Instance.RequestBannerTop();

        Time.timeScale = 1f;




        currentLevel = PlayerPrefs.GetInt("level");
        waypoint = levels[currentLevel].transform.GetChild(0).GetChild(0).GetComponent<BasicWaypoint>();

        for (int i = 0; i < Ai.Length; i++)
        {
            Ai[i].GetComponent<BasicWaypointFollowerDrift>().targetPoint = waypoint;


            //Ai[i].SetActive(true);
        }
        rm.checkPointHolder = levels[currentLevel].chkpointholder;
        rm.SetCheckPoints();


        Starting();
        SetEnvir();

    }

    // Update is called once per frame
    void Update()
    {




    }

    public void SetEnvir()
    {

        levels[currentLevel].gameObject.SetActive(true);
        CarIndex = PlayerPrefs.GetInt("SelectedBike") - 1;
        player[CarIndex].transform.position = levels[currentLevel].playerSpawnpoint.position;
        player[CarIndex].transform.rotation = levels[currentLevel].playerSpawnpoint.rotation;


        player[CarIndex].SetActive(true);

        minimaprenderer.minimapCameraToShow = player[CarIndex].GetComponent<MinimapCamera>();

        kc.targetKart = player[CarIndex].GetComponent<Kart>();
        UI.targetKart = player[CarIndex].GetComponent<Kart>();
        UI.Initialize(player[CarIndex].GetComponent<Kart>());
        Ai[0].transform.position = levels[currentLevel].AiSpwanPoint1.position;
        Ai[0].transform.rotation = levels[currentLevel].AiSpwanPoint1.rotation;
        Ai[1].transform.position = levels[currentLevel].AiSpwanPoint2.position;
        Ai[1].transform.rotation = levels[currentLevel].AiSpwanPoint2.rotation;
        Ai[2].transform.position = levels[currentLevel].AiSpwanPoint3.position;
        Ai[2].transform.rotation = levels[currentLevel].AiSpwanPoint3.rotation;


        if (currentLevel == 0)
        {


            level1.SetActive(true);
            level2.SetActive(false);
            level3.SetActive(false);
            level4.SetActive(false);

        }
        else if (currentLevel == 1 || currentLevel == 11)
        {
            level1.SetActive(false);
            level2.SetActive(true);
            level3.SetActive(false);
            level4.SetActive(false);
        }
        else if (currentLevel == 2 || currentLevel == 6 || currentLevel == 8)
        {
            level1.SetActive(false);
            level2.SetActive(false);
            level3.SetActive(true);
            level4.SetActive(false);
        }

        else
        {
            level1.SetActive(false);
            level2.SetActive(false);
            level3.SetActive(false);
            level4.SetActive(true);
        }




    }

    public void Levelcompleted()
    {

        StartCoroutine(LevelCompletedWait());

       
    }
    public void NextButton()
    {

        AudioManager.instance.ButtonClick();
        PlayerPrefs.SetInt("level", PlayerPrefs.GetInt("level") + 1);


        Time.timeScale = 1f;
        StartCoroutine(Wait("GamePlay"));


    }
    public void HomeButton()
    {
        AudioManager.instance.ButtonClick();
        AudioListener.volume = 1;
        Time.timeScale = 1f;
        StartCoroutine(Wait("MainMenu"));


    }

    public void Restart()
    {
        AudioManager.instance.ButtonClick();

        Time.timeScale = 1f;
        AudioListener.volume = 1;
        StartCoroutine(Wait("GamePlay"));

    }
    public void LevelFailed()
    {

        GameObject.Find("All Audio Sources").SetActive(false);
        lvlFailed.SetActive(true);
    }

    public void pauseBtn()
    {
        AudioManager.instance.ButtonClick();
        AdsPluginScript.Instance.ShowInterstitial();
        Time.timeScale = 0f;

        AudioListener.volume = 0;
        pausePnl.SetActive(true);


    }
    public void Resume()
    {
        AudioManager.instance.ButtonClick();
        Time.timeScale = 1f;
        AudioListener.volume = 1;
        pausePnl.SetActive(false);


    }

    IEnumerator Wait(string scene)
    {
        LoadingPnl.SetActive(true);
        AsyncOperation async = SceneManager.LoadSceneAsync(scene);

        if (loadingBar.value == 100)
            async.allowSceneActivation = true;

        while (!async.isDone)
        {
            loadingBar.value = (int)((async.progress) * 112);
            yield return null;
        }
    }
    public void Starting()
    {
        StartCoroutine(CarsWait());
    }
    IEnumerator CarsWait()
    {
        for (int i = 0; i < Ai.Length; i++)
        {

            Ai[i].GetComponent<BasicWaypointFollowerDrift>().enabled = false;
        }

        yield return new WaitForSeconds(3.5f);

        canvas.SetActive(true);

        for (int i = 0; i < Ai.Length; i++)
        {

            Ai[i].GetComponent<BasicWaypointFollowerDrift>().enabled = true;
        }
    }
    IEnumerator LevelCompletedWait()
    {


        yield return new WaitForSeconds(1f);
        //PlayerPrefs.SetInt("TotalEarning", PlayerPrefs.GetInt("TotalEarning") );
        //GameObject.Find("All Audio Sources").SetActive(false);
        Time.timeScale = 0;
        bg.SetActive(false);
        AudioManager.instance.LevelComplete();
        PlayerAudio[CarIndex].SetActive(false);
        //for(int i=0; i<AiAudio.Length; i++)
        //{
        //    AiAudio[i].SetActive(false);

        //}
        lvlCompleted.SetActive(true);



        if (currentLevel == PlayerPrefs.GetInt("UnlockedLevel"))
        {

            PlayerPrefs.SetInt("UnlockedLevel", PlayerPrefs.GetInt("UnlockedLevel") + 1);



        }

    }
}