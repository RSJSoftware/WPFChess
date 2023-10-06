using System;
using System.Collections.Generic;
using System.Diagnostics;
using Chess.BoardManager;

namespace Chess.Perft
{
	public static class Driver
	{
		public static long nodes;
		public static long captures;
		public static long castles;
		public static long promotions;
		public static long checks;
		public static long checkmates;
		public static long enPassants;
		public static Stopwatch stopwatch = new Stopwatch();
		public static string moveDebug;

		private static void RecursiveDriver(int depth, BoardState board)
		{
			if (depth == 0)
			{
				nodes++;
				return;
			}

			List<Move> moveList = new List<Move>(board.LegalMoves);
			foreach (Move move in moveList)
			{
				if (!MovePiece(board, move, depth))
					continue;
				//string debugLine = $"mov: {Driver.nodes}, cap: {Driver.captures}, ep: {Driver.enPassants}, " +
				//$"cas: {Driver.castles}, pro: {Driver.promotions}, che: {Driver.checks}, mat: {Driver.checkmates}, time: {Driver.stopwatch.ElapsedMilliseconds}ms ";
				//Console.WriteLine(depth + " " + copy.ToString() + " " + debugLine);
				RecursiveDriver(depth - 1, board);
				board.UnmoveBits();
			}
		}

		public static void PerftDriver(int depth, BoardState board)
		{
			moveDebug = "";
			ResetVars();
			stopwatch.Start();
			if (depth == 0)
			{
				nodes++;
				return;
			}

			List<Move> moveList = new List<Move>(board.LegalMoves);
			foreach (Move move in moveList)
			{
				long currentNode = nodes;
				if (!MovePiece(board, move, depth))
					continue;
				RecursiveDriver(depth - 1, board);
				board.UnmoveBits();
				moveDebug += move + ": " + (nodes - currentNode) + "\n";
			}
			stopwatch.Stop();
		}

		//special move function specficially for perft, does not create move names, increment clocks, or check for null promotion
		private static bool MovePiece(BoardState board, Move move, int depth)
		{
			board.MoveHistory.Push(new BoardState(board));


			Piece movePiece = move.MovePiece;
			Piece takePiece = move.TakePiece;

			//check for en passant
			Player opponent = (board.Turn == Player.White) ? Player.Black : Player.White;
			if (move.EnPassant)
			{
				//manually remove the pawn since it's not on its target square
				if (board.Turn == Player.White) board.Board.RemovePiece(Piece.Pawn, opponent, (Sq)((int)move.End + 8));
				else board.Board.RemovePiece(Piece.Pawn, opponent, (Sq)((int)move.End - 8));
			}
			//actually move the pieces on the bitboards
			board.Board.MoveBits(movePiece, takePiece, board.Turn, opponent, move.Start, move.End);

			//if the move ended up in a check for the player, undo the move
			if (board.Board.IsInCheck(board.Turn))
			{
				board.UnmoveBits();
				return false;
			}

			//if the depth ends here, do not handle any other housekeeping steps after this
			if (depth == 1)
				return true;

			//check for pawn promotion in the move object
			if (move.PromotePiece != Piece.None)
			{
				//remove the pawn and put the promotion piece in it's place
				board.Board.RemovePiece(movePiece, board.Turn, move.End);
				board.Board.SetPiece(move.PromotePiece, board.Turn, move.End);
			}
			//check to see if en passant square needs to be set, otherwise make sure it's empty
			if (movePiece == Piece.Pawn && Math.Abs((int)move.Start - (int)move.End) == 16)
				board.EnPassantSquare = (board.Turn == Player.White) ? (Sq)((int)move.Start - 8) : (Sq)((int)move.Start + 8);
			else
				board.EnPassantSquare = Sq.empty;
			//check for castling move, legal castling should have been checked before hand
			if (move.CastleMove != Castle.None)
			{
				//remove the existing castling rights from the player
				board.RemoveCastlingRights(board.Turn);

				bool isShortCastle = move.CastleMove.HasFlag(Castle.WhiteCastle) || move.CastleMove.HasFlag(Castle.BlackCastle);

				//find the start and end squares for the rook
				Sq rookStartSquare = isShortCastle ? (Sq)((int)move.End + 1) : (Sq)((int)move.End - 2);
				Sq rookEndSquare = isShortCastle ? (Sq)((int)move.End - 1) : (Sq)((int)move.End + 1);

				//move rook
				board.Board.MoveBits(Piece.Rook, Piece.None, board.Turn, opponent, rookStartSquare, rookEndSquare);
			}
			//if a rook moved, see if the castling rights need to be removed
			if (movePiece == Piece.Rook && board.CastleRights != Castle.None)
			{
				if (board.Turn == Player.White && move.Start == Sq.h1 && board.CastleRights.HasFlag(Castle.WhiteCastle))
					board.CastleRights ^= Castle.WhiteCastle;
				else if (board.Turn == Player.White && move.Start == Sq.a1 && board.CastleRights.HasFlag(Castle.WhiteQueenCastle))
					board.CastleRights ^= Castle.WhiteQueenCastle;
				else if (board.Turn == Player.Black && move.Start == Sq.h8 && board.CastleRights.HasFlag(Castle.BlackCastle))
					board.CastleRights ^= Castle.BlackCastle;
				else if (board.Turn == Player.Black && move.Start == Sq.a8 && board.CastleRights.HasFlag(Castle.BlackQueenCastle))
					board.CastleRights ^= Castle.BlackQueenCastle;
			}

			//if a rook was taken, see if the castling rights need to be removed
			if (takePiece == Piece.Rook && board.CastleRights != Castle.None)
			{
				if (board.Turn == Player.Black && move.End == Sq.h1 && board.CastleRights.HasFlag(Castle.WhiteCastle))
					board.CastleRights ^= Castle.WhiteCastle;
				else if (board.Turn == Player.Black && move.End == Sq.a1 && board.CastleRights.HasFlag(Castle.WhiteQueenCastle))
					board.CastleRights ^= Castle.WhiteQueenCastle;
				else if (board.Turn == Player.White && move.End == Sq.h8 && board.CastleRights.HasFlag(Castle.BlackCastle))
					board.CastleRights ^= Castle.BlackCastle;
				else if (board.Turn == Player.White && move.End == Sq.a8 && board.CastleRights.HasFlag(Castle.BlackQueenCastle))
					board.CastleRights ^= Castle.BlackQueenCastle;
			}

			//change turns and calculate new moves
			board.ChangeTurn();

			board.LegalMoves = board.Board.GetMoveDictionary(board.Turn, board.EnPassantSquare, board.CastleRights);

			return true;
		}

		public static void ResetVars()
		{
			nodes = 0;
			captures = 0;
			castles = 0;
			promotions = 0;
			checks = 0;
			checkmates = 0;
			enPassants = 0;
			stopwatch = new Stopwatch();
		}
	}
}