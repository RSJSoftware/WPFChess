using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Chess.AI;
using Chess.BoardManager;
using Chess.ViewModels;

namespace Chess.Views
{
	public static class VariableManager
	{
		public static BoardState Board { get; set; }
		public static Cell SelectedCell { get; set; }
		public static Point InitialPos { get; set; }
		public static LabelManager Labels { get; set; }
		public static ObservableCollection<Cell> CellList { get; set; }
          public static ObservableCollection<MoveVM> MoveList { get; set; }
          public static List<Cell> HighlightedCells { get; set; }
          public static Piece PromotionPiece { get; set; }
		public static List<BoardState> MoveHistory { get; set; }
		public static int CurrentMove { get; set; }
		public static bool IsBoardFlipped { get; set; }
		public static bool IsInSetupMode { get; set; }
		public static Piece SetupPiece { get; set; }
          public static Player SetupColor { get; set; }
		public static IChessAI AI { get; set; }
		public static Player PlayerColor { get; set; }
		public static GameState State { get; set; }

          //DEBUG VARIABLES
          public static string debug;

		public static void Initialize()
          {
               Board = new BoardState();
               Labels = new LabelManager();
               CellList = new ObservableCollection<Cell>();
               MoveList = new ObservableCollection<MoveVM>();
               HighlightedCells = new List<Cell>();
               PromotionPiece = Piece.None;
               debug = "";
               MoveHistory = new List<BoardState> { new BoardState(Board) };
               CurrentMove = 0;
               IsBoardFlipped = false;
			IsInSetupMode = false;
			SetupPiece = Piece.None;
               SetupColor = Player.White;
               InitCells();
               UpdateLabels();
			AI = null;
          }

		public static void NewGame()
          {
			NewGame(BoardState.INITIALPOS);
          }

          public static void NewGame(string fen)
          {
			if(SelectedCell != null)
			{
				SelectedCell.Pos = InitialPos;
				SelectedCell = null;
			}

               Board.ReadFen(fen);
               MoveList.Clear(); DehighlightBorders();
               PromotionPiece = Piece.None;
               debug = "";
               MoveHistory.Clear();
               MoveHistory.Add(new BoardState(Board));
               CurrentMove = 0;
               IsBoardFlipped = false;
               UpdateCells();
               UpdateLabels();
			State = Board.State;

			if(AI != null && PlayerColor != Board.Turn)
			{
				AI.Move(AI.Think());
				HandleMoveUpdates();
               }
          }

          public static void MoveCell(int start, int end)
		{
			CellList.Move(start, end);
		}

		public static void AddBoardState()
		{
			MoveHistory.Add(new BoardState(Board));
			CurrentMove++;
		}

