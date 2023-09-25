using System;

namespace Chess.BoardManager
{
	public static class BitboardController
	{

		public static string[] squareToCoord = new string[64]{
		"a8", "b8", "c8", "d8", "e8", "f8", "g8", "h8",
		"a7", "b7", "c7", "d7", "e7", "f7", "g7", "h7",
		"a6", "b6", "c6", "d6", "e6", "f6", "g6", "h6",
		"a5", "b5", "c5", "d5", "e5", "f5", "g5", "h5",
		"a4", "b4", "c4", "d4", "e4", "f4", "g4", "h4",
		"a3", "b3", "c3", "d3", "e3", "f3", "g3", "h3",
		"a2", "b2", "c2", "d2", "e2", "f2", "g2", "h2",
		"a1", "b1", "c1", "d1", "e1", "f1", "g1", "h1"
		};

		//====================
		//Controller Functions
		//====================

		//helper methods
		public static ulong GetBit(ulong bitboard, Sq square)
		{
			return bitboard & (1UL << (int)square);
		}


		public static ulong GetBit(ulong bitboard, int square)
		{
			return bitboard & (1UL << square);
		}

		public static ulong SetBit(ulong bitboard, Sq square)
		{
			return bitboard |= (1UL << (int)square);
		}


		public static ulong SetBit(ulong bitboard, int square)
		{
			return bitboard |= (1UL << square);
		}

		public static ulong PopBit(ulong bitboard, Sq square)
		{
			if (GetBit(bitboard, square) == 0)
				return bitboard;

			return bitboard ^= (1UL << (int)square);
		}

		public static ulong PopBit(ulong bitboard, int square)
		{
			if (GetBit(bitboard, square) == 0)
				return bitboard;

			return bitboard ^= (1UL << square);
		}

		public static int CountBit(ulong bitboard)
		{
			int count = 0;
			while (bitboard != 0)
			{
				count++;
				bitboard &= bitboard - 1;
			}

			return count;
		}

		public static int GetLSBitIndex(ulong bitboard)
		{
			if (bitboard == 0)
				return -1;
			return CountBit((ulong)((long)bitboard & -(long)bitboard) - 1);
		}

		//==============
		//	  Attacks
		//==============

		//constants for finding flank files
		public const ulong aFile = 0x0101010101010101;
		public const ulong abFile = 0x0303030303030303;
		public const ulong hFile = 0x8080808080808080;
		public const ulong hgFile = 0xc0c0c0c0c0c0c0c0;

		//constants for finding important ranks
		public const ulong firstRank = 0xFF00000000000000;
		public const ulong secondRank = 0x00FF000000000000;
		public const ulong seventhRank = 0x000000000000FF00;
		public const ulong eighthRank = 0x00000000000000FF;

		//occupancy square bit count arrays
		public static readonly int[] arrBishopRelevantOccupancyBits = new int[64] {6, 5, 5, 5, 5, 5, 5, 6,
																		5, 5, 5, 5, 5, 5, 5, 5,
																		5, 5, 7, 7, 7, 7, 5, 5,
																		5, 5, 7, 9, 9, 7, 5, 5,
																		5, 5, 7, 9, 9, 7, 5, 5,
																		5, 5, 7, 7, 7, 7, 5, 5,
																		5, 5, 5, 5, 5, 5, 5, 5,
																		6, 5, 5, 5, 5, 5, 5, 6};
		public static readonly int[] arrRookRelevantOccupancyBits = new int[64] {12, 11, 11, 11, 11, 11, 11, 12,
																	11, 10, 10, 10, 10, 10, 10, 11,
																	11, 10, 10, 10, 10, 10, 10, 11,
																	11, 10, 10, 10, 10, 10, 10, 11,
																	11, 10, 10, 10, 10, 10, 10, 11,
																	11, 10, 10, 10, 10, 10, 10, 11,
																	11, 10, 10, 10, 10, 10, 10, 11,
																	12, 11, 11, 11, 11, 11, 11, 12,};

