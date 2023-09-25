using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

//Board State should keep track of piece position, turn, castling rights, en passant, and halfmove checks
namespace Chess.BoardManager
{
	public class BoardState
	{
		public const string INITIALPOS = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

		public Bitboard Board { get; set; }
		public Sq EnPassantSquare { get; set; }
		public Player Turn { get; set; }
		public Castle CastleRights { get; set; }
		public int HalfMove { get; set; }
		public int FullMove { get; set; }
		public bool IsMate { get; set; }
		public Dictionary<Sq, List<Move>> LegalMoves { get; set; }
		public Stack<BoardState> MoveHistory { get; set; }
		public List<string> MoveList { get; set; }


		public BoardState() : this(INITIALPOS)
		{
		}

		public BoardState(string fen)
		{
			Board = new Bitboard();
			ReadFen(fen);
			LegalMoves = Board.GetMoveDictionary(Turn, EnPassantSquare, CastleRights);
			MoveHistory = new Stack<BoardState>();
			IsMate = CheckForMate();
			MoveList = new List<string>();
		}

		public BoardState(BoardState b)
		{
			Board = new Bitboard(b.Board);
			EnPassantSquare = b.EnPassantSquare;
			Turn = b.Turn;
			CastleRights = b.CastleRights;
			HalfMove = b.HalfMove;
			FullMove = b.FullMove;
			LegalMoves = b.LegalMoves;
			MoveHistory = b.MoveHistory;
			MoveList = b.MoveList;
		}

		//return true if a piece successfully moved, return false if it doesn't
		public bool MovePiece(Move move)
		{
			//save the current board state before starting move
			MoveHistory.Push(new BoardState(this));
			string moveName = "";

			Piece movePiece = move.MovePiece;
			Piece takePiece = move.TakePiece;

			//check for en passant
			Player opponent = (Turn == Player.White) ? Player.Black : Player.White;
			if (move.EnPassant)
			{
				//manually remove the pawn since it's not on its target square
				if (Turn == Player.White) Board.RemovePiece(Piece.Pawn, opponent, (Sq)((int)move.End + 8));
				else Board.RemovePiece(Piece.Pawn, opponent, (Sq)((int)move.End - 8));
			}

			//increment clocks
			//reset halfmove clock if piece moving is a pawn, or if a piece is being taken
			if (movePiece == Piece.Pawn || takePiece != Piece.None)
				HalfMove = 0;
			else
				HalfMove++;

			if (Turn == Player.Black)
				FullMove++;

			//actually move the pieces on the bitboards
			Board.MoveBits(movePiece, takePiece, Turn, opponent, move.Start, move.End);

			//if the move ended up in a check for the player, undo the move
			if (Board.IsInCheck(Turn))
			{
				UnmoveBits();
				Console.WriteLine("ERROR: Move " + move + ": Leaves player in check.");
				return false;
			}

			//check for pawn promotion in the move object
			if (move.PromotePiece != Piece.None)
			{
				Console.WriteLine("PROMOTING");

				//if promotion piece wasn't specified by the player for some reason (e.g. cancelling the move), undo moves and return false
				if (move.PromotePiece == Piece.None)
				{
					UnmoveBits();
					Console.WriteLine("ERROR: Moving pawn to " + move.End + ": No specified promotion pieces.");
					return false;
				}

				//remove the pawn and put the promotion piece in it's place
				Board.RemovePiece(movePiece, Turn, move.End);
				Board.SetPiece(move.PromotePiece, Turn, move.End);
			}

			//check to see if en passant square needs to be set, otherwise make sure it's empty
			if (movePiece == Piece.Pawn && Math.Abs((int)move.Start - (int)move.End) == 16)
				EnPassantSquare = (Turn == Player.White) ? (Sq)((int)move.Start - 8) : (Sq)((int)move.Start + 8);
			else
				EnPassantSquare = Sq.empty;

			//check for castling move, legal castling should have been checked before hand
			if (move.CastleMove != Castle.None)
			{
				//remove the existing castling rights from the player
				RemoveCastlingRights(Turn);

				bool isShortCastle = move.CastleMove.HasFlag(Castle.WhiteCastle) || move.CastleMove.HasFlag(Castle.BlackCastle);

				//find the start and end squares for the rook
				Sq rookStartSquare = isShortCastle ? (Sq)((int)move.End + 1) : (Sq)((int)move.End - 2);
				Sq rookEndSquare = isShortCastle ? (Sq)((int)move.End - 1) : (Sq)((int)move.End + 1);

				moveName = isShortCastle ? "O-O" : "O-O-O";

				//move rook
				Board.MoveBits(Piece.Rook, Piece.None, Turn, opponent, rookStartSquare, rookEndSquare);
			}

			//if a rook moved, see if the castling rights need to be removed
			if (movePiece == Piece.Rook && CastleRights != Castle.None)
			{
				if (Turn == Player.White && move.Start == Sq.h1 && CastleRights.HasFlag(Castle.WhiteCastle))
					CastleRights ^= Castle.WhiteCastle;
				else if (Turn == Player.White && move.Start == Sq.a1 && CastleRights.HasFlag(Castle.WhiteQueenCastle))
					CastleRights ^= Castle.WhiteQueenCastle;
				else if (Turn == Player.Black && move.Start == Sq.h8 && CastleRights.HasFlag(Castle.BlackCastle))
					CastleRights ^= Castle.BlackCastle;
				else if (Turn == Player.Black && move.Start == Sq.a8 && CastleRights.HasFlag(Castle.BlackQueenCastle))
					CastleRights ^= Castle.BlackQueenCastle;
			}

			//change turns and calculate new moves
			ChangeTurn();

			//see if another piece of the same type can move to the same square for naming convention purposes
			string canOtherMove = CheckOtherPieceMove(movePiece, move.Start, move.End);

			LegalMoves = Board.GetMoveDictionary(Turn, EnPassantSquare, CastleRights);

			//check mate
			IsMate = CheckForMate();

			if (moveName != "")
			{
				if (IsMate)
					moveName += "#";
				else if (Board.IsInCheck(opponent))
					moveName += "+";
				MoveList.Add(moveName);
			}

			else SetMoveName(move, opponent, canOtherMove);
			return true;

		}

