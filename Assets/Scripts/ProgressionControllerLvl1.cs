using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


[Serializable]
public class TextesLvl1
{
    [SerializeField]
    public string tutoJump;
    public string tutoParry;
    public string tutoAttack;
    public string tutoCombat;
    public string pause;
    public string finPartie;
    public string mortJoueur;
    public string shardHeal1;
    public string shardHeal2;
    public string shardHeal3;
        
    public static TextesLvl1 CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<TextesLvl1>(jsonString);
    }

}

public class ProgressionControllerLvl1 : MonoBehaviour
{
    public PauseControl pause;

    private string pathJson = Application.streamingAssetsPath + "/Textes/textsLvl1.json";
    public static TextesLvl1 textes;

    public GameObject zoneTexte;
    public TextMeshProUGUI texteMenu;
    public PlayerInput playerInput;

    public Collider2D triggerFinLvl;
    public Collider2D triggerTutoJump;
    public Collider2D triggerTutoAttack;
    public Collider2D triggerTutoParry;
    public Collider2D triggerTutoCombat;
    public Collider2D triggerShardHeal;

    private string[] shardHealDialog;
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
        textes = TextesLvl1.CreateFromJSON(reader.ReadToEnd());
        reader.Close();

        shardHealDialog = new string[] {textes.shardHeal1, textes.shardHeal2, textes.shardHeal3};
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
            if (triggerShardHeal.enabled) triggerShardHeal.enabled = false;
            if (countDialog == 0)
            {
                zoneTexte.GetComponent<TextMeshProUGUI>().color = dialogGrimmColor;
                zoneTexte.GetComponent<TextMeshProUGUI>().SetText(shardHealDialog[countDialog++]);
                GrimmSpeakShard();
            }

            if (Input.GetKeyUp(KeyCode.E) || Input.GetMouseButtonUp(0) || Input.GetButtonUp("Fire2"))
            {
                shortGrimmDialog.Stop();
                midGrimmDialog.Stop();
                longGrimmDialog.Stop();
                if (countDialog >= shardHealDialog.Length)
                {
                    playerInput.enabled = true;
                    waitingForInput = false;
                    zoneTexte.GetComponent<TextMeshProUGUI>().SetText("");
                }
                else
                {
                    GrimmSpeakShard();
                    zoneTexte.GetComponent<TextMeshProUGUI>().color = dialogGrimmColor;
                    zoneTexte.GetComponent<TextMeshProUGUI>().SetText(shardHealDialog[countDialog++]);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == triggerFinLvl)
        {
            EndGame();
        }
        else if (other == triggerTutoJump)
        {
            zoneTexte.GetComponent<TextMeshProUGUI>().color = dialogGrimmColor;
            zoneTexte.GetComponent<TextMeshProUGUI>().SetText(textes.tutoJump);
            shortGrimmDialog.volume = lowVolumeDialog;
            shortGrimmDialog.Play();
        }
        else if (other == triggerTutoAttack)
        {
            zoneTexte.GetComponent<TextMeshProUGUI>().SetText(textes.tutoAttack);
            zoneTexte.GetComponent<TextMeshProUGUI>().color = dialogGrimmColor;
            shortGrimmDialog.volume = lowVolumeDialog;
            shortGrimmDialog.Play();
        }
        else if (other == triggerTutoParry)
        {
            zoneTexte.GetComponent<TextMeshProUGUI>().SetText(textes.tutoParry);
            zoneTexte.GetComponent<TextMeshProUGUI>().color = dialogGrimmColor;
            longGrimmDialog.volume = lowVolumeDialog;
            longGrimmDialog.Play();

        }
        else if (other == triggerTutoCombat)
        {
            zoneTexte.GetComponent<TextMeshProUGUI>().SetText(textes.tutoCombat);
            zoneTexte.GetComponent<TextMeshProUGUI>().color = dialogGrimmColor;
            midGrimmDialog.volume = lowVolumeDialog;
            midGrimmDialog.Play();
        }
        else if (other == triggerShardHeal)
        {
            playerInput.enabled = false;
            waitingForInput = true;
        }


    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Level 1")
        {
            zoneTexte.GetComponent<TextMeshProUGUI>().SetText("");
            zoneTexte.GetComponent<TextMeshProUGUI>().color = Color.white;

            longGrimmDialog.volume = highVolumeDialog;
            midGrimmDialog.volume = highVolumeDialog;
            shortGrimmDialog.volume = highVolumeDialog;
        }
    }

    void GrimmSpeakShard()
        {
            switch (countDialog)
            {
                case 0:
                    shortGrimmDialog.Play();
                    break;
                case 1:
                    longGrimmDialog.Play();
                    break;
                case 2:
                    midGrimmDialog.Play();
                    break;

            }
        }

        private void EndGame()
        {
            pause.PauseGame();
            texteMenu.SetText(textes.finPartie);

            Time.timeScale = 0f;
        }

    }
