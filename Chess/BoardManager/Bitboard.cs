using System;
using System.Collections.Generic;

namespace Chess.BoardManager
{
	public class Bitboard
	{
		ulong[] pieceBitboards;
		ulong[] occBitboards;

		public Bitboard()
		{
			//piece bitboards array contains 6 bitboards for each 6 piece types
			pieceBitboards = new ulong[6] { 0UL, 0UL, 0UL, 0UL, 0UL, 0UL };

			//occ bitboards array contains 2 bitboards for each players pieces
			occBitboards = new ulong[2] { 0UL, 0UL };
		}

		public Bitboard(Bitboard b)
		{
			pieceBitboards = new ulong[6] { b.pieceBitboards[0], b.pieceBitboards[1], b.pieceBitboards[2], b.pieceBitboards[3], b.pieceBitboards[4], b.pieceBitboards[5] };
			occBitboards = new ulong[2] { b.occBitboards[0], b.occBitboards[1] };
		}

		public void SetPiece(Piece piece, Player player, Sq square)
		{
			//set specified piece in the corresponding bitboard in the specified square
			pieceBitboards[(int)piece] = BitboardController.SetBit(pieceBitboards[(int)piece], square);

			//set piece in the corresponding specified player bitboard to reflect the piece
			occBitboards[(int)player] = BitboardController.SetBit(occBitboards[(int)player], square);
		}

		public void RemovePiece(Piece piece, Player player, Sq square)
		{
			//set specified piece in the corresponding bitboard in the specified square
			pieceBitboards[(int)piece] = BitboardController.PopBit(pieceBitboards[(int)piece], square);

			//set piece in the corresponding specified player bitboard to reflect the piece
			occBitboards[(int)player] = BitboardController.PopBit(occBitboards[(int)player], square);
		}

		public ulong GetBitboard(Piece piece, Player player)
		{
			//return the overlap between the piece and occupancy bitboards to get the specific player pieces
			return pieceBitboards[(int)piece] & occBitboards[(int)player];
		}

		public ulong GetAttackMap(Player player)
		{
			//initialize and add all pieces to attack map
			ulong attacks = GetPieceAttacks(Piece.Pawn, player);
			attacks |= GetPieceAttacks(Piece.Knight, player);
			attacks |= GetPieceAttacks(Piece.Bishop, player);
			attacks |= GetPieceAttacks(Piece.Rook, player);
			attacks |= GetPieceAttacks(Piece.Queen, player);
			attacks |= GetPieceAttacks(Piece.King, player);

			return attacks;
		}

		public ulong GetPieceAttacks(Piece piece, Player player)
		{
			//get piece board
			ulong pieces = GetBitboard(piece, player);
			ulong attacks = 0UL;

			//initialize variables for specific pieces
			ulong occupancy = occBitboards[0] | occBitboards[1];

			//get precalculated attacks for each piece on board using least significant bit, then popping that bit
			while (pieces != 0)
			{
				int lsbit = BitboardController.GetLSBitIndex(pieces);

				switch (piece)
				{
					case Piece.Pawn: attacks |= BitboardController.arrPawnAttacks[(int)player, lsbit]; break;
					case Piece.Knight: attacks |= BitboardController.arrKnightAttacks[lsbit]; break;
					case Piece.Bishop: attacks |= BitboardController.GetBishopAttack(lsbit, occupancy); break;
					case Piece.Rook: attacks |= BitboardController.GetRookAttack(lsbit, occupancy); break;
					case Piece.Queen: attacks |= BitboardController.GetQueenAttack(lsbit, occupancy); break;
					case Piece.King: attacks |= BitboardController.arrKingAttacks[lsbit]; break;
				}

				pieces = BitboardController.PopBit(pieces, lsbit);
			}

			return attacks;
		}

		public bool IsSquareAttacked(Player player, Sq square)
		{
			//return true if the specified square is active in the attack map for the specified player
			return BitboardController.GetBit(GetAttackMap(player), square) > 0;
		}

		public bool IsInCheck(Player player)
		{
			Player opponent = (player == Player.White) ? Player.Black : Player.White;

			//return true if the king shares an active bit with the opponent attack map
			return IsSquareAttacked(opponent, (Sq)BitboardController.GetLSBitIndex(GetBitboard(Piece.King, player)));
		}

		public List<Move> TestMoves(Sq start, List<Move> moves, Player player, Player opponent)
		{
			List<Move> legalMoves = new List<Move>(moves);
			Piece piece;
			Piece takePiece;

			foreach (Move m in moves)
			{
				//create a copy of the current board to make test moves on
				Bitboard testBoard = new Bitboard(this);
				piece = m.MovePiece;
				takePiece = m.TakePiece;

				//make the move on a test board and check to see if the player is in check
				testBoard.MoveBits(piece, takePiece, player, opponent, start, m.End);

				if (testBoard.IsInCheck(player))
					legalMoves.Remove(m);
			}

			return legalMoves;
		}

