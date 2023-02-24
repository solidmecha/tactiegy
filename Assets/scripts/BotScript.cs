using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BotScript : MonoBehaviour {


	public static BotScript singleton;
	public int HighAtkIndex;
	public int HighAtkValue;
    enum ActionType { Move, Attack};

	List<int[]> MovePostions=new List<int[]> { };
	List<GamePieceReference> gamePieces= new List<GamePieceReference>();
	List<ActionType> actionTypes= new List<ActionType>();
	List<GamePieceReference>[][] EnemyPowerGrid;
    List<GamePieceReference>[][] PowerGrid;

    public void Move()
	{
        GameControl.singleton.CleanGamePieceList();
        MovePostions.Clear();
        gamePieces.Clear();
        actionTypes.Clear();
        SetGrid();
		HighAtkIndex = -1;
		HighAtkValue= -1;
		if(!AttackedPieces())
		{
			if(!ThreatenedPieces())
			{
				if(HighAtkIndex>-1)
				{
					TakeAction(HighAtkIndex);
				}
				else
				{
					bool hasMoved=false;
                    if (MovePostions.Count > 0)
                    {
						
						for (int i = 0; i < MovePostions.Count; i++)
						{
							if (actionTypes[i] == ActionType.Attack)
							{
                                GamePieceReference GPR = Physics2D.Raycast((Vector2)transform.position+new Vector2(MovePostions[i][0] + .5f, MovePostions[i][1] + .5f), Vector2.zero).collider.GetComponent<GamePieceReference>();
                                if (EnemyPowerGrid[GPR.BoardPos[0]][GPR.BoardPos[1]].Count == 0 || GameControl.singleton.Pieces[gamePieces[i].index].Value <=
									GameControl.singleton.Pieces[GPR.index].Value || GameControl.singleton.Pieces[gamePieces[i].index].isRanged)
								{
									hasMoved = true;
									TakeAction(i);
									break;
								}
							}

						}
                    }
                    HighAtkIndex = -1;
                    HighAtkValue = -1;
					foreach (GamePieceReference g in GameControl.singleton.GamePiecesOnBoard)
					{
						if (g.playerOne == GameControl.singleton.playerOne)
						{
							foreach (int[] ia in g.PossibleMoves())
							{
								int tv = FindThreatValue(ia, g);
								if (HighAtkValue < tv)
								{
									MovePostions.Add(ia);
									gamePieces.Add(g);
									actionTypes.Add(ActionType.Move);
									HighAtkValue = tv;
									HighAtkIndex = MovePostions.Count - 1;
								}
							}
						}
					}
                    if (HighAtkValue > -1)
                    {
                        TakeAction(HighAtkIndex);
                        hasMoved = true;
                        return;
                    }

                    if (!hasMoved)
					{
						List<GamePieceReference> GPRs = new List<GamePieceReference> { };
						foreach (GamePieceReference g in GameControl.singleton.GamePiecesOnBoard)
						{
							if (g.playerOne == GameControl.singleton.playerOne)
								GPRs.Add(g);
						}
						while (GPRs.Count > 0 && !hasMoved)
						{
							int r = GameControl.singleton.RNG.Next(GPRs.Count);
							hasMoved = Retreat(GPRs[r]);
							GPRs.RemoveAt(r);
						}
						if(!hasMoved)
							GameControl.singleton.PassTurn();
                    }
					
				}
			}
		}
	}

	public int FindThreatValue(int[] newPos, GamePieceReference Attacker)
	{
		int threatVal=-1;
		foreach(GamePieceReference G in Attacker.ThreatenedPieces(newPos))
		{
			if(G.index<0)
			{ threatVal = threatVal=Int32.MaxValue;}
			else
			{ threatVal = GameControl.singleton.Pieces[G.index].Value;}
		}
		return threatVal;
	}

	public void SetGrid()
	{

		int size = (int)GameControl.singleton.BoardSizeSlider.value;
		EnemyPowerGrid = new List<GamePieceReference>[size][];
        PowerGrid = new List<GamePieceReference>[size][];
        for (int i = 0; i < size; i++)
		{
			EnemyPowerGrid[i] = new List<GamePieceReference>[size];
			PowerGrid[i] = new List<GamePieceReference>[size];
            for (int j = 0; j < size; j++)
			{
				EnemyPowerGrid[i][j] = new List<GamePieceReference> ();
                PowerGrid[i][j] = new List<GamePieceReference> ();

            }
		}
		foreach (GamePieceReference g in GameControl.singleton.GamePiecesOnBoard)
		{
			if (g.playerOne != GameControl.singleton.playerOne)
			{
				foreach (int[] ia in g.AttackedSquares())
				{
					EnemyPowerGrid[ia[0]][ia[1]].Add(g);
				}
			}
			else
			{
                foreach (int[] ia in g.AttackedSquares())
                {
                    PowerGrid[ia[0]][ia[1]].Add(g);
                }
            }
		}
	}

	public bool AttackedPieces()
	{
		foreach(GamePieceReference g in GameControl.singleton.GamePiecesOnBoard)
		{
			if(g.playerOne==GameControl.singleton.playerOne)
			{
				List<GamePieceReference> L = g.ThreatenedPieces();
                foreach(GamePieceReference GPR in L)
				{
					if (GPR.index < 0)
					{
						MovePostions.Add(GPR.BoardPos);
						gamePieces.Add(g);
						actionTypes.Add(ActionType.Attack);
						TakeAction(MovePostions.Count-1);
						return true;
					}
                    else
                    {
                        MovePostions.Add(GPR.BoardPos);
						gamePieces.Add(g);
						actionTypes.Add(ActionType.Attack);
						if (GameControl.singleton.Pieces[g.index].Atk>=GPR.Def && HighAtkValue < GameControl.singleton.Pieces[GPR.index].Value
							&& (EnemyPowerGrid[GPR.BoardPos[0]][GPR.BoardPos[1]].Count == 0 || GameControl.singleton.Pieces[g.index].Value <=
                                GameControl.singleton.Pieces[GPR.index].Value || GameControl.singleton.Pieces[g.index].isRanged))

                        {

								HighAtkValue = GameControl.singleton.Pieces[GPR.index].Value;
								HighAtkIndex = MovePostions.Count - 1;
						}
					}
				}
			}
		}
		return false;
	}

	public bool EnemyValsAttackingPieceValCheck(int[] pos, int val)
	{
		foreach(GamePieceReference g in EnemyPowerGrid[pos[0]][pos[1]])
		{
			if (GameControl.singleton.Pieces[g.index].Value > val)
				return false;
		}
		return true;
	}

	public bool ThreatenedPieces()
	{
		List<int> TargetVals=new List<int> { };
		List<int[]> TargetPos=new List<int[]> { };
		List<int[]> ThreatenedPiecePos=new List<int[]> { };
        foreach (GamePieceReference g in GameControl.singleton.GamePiecesOnBoard)
        {
            if (g.playerOne != GameControl.singleton.playerOne)
            {
                List<GamePieceReference> L = g.ThreatenedPieces();
                foreach(GamePieceReference GPR in L)
				{
					if(GPR.index<0)
					{
						for(int i=0;i<MovePostions.Count;i++)
						{
							if (MovePostions[i][0] == g.BoardPos[0] && MovePostions[i][1] == g.BoardPos[1] &&
								GameControl.singleton.Pieces[gamePieces[i].index].Atk>=g.Def)
							{
								TakeAction(i);
								return true;
							}
						}

						if (Interpose(g.BoardPos, GPR.BoardPos, Int32.MaxValue))
							{ return true; }
					}
					else
					{
							TargetVals.Add(GameControl.singleton.Pieces[GPR.index].Value);
							TargetPos.Add(g.BoardPos);
						ThreatenedPiecePos.Add(GPR.BoardPos);
					}
				}
            }
        }
		if (TargetVals.Count > 0)
		{
			int HighestIndex = 0;
			int currHigh = 0;
			for (int i = 1; i < TargetVals.Count; i++)
			{
				if (TargetVals[i] > currHigh)
				{
					currHigh = TargetVals[i];
					HighestIndex = i;
				}
			}
			if(HighAtkValue>=currHigh)
			{
                TakeAction(HighAtkIndex);
				return true;
            }
			else
			{
				GamePieceReference gpr = Physics2D.Raycast((Vector2)transform.position + new Vector2(TargetPos[HighestIndex][0] + .5f, TargetPos[HighestIndex][1] + .5f), Vector2.zero).collider.GetComponent<GamePieceReference>();
                for (int i = 0; i < MovePostions.Count; i++)
                {
                    if (MovePostions[i][0] == TargetPos[HighestIndex][0] && MovePostions[i][1] == TargetPos[HighestIndex][1] &&
                        GameControl.singleton.Pieces[gamePieces[i].index].Atk >= gpr.Def)
                    {
                        TakeAction(i);
                        return true;
                    }
                }

				if(Retreat(Physics2D.Raycast((Vector2)transform.position + new Vector2(ThreatenedPiecePos[HighestIndex][0] + .5f, ThreatenedPiecePos[HighestIndex][1] + .5f), Vector2.zero).collider.GetComponent<GamePieceReference>()))
				{ return true; }
				if (Interpose(TargetPos[HighestIndex], ThreatenedPiecePos[HighestIndex], GameControl.singleton.Pieces[gpr.index].Value))
                { return true; }
            }
		}
		return false;
    }

	public bool Retreat(GamePieceReference GPR)
	{
		int currentIndex = MovePostions.Count - 1;
		foreach (int[] ia in GPR.PossibleMoves())
		{
			if (EnemyPowerGrid[ia[0]][ia[1]].Count == 0 ||
                (EnemyValsAttackingPieceValCheck(ia, GameControl.singleton.Pieces[GPR.index].Value) &&
                    (EnemyPowerGrid[ia[0]][ia[1]].Count < PowerGrid[ia[0]][ia[1]].Count ||
                    (EnemyPowerGrid[ia[0]][ia[1]].Count == PowerGrid[ia[0]][ia[1]].Count && PowerGridContainsOtherPiece(ia, GPR)))))
			{
				bool badRetreat = false;
				foreach (GamePieceReference g in EnemyPowerGrid[GPR.BoardPos[0]][GPR.BoardPos[1]])
				{
					if (InRangeOfAttack(ia, g))
					{ badRetreat = true; break; }
				}
				if (!badRetreat)
				{
					MovePostions.Add(ia);
					gamePieces.Add(GPR);
					actionTypes.Add(ActionType.Move);
				}
			}
			if (currentIndex < MovePostions.Count - 1)
			{
				HighAtkIndex = currentIndex+1;
				HighAtkValue = FindThreatValue(MovePostions[HighAtkIndex], GPR);
				for(int i=HighAtkIndex+1; i<MovePostions.Count-1;i++)
				{
					int tv = FindThreatValue(MovePostions[i], GPR);

                    if (HighAtkValue < tv)
					{
						HighAtkValue= tv;
						HighAtkIndex= i;
					}	
				}
				TakeAction(HighAtkIndex);
				return true;
			}
		}
		return false;
	}

	public bool InRangeOfAttack(int[] ia, GamePieceReference Attacker)
	{
		int[] Diff=new int[2] { ia[0] - Attacker.BoardPos[0], ia[1] - Attacker.BoardPos[1] };
		if (Mathf.Abs(Diff[0]) > GameControl.singleton.Pieces[Attacker.index].AP)
			return false;
		for(int i=0;i<8; i++)
		{
			if (GameControl.singleton.Pieces[Attacker.index].hasAttacks[i])
			{
                int x = Attacker.BoardPos[0] + (int)GameControl.singleton.dirRef[i].x * Mathf.Abs(Diff[0]);
                int y= Attacker.BoardPos[1] + (int)GameControl.singleton.dirRef[i].y * Mathf.Abs(Diff[1]);
				if (ia[0]==x && ia[1]==y)
					return true;
            }
		}
		return false;
	}


	public bool PowerGridContainsOtherPiece(int[] ia,GamePieceReference GPR)
	{
		foreach(GamePieceReference G in PowerGrid[ia[0]][ia[1]])
		{
			if (G.BoardPos[0] != GPR.BoardPos[0] || G.BoardPos[1] != GPR.BoardPos[1])
				return true;
		}
		return false;
	}

	public bool Interpose(int[] AttackerPosition, int[] DefenderPosition, int DefenderValue)
	{
        int currIndex = MovePostions.Count - 1;
		List<int> ProtectedIndices= new List<int>();
		int[] DiffIA= new int[2] { AttackerPosition[0] - DefenderPosition[0], AttackerPosition[1] - DefenderPosition[1] };
        if (Mathf.Abs(DiffIA[0])<=1 && Mathf.Abs(DiffIA[1])<=1)
		{
			return false;
		}
		if (DiffIA[0]<0)
		{ DiffIA[0] = -1;}
		else if (DiffIA[0]>0)
			DiffIA[0]=1;
		if (DiffIA[1]<0)
		{ DiffIA[1] = -1;} else if (DiffIA[1]>0) { DiffIA[1]=1;}
		List<int[]> TargetSquares=new List<int[]>();
		int i = DefenderPosition[0] + DiffIA[0];
		int j = DefenderPosition[1]+ DiffIA[1];
		while (i != AttackerPosition[0] || j != AttackerPosition[1])
		{
			TargetSquares.Add(new int[2] { i, j });
			i += DiffIA[0];
			j += DiffIA[1];
		}
		foreach(GamePieceReference g in GameControl.singleton.GamePiecesOnBoard)
		{
			if(g.playerOne ==GameControl.singleton.playerOne)
			{
				List<int[]> L = g.PossibleMoves();
				foreach(int[] ia in L)
				{
					foreach (int[] ts in TargetSquares) {
						bool protectedSquare = PowerGridContainsOtherPiece(ts, g);
						if (ts[0] == ia[0] && ts[1] == ia[1])
						{
							MovePostions.Add(ia);
							gamePieces.Add(g);
							actionTypes.Add(ActionType.Move);
							if (protectedSquare)
								ProtectedIndices.Add(MovePostions.Count - 1);
						}
					}
				}
			}
		}
		if(MovePostions.Count-1 > currIndex)
		{
			List<int> LowVals = new List<int>();
			int minVal= GameControl.singleton.Pieces[gamePieces[currIndex+1].index].Value;
			for(int t=currIndex+2; t<gamePieces.Count;t++)
			{
				if (GameControl.singleton.Pieces[gamePieces[t].index].Value<minVal)
				{
					minVal = GameControl.singleton.Pieces[gamePieces[t].index].Value;

                }
			}
			if (minVal < DefenderValue)
			{
                for (int t = currIndex + 1; t < gamePieces.Count; t++)
                {
                    if (GameControl.singleton.Pieces[gamePieces[t].index].Value == minVal
						&& ProtectedIndices.Contains(t))
                    {
                        LowVals.Add(t);
                    }
                }
            }
			if (ProtectedIndices.Count > 0)
			{
				if (LowVals.Count > 0)
				{
                    TakeAction(ProtectedIndices[GameControl.singleton.RNG.Next(LowVals.Count)]);
                }
				else
					TakeAction(ProtectedIndices[GameControl.singleton.RNG.Next(ProtectedIndices.Count)]);
            }
			else
				TakeAction(GameControl.singleton.RNG.Next(currIndex+1, MovePostions.Count-1));
			return true;
		}
		return false;
	}

	public void TakeAction(int index)
	{
		if (actionTypes[index] == ActionType.Move)
		{
			GameControl.singleton.MovePiece(gamePieces[index], MovePostions[index]);
		}
		else if (actionTypes[index] == ActionType.Attack)
		{
			GameControl.singleton.SelectedPiece = gamePieces[index].gameObject;
			GameControl.singleton.Capture(Physics2D.Raycast((Vector2)transform.position + new Vector2(MovePostions[index][0] +.5f, MovePostions[index][1]+.5f), Vector2.zero).collider.GetComponent<GamePieceReference>());
		}
	}

	void Awake() {
		singleton = this;
	}
}
