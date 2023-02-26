using UnityEngine;
using System.Collections;

public class AtkOutlineScript : MonoBehaviour {
    public GamePieceReference Target;
    private void OnMouseDown()
    {
        GameControl.singleton.SelectedPiece.GetComponent<GamePieceReference>().ClearMoves();
        GameControl.singleton.Capture(Target);
        GameControl.singleton.IncActions();
    }
}