		private static void InitCells()
		{
			ulong wPawn = Board.Board.GetBitboard(Piece.Pawn, Player.White);
			ulong wKnight = Board.Board.GetBitboard(Piece.Knight, Player.White);
			ulong wBishop = Board.Board.GetBitboard(Piece.Bishop, Player.White);
			ulong wRook = Board.Board.GetBitboard(Piece.Rook, Player.White);
			ulong wQueen = Board.Board.GetBitboard(Piece.Queen, Player.White);
			ulong wKing = Board.Board.GetBitboard(Piece.King, Player.White);
			ulong bPawn = Board.Board.GetBitboard(Piece.Pawn, Player.Black);
			ulong bKnight = Board.Board.GetBitboard(Piece.Knight, Player.Black);
			ulong bBishop = Board.Board.GetBitboard(Piece.Bishop, Player.Black);
			ulong bRook = Board.Board.GetBitboard(Piece.Rook, Player.Black);
			ulong bQueen = Board.Board.GetBitboard(Piece.Queen, Player.Black);
			ulong bKing = Board.Board.GetBitboard(Piece.King, Player.Black);

			ulong wOcc = wPawn | wKnight | wBishop | wRook | wQueen | wKing;
			ulong bOcc = bPawn | bKnight | bBishop | bRook | bQueen | bKing;

			for (int i = 0; i < 64; i++)
			{
				Piece pieceAtSq = Piece.None;
				Player playerAtSq = Player.White;

				if (BitboardController.GetBit(wOcc, i) > 0)
				{
					if (BitboardController.GetBit(wPawn, i) > 0) pieceAtSq = Piece.Pawn;
					else if (BitboardController.GetBit(wKnight, i) > 0) pieceAtSq = Piece.Knight;
					else if (BitboardController.GetBit(wBishop, i) > 0) pieceAtSq = Piece.Bishop;
					else if (BitboardController.GetBit(wRook, i) > 0) pieceAtSq = Piece.Rook;
					else if (BitboardController.GetBit(wQueen, i) > 0) pieceAtSq = Piece.Queen;
					else if (BitboardController.GetBit(wKing, i) > 0) pieceAtSq = Piece.King;
				}
				else if (BitboardController.GetBit(bOcc, i) > 0)
				{
					if (BitboardController.GetBit(bPawn, i) > 0) { pieceAtSq = Piece.Pawn; playerAtSq = Player.Black; }
					else if (BitboardController.GetBit(bKnight, i) > 0) { pieceAtSq = Piece.Knight; playerAtSq = Player.Black; }
					else if (BitboardController.GetBit(bBishop, i) > 0) { pieceAtSq = Piece.Bishop; playerAtSq = Player.Black; }
					else if (BitboardController.GetBit(bRook, i) > 0) { pieceAtSq = Piece.Rook; playerAtSq = Player.Black; }
					else if (BitboardController.GetBit(bQueen, i) > 0) { pieceAtSq = Piece.Queen; playerAtSq = Player.Black; }
					else if (BitboardController.GetBit(bKing, i) > 0) { pieceAtSq = Piece.King; playerAtSq = Player.Black; }
				}

				//Console.WriteLine("Adding cell: " + (Sq)i + " piece: " + pieceAtSq + " player: " + playerAtSq + " i: " + i);
				CellList.Add(new Cell
				{
					Pos = new Point(i % 8, i / 8),
					ChessPiece = new ChessPieceVM { Type = pieceAtSq, Player = playerAtSq },
					Name = (Sq)i,
					IsSelected = false,
					IsLegal = false,
					IsDragging = false,
					IsCheck = false,
					IsChecking = false,
                         IsHighlighted = false,
                         IsActivated = false
				});
			}
		}

		public static void UpdateCells()
		{
			ulong wPawn = Board.Board.GetBitboard(Piece.Pawn, Player.White);
			ulong wKnight = Board.Board.GetBitboard(Piece.Knight, Player.White);
			ulong wBishop = Board.Board.GetBitboard(Piece.Bishop, Player.White);
			ulong wRook = Board.Board.GetBitboard(Piece.Rook, Player.White);
			ulong wQueen = Board.Board.GetBitboard(Piece.Queen, Player.White);
			ulong wKing = Board.Board.GetBitboard(Piece.King, Player.White);
			ulong bPawn = Board.Board.GetBitboard(Piece.Pawn, Player.Black);
			ulong bKnight = Board.Board.GetBitboard(Piece.Knight, Player.Black);
			ulong bBishop = Board.Board.GetBitboard(Piece.Bishop, Player.Black);
			ulong bRook = Board.Board.GetBitboard(Piece.Rook, Player.Black);
			ulong bQueen = Board.Board.GetBitboard(Piece.Queen, Player.Black);
			ulong bKing = Board.Board.GetBitboard(Piece.King, Player.Black);

			ulong wOcc = wPawn | wKnight | wBishop | wRook | wQueen | wKing;
			ulong bOcc = bPawn | bKnight | bBishop | bRook | bQueen | bKing;

			for (int i = 0; i < 64; i++)
			{
				Cell toUpdate = CellList.Where(x => x.Name == (Sq)i).FirstOrDefault();
				Piece pieceAtSq = Piece.None;
				Player playerAtSq = Player.White;

				if (BitboardController.GetBit(wOcc, i) > 0)
				{
					if (BitboardController.GetBit(wPawn, i) > 0) pieceAtSq = Piece.Pawn;
					else if (BitboardController.GetBit(wKnight, i) > 0) pieceAtSq = Piece.Knight;
					else if (BitboardController.GetBit(wBishop, i) > 0) pieceAtSq = Piece.Bishop;
					else if (BitboardController.GetBit(wRook, i) > 0) pieceAtSq = Piece.Rook;
					else if (BitboardController.GetBit(wQueen, i) > 0) pieceAtSq = Piece.Queen;
					else if (BitboardController.GetBit(wKing, i) > 0) pieceAtSq = Piece.King;
				}
				else if (BitboardController.GetBit(bOcc, i) > 0)
				{
					if (BitboardController.GetBit(bPawn, i) > 0) { pieceAtSq = Piece.Pawn; playerAtSq = Player.Black; }
					else if (BitboardController.GetBit(bKnight, i) > 0) { pieceAtSq = Piece.Knight; playerAtSq = Player.Black; }
					else if (BitboardController.GetBit(bBishop, i) > 0) { pieceAtSq = Piece.Bishop; playerAtSq = Player.Black; }
					else if (BitboardController.GetBit(bRook, i) > 0) { pieceAtSq = Piece.Rook; playerAtSq = Player.Black; }
					else if (BitboardController.GetBit(bQueen, i) > 0) { pieceAtSq = Piece.Queen; playerAtSq = Player.Black; }
					else if (BitboardController.GetBit(bKing, i) > 0) { pieceAtSq = Piece.King; playerAtSq = Player.Black; }
				}

				//Console.WriteLine("Updating cell: " + (Sq)i + " piece: " + pieceAtSq + " player: " + playerAtSq + " i: " + i);
				toUpdate.ChessPiece = new ChessPieceVM { Type = pieceAtSq, Player = playerAtSq };
			}
		}