		//pre-computed magic numbers for bishop and rook attacks
		public static readonly ulong[] arrBishopMagicNumbers = new ulong[64] {0x4240020404002040,
																			0x124080210421000,
																			0x90246140C01000,
																			0x1020A020A102044,
																			0x101104000172000,
																			0x821040081400,
																			0x1C00880110900080,
																			0x80045086100E2000,
																			0x221210103020C480,
																			0x8100041018811501,
																			0x1004106C00822040,
																			0x200C2408900200,
																			0x232061114008028A,
																			0x20202204800,
																			0x8203448801492000,
																			0x804804644300800,
																			0x1040040408C80921,
																			0x732041102082908,
																			0x4004041002100,
																			0x1058000082024080,
																			0x84004A10140402,
																			0x3082000410540400,
																			0x808800908011001,
																			0x800211108821002,
																			0x202004040852C1,
																			0x411102021240100,
																			0x104010082020401,
																			0x8080086202020,
																			0x151001011004001,
																			0x1048002082008408,
																			0x202089800441010,
																			0x102020000904100,
																			0x2A02000440820,
																			0x10248020A841030,
																			0x4002420080240,
																			0xE400040400880210,
																			0x8808020400481010,
																			0x900880010084,
																			0x80818408A49CA0A,
																			0x5800841100018480,
																			0x81138A1026840,
																			0x404040202604800,
																			0x1A010041210804,
																			0x880110141006802,
																			0x1040102102400,
																			0x4000A085000880,
																			0x600424040C100040,
																			0x4002040102000030,
																			0x784008210900004,
																			0x2040422804022010,
																			0x20844044500008,
																			0x12544842020000,
																			0x4000201002020824,
																			0x800200481120001,
																			0x1010043024004020,
																			0x4120220486088450,
																			0x2020104108084080,
																			0x4B08004202012100,
																			0x400821080818,
																			0x2805000941403,
																			0x4000100204104401,
																			0xCB0441022C0D00,
																			0x1800086004840850,
																			0x202620806040040};
		public static readonly ulong[] arrRookMagicNumbers = new ulong[64] {0x3001120C1800100,
																			0x8040004020001001,
																			0x200084082001023,
																			0x100080420100100,
																			0x1200040200100820,
																			0x4080020004008001,
																			0x4200020001409824,
																			0x200008021040042,
																			0x40800023401980,
																			0x401000402002,
																			0x408802000831000,
																			0x8001001001000823,
																			0xB10004C8001100,
																			0x3402000502008810,
																			0x808001000200,
																			0x816800844800100,
																			0x808000400022,
																			0x820041020020,
																			0x80808020001000,
																			0xC8008008100080,
																			0x1001010008001004,
																			0x40808002000400,
																			0x82440001420830,
																			0x4001020001008044,
																			0x1200C00180008020,
																			0x200440100440,
																			0x5010040220080020,
																			0x809A004200100820,
																			0xC002000A00200410,
																			0x2000020080040080,
																			0x9000010400100208,
																			0x2E004200010084,
																			0x240400080800020,
																			0x11021082002040,
																			0x804200288801000,
																			0x1000800802801002,
																			0x1025005000800,
																			0xC2B0800200800400,
																			0x4010804000210,
																			0x1A2440082000041,
																			0x4840004084248008,
																			0x510408102020020,
																			0x100020008080,
																			0xA00040200A020010,
																			0x8080040801010010,
																			0x2006000910020004,
																			0x6044428110040008,
																			0x4210080420004,
																			0x2480028100402300,
																			0x2004090210200,
																			0x408410220200,
																			0x80100200C100100,
																			0x4042800402080080,
																			0x9009002400024900,
																			0x4838900801024400,
																			0x9000047081040200,
																			0x444110028408001,
																			0x242002415008042,
																			0x220004103105821,
																			0x1423201000080501,
																			0x202001804106006,
																			0x841000804000201,
																			0x208011000C4,
																			0x2004021088412};

		//jump/single move piece attack tables, (Pawn: [side, square])
		public static ulong[,] arrPawnAttacks = new ulong[2, 64];
		public static ulong[] arrKnightAttacks = new ulong[64];
		public static ulong[] arrKingAttacks = new ulong[64];

		//attack masks for sliding pieces
		public static ulong[] arrBishopMask = new ulong[64];
		public static ulong[] arrRookMask = new ulong[64];

		//sliding piece attack tables [square, occupancy]
		public static ulong[,] arrBishopAttacks = new ulong[64, 512];
		public static ulong[,] arrRookAttacks = new ulong[64, 4096];

		public static ulong PawnAttackMask(int side, int square)
		{
			//result attacks bitboard
			ulong attacks = 0UL;

			//piece bitboard
			ulong bitboard = 0UL;

			//set piece
			bitboard = SetBit(bitboard, square);

			//only generate attacks for both sides if the pawn is not on the A or H rank to avoid looping around
			//white pawns
			if (side == 0)
			{
				if ((bitboard & aFile) == 0)
					attacks |= bitboard >> 9;

				if ((bitboard & hFile) == 0)
					attacks |= bitboard >> 7;
			}
			//black pawns
			else
			{
				if ((bitboard & aFile) == 0)
					attacks |= bitboard << 7;

				if ((bitboard & hFile) == 0)
					attacks |= bitboard << 9;
			}

			return attacks;
		}

