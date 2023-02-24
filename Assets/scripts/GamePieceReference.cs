using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class GamePieceReference : MonoBehaviour {

    public int index;
    public Text AtkDefText;
    public Text RangedText;
    public Text NameText;
    public bool playerOne;
    public List<GameObject> MoveOutlines;
    public List<GameObject> AtkOutlines;
    public int[] BoardPos;
    public int Def;

    private void OnMouseDown()
    {
        if (GameControl.singleton.CurrentMode==GameControl.GameMode.Play && playerOne == GameControl.singleton.playerOne && index > -1)
        {
            if (GameControl.singleton.SelectedPiece != null)
                GameControl.singleton.SelectedPiece.GetComponent<GamePieceReference>().ClearMoves();
            GameControl.singleton.SelectedPiece = gameObject;
            ShowMoves();
        }
    }

    public void ShowMoves()
    {
        for (int j = 0; j < 8; j++)
        {
            if (GameControl.singleton.Pieces[index].hasMoves[j])
            {
                for (int i = 1; i < GameControl.singleton.Pieces[index].AP+1; i++)
                {
                   Vector2 v= (Vector2)transform.position + GameControl.singleton.dirRef[j]*i;
                    if(v.x>=GameControl.singleton.BoardBounds[0].x && v.y>=GameControl.singleton.BoardBounds[0].y
                        && v.x <= GameControl.singleton.BoardBounds[1].x && v.y <= GameControl.singleton.BoardBounds[1].y
                        )
                    {
                        if (!GameControl.singleton.BoardHasPiece[BoardPos[0] + (int)GameControl.singleton.dirRef[j].x * i]
                        [BoardPos[1] + (int)GameControl.singleton.dirRef[j].y * i])
                            MoveOutlines.Add(Instantiate(GameControl.singleton.Outline, v, Quaternion.identity) as GameObject);
                        else
                            break;
                    }   
                }
            }
            if (GameControl.singleton.Pieces[index].hasAttacks[j])
            {
                for (int i = 1; i < GameControl.singleton.Pieces[index].AP + 1; i++)
                {
                    Vector2 v = (Vector2)transform.position + GameControl.singleton.dirRef[j] * i;
                    if (v.x >= GameControl.singleton.BoardBounds[0].x && v.y >= GameControl.singleton.BoardBounds[0].y
                        && v.x <= GameControl.singleton.BoardBounds[1].x && v.y <= GameControl.singleton.BoardBounds[1].y
                        )
                    {
                        if (GameControl.singleton.BoardHasPiece[BoardPos[0] + (int)GameControl.singleton.dirRef[j].x * i]
                        [BoardPos[1] + (int)GameControl.singleton.dirRef[j].y * i])
                        {
                            RaycastHit2D hit = Physics2D.Raycast(v, Vector2.zero);
                            if (hit.collider == null)
                            {
                                print((BoardPos[0] + (int)GameControl.singleton.dirRef[j].x * i).ToString() + " , " +
                                    (BoardPos[1] + (int)GameControl.singleton.dirRef[j].y * i).ToString());
                                GameControl.singleton.BoardHasPiece[BoardPos[0] + (int)GameControl.singleton.dirRef[j].x * i]
                                [BoardPos[1] + (int)GameControl.singleton.dirRef[j].y * i] = false;
                                continue;
                            }
                            if (hit.collider.GetComponent<GamePieceReference>().playerOne != playerOne)
                            {
                                AtkOutlines.Add(Instantiate(GameControl.singleton.AtkOutline, v, Quaternion.identity) as GameObject);
                                AtkOutlines[AtkOutlines.Count - 1].GetComponent<AtkOutlineScript>().Target = hit.collider.GetComponent<GamePieceReference>();
                                hit.collider.enabled = false;
                                break;
                            }
                            else
                                break;
                        }
                    }
                }
            }
        }
    }

    public List<GamePieceReference> ThreatenedPieces()
    {
        if(index<0)
            return new List<GamePieceReference>();
        List<GamePieceReference> list = new List<GamePieceReference>();
        for(int j=0;j<8;  j++)
        {
            if (GameControl.singleton.Pieces[index].hasAttacks[j])
            {
                for (int i = 1; i < GameControl.singleton.Pieces[index].AP + 1; i++)
                {
                    Vector2 v = (Vector2)transform.position + GameControl.singleton.dirRef[j] * i;
                    if (v.x >= GameControl.singleton.BoardBounds[0].x && v.y >= GameControl.singleton.BoardBounds[0].y
                        && v.x <= GameControl.singleton.BoardBounds[1].x && v.y <= GameControl.singleton.BoardBounds[1].y
                        )
                    {
                        if (GameControl.singleton.BoardHasPiece[BoardPos[0] + (int)GameControl.singleton.dirRef[j].x * i]
                        [BoardPos[1] + (int)GameControl.singleton.dirRef[j].y * i])
                        {
                            RaycastHit2D hit = Physics2D.Raycast(v, Vector2.zero);
                            if(hit.collider == null)
                            {
                                print((BoardPos[0] + (int)GameControl.singleton.dirRef[j].x * i).ToString() + " , " +
                                    (BoardPos[1] + (int)GameControl.singleton.dirRef[j].y * i).ToString());
                                GameControl.singleton.BoardHasPiece[BoardPos[0] + (int)GameControl.singleton.dirRef[j].x * i]
                                [BoardPos[1] + (int)GameControl.singleton.dirRef[j].y * i] = false;
                                continue;
                            }
                            else if (hit.collider.GetComponent<GamePieceReference>().playerOne != playerOne)
                            {
                                list.Add(hit.collider.GetComponent<GamePieceReference>());
                                break;
                            }
                            else
                                break;
                        }
                    }
                }
            }
        }
        return list;
    }
    public List<GamePieceReference> ThreatenedPieces(int[] NewPosition)
    {
        int[] BP = new int[2] { BoardPos[0], BoardPos[1] };
        BoardPos[0] = NewPosition[0];
        BoardPos[1] = NewPosition[1];
        GameControl.singleton.BoardHasPiece[BP[0]][BP[1]] = false;
        List<GamePieceReference> list = new List<GamePieceReference>();
        if (index >= 0)
        { 
        for (int j = 0; j < 8; j++)
        {
            if (GameControl.singleton.Pieces[index].hasAttacks[j])
            {
                    for (int i = 1; i < GameControl.singleton.Pieces[index].AP + 1; i++)
                    {
                        int[] v = new int[2] { BoardPos[0] + (int)GameControl.singleton.dirRef[j].x * i,
                        BoardPos[1] + (int)GameControl.singleton.dirRef[j].y * i};
                        if (v[0] < (int)GameControl.singleton.BoardSizeSlider.value && v[1] < (int)GameControl.singleton.BoardSizeSlider.value
                        && v[0] > -1 && v[1] > -1
                        )
                    {
                        if (GameControl.singleton.BoardHasPiece[BoardPos[0] + (int)GameControl.singleton.dirRef[j].x * i]
                        [BoardPos[1] + (int)GameControl.singleton.dirRef[j].y * i])
                        {
                            RaycastHit2D hit = Physics2D.Raycast((Vector2)GameControl.singleton.Board.transform.position+new Vector2(v[0]+.5f, v[1]+.5f), Vector2.zero);
                            if (hit.collider.GetComponent<GamePieceReference>().playerOne != playerOne)
                            {
                                list.Add(hit.collider.GetComponent<GamePieceReference>());
                                break;
                            }
                            else
                                break;
                        }
                    }
                }
            }
        }
        }
        BoardPos[0] = BP[0];
        BoardPos[1]= BP[1];
        GameControl.singleton.BoardHasPiece[BP[0]][BP[1]] = true;
        return list;
    }

    public List<int[]> AttackedSquares()
    {
        if (index < 0)
            return new List<int[]>();
        List<int[]> list = new List<int[]>();
        for (int j = 0; j < 8; j++)
        {
            if (GameControl.singleton.Pieces[index].hasAttacks[j])
            {
                for (int i = 1; i < GameControl.singleton.Pieces[index].AP + 1; i++)
                {
                    Vector2 v = (Vector2)transform.position + GameControl.singleton.dirRef[j] * i;
                    if (v.x >= GameControl.singleton.BoardBounds[0].x && v.y >= GameControl.singleton.BoardBounds[0].y
                        && v.x <= GameControl.singleton.BoardBounds[1].x && v.y <= GameControl.singleton.BoardBounds[1].y
                        )
                    {
                        if (GameControl.singleton.BoardHasPiece[BoardPos[0] + (int)GameControl.singleton.dirRef[j].x * i]
                        [BoardPos[1] + (int)GameControl.singleton.dirRef[j].y * i])
                        {
                            list.Add(new int[2]{BoardPos[0] + Mathf.RoundToInt(GameControl.singleton.dirRef[j].x * i),
                        BoardPos[1] + Mathf.RoundToInt(GameControl.singleton.dirRef[j].y * i)});
                            break;
                        }
                        else
                        {
                            list.Add(new int[2]{BoardPos[0] + Mathf.RoundToInt(GameControl.singleton.dirRef[j].x * i),
                        BoardPos[1] + Mathf.RoundToInt(GameControl.singleton.dirRef[j].y * i)});
                        }
                    }
                }
            }
        }
        return list;
    }

    public List<int[]> PossibleMoves()
    {
        if (index < 0)
            return new List<int[]>();
        List<int[]> list = new List<int[]>();
        for (int j = 0; j < 8; j++)
        {
            if (GameControl.singleton.Pieces[index].hasMoves[j])
            {
                for (int i = 1; i < GameControl.singleton.Pieces[index].AP + 1; i++)
                {
                    Vector2 v = (Vector2)transform.position + GameControl.singleton.dirRef[j] * i;
                    if (v.x >= GameControl.singleton.BoardBounds[0].x && v.y >= GameControl.singleton.BoardBounds[0].y
                        && v.x <= GameControl.singleton.BoardBounds[1].x && v.y <= GameControl.singleton.BoardBounds[1].y
                        )
                    {
                        if (!GameControl.singleton.BoardHasPiece[BoardPos[0] + (int)GameControl.singleton.dirRef[j].x * i]
                        [BoardPos[1] + (int)GameControl.singleton.dirRef[j].y * i])
                            list.Add(new int[2]{BoardPos[0] + Mathf.RoundToInt(GameControl.singleton.dirRef[j].x * i),
                        BoardPos[1] + Mathf.RoundToInt(GameControl.singleton.dirRef[j].y * i)});
                        else
                            break;
                    }
                }
            }
        }
        return list;
    }

    public void ClearMoves()
    {
        foreach(GameObject g in MoveOutlines)
        {
            Destroy(g);
        }
        foreach(GameObject g in AtkOutlines)
        {
            if(g.GetComponent<AtkOutlineScript>().Target != null)
                g.GetComponent<AtkOutlineScript>().Target.GetComponent<Collider2D>().enabled = true;
            Destroy(g);
        }
        MoveOutlines.Clear();
        AtkOutlines.Clear();
    }

    public void ShowDamage()
    {
        AtkDefText.text = GameControl.singleton.Pieces[index].Atk.ToString() + "/" + Def.ToString();
    }

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
