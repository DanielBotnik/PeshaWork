using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void UpdateScoreDelegate(int score);
public delegate void UpdateTimerDelegate(string timer);
public delegate void NoParametersDelegate();
public class GameHandler : MonoBehaviour
{
    private static GameHandler instance;

    public event UpdateScoreDelegate UpdateCurrentScoreEvent;
    public event UpdateScoreDelegate UpdateBestScoreEvent;
    public event UpdateTimerDelegate UpdateTimerEvent;
    public event NoParametersDelegate TimerOnEvent;
    public event NoParametersDelegate TimerOffEvent;
    public event NoParametersDelegate SetPauseEvent;
    public event NoParametersDelegate SetUnPauseEvent;
    public event UpdateTimerDelegate UpdatePauseTimerEvent;
    public event NoParametersDelegate OnWinEvent;
    public event NoParametersDelegate OnWinClickEvent;

    public static GameHandler GetInstance()
    {
        return instance;
    }
    private void Awake()
    {
        instance = this;
    }

    private const float GAP_SIZE_FROM_CORNER_X = 3f;
    private const float GAP_SIZE_FROM_CORNER_Y = 3f;
    private const float GAP_SIZE_FROM_TOP = 20f;
    private const float GAP_SIZE_FROM_BOTTOM = 10f;
    private const int ALLOWD_PAUSE_TIME = 300;
    private const int TIME_NEED_TO_WIN = 120;


    private const int MAX_MEDITATION_NEED = 40;
    private const int MAX_ATTENTION_NEED = 40;

    private const float INCRASE_MOVING_SPEED = 30f;
    private const float DECREASE_MOVING_SPEED = 50f;

    private GameObject meditationObject;
    private GameObject attentionObject;
    private Camera mainCamera;
    private ThinkGear thinkgear;

    private AudioSource audioSource;
    [SerializeField]
    private AudioClip startCountingClip;
    [SerializeField]
    private AudioClip stopCountingClip;

    private bool bothGood;

    private float bothGoodTimer;
    private float elapsedTime;
    private float pauseTime;
    private float elapsedPauseTime;
    private int score;
    private int bestScore;

    private bool isPaused;

    private Vector3 startPositionMeditation;
    private Vector3 startPositionAttention;

    private int meditationValue;
    private int attentaionValue;

    private float currentMeditationValue;
    private float currentAttentionValue;

    private bool freeze;

    private void Start()
    {
        InitFields();
        InitPositions();
    }

    private void Update()
    {
        if (freeze)
            return;
        if (isPaused)
        {
            HandlePause();
        }
        else
        {
            HandleMeditation();
            HandleAttention();
            HandleBoth();
        }
    }

    private void InitFields(){
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        audioSource = GetComponent<AudioSource>();
        meditationObject = GameObject.Find("MeditationObject");
        attentionObject = GameObject.Find("AttentionObject");
        thinkgear = GameObject.Find("ThinkGear").GetComponent<ThinkGear>();
        thinkgear.UpdateConnectedStateEvent += () => { thinkgear.StartMonitoring(); };
        thinkgear.UpdateMeditationEvent += OnMeditationUpdate;
        thinkgear.UpdateAttentionEvent += OnAttentionUpdate;
        meditationValue = 0;
        currentMeditationValue = 0;
        attentaionValue = 0;
        currentAttentionValue = 0;
        bothGood = false;
        bothGoodTimer = 0;
        elapsedTime = 0;
        elapsedPauseTime = 0;
        bestScore = PlayerPrefs.GetInt("bestScore");
        isPaused = false;
        freeze = false;
    }

    private void InitPositions()
    {
        Vector3 edge = mainCamera.ScreenToWorldPoint(Vector3.zero);
        float meditationX = edge.x + meditationObject.transform.localScale.x / 2;
        meditationX += GAP_SIZE_FROM_CORNER_X / (mainCamera.aspect * 2f * mainCamera.orthographicSize);
        float medittaionY = edge.y + meditationObject.transform.localScale.y / 2;
        medittaionY += GAP_SIZE_FROM_CORNER_Y / (mainCamera.orthographicSize * 2f);
        medittaionY += GAP_SIZE_FROM_BOTTOM / (mainCamera.orthographicSize * 2f);
        startPositionMeditation = new Vector3(meditationX, medittaionY, 0);
        meditationObject.transform.position = startPositionMeditation;
        float attentionX = edge.x + attentionObject.transform.localScale.x / 2;
        attentionX += GAP_SIZE_FROM_CORNER_X / (mainCamera.aspect * 2f * mainCamera.orthographicSize);
        float attentionY = edge.y + attentionObject.transform.localScale.y / 2;
        attentionY += GAP_SIZE_FROM_CORNER_Y / (mainCamera.orthographicSize * 2f);
        attentionY += GAP_SIZE_FROM_TOP / (mainCamera.orthographicSize * 2f);
        startPositionAttention = new Vector3(-attentionX, -attentionY, 0);
        attentionObject.transform.position = startPositionAttention;
    }

    private void OnMeditationUpdate(int value)
    {
        if (isPaused) return;
        meditationValue = Mathf.Min(MAX_MEDITATION_NEED,value);
    }

    private void OnAttentionUpdate(int value)
    {
        if (isPaused) return;
        attentaionValue = Mathf.Min(MAX_ATTENTION_NEED,value);
    }
    private void HandleMeditation()
    {
        if (currentMeditationValue < meditationValue)
        {
            currentMeditationValue += INCRASE_MOVING_SPEED * Time.deltaTime;
            if (currentMeditationValue > meditationValue)
                currentMeditationValue = meditationValue;
            meditationObject.transform.position = Vector3.Lerp(startPositionMeditation, Vector2.zero,currentMeditationValue / MAX_MEDITATION_NEED);
        }
        else if (currentMeditationValue > meditationValue)
        {
            currentMeditationValue -= DECREASE_MOVING_SPEED * Time.deltaTime;
            if (currentMeditationValue < 0)
                currentMeditationValue = 0;
            meditationObject.transform.position = Vector3.Lerp(startPositionMeditation, Vector2.zero,currentMeditationValue / MAX_MEDITATION_NEED);
        }
    }