		public static ulong KingAttackMask(int square)
		{
			//result attacks bitboard
			ulong attacks = 0UL;

			//piece bitboard
			ulong bitboard = 0UL;

			bitboard = SetBit(bitboard, square);

			//check the file to make sure things don't loop around, offboard ranks will be discarded automatically
			if ((bitboard & aFile) == 0)
			{
				attacks |= bitboard >> 9;
				attacks |= bitboard >> 1;
				attacks |= bitboard << 7;
			}

			if ((bitboard & hFile) == 0)
			{
				attacks |= bitboard >> 7;
				attacks |= bitboard << 1;
				attacks |= bitboard << 9;
			}

			attacks |= bitboard << 8;
			attacks |= bitboard >> 8;

			return attacks;
		}

		public static ulong KnightAttackMask(int square)
		{
			//result attacks bitboard
			ulong attacks = 0UL;

			//piece bitboard
			ulong bitboard = 0UL;

			bitboard = SetBit(bitboard, square);

			//check the file to make sure things don't loop around, offboard ranks will be discarded automatically
			if ((bitboard & aFile) == 0)
			{
				attacks |= bitboard >> 17;
				attacks |= bitboard << 15;
			}

			if ((bitboard & abFile) == 0)
			{
				attacks |= bitboard >> 10;
				attacks |= bitboard << 6;
			}

			if ((bitboard & hFile) == 0)
			{
				attacks |= bitboard >> 15;
				attacks |= bitboard << 17;
			}

			if ((bitboard & hgFile) == 0)
			{
				attacks |= bitboard >> 6;
				attacks |= bitboard << 10;

			}

			return attacks;
		}

		public static ulong BishopRelevantOccupancyMask(int square)
		{
			//result occupancy squares bitboard
			ulong occSquares = 0UL;

			int file, rank;

			int targetFile = square % 8;
			int targetRank = square / 8;

			//mask bishop relevant occupancy squares bottom right
			for (rank = targetRank + 1, file = targetFile + 1; rank <= 6 && file <= 6; rank++, file++)
				occSquares |= (1UL << (rank * 8 + file));

			//mask bishop relevant occupancy squares bottom left
			for (rank = targetRank + 1, file = targetFile - 1; rank <= 6 && file >= 1; rank++, file--)
				occSquares |= (1UL << (rank * 8 + file));

			//mask bishop relevant occupancy squares top left
			for (rank = targetRank - 1, file = targetFile - 1; rank >= 1 && file >= 1; rank--, file--)
				occSquares |= (1UL << (rank * 8 + file));

			//mask bishop relevant occupancy squares top left
			for (rank = targetRank - 1, file = targetFile + 1; rank >= 1 && file <= 6; rank--, file++)
				occSquares |= (1UL << (rank * 8 + file));

			return occSquares;
		}

		public static ulong BishopAttackMask(int square, ulong blockboard)
		{
			//result attacks bitboard
			ulong attacks = 0UL;
			ulong block = 1UL;

			int file, rank;

			int targetFile = square % 8;
			int targetRank = square / 8;

			//mask bishop attacks bottom right
			for (rank = targetRank + 1, file = targetFile + 1; rank <= 7 && file <= 7; rank++, file++)
			{
				attacks |= (1UL << (rank * 8 + file));
				block = GetBit(blockboard, (rank * 8 + file));
				if ((block & attacks) > 1)
					break;
			}

			//mask bishop attacks bottom left
			for (rank = targetRank + 1, file = targetFile - 1; rank <= 7 && file >= 0; rank++, file--)
			{
				attacks |= (1UL << (rank * 8 + file));
				block = GetBit(blockboard, (rank * 8 + file));
				if ((block & attacks) > 1)
					break;
			}

			//mask bishop attacks top left
			for (rank = targetRank - 1, file = targetFile - 1; rank >= 0 && file >= 0; rank--, file--)
			{
				attacks |= (1UL << (rank * 8 + file));
				block = GetBit(blockboard, (rank * 8 + file));
				if ((block & attacks) > 1)
					break;
			}

			//mask bishop attacks top left
			for (rank = targetRank - 1, file = targetFile + 1; rank >= 0 && file <= 7; rank--, file++)
			{
				attacks |= (1UL << (rank * 8 + file));
				block = GetBit(blockboard, (rank * 8 + file));
				if ((block & attacks) > 1)
					break;
			}

			return attacks;
		}

