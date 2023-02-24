using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using System.Reflection;

public class GameControl : MonoBehaviour {
    public static GameControl singleton;
    public List<PieceScript> Pieces;
    public List<GamePieceReference> GamePiecesOnBoard;
    public System.Random RNG;
    public GameObject GamePiece;
    public GameObject Board;
    public bool[][] BoardHasPiece;
    public bool playerOne;
    public readonly Vector2[] dirRef = new Vector2[8] {new Vector2(0,1), new Vector2(1,1), new Vector2(1,0), new Vector2(1,-1), new Vector2(0,-1),
        new Vector2(-1,-1), new Vector2(-1,0), new Vector2(-1,1)};
    public Vector2[] BoardBounds;
    public GameObject Outline;
    public GameObject AtkOutline;
    public enum GameMode { Play, Edit, OppTurn, Over};
    public GameMode CurrentMode;
    public Slider ActionCountSlider;
    public Slider BoardSizeSlider;
    public Slider VsSlider;
    public Slider ClockSlider;
    public Toggle ForcedAttacks;
    public int[] ActionsCount;
    public bool[] isBot;
    public Sprite[] BoardSprites;
    public GameObject SelectedPiece;
    public GameObject GameInfoPanel;
    public InputField SeedVal;

    public void ChangeActionCount()
    {
        ActionsCount[1] = (int)ActionCountSlider.value;
        string s = " Actions Per Turn";
        if (ActionsCount[1] == 1)
            s = " Action Per Turn";
        transform.GetChild(3).GetChild(0).GetChild(2).GetComponent<Text>().text = ActionsCount[1].ToString()+s;
    }

    public string LetterLookup(int index)
    {
        string L = "ABCDEFGHIJ";
        return L[index].ToString();
    }

    public void ChagesVS()
    {
        switch((int)VsSlider.value)
        {
            case 0:
                transform.GetChild(3).GetChild(0).GetChild(0).GetComponent<Text>().text = "PLAYER vs COMP";
                isBot[0] = false;
                isBot[1] = true;
                break; 
            case 1:
                transform.GetChild(3).GetChild(0).GetChild(0).GetComponent<Text>().text = "COMP vs PLAYER";
                isBot[0] = true;
                isBot[1] = false;
                break;
            case 2:
                transform.GetChild(3).GetChild(0).GetChild(0).GetComponent<Text>().text = "PLAYER vs PLAYER";
                isBot[0] = false;
                isBot[1] = false;
                break;
            case 3:
                transform.GetChild(3).GetChild(0).GetChild(0).GetComponent<Text>().text = "COMP vs COMP";
                isBot[0] = true;
                isBot[1] = true;
                break;
        }
    }

    public void UpdateBoardSize()
    {
        Board.GetComponent<SpriteRenderer>().sprite = BoardSprites[(int)BoardSizeSlider.value - 7];
        for(int i= 0; i< 10; i++)
        {
            if (i < (int)BoardSizeSlider.value)
            {
                Board.transform.GetChild(0).GetChild(i).GetComponent<Text>().text = LetterLookup(i);
                Board.transform.GetChild(0).GetChild(10 + i).GetComponent<Text>().text = (i+1).ToString();
            }
            else
            {
                Board.transform.GetChild(0).GetChild(i).GetComponent<Text>().text = "";
                Board.transform.GetChild(0).GetChild(10+i).GetComponent<Text>().text = "";
            }
        }
    }

    public void UpdateGamePieces(int i)
    {
        foreach(GamePieceReference g in GamePiecesOnBoard)
        {
            if(g.index==i)
            {
                UpdateGamePiece(g);
            }

        }

    }

    public void CleanGamePieceList()
    {
        for(int i= GamePiecesOnBoard.Count-1; i>-1;i--)
        {
            if (GamePiecesOnBoard[i] == null)
                GamePiecesOnBoard.RemoveAt(i);
        }
    }

