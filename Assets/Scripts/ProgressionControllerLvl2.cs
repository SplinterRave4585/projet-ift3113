using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


[Serializable]
public class TextesLvl2
{
    [SerializeField] 
    public string startLvl;
    public string shard1;
    public string shard2;
    public string shard3;
    public string bandit1;
    public string bandit2;
        
    public static TextesLvl2 CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<TextesLvl2>(jsonString);
    }

}

public class ProgressionControllerLvl2 : MonoBehaviour
{
     public PauseControl pause;

    private string pathJson = Application.streamingAssetsPath + "/Textes/textsLvl2.json";
    public static TextesLvl2 textes;

    public GameObject zoneTexte;
    public TextMeshProUGUI texteMenu;
    public PlayerInput playerInput;
    
    // triggers
    public Collider2D triggerStartLvl;
    public Collider2D triggerShard;
    public Collider2D triggerBandit;
    public Collider2D triggerEndGame;

    private string currentDialog;
    private Dictionary<string, List<Tuple<string, AudioSource>>> dialogs = new Dictionary<string, List<Tuple<string, AudioSource>>>();
    private List<Tuple<string, AudioSource>> shardDialog;
    private List<Tuple<string, AudioSource>> startDialog;
    private List<Tuple<string, AudioSource>> banditDialog;
    
    private int countDialog = 0;
    private bool waitingForInput = false;
    
    private Color dialogGrimmColor;

    private AudioSource shortGrimmDialog;
    private AudioSource midGrimmDialog;
    private AudioSource longGrimmDialog;
    public float highVolumeDialog = 0.5f;
    public float lowVolumeDialog = 0.25f;
    
    void Start()
    {
        dialogGrimmColor = new Color(72f / 256f, 72f / 256f, 108f / 256f);

        StreamReader reader = new StreamReader(pathJson);
        textes = TextesLvl2.CreateFromJSON(reader.ReadToEnd());
        reader.Close();

        shardDialog = new List<Tuple<string, AudioSource>>
        {
            Tuple.Create(textes.shard1, shortGrimmDialog),
            Tuple.Create(textes.shard2, longGrimmDialog), 
            Tuple.Create(textes.shard3,longGrimmDialog)
        };
        startDialog = new List<Tuple<string, AudioSource>>{Tuple.Create(textes.startLvl, shortGrimmDialog)};
        
        banditDialog = new List<Tuple<string, AudioSource>>
        {
            Tuple.Create(textes.bandit1, midGrimmDialog), 
            Tuple.Create(textes.bandit2, shortGrimmDialog)
        };

        dialogs.Add("start", startDialog);
        dialogs.Add("shard",shardDialog);
        dialogs.Add("bandit", banditDialog);
        
    }

    private void Awake()
    {
        shortGrimmDialog = GameObject.Find("GrimmDialogSFX/Short").GetComponent<AudioSource>();
        midGrimmDialog = GameObject.Find("GrimmDialogSFX/Mid").GetComponent<AudioSource>();
        longGrimmDialog = GameObject.Find("GrimmDialogSFX/Long").GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (waitingForInput)
        {
            if (countDialog == 0)
            {
                zoneTexte.GetComponent<TextMeshProUGUI>().color = dialogGrimmColor;
                Debug.Log(dialogs[currentDialog][countDialog].Item1);
                zoneTexte.GetComponent<TextMeshProUGUI>().SetText(dialogs[currentDialog][countDialog].Item1);
                dialogs[currentDialog][countDialog++].Item2.Play();
            }

            else if (Input.GetKeyUp(KeyCode.E) || Input.GetMouseButtonUp(0) || Input.GetButtonUp("Fire2"))
            {
                shortGrimmDialog.Stop();
                midGrimmDialog.Stop();
                longGrimmDialog.Stop();
                if (countDialog >= dialogs[currentDialog].Count)
                {
                    playerInput.enabled = true;
                    waitingForInput = false;
                    zoneTexte.GetComponent<TextMeshProUGUI>().SetText("");
                    zoneTexte.GetComponent<TextMeshProUGUI>().color = Color.white;
                    countDialog = 0;
                }
                else
                {
                    zoneTexte.GetComponent<TextMeshProUGUI>().color = dialogGrimmColor;
                    zoneTexte.GetComponent<TextMeshProUGUI>().SetText(dialogs[currentDialog][countDialog].Item1);
                    dialogs[currentDialog][countDialog++].Item2.Play();
                }
            }


            if (currentDialog == "start" && triggerStartLvl.enabled) triggerStartLvl.enabled = false;
            else if (currentDialog == "shard" && triggerShard.enabled) triggerShard.enabled = false;
            else if (currentDialog == "bandit" && triggerBandit.enabled) triggerBandit.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        countDialog = 0;
        if (other == triggerStartLvl)
        {
            playerInput.enabled = false;
            currentDialog = "start";
            waitingForInput = true;

        }
        else if (other == triggerShard)
        {
            playerInput.enabled = false;
            currentDialog = "shard";
            waitingForInput = true;
        }
        else if (other == triggerBandit)
        {
            playerInput.enabled = false;
            currentDialog = "bandit";
            waitingForInput = true;
        }
        else if (other == triggerEndGame)
        {
            EndGame();
        }

        
    }
/*
    private void OnTriggerExit2D(Collider2D other)
    {
        zoneTexte.GetComponent<TextMeshProUGUI>().SetText("");
        zoneTexte.GetComponent<TextMeshProUGUI>().color = Color.white;
        
        longGrimmDialog.volume = highVolumeDialog;
        midGrimmDialog.volume = highVolumeDialog;
        shortGrimmDialog.volume = highVolumeDialog;
    }
*/
    private void EndGame()
    {
        SceneManager.LoadScene("EndScene");
    }
}
