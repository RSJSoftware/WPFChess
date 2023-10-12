using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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
		public List<Move> LegalMoves { get; set; }
		public Stack<BoardState> MoveHistory { get; set; }
		public List<string> MoveList { get; set; }
		public GameState State { get; set; }


		public BoardState() : this(INITIALPOS)
		{
		}

		public BoardState(string fen)
		{
			Board = new Bitboard();
			MoveHistory = new Stack<BoardState>();
			MoveList = new List<string>();
			ReadFen(fen);
			LegalMoves = Board.GetMoveDictionary(Turn, EnPassantSquare, CastleRights);
			State = CurrentState();
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

			//if a rook was taken, see if the castling rights need to be removed
			if (takePiece == Piece.Rook && CastleRights != Castle.None)
			{
				if (Turn == Player.Black && move.End == Sq.h1 && CastleRights.HasFlag(Castle.WhiteCastle))
					CastleRights ^= Castle.WhiteCastle;
				else if (Turn == Player.Black && move.End == Sq.a1 && CastleRights.HasFlag(Castle.WhiteQueenCastle))
					CastleRights ^= Castle.WhiteQueenCastle;
				else if (Turn == Player.White && move.End == Sq.h8 && CastleRights.HasFlag(Castle.BlackCastle))
					CastleRights ^= Castle.BlackCastle;
				else if (Turn == Player.White && move.End == Sq.a8 && CastleRights.HasFlag(Castle.BlackQueenCastle))
					CastleRights ^= Castle.BlackQueenCastle;
			}

			//change turns and calculate new moves
			ChangeTurn();

			//see if another piece of the same type can move to the same square for naming convention purposes
			string canOtherMove = CheckOtherPieceMove(movePiece, move.Start, move.End);

			LegalMoves = Board.GetMoveDictionary(Turn, EnPassantSquare, CastleRights);

			//check mate
			State = CurrentState();

			if (moveName != "")
			{
				if (State == GameState.Checkmate)
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
			//find legal move in the list, printing an error if it doesn't exist
			Move move = null;
			ulong possibleMoves = 0UL;
			foreach (Move m in LegalMoves)
			{
				if (m.Start == start && m.End == end && m.PromotePiece == promotePiece)
				{
					move = m;
				}
				possibleMoves |= (ulong)m.End;
			}

			if (move == null)
			{
				Console.WriteLine("ERROR: Move " + start + "" + end + " Does not exist.");
				return false;
			}

			return MovePiece(move);
		}

		private string CheckOtherPieceMove(Piece movePiece, Sq start, Sq end)
		{
			//pawns don't have ambiguous move notation
			if (movePiece == Piece.Pawn)
				return "";
			string otherPiece = "";
			bool sameRank = false;
			bool sameFile = false;
			int moveCount = 0;

			//find and cache all moves that are by the same type of piece that can end on the same square as the original that are not the original
			foreach (Move m in LegalMoves)
			{
				if (m.End == end && m.MovePiece == movePiece && m.Start != start)
				{
					//find if the piece is on the same file or rank
					if (((int)start % 8) == ((int)m.Start % 8))
						sameFile = true;
					else if (((int)start / 8) == ((int)m.Start / 8))
						sameRank = true;

					moveCount++;
				}
			}

			//return the whole start coord if other pieces are on the same file and rank
			if (sameFile && sameRank)
				otherPiece += BitboardController.squareToCoord[(int)start];
			//return just the rank if other piece is on same file and none on same rank
			else if (sameFile)
				otherPiece += BitboardController.squareToCoord[(int)start][1];
			//return just file if there are other pieces otherwise
			else if (moveCount > 0)
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
			if (move.TakePiece != Piece.None || move.EnPassant)
			{
				if (move.MovePiece != Piece.Pawn)
					moveName += "x";
				else
					moveName += BitboardController.squareToCoord[(int)move.Start][0] + "x";
			}

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
			if (State == GameState.Checkmate)
				moveName += "#";
			else if (Board.IsInCheck(opponent))
				moveName += "+";

			MoveList.Add(moveName);
		}

		public void UnmoveBits()
		{
			if (MoveHistory.Count == 0)
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

		public GameState CurrentState()
		{
			//TODO add checks for draw by repetation

			//if both sides have insufficient material, game state is a draw
			if (Board.IsInsufficientMaterial(Player.White) && Board.IsInsufficientMaterial(Player.Black))
				return GameState.Insufficient;

			TestMoves();
			//if there are still legal moves left the game state is either continuing playing or draw by a technicality
			if (LegalMoves.Count > 0)
			{
				if (HalfMove > 100)
					return GameState.MoveCount;
			}
			//otherwise it's checkmate if the king is in check or stalemate if the king is not in check
			else
			{
				if (Board.IsInCheck(Turn))
					return GameState.Checkmate;
				else
					return GameState.Stalemate;
			}

			return GameState.Playing;

		}
		//test all moves from the start square to find which are allowed or not for gui visualization
		public void TestMoves()
		{
			Player opponent = (Turn == Player.White) ? Player.Black : Player.White;
			List<bool> movesToRemove = new List<bool>();

			//test all moves from the start square to find which are allowed or not for gui visualization
			foreach (Move m in LegalMoves)
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
					LegalMoves.RemoveAt(i);
			}
		}

		public void ReadFen(string fen)
		{
			//regex pattern for a full FEN code including piece placement, side to move, castling ability, en passant target square, halfmove clock, and full move counter
			string basePattern = @"(^([1-8pnbrqk]{1,8}/){7}([1-8pnbrqk]{1,8}){1})";
			string fullPattern = @"(^([1-8pnbrqk]{1,8}/){7}([1-8pnbrqk]{1,8}){1}(\s[w|b]{1})(\s(([-]{1})|([kq]{1,4})){1})((\s(([-]{1})|[a-h]{1}[1-8]{1})){1})((\s\d{1,3}){2})+$)";

			Regex basefenrg = new Regex(basePattern, RegexOptions.IgnoreCase);
			Regex fullfenrg = new Regex(fullPattern, RegexOptions.IgnoreCase);

			//immediately return if the fen doesn't match the base pattern
			if (!basefenrg.IsMatch(fen))
			{
				Console.WriteLine("Invalid FEN pattern: " + fen);
				return;
			}

			//split fen into 6 parts
			string[] parts = fen.Split(' ');

			ReadBaseFen(parts[0]);

			//if the passed fen was a full fen, parse the rest of it for variables
			if (fullfenrg.IsMatch(fen))
			{
				ReadPlayerFen(parts[1]);
				ReadCastleFen(parts[2]);
				ReadEnPassFen(parts[3]);
				ReadMovesFen(parts[4], parts[5]);
				LegalMoves = Board.GetMoveDictionary(Turn, EnPassantSquare, CastleRights);
				MoveList.Clear();
				MoveHistory.Clear();
				State = CurrentState();
				return;
			}

			//otherwise initialize all variables
			Turn = Player.White;
			EnPassantSquare = Sq.empty;
			FullMove = 0;
			HalfMove = 0;

			//set default castle rights based on king and rook positions
			CastleRights = Castle.None;
			if (BitboardController.GetBit(Board.GetBitboard(Piece.King, Player.White), Sq.e1) > 0)
			{
				if (BitboardController.GetBit(Board.GetBitboard(Piece.Rook, Player.White), Sq.a1) > 0)
				{
					CastleRights |= Castle.WhiteQueenCastle;
				}
				if (BitboardController.GetBit(Board.GetBitboard(Piece.Rook, Player.White), Sq.h1) > 0)
				{
					CastleRights |= Castle.WhiteCastle;
				}
			}

			if (BitboardController.GetBit(Board.GetBitboard(Piece.King, Player.Black), Sq.e8) > 0)
			{
				if (BitboardController.GetBit(Board.GetBitboard(Piece.Rook, Player.Black), Sq.a8) > 0)
				{
					CastleRights |= Castle.BlackQueenCastle;
				}
				if (BitboardController.GetBit(Board.GetBitboard(Piece.Rook, Player.Black), Sq.h8) > 0)
				{
					CastleRights |= Castle.BlackCastle;
				}
			}

			//get legal moves
			LegalMoves = Board.GetMoveDictionary(Turn, EnPassantSquare, CastleRights);
			MoveList.Clear();
			MoveHistory.Clear();
			State = CurrentState();
		}

		private void ReadBaseFen(string fen)
		{
			//1. make sure the piece placement part of the fen has only 2 kings
			int whiteKing = CharCount(fen, 'K');
			int blackKing = CharCount(fen, 'k');
			if (whiteKing != 1 || blackKing != 1)
			{
				Console.WriteLine("Invalid number of kings: " + fen);
				return;
			}

			//split the piece placement part of the fen into 8 rows
			string[] rows = fen.Split('/');

			int col;

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
		}

		private void ReadPlayerFen(string fen)
		{

			//find the active player
			if (fen.Equals("w"))
				Turn = Player.White;
			else if (fen.Equals("b"))
				Turn = Player.Black;
			else
			{
				//sanity check
				Turn = Player.White;
				Console.WriteLine("Invalid FEN at player: " + fen);
			}
		}

		private void ReadCastleFen(string fen)
		{

			//if there are still castling rights, make sure each character is used 0 or 1 times only
			int whiteKing = 0;
			int blackKing = 0;
			int whiteQueen = 0;
			int blackQueen = 0;
			if (!fen.Equals("-"))
			{
				whiteKing = CharCount(fen, 'K');
				blackKing = CharCount(fen, 'k');
				whiteQueen = CharCount(fen, 'Q');
				blackQueen = CharCount(fen, 'q');
				if (whiteKing > 1 | blackKing > 1 | whiteQueen > 1 | blackQueen > 1)
				{
					CastleRights = Castle.None;

					//set default castle rights based on king and rook positions
					if (BitboardController.GetBit(Board.GetBitboard(Piece.King, Player.White), Sq.e1) > 0)
					{
						if (BitboardController.GetBit(Board.GetBitboard(Piece.Rook, Player.White), Sq.a1) > 0)
						{
							CastleRights |= Castle.WhiteQueenCastle;
						}
						if (BitboardController.GetBit(Board.GetBitboard(Piece.Rook, Player.White), Sq.h1) > 0)
						{
							CastleRights |= Castle.WhiteCastle;
						}
					}

					if (BitboardController.GetBit(Board.GetBitboard(Piece.King, Player.Black), Sq.e8) > 0)
					{
						if (BitboardController.GetBit(Board.GetBitboard(Piece.Rook, Player.Black), Sq.a8) > 0)
						{
							CastleRights |= Castle.BlackQueenCastle;
						}
						if (BitboardController.GetBit(Board.GetBitboard(Piece.Rook, Player.Black), Sq.h8) > 0)
						{
							CastleRights |= Castle.BlackCastle;
						}
					}
					Console.WriteLine("Invalid castling rights: " + fen);
					return;
				}
			}

			//find castling rights, perform checks using previously defined variables if castling phrase is not '-'
			CastleRights = Castle.None;
			if (!fen.Equals("-"))
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
		}

		private void ReadEnPassFen(string fen)
		{
			//find en passant square, set to none if phrase is '-'
			if (fen.Equals("-"))
				EnPassantSquare = Sq.empty;
			else
			{
				//check to make sure the enum exists and parse it into the en passant square
				if (Enum.IsDefined(typeof(Sq), fen))
					EnPassantSquare = (Sq)Enum.Parse(typeof(Sq), fen);
				else
				{
					//sanity check
					EnPassantSquare = Sq.empty;
					Console.WriteLine("Invalid en passant square: " + fen);
				}
			}
		}

		private void ReadMovesFen(string fen1, string fen2)
		{
			//half move clock should not be greater than 100 (100 may be loaded to analyze the position) -- pattern check should ensure a positive int value
			int halfMoveClock = int.Parse(fen1);
			if (halfMoveClock > 100)
			{
				halfMoveClock = 0;
				Console.WriteLine("Invalid halfmove clock: " + halfMoveClock);
			}


			//make sure full move clock is more than 0 since initial position starts at 1 (note: longest chess game in history is 269 moves, so 4 digit check would be invalid)
			int fullMoveClock = int.Parse(fen2);
			if (fullMoveClock < 1)
			{
				fullMoveClock = 0;
				Console.WriteLine("Invalid fullmove clock: " + fullMoveClock);
			}

			//set the move timers that were set earlier
			HalfMove = halfMoveClock;
			FullMove = fullMoveClock;
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

		public void PrintBoard()
		{
			Board.PrintBoard();
			Console.WriteLine("Turn    : " + Turn);
			Console.WriteLine("EnPass  : " + EnPassantSquare);
			Console.Write($"Castle  : {(CastleRights.HasFlag(Castle.WhiteCastle) ? 'K' : '-')} {(CastleRights.HasFlag(Castle.WhiteQueenCastle) ? 'Q' : '-')}");
			Console.WriteLine($" {(CastleRights.HasFlag(Castle.BlackCastle) ? 'k' : '-')} {(CastleRights.HasFlag(Castle.BlackQueenCastle) ? 'q' : '-')}");
			Console.WriteLine();
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
