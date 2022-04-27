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
    
    public Collider2D triggerFinLvl;
    public Collider2D triggerTutoJump;
    public Collider2D triggerTutoAttack;
    public Collider2D triggerTutoParry;
    public Collider2D triggerTutoCombat;

    public Color dialogGrimmColor;
    
    void Start()
    {
        dialogGrimmColor = new Color(72f / 256f, 72f / 256f, 108f / 256f);

        StreamReader reader = new StreamReader(pathJson);
        textes = TextesLvl1.CreateFromJSON(reader.ReadToEnd());
        reader.Close();
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
            
        }
        else if (other == triggerTutoAttack)
        {
            zoneTexte.GetComponent<TextMeshProUGUI>().SetText(textes.tutoAttack);
            zoneTexte.GetComponent<TextMeshProUGUI>().color = dialogGrimmColor;
        }
        else if (other == triggerTutoParry)
        {
            zoneTexte.GetComponent<TextMeshProUGUI>().SetText(textes.tutoParry);
            zoneTexte.GetComponent<TextMeshProUGUI>().color = dialogGrimmColor;
        }
        else if (other == triggerTutoCombat)
        {
            zoneTexte.GetComponent<TextMeshProUGUI>().SetText(textes.tutoCombat);
            zoneTexte.GetComponent<TextMeshProUGUI>().color = dialogGrimmColor;
        }

        
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        zoneTexte.GetComponent<TextMeshProUGUI>().SetText("");
        zoneTexte.GetComponent<TextMeshProUGUI>().color = Color.white;
    }

    private void EndGame()
    {
        pause.PauseGame();
        texteMenu.SetText(textes.finPartie);

        Time.timeScale = 0f;
    }
    
}
