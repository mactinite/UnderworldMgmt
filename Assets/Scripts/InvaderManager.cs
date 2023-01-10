using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class InvaderManager : MonoBehaviour
{
    public InvaderBrain invaderPrefab;
    public AnimationCurve difficultyCurve;
    public int baseAmount = 2;
    public int maxAmount = 50;
    public int maxRounds = 20;
    public float interval = 2f * 60f;
    public float currentTime = 0f;
    public int currentWave = 1;
    private List<InvaderBrain> spawnedInvaders = new List<InvaderBrain>();
    private bool invasionsStarted;
    public TMP_Text timer;
    public TMP_Text roundCounter;
    public GameObject winScreen;
    public GameObject loseScreen;

    [SerializeField] private float spawnRadius = 10f;
    public static InvaderManager Instance;

    private void Awake()
    {
        if (InvaderManager.Instance != null)
        {
            DestroyImmediate(this.gameObject);
        }
        else
        {
            InvaderManager.Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        timer.gameObject.SetActive(false);
    }

    void BeginInvasions()
    {
        invasionsStarted = true;
        timer.gameObject.SetActive(true);
        StartCoroutine(InvasionRoutine());
    }

    IEnumerator InvasionRoutine()
    {
        currentTime = interval;
        while (currentTime > 0)
        {
            currentTime -= Time.deltaTime;

            float minutes = Mathf.FloorToInt(currentTime / 60f);
            float seconds = currentTime % 60;
            roundCounter.text = $"{maxRounds - currentWave} Waves Remain";
            timer.text = $"Time until invasion: {minutes:00}:{seconds:00}";
            yield return null;
        }

        currentTime = 0;

        int enemiesToSpawn = Mathf.RoundToInt(Mathf.Lerp(baseAmount, maxAmount,
            difficultyCurve.Evaluate((float) currentWave / maxRounds)));

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            iTween.PunchScale(timer.gameObject, Vector3.one * 1.2f, 0.3f);
            timer.text = $"Invasion";
            var invader = Instantiate(invaderPrefab,
                transform.position + (Vector3) Random.insideUnitCircle.normalized * (spawnRadius),
                invaderPrefab.transform.rotation);
            spawnedInvaders.Add(invader);
            yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
        }

        while (spawnedInvaders.Count > 0)
        {
            timer.text = $"Invasion";
            for (int i = spawnedInvaders.Count - 1; i >= 0; i--)
            {
                if (spawnedInvaders[i] == null || spawnedInvaders[i].dead)
                {
                    spawnedInvaders.RemoveAt(i);
                }
            }

            yield return null;
        }

        if (currentWave + 1 < maxRounds)
        {
            iTween.PunchScale(roundCounter.gameObject, Vector3.one * 1.2f, 0.3f);
            currentWave++;
            StartCoroutine(InvasionRoutine());
        }
        else
        {
            Win();
        }
        

    }

    // Update is called once per frame
    void Update()
    {
        if (HordeManager.Instance.hordeMembers.Count > 0 && !invasionsStarted)
        {
            BeginInvasions();
        }
    }

    public void Win()
    {
        winScreen.SetActive(true);
    }

    public void Lose()
    {
        loseScreen.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }
    
    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}