		public List<Move> GetPieceMoves(Piece piece, Player player, Sq enPassSq)
		{
			List<Move> allMoves = new List<Move>();
			//get piece and occupancy boards
			ulong pieces = GetBitboard(piece, player);
			ulong occupancy = occBitboards[0] | occBitboards[1];
			int opponent = (player == Player.White) ? (int)Player.Black : (int)Player.White;
			ulong rank = (player == Player.White) ? BitboardController.eighthRank : BitboardController.firstRank;

			//get precalculated attacks for each piece on board using least significant bit, then popping that bit
			while (pieces != 0)
			{
				int lsbit = BitboardController.GetLSBitIndex(pieces);
				ulong moves = 0UL;

				//add a bitboard with all the moves that don't attack a friendly piece and only add pawn attacks that attack an opponent
				switch (piece)
				{
					case Piece.Pawn:
						//add enPassant square if it exists and is within the pawn's line of sight
						ulong enPassCheck = 0UL;
						if (enPassSq != Sq.empty)
						{
							enPassCheck = BitboardController.SetBit(enPassCheck, enPassSq);
							moves |= BitboardController.arrPawnAttacks[(int)player, lsbit] & enPassCheck;
						}
						moves |= BitboardController.arrPawnAttacks[(int)player, lsbit] & occBitboards[opponent];
						break;
					case Piece.Knight: moves |= BitboardController.arrKnightAttacks[lsbit] & ~occBitboards[(int)player]; break;
					case Piece.Bishop: moves |= BitboardController.GetBishopAttack(lsbit, occupancy) & ~occBitboards[(int)player]; break;
					case Piece.Rook: moves |= BitboardController.GetRookAttack(lsbit, occupancy) & ~occBitboards[(int)player]; break;
					case Piece.Queen: moves |= BitboardController.GetQueenAttack(lsbit, occupancy) & ~occBitboards[(int)player]; break;
					case Piece.King: moves |= BitboardController.arrKingAttacks[lsbit] & ~occBitboards[(int)player]; break;
				}

				//convert move bits to move objects
				while (moves != 0)
				{
					int lsbitMove = BitboardController.GetLSBitIndex(moves);

					//add a special case for en passant captures
					if (piece == Piece.Pawn && (Sq)lsbitMove == enPassSq)
					{
						allMoves.Add(new Move((Sq)lsbit, (Sq)lsbitMove, piece, true));
						moves = BitboardController.PopBit(moves, lsbitMove);
						continue;
					}

					//find if a piece is being taken or not
					Piece takePiece = FindPieceOnSquare((Sq)lsbitMove, (Player)opponent);

					//check for pawn promotions and add all 4 if possible
					if (piece == Piece.Pawn && BitboardController.GetBit(rank, lsbitMove) > 0)
					{
						for (int promotionPiece = (int)Piece.Knight; promotionPiece <= (int)Piece.Queen; promotionPiece++)
							allMoves.Add(new Move((Sq)lsbit, (Sq)lsbitMove, piece, takePiece, (Piece)promotionPiece));

						moves = BitboardController.PopBit(moves, lsbit);
						continue;
					}

					allMoves.Add(new Move((Sq)lsbit, (Sq)lsbitMove, piece, takePiece));

					moves = BitboardController.PopBit(moves, lsbitMove);
				}

				pieces = BitboardController.PopBit(pieces, lsbit);
			}

			return allMoves;
		}

		public Piece FindPieceOnSquare(Sq square, Player player)
		{
			if (BitboardController.GetBit(occBitboards[(int)player], square) > 0)
			{
				for (int i = (int)Piece.Pawn; i <= (int)Piece.King; i++)
				{
					if (BitboardController.GetBit(pieceBitboards[i], square) > 0)
						return (Piece)i;

				}
			}

			return Piece.None;
		}