		//return true if a piece successfully moved, return false if it doesn't
		public bool MovePiece(Sq start, Sq end, Piece promotePiece)
		{
			//make sure a piece is trying to move
			if (!LegalMoves.TryGetValue(start, out List<Move> moves))
			{
				Console.WriteLine("ERROR: Piece at " + start + " does not exist in legal move dictionary.");
				return false;
			}

			//find legal move in the list, printing an error if it doesn't exist
			Move move = null;
			ulong possibleMoves = 0UL;
			foreach (Move m in moves)
			{
				if (m.End == end && m.PromotePiece == promotePiece)
				{
					move = m;
				}
				possibleMoves |= (ulong)m.End;
			}

			if (move == null)
			{
				Console.WriteLine("ERROR: Piece at " + start + " cannot move to " + end + " legal move bitboard: ");

				BitboardController.printBitboard(possibleMoves, (int)start);
				return false;
			}

			return MovePiece(move);
		}

		private string CheckOtherPieceMove(Piece movePiece, Sq start, Sq end)
		{
			string otherPiece = "";
			bool sameRank = false;
			bool sameFile = false;
			int otherPieceCount = 0;
			ulong pieces = Board.GetBitboard(movePiece, Turn);

			while (pieces != 0)
			{
				int lsbit = BitboardController.GetLSBitIndex(pieces);

				//check to see if another piece of the same type can go to the same square
				if (lsbit != (int)start)
				{
					if (!LegalMoves.TryGetValue(start, out List<Move> moves)) continue;

					foreach (Move m in moves)
					{
						if (m.End == end)
						{
							if (((int)start % 8) == (lsbit % 8))
								sameFile = true;
							else if (((int)start / 8) == (lsbit / 8))
								sameRank = true;

							otherPieceCount++;
						}
					}
				}

				pieces = BitboardController.PopBit(pieces, lsbit);
			}

			//return the whole start coord if other pieces are on the same file and rank
			if (sameFile && sameRank)
				otherPiece += BitboardController.squareToCoord[(int)start];
			//return just the rank if other piece is on same file and none on same rank
			else if (sameFile)
				otherPiece += BitboardController.squareToCoord[(int)start][1];
			//return just file if there are other pieces otherwise
			else if (otherPieceCount > 0)
				otherPiece += BitboardController.squareToCoord[(int)start][0];

			return otherPiece;
		}

