using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

[System.Serializable]
public class TextesLvl1
{
    [SerializeField]
    public string tutoJump;
    public string tutoParry;
    public string tutoAttack;
    public string tutoCombat;
        
    public static TextesLvl1 CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<TextesLvl1>(jsonString);
    }

}

public class ProgressionControllerLvl1 : MonoBehaviour
{
    public string pathJson;
    private TextesLvl1 textes;

    public GameObject zoneTexte; 
    
    public Collider2D triggerFinLvl;
    public Collider2D triggerTutoJump;
    public Collider2D triggerTutoAttack;
    public Collider2D triggerTutoParry;
    public Collider2D triggerTutoCombat;
    
    void Start()
    {
        StreamReader reader = new StreamReader(pathJson);
        textes = TextesLvl1.CreateFromJSON(reader.ReadToEnd());
        reader.Close();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == triggerFinLvl)
        {
            Application.Quit();
            Debug.Log("quitte");
        }
        else if (other == triggerTutoJump)
        {
            zoneTexte.GetComponent<TextMeshProUGUI>().SetText(textes.tutoJump);
        }
        else if (other == triggerTutoAttack)
        {
            zoneTexte.GetComponent<TextMeshProUGUI>().SetText(textes.tutoAttack);
        }
        else if (other == triggerTutoParry)
        {
            zoneTexte.GetComponent<TextMeshProUGUI>().SetText(textes.tutoParry);
        }
        else if (other == triggerTutoCombat)
        {
            zoneTexte.GetComponent<TextMeshProUGUI>().SetText(textes.tutoCombat);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        zoneTexte.GetComponent<TextMeshProUGUI>().SetText("");
    }

}
