namespace Chess.BoardManager
{
	public class Move
	{
		public Sq Start { get; set; }
		public Sq End { get; set; }
		public Piece MovePiece { get; set; }
		public Piece TakePiece { get; set; }
		public Piece PromotePiece { get; set; }
		public Castle CastleMove { get; set; }
		public bool EnPassant { get; set; }

		public Move(Sq start, Sq end, Piece movePiece, Piece takePiece, Piece promotePiece, Castle castle, bool enPassant)
		{
			Start = start;
			End = end;
			MovePiece = movePiece;
			TakePiece = takePiece;
			PromotePiece = promotePiece;
			CastleMove = castle;
			EnPassant = enPassant;
		}

		public Move(Sq start, Sq end, Piece movePiece) : this(start, end, movePiece, Piece.None, Piece.None, Castle.None, false)
		{
		}

		public Move(Sq start, Sq end, Piece movePiece, Piece takePiece) : this(start, end, movePiece, takePiece, Piece.None, Castle.None, false)
		{
		}

		public Move(Sq start, Sq end, Piece movePiece, Piece takePiece, Piece promotePiece) : this(start, end, movePiece, takePiece, promotePiece, Castle.None, false)
		{
		}

		public Move(Sq start, Sq end, Piece movePiece, Piece takePiece, Castle castle) : this(start, end, movePiece, takePiece, Piece.None, castle, false)
		{
		}

		public Move(Sq start, Sq end, Piece movePiece, Castle castle) : this(start, end, movePiece, Piece.None, Piece.None, castle, false)
		{
		}

		//en passant passes in no take pieces because it's a special case
		public Move(Sq start, Sq end, Piece movePiece, bool enPassant) : this(start, end, movePiece, Piece.None, Piece.None, Castle.None, enPassant)
		{
		}

		override
		public string ToString()
		{
			return Start + "" + End;
		}
	}
}