		public void SetMoveName(Move move, Player opponent, string canOtherMove)
		{
			string moveName = "";

			//get the piece name (pawns are not specified)
			if (move.MovePiece == Piece.Knight) moveName += "N";
			else if (move.MovePiece == Piece.Bishop) moveName += "B";
			else if (move.MovePiece == Piece.Rook) moveName += "R";
			else if (move.MovePiece == Piece.Queen) moveName += "Q";
			else if (move.MovePiece == Piece.King) moveName += "K";

			//add string calculated before new moves were generated
			moveName += canOtherMove;

			//add an x if a piece is taken, and the start square if taken by a pawn
			if (move.TakePiece != Piece.None)
				moveName += "x";
			else if (move.MovePiece == Piece.Pawn)
				moveName += BitboardController.squareToCoord[(int)move.Start][0] + "x";

			//add the name of the end square
			moveName += move.End;

			//if pawn is being promoted, add what piece it is being promoted to
			if (move.MovePiece == Piece.Pawn && move.PromotePiece != Piece.None)
			{
				if (move.PromotePiece == Piece.Knight) moveName += "=N";
				else if (move.PromotePiece == Piece.Bishop) moveName += "=B";
				else if (move.PromotePiece == Piece.Rook) moveName += "=R";
				else if (move.PromotePiece == Piece.Queen) moveName += "=Q";
				else if (move.PromotePiece == Piece.King) moveName += "=K";
			}

			//add a # for mate and a + for a check
			if (IsMate)
				moveName += "#";
			else if (Board.IsInCheck(opponent))
				moveName += "+";

			MoveList.Add(moveName);
		}

		public void UnmoveBits()
		{
			if(MoveHistory.Count == 0)
			{
				Console.WriteLine("ERROR: move history is empty.");
				PrintBoard();
				return;
			}
			BoardState undoBoard = MoveHistory.Pop();

			//restore last legal state
			Board = undoBoard.Board;
			EnPassantSquare = undoBoard.EnPassantSquare;
			Turn = undoBoard.Turn;
			CastleRights = undoBoard.CastleRights;
			HalfMove = undoBoard.HalfMove;
			FullMove = undoBoard.FullMove;
			LegalMoves = undoBoard.LegalMoves;
		}

		public void RemoveCastlingRights(Player player)
		{
			if (player == Player.White)
			{
				if (CastleRights.HasFlag(Castle.WhiteCastle))
					CastleRights ^= Castle.WhiteCastle;
				if (CastleRights.HasFlag(Castle.WhiteQueenCastle))
					CastleRights ^= Castle.WhiteQueenCastle;
			}
			else
			{
				if (CastleRights.HasFlag(Castle.BlackCastle))
					CastleRights ^= Castle.BlackCastle;
				if (CastleRights.HasFlag(Castle.BlackQueenCastle))
					CastleRights ^= Castle.BlackQueenCastle;
			}
		}

		public void ChangeTurn()
		{
			if (Turn == Player.White)
				Turn = Player.Black;
			else
				Turn = Player.White;
		}

