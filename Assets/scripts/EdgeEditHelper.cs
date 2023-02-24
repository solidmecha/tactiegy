using UnityEngine;
using System.Collections;

public class EdgeEditHelper : MonoBehaviour {

    private void OnMouseDown()
    {
        bool m = PieceEditor.singleton.SelectedPiece.hasMoves[transform.GetSiblingIndex()];
        bool a = PieceEditor.singleton.SelectedPiece.hasAttacks[transform.GetSiblingIndex()];
        if (m && a)
        {
            a = !a;
        }
        else if(a)
        {
            a = !a;
        }
        else
        {
            m = !m;
            a = !a;
        }
        PieceEditor.singleton.SelectedPiece.UpdateMoveIndex(m, a, transform.GetSiblingIndex());
        PieceEditor.singleton.PieceEditPreview.UpdateMoveIndex(m, a, transform.GetSiblingIndex());
        GameControl.singleton.UpdateGamePieces(PieceEditor.singleton.SelectedPiece.Index);
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
