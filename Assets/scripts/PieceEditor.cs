using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PieceEditor : MonoBehaviour {

    public static PieceEditor singleton;
    public PieceScript PieceEditPreview;
    public PieceScript SelectedPiece;
    public Slider AtkSlide;
    public Slider DefSlide;
    public Slider RangeSlide;
    public Toggle isRanged;
    public InputField nameField;
    public GameObject Outline;
    public bool ShowingPromo;
    public Text PromoText;
    public GameObject EventSys;
    public bool EditVisible;
    private void Awake()
    {
        singleton = this;
    }

    public void ShowPromo()
    {
        Vector2 p=SelectedPiece.transform.position;
        if(ShowingPromo)
        {
            GameControl.singleton.transform.GetChild(0).position = GameControl.singleton.transform.GetChild(1).position;
            GameControl.singleton.transform.GetChild(1).position = new Vector3(-100, -100, -100);
            PromoText.text = "Show Promoted";
        }
        else
        {
            GameControl.singleton.transform.GetChild(1).position = GameControl.singleton.transform.GetChild(0).position;
            GameControl.singleton.transform.GetChild(0).position = new Vector3(-100, -100, -100);
            PromoText.text = "Show Base Pieces";
        }
        Physics2D.Raycast(p, Vector2.zero).collider.GetComponent<PieceEditSelect>().SelectPiece();
        ShowingPromo= !ShowingPromo;
    }

    public void EditAtk()
    {
        SelectedPiece.UpdateAtk((int)AtkSlide.value);
        PieceEditPreview.UpdateAtk((int)AtkSlide.value);
        GameControl.singleton.UpdateGamePieces(SelectedPiece.Index);
    }

    public void EditDef()
    {
        SelectedPiece.UpdateDef((int)DefSlide.value);
        PieceEditPreview.UpdateDef((int)DefSlide.value);
        GameControl.singleton.UpdateGamePieces(SelectedPiece.Index);
    }

    public void EditRange()
    {
        SelectedPiece.UpdateRange(isRanged.isOn, (int)RangeSlide.value);
        PieceEditPreview.UpdateRange(isRanged.isOn, (int)RangeSlide.value);
        GameControl.singleton.UpdateGamePieces(SelectedPiece.Index);
    }

    public void EditName()
    {
        SelectedPiece.UpdateName(nameField.text);
        PieceEditPreview.UpdateName(nameField.text);
        GameControl.singleton.UpdateGamePieces(SelectedPiece.Index);
    }

    public void ToggleEditor()
    {
        if (!EditVisible)
        {
            GameControl.singleton.transform.GetChild(2).position = GameControl.singleton.transform.GetChild(3).position;
            GameControl.singleton.transform.GetChild(3).position=new Vector3(-100,-100, -100);
        }
        else
        {
            GameControl.singleton.transform.GetChild(3).position = GameControl.singleton.transform.GetChild(2).position;
            GameControl.singleton.transform.GetChild(2).position = new Vector3(-100, -100, -100);
        }
        EditVisible= !EditVisible;
    }

    public void UpdatePreview()
    {
        for(int i=0;i<8;i++)
        {
            PieceEditPreview.UpdateMoveIndex(SelectedPiece.hasMoves[i], SelectedPiece.hasAttacks[i], i);
        }
        AtkSlide.value = SelectedPiece.Atk;
        DefSlide.value = SelectedPiece.Def;
        nameField.text=SelectedPiece.name;
        RangeSlide.value = SelectedPiece.AP;
        isRanged.isOn = SelectedPiece.isRanged;
        PieceEditPreview.GetComponent<SpriteRenderer>().sprite = SelectedPiece.GetComponent<SpriteRenderer>().sprite;
        PieceEditPreview.UpdateName(SelectedPiece.name);
        PieceEditPreview.UpdateAtk(SelectedPiece.Atk);
        PieceEditPreview.UpdateDef(SelectedPiece.Def);
        PieceEditPreview.UpdateRange(SelectedPiece.isRanged, SelectedPiece.AP);  
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