		public void ReadFen(string fen)
		{
			//regex pattern for a full FEN code including piece placement, side to move, castling ability, en passant target square, halfmove clock, and full move counter
			string pattern = @"(^([1-8pnbrqk]{1,8}/){7}([1-8pnbrqk]{1,8}){1}(\s[w|b]{1})(\s(([-]{1})|([kq]{1,4})){1})((\s(([-]{1})|[a-h]{1}[1-8]{1})){1})((\s\d{1,3}){2})+$)";
			Regex fenrg = new Regex(pattern, RegexOptions.IgnoreCase);

			//immediately return if the fen doesn't match the pattern
			if (!fenrg.IsMatch(fen))
			{
				Console.WriteLine("Invalid FEN pattern: " + fen);
				return;
			}

			//split fen into 6 parts
			string[] parts = fen.Split(' ');

			//perform simple checks to make sure nothing illegal is in the FEN

			//1. make sure the piece placement part of the fen has only 2 kings
			int whiteKing = CharCount(parts[0], 'K');
			int blackKing = CharCount(parts[0], 'k');
			if (whiteKing != 1 || blackKing != 1)
			{
				Console.WriteLine("Invalid number of kings: " + parts[0]);
				return;
			}

			//2. pattern check should have ruled out invalid turn input

			//3. if there are still castling rights, make sure each character is used 0 or 1 times only
			int whiteQueen = 0;
			int blackQueen = 0;
			if (!parts[2].Equals("-"))
			{
				whiteKing = CharCount(parts[2], 'K');
				blackKing = CharCount(parts[2], 'k');
				whiteQueen = CharCount(parts[2], 'Q');
				blackQueen = CharCount(parts[2], 'q');
				if (whiteKing > 1 | blackKing > 1 | whiteQueen > 1 | blackQueen > 1)
				{
					Console.WriteLine("Invalid castling rights: " + parts[2]);
					return;
				}
			}

			//4. pattern check should have ruled out invalid en passant target squares

			//5. half move clock should not be greater than 100 (100 may be loaded to analyze the position) -- pattern check should ensure a positive int value
			int halfMoveClock = Int32.Parse(parts[4]);
			if (halfMoveClock > 100)
			{
				Console.WriteLine("Invalid halfmove clock: " + halfMoveClock);
				return;
			}

			//6. make sure full move clock is more than 0 since initial position starts at 1 (note: longest chess game in history is 269 moves, so 4 digit check would be invalid)
			int fullMoveClock = Int32.Parse(parts[5]);
			if (fullMoveClock < 1)
			{
				Console.WriteLine("Invalid fullmove clock: " + fullMoveClock);
				return;
			}

			//split the piece placement part of the fen into 8 rows
			string[] rows = parts[0].Split('/');

			int col = 0;

			//Make sure all rows add up to 8 columns
			foreach (string s in rows)
			{
				col = 0;
				foreach (var c in s.AsSpan())
				{
					if (Char.IsDigit(c))
						//try to count the number into column
						col += (int)char.GetNumericValue(c);
					else
						//add 1 to the column counter since the value is a piece
						col++;
				}

				//column should always add up to 8
				if (col != 8)
				{
					Console.WriteLine("Invalid number of files in rank: " + s + " Col counted: " + col);
					return;
				}
			}


			//Begin setting values since everything seems to be working
			//reset bitboards
			Board = new Bitboard();

			//go through each row to find where all pieces are
			for (int row = 0; row < rows.Length; row++)
			{
				//iterate through strings keeping track of board column to get position
				col = 0;
				for (int j = 0; j < rows[row].Length; j++)
				{
					char letter = rows[row][j];
					Sq square = (Sq)(row * 8 + col);
					switch (letter)
					{
						case 'p':
							Board.SetPiece(Piece.Pawn, Player.Black, square);
							col++;
							break;
						case 'P':
							Board.SetPiece(Piece.Pawn, Player.White, square);
							col++;
							break;
						case 'b':
							Board.SetPiece(Piece.Bishop, Player.Black, square);
							col++;
							break;
						case 'B':
							Board.SetPiece(Piece.Bishop, Player.White, square);
							col++;
							break;
						case 'n':
							Board.SetPiece(Piece.Knight, Player.Black, square);
							col++;
							break;
						case 'N':
							Board.SetPiece(Piece.Knight, Player.White, square);
							col++;
							break;
						case 'r':
							Board.SetPiece(Piece.Rook, Player.Black, square);
							col++;
							break;
						case 'R':
							Board.SetPiece(Piece.Rook, Player.White, square);
							col++;
							break;
						case 'k':
							Board.SetPiece(Piece.King, Player.Black, square);
							col++;
							break;
						case 'K':
							Board.SetPiece(Piece.King, Player.White, square);
							col++;
							break;
						case 'q':
							Board.SetPiece(Piece.Queen, Player.Black, square);
							col++;
							break;
						case 'Q':
							Board.SetPiece(Piece.Queen, Player.White, square);
							col++;
							break;
						default:
							try
							{
								col += (int)char.GetNumericValue(letter);
							}
							catch (Exception e)
							{
								Console.WriteLine(e.Message + " letter: " + letter);
							}
							break;

					}
				}
			}

			//find the active player
			if (parts[1].Equals("w"))
				Turn = Player.White;
			else if (parts[1].Equals("b"))
				Turn = Player.Black;
			else
				//sanity check
				Console.WriteLine("Invalid FEN at player: " + parts[1]);

			//find castling rights, perform checks using previously defined variables if castling phrase is not '-'
			CastleRights = Castle.None;
			if (!parts[2].Equals("-"))
			{
				if (whiteKing > 0)
					CastleRights |= Castle.WhiteCastle;
				if (blackKing > 0)
					CastleRights |= Castle.BlackCastle;
				if (whiteQueen > 0)
					CastleRights |= Castle.WhiteQueenCastle;
				if (blackQueen > 0)
					CastleRights |= Castle.BlackQueenCastle;
			}

			//find en passant square, set to none if phrase is '-'
			if (parts[3].Equals("-"))
				EnPassantSquare = Sq.empty;
			else
			{
				//check to make sure the enum exists and parse it into the en passant square
				if (Enum.IsDefined(typeof(Sq), parts[3]))
					EnPassantSquare = (Sq)Enum.Parse(typeof(Sq), parts[3]);
				else
					//sanity check
					Console.WriteLine("Invalid en passant square: " + parts[3]);
			}

			//set the move timers that were set earlier
			HalfMove = halfMoveClock;
			FullMove = fullMoveClock;

			//TODO add a board check for legal positions, if illegal, recursively call for an initial position

			//get legal moves
			LegalMoves = Board.GetMoveDictionary(Turn, EnPassantSquare, CastleRights);
		}