    public void UpdateGamePiece(GamePieceReference g)
    {
        int i = g.index;
        for (int c = 0; c < 8; c++)
        {
            g.transform.GetChild(c).GetComponent<SpriteRenderer>().enabled = Pieces[i].transform.GetChild(c).GetComponent<SpriteRenderer>().enabled;
            g.transform.GetChild(c).GetComponent<SpriteRenderer>().color = Pieces[i].transform.GetChild(c).GetComponent<SpriteRenderer>().color;
        }
        g.AtkDefText.text = Pieces[i].AtkDefText.text;
        g.RangedText.text = Pieces[i].RangedText.text;
        g.NameText.text = Pieces[i].NameText.text;
        g.Def= Pieces[i].Def;

    }

    public void PlacePiece(PieceScript P, int x, int y)
    {
        GamePieceReference g = (Instantiate(GamePiece, (Vector2)Board.transform.position + new Vector2(x, y) + new Vector2(.5f, .5f), Quaternion.identity) as GameObject).GetComponent<GamePieceReference>();
        GamePiecesOnBoard.Add(g);
        g.Def = P.Def;
        g.playerOne = P.playerOne;
        g.index=P.Index;
        g.GetComponent<SpriteRenderer>().sprite = P.GetComponent<SpriteRenderer>().sprite;
        g.AtkDefText = g.transform.GetChild(8).GetChild(0).GetComponent<Text>();
        g.RangedText = g.transform.GetChild(8).GetChild(1).GetComponent<Text>();
        g.NameText = g.transform.GetChild(8).GetChild(2).GetComponent<Text>();
        g.BoardPos[0] = x;
        g.BoardPos[1] = y;
        UpdateGamePiece(g);
        g.transform.SetParent(Board.transform);
    }

    public void StartGame()
    {
        CleanGamePieceList();
        GameInfoPanel.transform.position = new Vector2(6.7f, 2.61f);
        Board.GetComponent<Collider2D>().enabled = false;
        Board.transform.position=new Vector2(transform.GetChild(3).position.x, Board.transform.position.y);
        transform.position=new Vector3(-100,-100,-100);
        BoardHasPiece = new bool[(int)BoardSizeSlider.value][];
        BoardBounds[0]= Board.transform.position;
        BoardBounds[1] = new Vector2(BoardSizeSlider.value, BoardSizeSlider.value) + BoardBounds[0];
        for(int i=0;i< (int)BoardSizeSlider.value;i++)
        {
            BoardHasPiece[i] = new bool[(int)BoardSizeSlider.value];
        }
        foreach(GamePieceReference g in GamePiecesOnBoard)
        {
            BoardHasPiece[g.BoardPos[0]][g.BoardPos[1]] = true;
            g.GetComponent<Collider2D>().enabled = true;
        }
        foreach (PieceScript p in Pieces)
            p.PieceVal();
        for(int i=0;i<16;i++)
        {
            if(Pieces[i + 16].Value > 0)
                Pieces[i].hasPromo= true;
        }
        ActionsCount[0] = ActionsCount[1] - 1;
        IncActions();
    }

    public void SetSeed()
    {
        int seed;
        if(Int32.TryParse(SeedVal.text, out seed))
        {
            foreach (GamePieceReference g in GamePiecesOnBoard)
            {
                if(g.index>-1)
                    DestroyImmediate(g.gameObject);
            }
            CleanGamePieceList();
            RNG = new System.Random(seed);
            Board.GetComponent<PreSetups>().Setup();
        }
    }

    public void MovePiece(GamePieceReference P, int[] Pos)
    {
        GameInfoPanel.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = LetterLookup(P.BoardPos[0]) + (P.BoardPos[1]+1).ToString()+"-"
            + LetterLookup(Pos[0]) + (Pos[1]+1).ToString();
        BoardHasPiece[P.BoardPos[0]][P.BoardPos[1]]=false;
        BoardHasPiece[Pos[0]][Pos[1]]=true;
        P.transform.position = (Vector2)Board.transform.position + new Vector2(Pos[0]+.5f, Pos[1]+.5f);
        P.BoardPos[0] = Pos[0];
        P.BoardPos[1] = Pos[1];
        if (((Pos[1]==(int)BoardSizeSlider.value-1 && playerOne) || (Pos[1] == 0 && !playerOne)) && Pieces[P.index].hasPromo)
        {
            P.index += 16;
            UpdateGamePiece(P);
        }
        IncActions();
    }