		public static ulong RookRelevantOccupancyMask(int square)
		{
			//result occupancy squares bitboard
			ulong occSquares = 0UL;

			int file, rank;

			int targetFile = square % 8;
			int targetRank = square / 8;

			//mask rook relevant occupancy squares bottom
			for (rank = targetRank + 1; rank <= 6; rank++)
				occSquares |= (1UL << (rank * 8 + targetFile));

			//mask rook relevant occupancy squares top
			for (rank = targetRank - 1; rank >= 1; rank--)
				occSquares |= (1UL << (rank * 8 + targetFile));

			//mask rook relevant occupancy squares left
			for (file = targetFile - 1; file >= 1; file--)
				occSquares |= (1UL << (targetRank * 8 + file));

			//mask rook relevant occupancy squares right
			for (file = targetFile + 1; file <= 6; file++)
				occSquares |= (1UL << (targetRank * 8 + file));

			return occSquares;
		}

		public static ulong RookAttackMask(int square, ulong blockboard)
		{
			//result attacks bitboard
			ulong attacks = 0UL;
			ulong block;

			int file, rank;

			int targetFile = square % 8;
			int targetRank = square / 8;

			//mask rook attacks bottom
			for (rank = targetRank + 1; rank <= 7; rank++)
			{
				attacks |= (1UL << (rank * 8 + targetFile));
				block = GetBit(blockboard, (rank * 8 + targetFile));
				if ((block & attacks) > 1)
					break;
			}

			//mask rook attacks top
			for (rank = targetRank - 1; rank >= 0; rank--)
			{
				attacks |= (1UL << (rank * 8 + targetFile));
				block = GetBit(blockboard, (rank * 8 + targetFile));
				if ((block & attacks) > 1)
					break;
			}

			//mask rook attacks left
			for (file = targetFile - 1; file >= 0; file--)
			{
				attacks |= (1UL << (targetRank * 8 + file));
				block = GetBit(blockboard, (targetRank * 8 + file));
				if ((block & attacks) > 1)
					break;
			}

			//mask rook attacks right
			for (file = targetFile + 1; file <= 7; file++)
			{
				attacks |= (1UL << (targetRank * 8 + file));
				block = GetBit(blockboard, (targetRank * 8 + file));
				if ((block & attacks) > 1)
					break;
			}

			return attacks;
		}

		public static ulong SetOccupancy(int index, int activeBits, ulong attackMask)
		{
			//init occ map
			ulong occupancy = 0UL;

			for (int i = 0; i < activeBits; i++)
			{
				//get index of LS1Bit
				int square = GetLSBitIndex(attackMask);

				//pop LS1Bit
				attackMask = PopBit(attackMask, square);

				//make sure occupancy is on board
				if ((index & (1 << i)) > 0)
				{
					occupancy |= (1UL << square);
				}
			}

			return occupancy;
		}


		public static void InitializeAttacks()
		{
			Console.WriteLine("Precalculating all attacks");
			for (int square = 0; square < 64; square++)
			{
				//jumping and single attackers
				arrPawnAttacks[0, square] = PawnAttackMask(0, square);
				arrPawnAttacks[1, square] = PawnAttackMask(1, square);
				arrKnightAttacks[square] = KnightAttackMask(square);
				arrKingAttacks[square] = KingAttackMask(square);

				//sliders
				arrBishopMask[square] = BishopRelevantOccupancyMask(square);
				arrRookMask[square] = RookRelevantOccupancyMask(square);

				ulong bishopAttackMask = arrBishopMask[square];
				ulong rookAttackMask = arrRookMask[square];

				int bishopRelevantBits = CountBit(bishopAttackMask);
				int rookRelevantBits = CountBit(rookAttackMask);

				int bishopOccIndex = 1 << bishopRelevantBits;
				int rookOccIndex = 1 << rookRelevantBits;

				//bishop
				for (int i = 0; i < bishopOccIndex; i++)
				{
					//occupancy variation
					ulong occ = SetOccupancy(i, bishopRelevantBits, bishopAttackMask);
					int magicIndex = (int)(((occ * arrBishopMagicNumbers[square]) & 0xFFFFFFFFFFFFFFFF) >> (64 - arrBishopRelevantOccupancyBits[square]));

					arrBishopAttacks[square, magicIndex] = BishopAttackMask(square, occ);
				}

				//rook
				for (int i = 0; i < rookOccIndex; i++)
				{
					//occupancy variation
					ulong occ = SetOccupancy(i, rookRelevantBits, rookAttackMask);
					int magicIndex = (int)(((occ * arrRookMagicNumbers[square]) & 0xFFFFFFFFFFFFFFFF) >> (64 - arrRookRelevantOccupancyBits[square]));

					arrRookAttacks[square, magicIndex] = RookAttackMask(square, occ);
				}
			}
		}