		private int CharCount(string source, char target)
		{
			int output = 0;

			foreach (var c in source.AsSpan())
			{
				if (c == target)
					output++;
			}

			return output;
		}

		public bool CheckForMate()
		{
			foreach (var kvp in LegalMoves)
			{
				if (kvp.Value.Count > 0)
					return false;
			}

			return true;
		}

		public void PrintBoard()
		{
			Board.PrintBoard();
			Console.WriteLine("Turn    : " + Turn);
			Console.WriteLine("EnPass  : " + EnPassantSquare);
			Console.Write($"Castle  : {(CastleRights.HasFlag(Castle.WhiteCastle) ? 'K' : '-')} {(CastleRights.HasFlag(Castle.WhiteQueenCastle) ? 'Q' : '-')}");
			Console.WriteLine($" {(CastleRights.HasFlag(Castle.BlackCastle) ? 'k' : '-')} {(CastleRights.HasFlag(Castle.BlackQueenCastle) ? 'q' : '-')}");
			Console.WriteLine();
		}

		//test all moves from the start square to find which are allowed or not for gui visualization
		public void TestMoves(Sq start)
		{
			Player opponent = (Turn == Player.White) ? Player.Black : Player.White;
			List<bool> movesToRemove = new List<bool>();

			//test all moves from the start square to find which are allowed or not for gui visualization
			foreach (Move m in LegalMoves[start])
			{
				//save the current board state before starting move
				MoveHistory.Push(new BoardState(this));
				if (m.EnPassant)
				{
					//manually remove the pawn since it's not on its target square
					if (Turn == Player.White) Board.RemovePiece(Piece.Pawn, opponent, (Sq)((int)m.End + 8));
					else Board.RemovePiece(Piece.Pawn, opponent, (Sq)((int)m.End - 8));
				}
				Board.MoveBits(m.MovePiece, m.TakePiece, Turn, opponent, m.Start, m.End);
				movesToRemove.Add(Board.IsInCheck(Turn));
				UnmoveBits();
			}

			for (int i = movesToRemove.Count - 1; i >= 0; i--)
			{
				if (movesToRemove[i])
					LegalMoves[start].RemoveAt(i);
			}
		}

