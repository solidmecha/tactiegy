using UnityEngine;
using System.Collections;

public class CloseEditor : MonoBehaviour {
private void OnMouseDown()
    {
        if (PieceEditor.singleton.ShowingPromo)
            PieceEditor.singleton.ShowPromo();
        PieceEditor.singleton.ToggleEditor();
    }
}