		public static void SetCell(Cell cell)
		{
			if(SetupPiece == Piece.None) 
				return;

			//set the visuals, these will be removed or added to the actual board when the user leaves setup mode
               CellList.Where(x => x == cell).FirstOrDefault().ChessPiece = new ChessPieceVM { Type = SetupPiece, Player = SetupColor };
          }

		public static void UpdateLabels()
		{
			Labels.CurrentMove = (Board.Turn == Player.White) ? "White to play" : "Black to play";
		}

		public static void FlipCells()
		{
			foreach (Cell c in CellList)
			{
				//get cell's positional index, reverse it, set the new position
				int cellIndex = (int)((c.Pos.X * 8) + c.Pos.Y);
				cellIndex = 63 - cellIndex;
				c.Pos = new Point(cellIndex / 8, cellIndex % 8);

				//fix the initial position of the selected cell so that it doesn't get updated to the opposite side of the board if an illegal move is attempted
				if (SelectedCell != null && SelectedCell.Equals(c))
				{
					InitialPos = c.Pos;
				}
			}
		}

		public static void AddMoveList()
		{
			//if it's black's turn, that means white's move is being added to the list
			if(Board.Turn == Player.Black) 
			{
				MoveList.Add(new MoveVM { MoveNum = (MoveList.Count + 1), WhiteMove = Board.MoveList.Last(), BlackMove = "" });
			}
			else
			{
				//add a case for an empty move list in case someone starts a board on black to move
				if (MoveList.Count == 0)
					MoveList.Add(new MoveVM { MoveNum = (MoveList.Count + 1), WhiteMove = "", BlackMove = Board.MoveList.Last() });
				else
					MoveList.Last().BlackMove = Board.MoveList.Last();
               }
          }

          public static void HighlightBorders()
          {
               HighlightedCells.Clear();
               if (Board.LegalMoves.Count <= 0)
                    return;

               foreach (Move move in Board.LegalMoves)
               {
                    if (SelectedCell.Name == move.Start)
                         HighlightedCells.Add(CellList.Where(x => x.Name == move.End).FirstOrDefault());
               }

               foreach (Cell c in HighlightedCells)
               {
                    c.IsLegal = true;
               }
          }

          public static void DehighlightBorders()
          {
               foreach (Cell c in HighlightedCells)
               {
                    c.IsLegal = false;
               }
          }

		public static void SetBoardState(int moveIndex)
          {
			if(SelectedCell != null)
               {
                    SelectedCell.IsSelected = false;
                    SelectedCell.IsDragging = false;
                    SelectedCell = null;
                    DehighlightBorders();
               }

               Board = MoveHistory[moveIndex];
               CurrentMove = moveIndex;
               UpdateCells();
          }

		public static void UpdateAI(int AISelection)
		{
			if(AI == null)
			{
				PlayerColor = Board.Turn;
			}

               switch (AISelection)
               {
                    case 0: AI = null; break;
                    case 1: AI = new RandomAI(Board); break;
                    default: Console.WriteLine("ERROR: Index out of range in AISelection"); break;
               }
          }

		public static void ActivateAI()
		{
			if (AI == null)
				return;

			int move = AI.Think();
			AI.Move(move);

			HandleMoveUpdates();
		}

		public static void HandleMoveUpdates()
		{
               UpdateCells();
               AddBoardState();
               AddMoveList();
               UpdateLabels();
			State = Board.State;
          }
     }
}