    private void HandleAttention()
    {
        if(currentAttentionValue < attentaionValue)
        {
            currentAttentionValue += INCRASE_MOVING_SPEED * Time.deltaTime;
            if (currentAttentionValue > attentaionValue)
                currentAttentionValue = attentaionValue;
            attentionObject.transform.position = Vector3.Lerp(startPositionAttention, Vector2.zero, currentAttentionValue / MAX_ATTENTION_NEED);
        }
        else if(currentAttentionValue > attentaionValue)
        {
            currentAttentionValue -= DECREASE_MOVING_SPEED * Time.deltaTime;
            if (currentAttentionValue < 0)
                currentAttentionValue = 0;
            attentionObject.transform.position = Vector3.Lerp(startPositionAttention, Vector2.zero, currentAttentionValue / MAX_ATTENTION_NEED);
        }
    }

    private void HandleBoth()
    {
        if (meditationValue < MAX_MEDITATION_NEED || attentaionValue < MAX_ATTENTION_NEED)
        {
            if (bothGood)
            {
                audioSource.PlayOneShot(stopCountingClip);
                bothGood = false;
                TimerOffEvent?.Invoke();
                score = 0;
                UpdateCurrentScoreEvent?.Invoke(score);
            }
            return;
        }
        if (!bothGood)
        {
            bothGoodTimer = 0;
            audioSource.PlayOneShot(startCountingClip);
            bothGood = true;
            TimerOnEvent?.Invoke();
            UpdateTimerEvent?.Invoke($"{TIME_NEED_TO_WIN/60}:{(TIME_NEED_TO_WIN%60)/10}{(TIME_NEED_TO_WIN%60)%10}");
        }

        bothGoodTimer += Time.deltaTime;
        elapsedTime += Time.deltaTime;
        if(elapsedTime > 1f)
        {
            elapsedTime %= 1f;
            score = (int)((int)bothGoodTimer * 8.34f);
            if(score > bestScore)
            {
                bestScore = score;
                UpdateBestScoreEvent?.Invoke(bestScore);
                PlayerPrefs.SetInt("bestScore", bestScore);
            }
            int clockTime = TIME_NEED_TO_WIN - (int)bothGoodTimer;
            UpdateTimerEvent?.Invoke($"{clockTime / 60}:{(clockTime % 60) / 10}{(clockTime % 60) % 10}");
            UpdateCurrentScoreEvent(score);
        }
        
        if(bothGoodTimer > TIME_NEED_TO_WIN)
        {
            Win();
        }

        
    }

    public void Pause()
    {
        meditationObject.GetComponent<Animator>().enabled = false;
        attentionObject.GetComponent<Animator>().enabled = false;
        pauseTime = 0;
        elapsedPauseTime = 0;
        isPaused = true;
        SetPauseEvent?.Invoke();
    }

    public void UnPause()
    {
        meditationObject.GetComponent<Animator>().enabled = true;
        attentionObject.GetComponent<Animator>().enabled = true;
        isPaused = false;
        SetUnPauseEvent?.Invoke();
        UpdatePauseTimerEvent?.Invoke($"{ALLOWD_PAUSE_TIME/60}:{(ALLOWD_PAUSE_TIME%60)/10}{(ALLOWD_PAUSE_TIME % 60) % 10}");
    }

    private void HandlePause()
    {
        pauseTime += Time.deltaTime;
        elapsedPauseTime += Time.deltaTime;
        if(pauseTime > ALLOWD_PAUSE_TIME)
        {
            bothGood = false;
            TimerOffEvent?.Invoke();
            score = 0;
            meditationValue = 0;
            meditationObject.transform.position = startPositionMeditation;
            attentaionValue = 0;
            attentionObject.transform.position = startPositionAttention;
            UpdateCurrentScoreEvent?.Invoke(score);
            UpdatePauseTimerEvent?.Invoke("Pause Was to long...\nRestarting.");
            freeze = true;
        }
        else
        {
            if(elapsedPauseTime > 1f)
            {
                elapsedPauseTime %= 1f;
                int clockTime = ALLOWD_PAUSE_TIME - (int)pauseTime;
                UpdatePauseTimerEvent?.Invoke($"{clockTime / 60}:{(clockTime % 60) / 10}{(clockTime % 60) % 10}");
            }
        }
    }

    private void Win()
    {
        freeze = true;
        meditationObject.GetComponent<Animator>().enabled = false;
        attentionObject.GetComponent<Animator>().enabled = false;
        OnWinEvent?.Invoke();
    }

    public void OnWinClick()
    {
        bothGood = false;
        score = 0;
        meditationValue = 0;
        attentaionValue = 0;
        UpdateTimerEvent?.Invoke($"{TIME_NEED_TO_WIN / 60}:{(TIME_NEED_TO_WIN % 60) / 10}{(TIME_NEED_TO_WIN % 60) % 10}");
        UpdateCurrentScoreEvent(TIME_NEED_TO_WIN);
        OnWinClickEvent?.Invoke();
        UpdateCurrentScoreEvent?.Invoke(score);
        meditationObject.GetComponent<Animator>().enabled = true;
        attentionObject.GetComponent<Animator>().enabled = true;
        freeze = false;
    }

}
