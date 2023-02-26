using UnityEngine;
using System.Collections;

public class MoveOutlineScript : MonoBehaviour {
private void OnMouseDown()
    {
        int[] pos = new int[2]
        {
            (int)(transform.position.x-.5f-GameControl.singleton.Board.transform.position.x),
            (int)(transform.position.y-.5f-GameControl.singleton.Board.transform.position.y)
        };
        GameControl.singleton.SelectedPiece.GetComponent<GamePieceReference>().ClearMoves();
        GameControl.singleton.MovePiece(GameControl.singleton.SelectedPiece.GetComponent<GamePieceReference>(), pos);
        GameControl.singleton.SelectedPiece = null;
        GameControl.singleton.IncActions();
    }
}