		public List<Move> GetMoveDictionary(Player player, Sq enPassSq, Castle castle)
		{
			List<Move> allMoves = new List<Move>();
			ulong occupancy = occBitboards[0] | occBitboards[1];

			allMoves.AddRange(GetPieceMoves(Piece.Pawn, player, enPassSq));
			allMoves.AddRange(GetPieceMoves(Piece.Knight, player, enPassSq));
			allMoves.AddRange(GetPieceMoves(Piece.Bishop, player, enPassSq));
			allMoves.AddRange(GetPieceMoves(Piece.Rook, player, enPassSq));
			allMoves.AddRange(GetPieceMoves(Piece.Queen, player, enPassSq));
			allMoves.AddRange(GetPieceMoves(Piece.King, player, enPassSq));

			//get pawn push moves
			ulong pieces = GetBitboard(Piece.Pawn, player);
			int lsbit;

			//get push moves for each pawn on board using least significant bit, then popping that bit
			while (pieces != 0)
			{
				lsbit = BitboardController.GetLSBitIndex(pieces);
				//check single square push
				int possibleMove = (player == Player.White) ? (lsbit - 8) : (lsbit + 8);

				//make sure nothing is right in front of the pawn, if there is immediately continue
				if (BitboardController.GetBit(occupancy, possibleMove) > 0)
				{
					pieces = BitboardController.PopBit(pieces, lsbit);
					continue;
				}

				//if the pawn is being promoted, add all 4 promotion moves and immediately continue
				ulong rank = (player == Player.White) ? BitboardController.eighthRank : BitboardController.firstRank;
				if (BitboardController.GetBit(rank, possibleMove) > 0)
				{
					for (int promotionPiece = (int)Piece.Knight; promotionPiece <= (int)Piece.Queen; promotionPiece++)
						allMoves.Add(new Move((Sq)lsbit, (Sq)possibleMove, Piece.Pawn, Piece.None, (Piece)promotionPiece));

					pieces = BitboardController.PopBit(pieces, lsbit);
					continue;
				}

				//otherwise add the single push without a promotion flag
				allMoves.Add(new Move((Sq)lsbit, (Sq)possibleMove, Piece.Pawn));

				//check 2 square push
				possibleMove = (player == Player.White) ? (lsbit - 16) : (lsbit + 16);
				rank = (player == Player.White) ? BitboardController.secondRank : BitboardController.seventhRank;

				//if the pawn is on its starting square and nothing is on the destination square, add a 2 space move (nothing will be on the square directly in front)
				if ((BitboardController.GetBit(rank, lsbit) > 0) && !(BitboardController.GetBit(occupancy, possibleMove) > 0))
					allMoves.Add(new Move((Sq)lsbit, (Sq)possibleMove, Piece.Pawn));

				pieces = BitboardController.PopBit(pieces, lsbit);
			}

			//add castling moves
			pieces = GetBitboard(Piece.King, player);
			lsbit = BitboardController.GetLSBitIndex(pieces);
			ulong attacks = (player == Player.White) ? GetAttackMap(Player.Black) : GetAttackMap(Player.White);

			if (player == Player.White)
			{
				//make sure the flag is still set
				if (castle.HasFlag(Castle.WhiteCastle))
				{
					//make sure no pieces are in the way of the castling, that player will not castle through or into check, and that the player is not in check
					int possibleMove = lsbit + 2;
					bool isPieceBlocking = BitboardController.GetBit(occupancy, lsbit + 1) > 0 || BitboardController.GetBit(occupancy, possibleMove) > 0;
					bool isEnemyGuarding = BitboardController.GetBit(attacks, lsbit + 1) > 0 || BitboardController.GetBit(attacks, possibleMove) > 0;
					if (!isPieceBlocking && !isEnemyGuarding && !IsInCheck(player))
						allMoves.Add(new Move((Sq)lsbit, (Sq)possibleMove, Piece.King, Castle.WhiteCastle));
				}

				//make sure the flag is still set
				if (castle.HasFlag(Castle.WhiteQueenCastle))
				{
					//make sure no pieces are in the way of the castling, that player will not castle through or into check, and that the player is not in check
					int possibleMove = lsbit - 2;
					bool isPieceBlocking = BitboardController.GetBit(occupancy, lsbit - 1) > 0 || BitboardController.GetBit(occupancy, possibleMove) > 0;
					bool isEnemyGuarding = BitboardController.GetBit(attacks, lsbit - 1) > 0 || BitboardController.GetBit(attacks, possibleMove) > 0;
					if (!isPieceBlocking && !isEnemyGuarding && !IsInCheck(player))
						allMoves.Add(new Move((Sq)lsbit, (Sq)possibleMove, Piece.King, Castle.WhiteQueenCastle));
				}
			}
			else
			{
				//make sure the flag is still set
				if (castle.HasFlag(Castle.BlackCastle))
				{
					//make sure no pieces are in the way of the castling, that player will not castle through or into check, and that the player is not in check
					int possibleMove = lsbit + 2;
					bool isPieceBlocking = BitboardController.GetBit(occupancy, lsbit + 1) > 0 || BitboardController.GetBit(occupancy, possibleMove) > 0;
					bool isEnemyGuarding = BitboardController.GetBit(attacks, lsbit + 1) > 0 || BitboardController.GetBit(attacks, possibleMove) > 0;
					if (!isPieceBlocking && !isEnemyGuarding && !IsInCheck(player))
						allMoves.Add(new Move((Sq)lsbit, (Sq)possibleMove, Piece.King, Castle.BlackCastle));
				}

				//make sure the flag is still set
				if (castle.HasFlag(Castle.BlackQueenCastle))
				{
					//make sure no pieces are in the way of the castling, that player will not castle through or into check, and that the player is not in check
					int possibleMove = lsbit - 2;
					bool isPieceBlocking = BitboardController.GetBit(occupancy, lsbit - 1) > 0 || BitboardController.GetBit(occupancy, possibleMove) > 0;
					bool isEnemyGuarding = BitboardController.GetBit(attacks, lsbit - 1) > 0 || BitboardController.GetBit(attacks, possibleMove) > 0;
					if (!isPieceBlocking && !isEnemyGuarding && !IsInCheck(player))
						allMoves.Add(new Move((Sq)lsbit, (Sq)possibleMove, Piece.King, Castle.BlackQueenCastle));
				}
			}

			//go through each move and make sure none of them put the king in check
			// pieces = occBitboards[(int)player];
			// while (pieces != 0)
			// {
			// 	lsbit = BitboardController.GetLSBitIndex(pieces);

			// 	moveDic[(Sq)lsbit] = TestMoves((Sq)lsbit, moveDic[(Sq)lsbit], player, (Player)opponent);

			// 	pieces = BitboardController.PopBit(pieces, lsbit);
			// }

			return allMoves;
		}

