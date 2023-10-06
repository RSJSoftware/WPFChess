using Chess.BoardManager;
using System;

namespace Chess.AI
{
     //this AI makes random moves each turn
     public class RandomAI : IChessAI
     {
          public BoardState Board { get; set; }
          private Random randomNumber;

          public RandomAI(BoardState board)
          {
               Board = board;
               randomNumber = new Random();
          }

          public int Think()
          {
               return randomNumber.Next(0, Board.LegalMoves.Count);
          }

          public bool Move(int moveIndex)
          {
               return Board.MovePiece(Board.LegalMoves[moveIndex]);
          }
     }
}
