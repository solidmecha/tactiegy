using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PreSetups : MonoBehaviour {

	public GameObject[] Flags;
    int Row;
    bool PiecesSet;
    bool FixedPower;

    // Use this for initialization
    void Start () {
        GameControl.singleton.SeedVal.text = GameControl.singleton.RNG.Next(0, Int32.MaxValue).ToString(); ;
    }

    public void Setup()
    {
        PiecesSet = false;
        foreach(PieceScript p in GameControl.singleton.Pieces)
        {
            for(int i=0;i<8;i++)
            {
                p.UpdateMoveIndex(false, false, i);
            }
        }
        FixedPower = 5 < GameControl.singleton.RNG.Next(10);
        SetBoardSize();
        SetFlags();
        SetPawnLike();
        SetPieces();
    }

	public void SetBoardSize()
	{
        GameControl.singleton.BoardSizeSlider.value=GameControl.singleton.RNG.Next(7, 11);
	}

	public void SetFlags()
	{
		int offset=GameControl.singleton.RNG.Next(0,(int)GameControl.singleton.BoardSizeSlider.value);
		Flags[0].transform.position = (Vector2)transform.position + new Vector2(offset+.5f, .5f);
        Flags[0].GetComponent<GamePieceReference>().BoardPos[0] = offset;
        Flags[0].GetComponent<GamePieceReference>().BoardPos[1] = 0;

        if (GameControl.singleton.RNG.Next(2)==0)
		{
			Flags[1].transform.position = (Vector2)Flags[0].transform.position + Vector2.up * ((int)GameControl.singleton.BoardSizeSlider.value - 1);
        }
		else
		{
            Flags[1].transform.position = (Vector2)transform.position+ new Vector2((int)GameControl.singleton.BoardSizeSlider.value, (int)GameControl.singleton.BoardSizeSlider.value) - new Vector2(offset + .5f, .5f);
        }
        Flags[1].GetComponent<GamePieceReference>().BoardPos[0] = (int)(Flags[1].transform.position.x-transform.position.x - .5f);
        Flags[1].GetComponent<GamePieceReference>().BoardPos[1] = (int)(Flags[1].transform.position.y-transform.position.y - .5f);
    }

	public void SetPawnLike()
	{
        Row = 1;
        if((int)GameControl.singleton.BoardSizeSlider.value>7 && GameControl.singleton.RNG.Next(3)==0)
            Row= 2;
		GameControl.singleton.Pieces[0].name = "Pawn";
		int F = GameControl.singleton.RNG.Next(3);
		if (F == 0)
		{
			GameControl.singleton.Pieces[0].UpdateMoveIndex(true, false, 0);
            GameControl.singleton.Pieces[0].UpdateMoveIndex(false, true, 1);
            GameControl.singleton.Pieces[0].UpdateMoveIndex(false, true, 7);
        }
		else if(F == 1)
		{
			GameControl.singleton.Pieces[0].UpdateMoveIndex(false, true, 0);
            GameControl.singleton.Pieces[0].UpdateMoveIndex(true, false, 1);
            GameControl.singleton.Pieces[0].UpdateMoveIndex(true, false, 7);
        }
		else
		{
            GameControl.singleton.transform.GetChild(0).GetChild(0).GetComponent<PieceScript>().UpdateMoveIndex(true, true, 0);
        }
		F = GameControl.singleton.RNG.Next(5);
		if(F == 0)
		{
            if (!FixedPower)
            {
                GameControl.singleton.Pieces[0].Atk = GameControl.singleton.RNG.Next(1, 3);
                GameControl.singleton.Pieces[0].Def = GameControl.singleton.RNG.Next(1, 3);
                GameControl.singleton.Pieces[0].isRanged = GameControl.singleton.RNG.Next(3) == 0;
            }
        }
        GameControl.singleton.Pieces[0].UpdateAll();
        QueenVariants(16, 24);
        GameControl.singleton.Pieces[16].UpdateName("Pawn+");
        GameControl.singleton.Pieces[24].UpdateName("Pawn+");
        GameControl.singleton.Pieces[0].hasPromo = true;
        for (int i=0;i<(int)GameControl.singleton.BoardSizeSlider.value;i++) {
            GameControl.singleton.PlacePiece(GameControl.singleton.Pieces[0],i, Row);
        }
		SetAnti(0, 8);
        GameControl.singleton.Pieces[8].UpdateAll();
        for (int i = 0; i < (int)GameControl.singleton.BoardSizeSlider.value; i++)
        {
            GameControl.singleton.PlacePiece(GameControl.singleton.Pieces[8], i, (int)GameControl.singleton.BoardSizeSlider.value-(Row+1));
        }
        PiecesSet = true;

    }

	public void SetAnti(int P1, int P2)
	{
        GameControl.singleton.Pieces[P2].name= GameControl.singleton.Pieces[P1].name;
		GameControl.singleton.Pieces[P2].isRanged= GameControl.singleton.Pieces[P1].isRanged;
		GameControl.singleton.Pieces[P2].AP=GameControl.singleton.Pieces[P1].AP;
		GameControl.singleton.Pieces[P2].Atk=GameControl.singleton.Pieces[P1].Atk;
		GameControl.singleton.Pieces[P2].Def=GameControl.singleton.Pieces[P1].Def;
        GameControl.singleton.Pieces[P2].hasPromo = GameControl.singleton.Pieces[P1].hasPromo;
		List<int> IndexOrder= new List<int> {4,3,2,1,0,7,6,5};
		for(int i=0;i<8;i++)
		{
			GameControl.singleton.Pieces[P2].hasAttacks[IndexOrder[i]] = GameControl.singleton.Pieces[P1].hasAttacks[i];
			GameControl.singleton.Pieces[P2].hasMoves[IndexOrder[i]]= GameControl.singleton.Pieces[P1].hasMoves[i];
		}
	}

	public void SetPieces()
    {
        int Flag1Pos = (int)(Flags[0].transform.position.x-transform.position.x-.5f);
        int Flag2Pos = (int)(Flags[1].transform.position.x - transform.position.x - .5f);
        List<int> OpenSquares=new List<int> { };
        List<int> AntiOpenSquares=new List<int>();
        for(int i=0;i<(int)GameControl.singleton.BoardSizeSlider.value;i++)
        {
            if (i != Flag1Pos)
            {
                OpenSquares.Add(i);
            }
            if (Flag1Pos == Flag2Pos && i != Flag2Pos)
                AntiOpenSquares.Add(i);
            else if(Flag1Pos != Flag2Pos && (int)GameControl.singleton.BoardSizeSlider.value-1-i != Flag2Pos)
                AntiOpenSquares.Add((int)GameControl.singleton.BoardSizeSlider.value - 1 - i);

        }
        int PieceIndex=1;
        int AntiIndex=9;
        List<int> UnitIDs=new List<int> { 0,1,2,3,4,5,6,7};
        if (OpenSquares.Count % 2 != 0)
        {
            int r=GameControl.singleton.RNG.Next(OpenSquares.Count);
            int U=GameControl.singleton.RNG.Next(1, UnitIDs.Count);
            SetByID(UnitIDs[U], PieceIndex, AntiIndex);
            GameControl.singleton.PlacePiece(GameControl.singleton.Pieces[PieceIndex], OpenSquares[r], 0);
            GameControl.singleton.PlacePiece(GameControl.singleton.Pieces[AntiIndex], AntiOpenSquares[r], (int)GameControl.singleton.BoardSizeSlider.value - 1);
            UnitIDs.RemoveAt(U);
            OpenSquares.RemoveAt(r);
            AntiOpenSquares.RemoveAt(r);
            PieceIndex++;
            AntiIndex++;
        }
        while (OpenSquares.Count>0)
        {
            int U = GameControl.singleton.RNG.Next(UnitIDs.Count);
            SetByID(UnitIDs[U], PieceIndex, AntiIndex);
            GameControl.singleton.PlacePiece(GameControl.singleton.Pieces[PieceIndex], OpenSquares[0], 0);
            GameControl.singleton.PlacePiece(GameControl.singleton.Pieces[AntiIndex], AntiOpenSquares[0], (int)GameControl.singleton.BoardSizeSlider.value - 1);
            GameControl.singleton.PlacePiece(GameControl.singleton.Pieces[PieceIndex], OpenSquares[OpenSquares.Count-1], 0);
            GameControl.singleton.PlacePiece(GameControl.singleton.Pieces[AntiIndex], AntiOpenSquares[AntiOpenSquares.Count-1], (int)GameControl.singleton.BoardSizeSlider.value - 1);
            UnitIDs.RemoveAt(U);
            OpenSquares.RemoveAt(OpenSquares.Count-1);
            OpenSquares.RemoveAt(0);
            AntiOpenSquares.RemoveAt(AntiOpenSquares.Count - 1);
            AntiOpenSquares.RemoveAt(0);
            PieceIndex++;
            AntiIndex++;
        }
        int OverflowIndex = PieceIndex;
        while(PieceIndex<8)
        {
            int U = GameControl.singleton.RNG.Next(UnitIDs.Count);
            SetByID(UnitIDs[U], PieceIndex, AntiIndex);
            UnitIDs.RemoveAt(U);
            PieceIndex++;
            AntiIndex++;
        }
        if(Row==2)
        {
            for (int i = 0; i < (int)GameControl.singleton.BoardSizeSlider.value; i++)
            {
                OpenSquares.Add(i);
                if (Flag1Pos == Flag2Pos)
                    AntiOpenSquares.Add(i);
                else
                    AntiOpenSquares.Add((int)GameControl.singleton.BoardSizeSlider.value - 1 - i);
            }
            for (int i=OverflowIndex; i<8;i++)
            {
                int r = GameControl.singleton.RNG.Next((OpenSquares.Count/2));
               
                int c = GameControl.singleton.RNG.Next(3);
                
                if(c==0)
                {
                    GameControl.singleton.PlacePiece(GameControl.singleton.Pieces[i], OpenSquares[OpenSquares.Count - r - 1], 1);
                    GameControl.singleton.PlacePiece(GameControl.singleton.Pieces[i+8], AntiOpenSquares[AntiOpenSquares.Count - r - 1], (int)GameControl.singleton.BoardSizeSlider.value - 2);
                    OpenSquares.RemoveAt(OpenSquares.Count - r - 1);
                    AntiOpenSquares.RemoveAt(AntiOpenSquares.Count - r - 1);
                }
                else if(c==1)
                {
                    GameControl.singleton.PlacePiece(GameControl.singleton.Pieces[i], OpenSquares[r], 1);
                    GameControl.singleton.PlacePiece(GameControl.singleton.Pieces[i + 8], AntiOpenSquares[r], (int)GameControl.singleton.BoardSizeSlider.value - 2);
                    OpenSquares.RemoveAt(r);
                    AntiOpenSquares.RemoveAt(r);
                }
                else if(OpenSquares.Count>1)
                {
                    GameControl.singleton.PlacePiece(GameControl.singleton.Pieces[i], OpenSquares[r], 1);
                    GameControl.singleton.PlacePiece(GameControl.singleton.Pieces[i + 8], AntiOpenSquares[r], (int)GameControl.singleton.BoardSizeSlider.value - 2);
                    GameControl.singleton.PlacePiece(GameControl.singleton.Pieces[i], OpenSquares[OpenSquares.Count - r -1], 1);
                    GameControl.singleton.PlacePiece(GameControl.singleton.Pieces[i + 8], AntiOpenSquares[AntiOpenSquares.Count - r - 1], (int)GameControl.singleton.BoardSizeSlider.value - 2);
                    OpenSquares.RemoveAt(OpenSquares.Count - r - 1);
                    AntiOpenSquares.RemoveAt(AntiOpenSquares.Count - r - 1);
                    OpenSquares.RemoveAt(r);
                    AntiOpenSquares.RemoveAt(r);
                }
            }
        }
    }

    public void PlacePieces()
    {
        if (PiecesSet)
        {
            foreach (GamePieceReference g in GameControl.singleton.GamePiecesOnBoard)
            {
                if (g.index > -1)
                    DestroyImmediate(g.gameObject);
            }
            GameControl.singleton.CleanGamePieceList();
            SetFlags();
            Row = 1;
            if ((int)GameControl.singleton.BoardSizeSlider.value > 7 && GameControl.singleton.RNG.Next(3) == 0)
                Row = 2;
            for (int i = 0; i < (int)GameControl.singleton.BoardSizeSlider.value; i++)
            {
                GameControl.singleton.PlacePiece(GameControl.singleton.Pieces[0], i, Row);
            }
            for (int i = 0; i < (int)GameControl.singleton.BoardSizeSlider.value; i++)
            {
                GameControl.singleton.PlacePiece(GameControl.singleton.Pieces[8], i, (int)GameControl.singleton.BoardSizeSlider.value - (Row + 1));
            }
            int Flag1Pos = (int)(Flags[0].transform.position.x - transform.position.x - .5f);
            int Flag2Pos = (int)(Flags[1].transform.position.x - transform.position.x - .5f);
            List<int> OpenSquares = new List<int> { };
            List<int> AntiOpenSquares = new List<int>();
            for (int i = 0; i < (int)GameControl.singleton.BoardSizeSlider.value; i++)
            {
                if (i != Flag1Pos)
                {
                    OpenSquares.Add(i);
                }
                if (Flag1Pos == Flag2Pos && i != Flag2Pos)
                    AntiOpenSquares.Add(i);
                else if (Flag1Pos != Flag2Pos && (int)GameControl.singleton.BoardSizeSlider.value - 1 - i != Flag2Pos)
                    AntiOpenSquares.Add((int)GameControl.singleton.BoardSizeSlider.value - 1 - i);

            }
            int PieceIndex = 1;
            int AntiIndex = 9;
            if (OpenSquares.Count % 2 != 0)
            {
                int r = GameControl.singleton.RNG.Next(OpenSquares.Count);
                GameControl.singleton.PlacePiece(GameControl.singleton.Pieces[PieceIndex], OpenSquares[r], 0);
                GameControl.singleton.PlacePiece(GameControl.singleton.Pieces[AntiIndex], AntiOpenSquares[r], (int)GameControl.singleton.BoardSizeSlider.value - 1);
                OpenSquares.RemoveAt(r);
                AntiOpenSquares.RemoveAt(r);
                PieceIndex++;
                AntiIndex++;
            }
            while (OpenSquares.Count > 0)
            {
                GameControl.singleton.PlacePiece(GameControl.singleton.Pieces[PieceIndex], OpenSquares[0], 0);
                GameControl.singleton.PlacePiece(GameControl.singleton.Pieces[AntiIndex], AntiOpenSquares[0], (int)GameControl.singleton.BoardSizeSlider.value - 1);
                GameControl.singleton.PlacePiece(GameControl.singleton.Pieces[PieceIndex], OpenSquares[OpenSquares.Count - 1], 0);
                GameControl.singleton.PlacePiece(GameControl.singleton.Pieces[AntiIndex], AntiOpenSquares[AntiOpenSquares.Count - 1], (int)GameControl.singleton.BoardSizeSlider.value - 1);
                OpenSquares.RemoveAt(OpenSquares.Count - 1);
                OpenSquares.RemoveAt(0);
                AntiOpenSquares.RemoveAt(AntiOpenSquares.Count - 1);
                AntiOpenSquares.RemoveAt(0);
                PieceIndex++;
                AntiIndex++;
            }
            int OverflowIndex = PieceIndex;
            while (PieceIndex < 8)
            {
                PieceIndex++;
                AntiIndex++;
            }
            if (Row == 2)
            {
                for (int i = 0; i < (int)GameControl.singleton.BoardSizeSlider.value; i++)
                {
                    OpenSquares.Add(i);
                    if (Flag1Pos == Flag2Pos)
                        AntiOpenSquares.Add(i);
                    else
                        AntiOpenSquares.Add((int)GameControl.singleton.BoardSizeSlider.value - 1 - i);
                }
                for (int i = OverflowIndex; i < 8; i++)
                {
                    int r = GameControl.singleton.RNG.Next((OpenSquares.Count / 2));

                    int c = GameControl.singleton.RNG.Next(3);

                    if (c == 0)
                    {
                        GameControl.singleton.PlacePiece(GameControl.singleton.Pieces[i], OpenSquares[OpenSquares.Count - r - 1], 1);
                        GameControl.singleton.PlacePiece(GameControl.singleton.Pieces[i + 8], AntiOpenSquares[AntiOpenSquares.Count - r - 1], (int)GameControl.singleton.BoardSizeSlider.value - 2);
                        OpenSquares.RemoveAt(OpenSquares.Count - r - 1);
                        AntiOpenSquares.RemoveAt(AntiOpenSquares.Count - r - 1);
                    }
                    else if (c == 1)
                    {
                        GameControl.singleton.PlacePiece(GameControl.singleton.Pieces[i], OpenSquares[r], 1);
                        GameControl.singleton.PlacePiece(GameControl.singleton.Pieces[i + 8], AntiOpenSquares[r], (int)GameControl.singleton.BoardSizeSlider.value - 2);
                        OpenSquares.RemoveAt(r);
                        AntiOpenSquares.RemoveAt(r);
                    }
                    else if (OpenSquares.Count > 1)
                    {
                        GameControl.singleton.PlacePiece(GameControl.singleton.Pieces[i], OpenSquares[r], 1);
                        GameControl.singleton.PlacePiece(GameControl.singleton.Pieces[i + 8], AntiOpenSquares[r], (int)GameControl.singleton.BoardSizeSlider.value - 2);
                        GameControl.singleton.PlacePiece(GameControl.singleton.Pieces[i], OpenSquares[OpenSquares.Count - r - 1], 1);
                        GameControl.singleton.PlacePiece(GameControl.singleton.Pieces[i + 8], AntiOpenSquares[AntiOpenSquares.Count - r - 1], (int)GameControl.singleton.BoardSizeSlider.value - 2);
                        OpenSquares.RemoveAt(OpenSquares.Count - r - 1);
                        AntiOpenSquares.RemoveAt(AntiOpenSquares.Count - r - 1);
                        OpenSquares.RemoveAt(r);
                        AntiOpenSquares.RemoveAt(r);
                    }
                }
            }
        }
    }


    public void SetByID(int id, int PieceIndex, int AntiIndex)
    {
        switch(id)
        {
            case 0:
                BishopVariant(PieceIndex, AntiIndex);
                break;
            case 1:
                KnightVariant(PieceIndex, AntiIndex);
                break;
            case 2:
                ArcherVariant(PieceIndex, AntiIndex);
                break;
            case 3:
                SilverVariant(PieceIndex, AntiIndex);
                break;
            case 4:
                GoldVariant(PieceIndex, AntiIndex);
                break;
            case 5:
                LancerVariant(PieceIndex, AntiIndex);
                break;
            case 6:
                RookVariant(PieceIndex, AntiIndex);
                break;
            case 7:
                QueenVariants(PieceIndex, AntiIndex);
                break;
        }
    }

    public void KnightVariant(int PieceIndex, int AntiIndex)
	{
		for (int i = 0; i < 8; i++)
		{
			GameControl.singleton.Pieces[PieceIndex].hasMoves[i] = true;
            GameControl.singleton.Pieces[PieceIndex].hasAttacks[i] = true;
        }
		GameControl.singleton.Pieces[PieceIndex].isRanged = GameControl.singleton.RNG.Next(4) == 0;
        GameControl.singleton.Pieces[PieceIndex].AP = GameControl.singleton.RNG.Next(1,4);
        GameControl.singleton.Pieces[PieceIndex].name = "Knight";
        if (!FixedPower)
        {
            GameControl.singleton.Pieces[PieceIndex].Atk = GameControl.singleton.RNG.Next(1, 5);
            GameControl.singleton.Pieces[PieceIndex].Def = GameControl.singleton.RNG.Next(1, 5);
        }
        SetAnti(PieceIndex, AntiIndex);
        GameControl.singleton.Pieces[PieceIndex].UpdateAll();
        GameControl.singleton.Pieces[AntiIndex].UpdateAll();
    }

	public void BishopVariant(int PieceIndex, int AntiIndex)
	{
        for (int i = 0; i < 8; i++)
        {
			if (i % 2 != 0)
			{
				GameControl.singleton.Pieces[PieceIndex].hasMoves[i] = true;
				GameControl.singleton.Pieces[PieceIndex].hasAttacks[i] = true;
			}
        }
        GameControl.singleton.Pieces[PieceIndex].isRanged = GameControl.singleton.RNG.Next(4) == 0;
        GameControl.singleton.Pieces[PieceIndex].AP = GameControl.singleton.RNG.Next(1, (int)GameControl.singleton.BoardSizeSlider.value);
        GameControl.singleton.Pieces[PieceIndex].name = "Bishop";
        if (!FixedPower)
        {
            GameControl.singleton.Pieces[PieceIndex].Atk = GameControl.singleton.RNG.Next(1, 5);
            GameControl.singleton.Pieces[PieceIndex].Def = GameControl.singleton.RNG.Next(1, 5);
        }
        SetAnti(PieceIndex, AntiIndex);
        GameControl.singleton.Pieces[PieceIndex].UpdateAll();
        GameControl.singleton.Pieces[AntiIndex].UpdateAll();
    }

	public void ArcherVariant(int PieceIndex, int AntiIndex)
	{
		bool b = GameControl.singleton.RNG.Next(2) == 0;
        for (int i = 0; i < 8; i++)
        {
            if (i % 2 != 0)
            {
                GameControl.singleton.Pieces[PieceIndex].hasMoves[i] = b;
                GameControl.singleton.Pieces[PieceIndex].hasAttacks[i] = !b;
            }
			else
			{
                GameControl.singleton.Pieces[PieceIndex].hasMoves[i] = !b;
                GameControl.singleton.Pieces[PieceIndex].hasAttacks[i] = b;
            }
        }
        GameControl.singleton.Pieces[PieceIndex].isRanged = true;
        GameControl.singleton.Pieces[PieceIndex].AP = GameControl.singleton.RNG.Next(1, 4);
        GameControl.singleton.Pieces[PieceIndex].name = "Archer";
        if (!FixedPower)
        {
            GameControl.singleton.Pieces[PieceIndex].Atk = GameControl.singleton.RNG.Next(1, 5);
            GameControl.singleton.Pieces[PieceIndex].Def = GameControl.singleton.RNG.Next(1, 5);
        }
        SetAnti(PieceIndex, AntiIndex);
        GameControl.singleton.Pieces[PieceIndex].UpdateAll();
        GameControl.singleton.Pieces[AntiIndex].UpdateAll();
    }

	public void SilverVariant(int PieceIndex, int AntiIndex)
	{

        GameControl.singleton.Pieces[PieceIndex].hasMoves[7] = true;
        GameControl.singleton.Pieces[PieceIndex].hasAttacks[7] = true;
        GameControl.singleton.Pieces[PieceIndex].hasMoves[0] = true;
        GameControl.singleton.Pieces[PieceIndex].hasAttacks[0] = true;
        GameControl.singleton.Pieces[PieceIndex].hasMoves[1] = true;
        GameControl.singleton.Pieces[PieceIndex].hasAttacks[1] = true;
        GameControl.singleton.Pieces[PieceIndex].hasMoves[3] = true;
        GameControl.singleton.Pieces[PieceIndex].hasAttacks[3] = true;
        GameControl.singleton.Pieces[PieceIndex].hasMoves[5] = true;
        GameControl.singleton.Pieces[PieceIndex].hasAttacks[5] = true;
        GameControl.singleton.Pieces[PieceIndex].isRanged = GameControl.singleton.RNG.Next(4) == 0;
        GameControl.singleton.Pieces[PieceIndex].AP = GameControl.singleton.RNG.Next(1, 3);
        GameControl.singleton.Pieces[PieceIndex].name = "Silver";
        if (!FixedPower)
        {
            GameControl.singleton.Pieces[PieceIndex].Atk = GameControl.singleton.RNG.Next(1, 5);
            GameControl.singleton.Pieces[PieceIndex].Def = GameControl.singleton.RNG.Next(1, 5);
        }
        SetAnti(PieceIndex, AntiIndex);
        GameControl.singleton.Pieces[PieceIndex].UpdateAll();
        GameControl.singleton.Pieces[AntiIndex].UpdateAll();
    }

    public void GoldVariant(int PieceIndex, int AntiIndex)
    {
        for (int i = 0; i < 8; i++)
        {
            GameControl.singleton.Pieces[PieceIndex].hasMoves[i] = true;
            GameControl.singleton.Pieces[PieceIndex].hasAttacks[i] = true;
        }
        GameControl.singleton.Pieces[PieceIndex].hasMoves[3] = false;
        GameControl.singleton.Pieces[PieceIndex].hasAttacks[3] = false;
        GameControl.singleton.Pieces[PieceIndex].hasMoves[5] = false;
        GameControl.singleton.Pieces[PieceIndex].hasAttacks[5] = false;
        GameControl.singleton.Pieces[PieceIndex].isRanged = GameControl.singleton.RNG.Next(4) == 0;
        GameControl.singleton.Pieces[PieceIndex].AP = GameControl.singleton.RNG.Next(1, 4);
        GameControl.singleton.Pieces[PieceIndex].name = "Gold";
        if (!FixedPower)
        {
            GameControl.singleton.Pieces[PieceIndex].Atk = GameControl.singleton.RNG.Next(1, 5);
            GameControl.singleton.Pieces[PieceIndex].Def = GameControl.singleton.RNG.Next(1, 5);
        }
        SetAnti(PieceIndex, AntiIndex);
        GameControl.singleton.Pieces[PieceIndex].UpdateAll();
        GameControl.singleton.Pieces[AntiIndex].UpdateAll();
    }

    public void LancerVariant(int PieceIndex, int AntiIndex)
    {
        GameControl.singleton.Pieces[PieceIndex].hasMoves[0] = true;
        GameControl.singleton.Pieces[PieceIndex].hasAttacks[0] = true;
        GameControl.singleton.Pieces[PieceIndex].hasMoves[2] = true;
        GameControl.singleton.Pieces[PieceIndex].hasMoves[6] = true;
        GameControl.singleton.Pieces[PieceIndex].AP = GameControl.singleton.RNG.Next(4, (int)GameControl.singleton.BoardSizeSlider.value);
        GameControl.singleton.Pieces[PieceIndex].name = "Lancer";
        if (!FixedPower)
        {
            GameControl.singleton.Pieces[PieceIndex].Atk = GameControl.singleton.RNG.Next(1, 5);
            GameControl.singleton.Pieces[PieceIndex].Def = GameControl.singleton.RNG.Next(1, 5);
        }
        RookVariant(PieceIndex+16, AntiIndex+16);
        GameControl.singleton.Pieces[PieceIndex+16].UpdateName("Lancer+");
        GameControl.singleton.Pieces[AntiIndex+16].UpdateName("Lancer+");
        GameControl.singleton.Pieces[PieceIndex + 16].hasPromo = true;
        SetAnti(PieceIndex, AntiIndex);
        GameControl.singleton.Pieces[PieceIndex].UpdateAll();
        GameControl.singleton.Pieces[AntiIndex].UpdateAll();
    }

    public void QueenVariants(int PieceIndex, int AntiIndex)
    {
        for (int i = 0; i < 8; i++)
        {
            GameControl.singleton.Pieces[PieceIndex].hasMoves[i] = true;
            GameControl.singleton.Pieces[PieceIndex].hasAttacks[i] = true;
        }
        GameControl.singleton.Pieces[PieceIndex].AP = (int)GameControl.singleton.BoardSizeSlider.value-1;
        GameControl.singleton.Pieces[PieceIndex].name = "Queen";
        if (!FixedPower)
        {
            GameControl.singleton.Pieces[PieceIndex].Atk = 1;//GameControl.singleton.RNG.Next(1, 5);
            GameControl.singleton.Pieces[PieceIndex].Def = 9;// GameControl.singleton.RNG.Next(1, 5);
        }
        SetAnti(PieceIndex, AntiIndex);
        GameControl.singleton.Pieces[PieceIndex].UpdateAll();
        GameControl.singleton.Pieces[AntiIndex].UpdateAll();
    }
    public void RookVariant(int PieceIndex, int AntiIndex)
    {
        GameControl.singleton.Pieces[PieceIndex].hasMoves[0] = true;
        GameControl.singleton.Pieces[PieceIndex].hasAttacks[0] = true;
        GameControl.singleton.Pieces[PieceIndex].hasMoves[2] = true;
        GameControl.singleton.Pieces[PieceIndex].hasAttacks[2] = true;
        GameControl.singleton.Pieces[PieceIndex].hasMoves[4] = true;
        GameControl.singleton.Pieces[PieceIndex].hasAttacks[4] = true;
        GameControl.singleton.Pieces[PieceIndex].hasMoves[6] = true;
        GameControl.singleton.Pieces[PieceIndex].hasAttacks[6] = true;
        GameControl.singleton.Pieces[PieceIndex].AP = GameControl.singleton.RNG.Next(4, (int)GameControl.singleton.BoardSizeSlider.value);
        GameControl.singleton.Pieces[PieceIndex].name = "Rook";
        GameControl.singleton.Pieces[PieceIndex].isRanged = GameControl.singleton.RNG.Next(5) == 0;
        if (!FixedPower)
        {
            GameControl.singleton.Pieces[PieceIndex].Atk = GameControl.singleton.RNG.Next(1, 5);
            GameControl.singleton.Pieces[PieceIndex].Def = GameControl.singleton.RNG.Next(1, 5);
        }
        SetAnti(PieceIndex, AntiIndex);
        GameControl.singleton.Pieces[PieceIndex].UpdateAll();
        GameControl.singleton.Pieces[AntiIndex].UpdateAll();
    }

    // Update is called once per frame
    void Update () {
	
	}
}