		override
		public string ToString()
		{
			string fen = "";
			ulong wPawn = Board.GetBitboard(Piece.Pawn, Player.White);
			ulong wKnight = Board.GetBitboard(Piece.Knight, Player.White);
			ulong wBishop = Board.GetBitboard(Piece.Bishop, Player.White);
			ulong wRook = Board.GetBitboard(Piece.Rook, Player.White);
			ulong wQueen = Board.GetBitboard(Piece.Queen, Player.White);
			ulong wKing = Board.GetBitboard(Piece.King, Player.White);
			ulong bPawn = Board.GetBitboard(Piece.Pawn, Player.Black);
			ulong bKnight = Board.GetBitboard(Piece.Knight, Player.Black);
			ulong bBishop = Board.GetBitboard(Piece.Bishop, Player.Black);
			ulong bRook = Board.GetBitboard(Piece.Rook, Player.Black);
			ulong bQueen = Board.GetBitboard(Piece.Queen, Player.Black);
			ulong bKing = Board.GetBitboard(Piece.King, Player.Black);
			ulong wOcc = wPawn | wKnight | wBishop | wRook | wQueen | wKing;
			ulong bOcc = bPawn | bKnight | bBishop | bRook | bQueen | bKing;

			int spaces = 0;

			for (int square = 0; square < 64; square++)
			{
				if (square != 0 && (square % 8 == 0))
				{
					if (spaces > 0)
					{
						fen += spaces + "";
						spaces = 0;
					}
					fen += "/";
				}
				if (BitboardController.GetBit(wOcc, square) > 0)
				{
					if (spaces > 0)
					{
						fen += spaces + "";
						spaces = 0;
					}
					if (BitboardController.GetBit(wPawn, square) > 0) fen += "P";
					else if (BitboardController.GetBit(wKnight, square) > 0) fen += "N";
					else if (BitboardController.GetBit(wBishop, square) > 0) fen += "B";
					else if (BitboardController.GetBit(wRook, square) > 0) fen += "R";
					else if (BitboardController.GetBit(wQueen, square) > 0) fen += "Q";
					else if (BitboardController.GetBit(wKing, square) > 0) fen += "K";
				}
				else if (BitboardController.GetBit(bOcc, square) > 0)
				{
					if (spaces > 0)
					{
						fen += spaces + "";
						spaces = 0;
					}
					if (BitboardController.GetBit(bPawn, square) > 0) fen += "p";
					else if (BitboardController.GetBit(bKnight, square) > 0) fen += "n";
					else if (BitboardController.GetBit(bBishop, square) > 0) fen += "b";
					else if (BitboardController.GetBit(bRook, square) > 0) fen += "r";
					else if (BitboardController.GetBit(bQueen, square) > 0) fen += "q";
					else if (BitboardController.GetBit(bKing, square) > 0) fen += "k";
				}
				else spaces++;
			}

			fen += (Turn == Player.White) ? " w " : " b ";
			if (CastleRights == Castle.None) fen += "-";
			else
			{
				if (CastleRights.HasFlag(Castle.WhiteCastle)) fen += "K";
				if (CastleRights.HasFlag(Castle.WhiteQueenCastle)) fen += "Q";
				if (CastleRights.HasFlag(Castle.BlackCastle)) fen += "k";
				if (CastleRights.HasFlag(Castle.BlackQueenCastle)) fen += "q";
			}
			if (EnPassantSquare == Sq.empty) fen += " -";
			else fen += $" {EnPassantSquare}";

			fen += $" {HalfMove} {FullMove}";

			return fen;
		}
	}
}
