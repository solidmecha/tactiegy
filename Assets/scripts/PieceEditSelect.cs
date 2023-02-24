using UnityEngine;
using System.Collections;

public class PieceEditSelect : MonoBehaviour {

    private void OnMouseDown()
    {
		SelectPiece();
    }

	public void SelectPiece()
	{
		if (!PieceEditor.singleton.EditVisible)
			PieceEditor.singleton.ToggleEditor();
		PieceEditor.singleton.SelectedPiece=GetComponent<PieceScript>();
		PieceEditor.singleton.UpdatePreview();
		PieceEditor.singleton.Outline.transform.position = transform.position;
	}

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