		public static ulong GetBishopAttack(int square, ulong occupancy)
		{
			//get bishop attacks assuming current board occupancy
			occupancy &= arrBishopMask[square];
			occupancy *= arrBishopMagicNumbers[square];
			occupancy >>= (64 - arrBishopRelevantOccupancyBits[square]);

			return arrBishopAttacks[square, occupancy];
		}

		public static ulong GetRookAttack(int square, ulong occupancy)
		{
			//get rook attacks assuming current board occupancy
			occupancy &= arrRookMask[square];
			occupancy *= arrRookMagicNumbers[square];
			occupancy >>= (64 - arrRookRelevantOccupancyBits[square]);

			return arrRookAttacks[square, occupancy];
		}

		public static ulong GetQueenAttack(int square, ulong occupancy)
		{
			//get queen attacks by combining rook and bishop attacks
			return GetRookAttack(square, occupancy) | GetBishopAttack(square, occupancy);
		}

		//print bitboard
		public static void printBitboard(UInt64 bitboard)
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
					if ((GetBit(bitboard, square) > 0))
					{
						Console.ForegroundColor = ConsoleColor.Cyan;
						Console.Write(" 1");
						Console.ForegroundColor = ConsoleColor.White;
					}
					else
					{
						Console.ForegroundColor = ConsoleColor.DarkGray;
						Console.Write(" 0");
						Console.ForegroundColor = ConsoleColor.White;
					}
				}
				//print new line on ranks
				Console.WriteLine();
			}

			Console.ForegroundColor = ConsoleColor.DarkRed;
			Console.WriteLine("     a b c d e f g h");
			Console.ForegroundColor = ConsoleColor.White;

			Console.WriteLine($"       Bitboard: {bitboard}");
		}

		public static void printBitboard(UInt64 bitboard, int inputSquare)
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

					if ((rank * 8 + file) == inputSquare)
					{
						Console.ForegroundColor = ConsoleColor.Yellow;
						Console.Write(" *");
						Console.ForegroundColor = ConsoleColor.White;
						continue;
					}
					//convert into square index
					int square = rank * 8 + file;
					if ((GetBit(bitboard, square) > 0))
					{
						Console.ForegroundColor = ConsoleColor.Cyan;
						Console.Write(" 1");
						Console.ForegroundColor = ConsoleColor.White;
					}
					else
					{
						Console.ForegroundColor = ConsoleColor.DarkGray;
						Console.Write(" 0");
						Console.ForegroundColor = ConsoleColor.White;
					}
				}
				//print new line on ranks
				Console.WriteLine();
			}

			Console.ForegroundColor = ConsoleColor.DarkRed;
			Console.WriteLine("     a b c d e f g h");
			Console.ForegroundColor = ConsoleColor.White;

			Console.WriteLine($"       Bitboard: {bitboard}");
		}

		public static void printBitboard(UInt64 bitboard, int inputSquare, Piece piece)
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

					if ((rank * 8 + file) == inputSquare)
					{
						Console.ForegroundColor = ConsoleColor.Yellow;
						switch (piece)
						{
							case Piece.Pawn: Console.Write(" P"); break;
							case Piece.Knight: Console.Write(" K"); break;
							case Piece.Bishop: Console.Write(" B"); break;
							case Piece.Rook: Console.Write(" R"); break;
							case Piece.Queen: Console.Write(" Q"); break;
							case Piece.King: Console.Write(" K"); break;
						}
						Console.ForegroundColor = ConsoleColor.White;
						continue;
					}
					//convert into square index
					int square = rank * 8 + file;
					if ((GetBit(bitboard, square) > 0))
					{
						Console.ForegroundColor = ConsoleColor.Cyan;
						Console.Write(" 1");
						Console.ForegroundColor = ConsoleColor.White;
					}
					else
					{
						Console.ForegroundColor = ConsoleColor.DarkGray;
						Console.Write(" 0");
						Console.ForegroundColor = ConsoleColor.White;
					}
				}
				//print new line on ranks
				Console.WriteLine();
			}

			Console.ForegroundColor = ConsoleColor.DarkRed;
			Console.WriteLine("     a b c d e f g h");
			Console.ForegroundColor = ConsoleColor.White;

			Console.WriteLine($"       Bitboard: {bitboard}");
		}

		public static string PrintBits(ulong bitboard)
		{
			string output = "";

			for (int i = 0; i < 64; i++)
			{
				if ((bitboard & (1UL << i)) == 0)
					output += "0";
				else
					output += "1";
			}

			return output;
		}

		//=======================
		//		Magic Numbers
		//=======================

		// public static uint state = 298457653;

		// public static uint GetRandomUInt()
		// {
		// 	uint randNum = state;

		// 	//XOR Shift
		// 	randNum ^= randNum << 13;
		// 	randNum ^= randNum >> 17;
		// 	randNum ^= randNum << 5;

		// 	state = randNum;

		// 	return randNum;
		// }

		// public static ulong GetRandomULong()
		// {
		// 	ulong n1, n2, n3, n4;

		// 	n1 = (ulong)(GetRandomUInt() & 0xFFFF);
		// 	n2 = (ulong)(GetRandomUInt() & 0xFFFF);
		// 	n3 = (ulong)(GetRandomUInt() & 0xFFFF);
		// 	n4 = (ulong)(GetRandomUInt() & 0xFFFF);

		// 	return n1 | (n2 << 16) | (n3 << 32) | (n4 << 48);
		// }

		// public static ulong GenerateMagicNumber()
		// {
		// 	return GetRandomULong() & GetRandomULong() & GetRandomULong();
		// }

		// public static ulong FindMagicNumber(int square, int relevantBits, Piece p)
		// {
		// 	ulong[] occ = new ulong[4096];
		// 	ulong[] attacks = new ulong[4096];
		// 	ulong[] usedAttacks = new ulong[4096];

		// 	ulong attackMask = (p == Piece.Bishop) ? BishopRelevantOccupancyMask(square) : RookRelevantOccupancyMask(square);

		// 	int occ_index = 1 << relevantBits;

		// 	for (int i = 0; i < occ_index; i++)
		// 	{
		// 		occ[i] = SetOccupancy(i, relevantBits, attackMask);
		// 		attacks[i] = (p == Piece.Bishop) ? BishopAttackMask(square, occ[i]) : RookAttackMask(square, occ[i]);
		// 	}

		// 	//test
		// 	for (int i = 0; i < 100000000; i++)
		// 	{
		// 		//generate candidate
		// 		ulong magicNum = GenerateMagicNumber();

		// 		//skip bad nums
		// 		if (CountBit((attackMask * magicNum) & 0xFF00000000000000) < 6) continue;

		// 		//init used attacks
		// 		usedAttacks = Enumerable.Repeat(0UL, 4096).ToArray();

		// 		//init index and fail flag
		// 		int index;
		// 		bool fail;

		// 		//test magic index
		// 		for (index = 0, fail = false; !fail && index < occ_index; index++)
		// 		{
		// 			uint magicIndex = (uint)(((occ[index] * magicNum) & 0xFFFFFFFFFFFFFFFF) >> (64 - relevantBits));

		// 			if (usedAttacks[magicIndex] == 0UL)
		// 			{
		// 				usedAttacks[magicIndex] = attacks[index];
		// 			}
		// 			else if (usedAttacks[magicIndex] != attacks[index])
		// 			{
		// 				fail = true;
		// 			}
		// 		}

		// 		if (!fail)
		// 			return magicNum;
		// 	}

		// 	Console.WriteLine("Failed magic numbers");

		// 	return 0UL;
		// }

		// public static void InitializeMagicNums()
		// {
		// 	Console.WriteLine("Rooks:");
		// 	Console.WriteLine("===================");
		// 	for (int i = 0; i < 64; i++)
		// 	{
		// 		ulong temp = FindMagicNumber(i, arrRookRelevantOccupancyBits[i], Piece.Rook);
		// 		Console.WriteLine(string.Format("0x{0:X},", temp));
		// 	}

		// 	Console.WriteLine("\n\nBishops:");
		// 	Console.WriteLine("===================");

		// 	for (int i = 0; i < 64; i++)
		// 	{
		// 		ulong temp = FindMagicNumber(i, arrBishopRelevantOccupancyBits[i], Piece.Bishop);
		// 		Console.WriteLine(string.Format("0x{0:X},", temp));

		// 	}
		// }
	}
}
