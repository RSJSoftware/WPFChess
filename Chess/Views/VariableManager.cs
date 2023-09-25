using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Chess.BoardManager;

namespace Chess.Views
{
	static class VariableManager
	{
		static BoardState board;
		static Cell selectedCell;
		static LabelManager labels;
		static ObservableCollection<Cell> cellList;
		static Piece promotionPiece;

		//DEBUG VARIABLES
		public static string debug;

		public static void Initialize()
		{
			board = new BoardState();
			labels = new LabelManager();
			cellList = new ObservableCollection<Cell>();
			promotionPiece = Piece.None;
			debug = "";
			InitCells();
		}

		public static BoardState GetBoard()
		{
			return board;
		}

		public static void SetSelectedCell(Cell cell)
		{
			selectedCell = cell;
		}

		public static Cell GetSelectedCell()
		{
			return selectedCell;
		}

		public static LabelManager GetLabelManager()
		{
			return labels;
		}

		public static ObservableCollection<Cell> GetCells()
		{
			return cellList;
		}

		public static void MoveCell(int start, int end)
		{
			cellList.Move(start, end);
		}

		public static void SetPromotionPiece(Piece piece)
		{
			promotionPiece = piece;
		}

		public static Piece GetPromotionPiece()
		{
			return promotionPiece;
		}

		private static void InitCells()
		{
			ulong wPawn = board.Board.GetBitboard(Piece.Pawn, Player.White);
			ulong wKnight = board.Board.GetBitboard(Piece.Knight, Player.White);
			ulong wBishop = board.Board.GetBitboard(Piece.Bishop, Player.White);
			ulong wRook = board.Board.GetBitboard(Piece.Rook, Player.White);
			ulong wQueen = board.Board.GetBitboard(Piece.Queen, Player.White);
			ulong wKing = board.Board.GetBitboard(Piece.King, Player.White);
			ulong bPawn = board.Board.GetBitboard(Piece.Pawn, Player.Black);
			ulong bKnight = board.Board.GetBitboard(Piece.Knight, Player.Black);
			ulong bBishop = board.Board.GetBitboard(Piece.Bishop, Player.Black);
			ulong bRook = board.Board.GetBitboard(Piece.Rook, Player.Black);
			ulong bQueen = board.Board.GetBitboard(Piece.Queen, Player.Black);
			ulong bKing = board.Board.GetBitboard(Piece.King, Player.Black);

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
				cellList.Add(new Cell
				{
					Pos = new Point((i % 8), (i / 8)),
					ChessPiece = new ChessPieceVM { Type = pieceAtSq, Player = playerAtSq },
					Name = (Sq)i,
					IsSelected = false,
					IsLegal = false,
					IsDragging = false,
					IsCheck = false,
					IsChecking = false,
					IsActivated = false
				});
			}
		}

		public static void UpdateCells()
          {
               ulong wPawn = board.Board.GetBitboard(Piece.Pawn, Player.White);
               ulong wKnight = board.Board.GetBitboard(Piece.Knight, Player.White);
               ulong wBishop = board.Board.GetBitboard(Piece.Bishop, Player.White);
               ulong wRook = board.Board.GetBitboard(Piece.Rook, Player.White);
               ulong wQueen = board.Board.GetBitboard(Piece.Queen, Player.White);
               ulong wKing = board.Board.GetBitboard(Piece.King, Player.White);
               ulong bPawn = board.Board.GetBitboard(Piece.Pawn, Player.Black);
               ulong bKnight = board.Board.GetBitboard(Piece.Knight, Player.Black);
               ulong bBishop = board.Board.GetBitboard(Piece.Bishop, Player.Black);
               ulong bRook = board.Board.GetBitboard(Piece.Rook, Player.Black);
               ulong bQueen = board.Board.GetBitboard(Piece.Queen, Player.Black);
               ulong bKing = board.Board.GetBitboard(Piece.King, Player.Black);

			ulong wOcc = wPawn | wKnight | wBishop | wRook | wQueen | wKing;
			ulong bOcc = bPawn | bKnight | bBishop | bRook | bQueen | bKing;

               for (int i = 0; i < 64; i++)
               {
                    Cell toUpdate = cellList.Where(x => x.Name == (Sq)i).FirstOrDefault();
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
	}
}
