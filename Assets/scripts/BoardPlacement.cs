using UnityEngine;
using System.Collections;

public class BoardPlacement : MonoBehaviour {

    public bool MovingFlag;
    public GameObject Flag;
    private void OnMouseDown()
    {
        if (!MovingFlag)
        {
            if (PieceEditor.singleton.ShowingPromo)
            {
                PieceEditor.singleton.ShowPromo();
            }
            Vector2 v = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position - new Vector2(.5f, .5f);
            int x = Mathf.RoundToInt(v.x);
            int y = Mathf.RoundToInt(v.y);
            GetComponent<BoxCollider2D>().enabled = false;
            foreach(GamePieceReference g in GameControl.singleton.GamePiecesOnBoard)
                g.GetComponent<BoxCollider2D>().enabled = true;
            RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + new Vector2(x + .5f, y + .5f), Vector2.zero);
            if (hit.collider != null)
            {
                if (hit.collider.GetComponent<GamePieceReference>().index == -1)
                {
                    MovingFlag= true;
                    PieceEditor.singleton.Outline.transform.position=hit.collider.transform.position;
                    Flag = hit.collider.gameObject;
                }
                else
                {
                    DestroyImmediate(hit.collider.gameObject);
                    GameControl.singleton.CleanGamePieceList();
                }
            }
            else
                GameControl.singleton.PlacePiece(PieceEditor.singleton.SelectedPiece, x, y);
            GetComponent<BoxCollider2D>().enabled = true;
            foreach (GamePieceReference g in GameControl.singleton.GamePiecesOnBoard)
                g.GetComponent<BoxCollider2D>().enabled = false;
        }
        else
        {
            Vector2 v = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position - new Vector2(.5f, .5f);
            int x = Mathf.RoundToInt(v.x);
            int y = Mathf.RoundToInt(v.y);
            GetComponent<BoxCollider2D>().enabled = false;
            foreach (GamePieceReference g in GameControl.singleton.GamePiecesOnBoard)
                g.GetComponent<BoxCollider2D>().enabled = true;
            RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + new Vector2(x + .5f, y + .5f), Vector2.zero);
            if (hit.collider != null)
            {
                if (hit.collider.GetComponent<GamePieceReference>().index == -1)
                {
                    MovingFlag = false;
                    PieceEditor.singleton.Outline.transform.position = PieceEditor.singleton.SelectedPiece.transform.position;
                    Flag = null;
                }
                else
                {
                    DestroyImmediate(hit.collider.gameObject);
                    GameControl.singleton.CleanGamePieceList();
                }
            }
            else
            {
                Flag.transform.position = (Vector2)transform.position + new Vector2(x + .5f, y + .5f);
                Flag.GetComponent<GamePieceReference>().BoardPos[0] = x;
                Flag.GetComponent<GamePieceReference>().BoardPos[1] = y;
                MovingFlag = false;
                PieceEditor.singleton.Outline.transform.position = PieceEditor.singleton.SelectedPiece.transform.position;
                Flag = null;

            }
            GetComponent<BoxCollider2D>().enabled = true;
            foreach (GamePieceReference g in GameControl.singleton.GamePiecesOnBoard)
                g.GetComponent<BoxCollider2D>().enabled = false;
        }


    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
