using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Reflection;

public class PieceScript : MonoBehaviour {
    public string name;
    public int Atk;
    public int Def;
    public int AP;
    public bool[] hasMoves;
    public bool[] hasAttacks;
    public bool isRanged;
    public Text AtkDefText;
    public Text RangedText;
    public Text NameText;
    public int Index;
    public bool hasPromo;
    public bool playerOne;
    public int Value;

    // Use this for initialization
    void Start () {
        AtkDefText = transform.GetChild(8).GetChild(0).GetComponent<Text>();
        RangedText = transform.GetChild(8).GetChild(1).GetComponent<Text>();
        NameText = transform.GetChild(8).GetChild(2).GetComponent<Text>();
        hasMoves = new bool[8];
        hasAttacks = new bool[8];
    }

    public void UpdateMoveIndex(bool mov, bool atk, int index)
    {
        hasMoves[index] = mov;
        hasAttacks[index] = atk;
        if (mov && atk)
        {
            transform.GetChild(index).GetComponent<SpriteRenderer>().enabled = true;
            transform.GetChild(index).GetComponent<SpriteRenderer>().color = Color.yellow;
        }
        else if(mov)
        {
            transform.GetChild(index).GetComponent<SpriteRenderer>().enabled = true;
            transform.GetChild(index).GetComponent<SpriteRenderer>().color = Color.green;
        }
        else if(atk)
        {
            transform.GetChild(index).GetComponent<SpriteRenderer>().enabled = true;
            transform.GetChild(index).GetComponent<SpriteRenderer>().color = Color.red;
        }
        else
            transform.GetChild(index).GetComponent<SpriteRenderer>().enabled = false;
    }

    public void UpdateAtk(int A)
    {
        Atk = A;
        AtkDefText.text = Atk.ToString() + "/" + Def.ToString();
    }

    public void UpdateDef(int D)
    {
        Def = D;
        AtkDefText.text = Atk.ToString() + "/" + Def.ToString();
    }

    public void UpdateRange(bool ranged, int ap)
    {
        isRanged = ranged;
        AP = ap;
        if (isRanged)
            RangedText.text = "Ranged " + AP.ToString();
        else
            RangedText.text = "Melee " + AP.ToString();
    }

    public void UpdateName(string n)
    {
        name = n;
        NameText.text = name;
    }

    public void PieceVal()
    {
        float a = 0;
        float m = 0;
        for (int i = 0; i < 8; i++)
        {
            if (hasMoves[i])
                m++;
            if (hasAttacks[i])
                a++;
        }
        Value=Mathf.RoundToInt((m + 1.5f * a) * AP * (1.5f * Def + Atk));

    }

    public void UpdateAll()
    {
        NameText.text = name;
        if (isRanged)
            RangedText.text = "Ranged " + AP.ToString();
        else
            RangedText.text = "Melee " + AP.ToString();
        AtkDefText.text = Atk.ToString() + "/" + Def.ToString();
        for(int i=0;i<8;i++)
        {
            if (hasMoves[i] && hasAttacks[i])
            {
                transform.GetChild(i).GetComponent<SpriteRenderer>().enabled = true;
                transform.GetChild(i).GetComponent<SpriteRenderer>().color = Color.yellow;
            }
            else if (hasMoves[i])
            {
                transform.GetChild(i).GetComponent<SpriteRenderer>().enabled = true;
                transform.GetChild(i).GetComponent<SpriteRenderer>().color = Color.green;
            }
            else if (hasAttacks[i])
            {
                transform.GetChild(i).GetComponent<SpriteRenderer>().enabled = true;
                transform.GetChild(i).GetComponent<SpriteRenderer>().color = Color.red;
            }
            else
                transform.GetChild(i).GetComponent<SpriteRenderer>().enabled = false;
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