    public void Capture(GamePieceReference T)
    {
        GameInfoPanel.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = LetterLookup(SelectedPiece.GetComponent<GamePieceReference>().BoardPos[0]) + (SelectedPiece.GetComponent<GamePieceReference>().BoardPos[1] + 1).ToString() + "x"
           + LetterLookup(T.BoardPos[0]) + (T.BoardPos[1] + 1).ToString();
        T.Def-=Pieces[SelectedPiece.GetComponent<GamePieceReference>().index].Atk;
        if(T.Def<=0)
        {
            if(T.index==-1)
            {
                CurrentMode = GameMode.Over;
                if(playerOne)
                { GetComponent<ClockScript>().TimeText[0].text = "Player 1 Wins"; }
                else
                    GetComponent<ClockScript>().TimeText[1].text = "Player 2 Wins";
                isBot[0] = false;
                isBot[1] = false;
                DestroyImmediate(T.gameObject);
                CleanGamePieceList();
                ActionsCount[0]--;
                MovePiece(SelectedPiece.GetComponent<GamePieceReference>(), T.BoardPos);
            }
            else if(Pieces[SelectedPiece.GetComponent<GamePieceReference>().index].isRanged)
            {
                BoardHasPiece[T.BoardPos[0]][T.BoardPos[1]]=false;
                DestroyImmediate(T.gameObject);
                CleanGamePieceList();
                IncActions();
            }
            else
            {
                DestroyImmediate(T.gameObject);
                CleanGamePieceList();
                MovePiece(SelectedPiece.GetComponent<GamePieceReference>(), T.BoardPos);
            }
        }
        else
        {
            T.ShowDamage();
            if (!Pieces[SelectedPiece.GetComponent<GamePieceReference>().index].isRanged
                && (Mathf.Abs(T.BoardPos[0]-SelectedPiece.GetComponent<GamePieceReference>().BoardPos[0])>1 ||
                Mathf.Abs(T.BoardPos[1] - SelectedPiece.GetComponent<GamePieceReference>().BoardPos[1]) > 1))
            {
                int x = T.BoardPos[0] - SelectedPiece.GetComponent<GamePieceReference>().BoardPos[0];
                int y = T.BoardPos[1] - SelectedPiece.GetComponent<GamePieceReference>().BoardPos[1];
                if (x < 0)
                    x = -1;
                if (x > 0)
                    x = 1;
                if (y < 0)
                    y = -1;
                if(y > 0)
                    y = 1;
                MovePiece(SelectedPiece.GetComponent<GamePieceReference>(), new int[2] { T.BoardPos[0] - x, T.BoardPos[1] - y });
            }
            else
                IncActions();
        }
    }

    public void PassTurn()
    {
        ActionsCount[0] = ActionsCount[1]-1;
        IncActions();
        GameInfoPanel.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "Turn Passed";
    }
    public void IncActions()
    {
        if (GameControl.singleton.CurrentMode != GameMode.Over)
        {
            ActionsCount[0]++;
            if (ActionsCount[0] == ActionsCount[1])
            {
                playerOne = !playerOne;
                ActionsCount[0] = 0;
            }
            int botIndex = 0;
            if (!playerOne)
                botIndex = 1;
            if (isBot[botIndex])
            {
                CurrentMode = GameMode.OppTurn;
                BotScript.singleton.Move();
            }
            else
                CurrentMode = GameMode.Play;
        }
    }

    private void Awake()
    {
        singleton = this;
        RNG = new System.Random();
    }

    // Use this for initialization
    void Start () {
        for(int i=0;i<Pieces.Count;i++)
        {
            Pieces[i].Index= i;
        }
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.R))
            Application.LoadLevel(0);
	}
}