		public void MoveBits(Piece movePiece, Piece takePiece, Player player, Player opp, Sq start, Sq end)
		{
			if (takePiece != Piece.None)
				RemovePiece(takePiece, opp, end);

			SetPiece(movePiece, player, end);
			RemovePiece(movePiece, player, start);
		}

		public void PrintBoard()
		{
			Console.WriteLine();
			//loop over ranks
			for (int rank = 0; rank < 8; rank++)
			{
				//loop over files
				for (int file = 0; file < 8; file++)
				{

					if (file == 0)
					{
						Console.ForegroundColor = ConsoleColor.DarkRed;
						Console.Write($"  {8 - rank} ");
						Console.ForegroundColor = ConsoleColor.White;
					}

					//convert into square index
					int square = rank * 8 + file;

					//check for white pieces
					if (BitboardController.GetBit(occBitboards[(int)Player.White], square) > 0)
					{
						Console.ForegroundColor = ConsoleColor.Cyan;
						if (BitboardController.GetBit(pieceBitboards[(int)Piece.Pawn], square) > 0)
							Console.Write(" P");
						else if (BitboardController.GetBit(pieceBitboards[(int)Piece.Knight], square) > 0)
							Console.Write(" N");
						else if (BitboardController.GetBit(pieceBitboards[(int)Piece.Bishop], square) > 0)
							Console.Write(" B");
						else if (BitboardController.GetBit(pieceBitboards[(int)Piece.Rook], square) > 0)
							Console.Write(" R");
						else if (BitboardController.GetBit(pieceBitboards[(int)Piece.Queen], square) > 0)
							Console.Write(" Q");
						else if (BitboardController.GetBit(pieceBitboards[(int)Piece.King], square) > 0)
							Console.Write(" K");
						else
							Console.Write("Bitboard mismatch on white: " + square);
						Console.ForegroundColor = ConsoleColor.White;
					}
					//check for black pieces
					else if (BitboardController.GetBit(occBitboards[(int)Player.Black], square) > 0)
					{
						Console.ForegroundColor = ConsoleColor.DarkMagenta;
						if (BitboardController.GetBit(pieceBitboards[(int)Piece.Pawn], square) > 0)
							Console.Write(" P");
						else if (BitboardController.GetBit(pieceBitboards[(int)Piece.Knight], square) > 0)
							Console.Write(" N");
						else if (BitboardController.GetBit(pieceBitboards[(int)Piece.Bishop], square) > 0)
							Console.Write(" B");
						else if (BitboardController.GetBit(pieceBitboards[(int)Piece.Rook], square) > 0)
							Console.Write(" R");
						else if (BitboardController.GetBit(pieceBitboards[(int)Piece.Queen], square) > 0)
							Console.Write(" Q");
						else if (BitboardController.GetBit(pieceBitboards[(int)Piece.King], square) > 0)
							Console.Write(" K");
						else
							Console.Write("Bitboard mismatch on black: " + square);
						Console.ForegroundColor = ConsoleColor.White;
					}
					else
					{
						Console.ForegroundColor = ConsoleColor.DarkGray;
						Console.Write(" .");
						Console.ForegroundColor = ConsoleColor.White;
					}
				}
				//print new line on ranks
				Console.WriteLine();
			}

			Console.ForegroundColor = ConsoleColor.DarkRed;
			Console.WriteLine("     a b c d e f g h");
			Console.ForegroundColor = ConsoleColor.White;

			Console.WriteLine($"Bitboard: {occBitboards[0] | occBitboards[1]}");
		}
	}
}