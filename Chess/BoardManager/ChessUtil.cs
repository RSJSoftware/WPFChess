using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Chess.BoardManager
{
     //static class ChessUtil
     //{
     //     private const int SIZE = 8;
     //     public static string PrintCell(Point pos)
     //     {
     //          return (char)(pos.X + 65) + "" + (8 - pos.Y);
     //     }
     //     public static string PrintCell(Cell cell)
     //     {
     //          return PrintCell(cell.Pos);
     //     }

     //     public static Dictionary<Cell, List<Cell>> LegalMoves(BoardState boardState, Player turn)
     //     {
     //          ObservableCollection<Cell> board = boardState.board;
     //          Dictionary<Cell, List<Cell>> legalMoves = new Dictionary<Cell, List<Cell>>();

     //          for(int i = 0; i < board.Count; i++)
     //          {
     //               ChessPiece piece = board[i].Piece;
     //               List<Cell> moves = new List<Cell>();
     //               //continue if there are no pieces/no pieces of the current player on the cell
     //               if (piece == null || piece.Player != turn)
     //                    continue;

     //               switch(piece.Type){
     //                    case (PieceType.Pawn):
     //                         moves = PawnMoves(board, board[i], i);
     //                         legalMoves.Add(board[i], moves);
     //                         break;
     //                    case (PieceType.Knight):
     //                         moves = KnightMoves(board, board[i], i);
     //                         legalMoves.Add(board[i], moves);
     //                         break;
     //                    case (PieceType.Bishop):
     //                         moves = BishopMoves(board, board[i], i);
     //                         legalMoves.Add(board[i], moves);
     //                         break;
     //                    case (PieceType.Rook):
     //                         moves = RookMoves(board, board[i], i);
     //                         legalMoves.Add(board[i], moves);
     //                         break;
     //                    case (PieceType.Queen):
     //                         moves = QueenMoves(board, board[i], i);
     //                         legalMoves.Add(board[i], moves);
     //                         break;
     //                    case (PieceType.King):
     //                         moves = KingMoves(board, board[i], i);
     //                         legalMoves.Add(board[i], moves);
     //                         break;
     //                    default: Console.WriteLine("How did you get here? PieceType=" + piece.Type); break;

     //               }
     //          }

     //          return legalMoves;
     //     }

     //     private static List<Cell> PawnMoves(ObservableCollection<Cell> board, Cell cell, int index)
     //     {
     //          List<Cell> moves = new List<Cell>();
     //          Point pos = cell.Pos;
               
     //          //pawns move up the board if white, down if black
     //          if(cell.Piece.Player == Player.White)
     //          {
     //               //pawns can only move forward if there is not something directly in front of them
     //               if(board[index-SIZE].Piece == null)
     //               {

     //                    //allow 2 moves forward on first move and the cell is free
     //                    if (pos.Y == 6 && board[index - (SIZE * 2)].Piece == null)
     //                         moves.Add(board[index - (SIZE * 2)]);

     //                    //pawns move forward one square
     //                    moves.Add(board[index - (SIZE)]);
     //               }

     //               //if there are capturable pieces to the diagonals, those moves become possible
     //               if (pos.X > 0 && board[index-(SIZE-1)].Piece != null)
     //               {
     //                    if (PawnTake(cell.Piece.Player, board[index - (SIZE - 1)]))
     //                         moves.Add(board[index - (SIZE - 1)]);
     //               }
                    
     //               if(pos.X < (SIZE-2) && board[index - (SIZE + 1)].Piece != null)
     //               {
     //                    if (PawnTake(cell.Piece.Player, board[index - (SIZE + 1)]))
     //                         moves.Add(board[index - (SIZE + 1)]);
     //               }
     //          }
     //          else
     //          {
     //               if (board[index + SIZE].Piece == null)
     //               {
     //                    //allow 2 moves forward on first move
     //                    if (pos.Y == 1 && board[index + (SIZE * 2)].Piece == null)
     //                         moves.Add(board[index + (SIZE * 2)]);

     //                    //pawns move forward one square
     //                    moves.Add(board[index + (SIZE)]);

     //               }

     //               //if there are capturable pieces to the diagonals, those moves become possible
     //               if (pos.X > 0 && board[index + (SIZE - 1)].Piece != null)
     //               {
     //                    if (PawnTake(cell.Piece.Player, board[index + (SIZE - 1)]))
     //                         moves.Add(board[index + (SIZE - 1)]);
     //               }

     //               if (pos.X < (SIZE - 2) && board[index + (SIZE + 1)].Piece != null)
     //               {
     //                    if (PawnTake(cell.Piece.Player, board[index + (SIZE + 1)]))
     //                         moves.Add(board[index + (SIZE + 1)]);
     //               }

     //          }

     //          return moves;
     //     }

     //     private static List<Cell> KnightMoves(ObservableCollection<Cell> board, Cell cell, int index)
     //     {
     //          List<Cell> moves = new List<Cell>();
     //          Point pos = cell.Pos;
     //          Player turn = cell.Piece.Player;

     //          //knight moves consist of +/- 6, 10, 15, 17 for all 8 moves depending on how close to the center the knight is
     //          if (pos.X > 0)
     //          {
     //               if (pos.Y > 1)
     //               {
     //                    if (IsFree(turn, board[index - ((SIZE * 2) + 1)]) != -1)
     //                         moves.Add(board[index - ((SIZE * 2) + 1)]);
     //               }

     //               if (pos.Y < (SIZE - 2))
     //               {
     //                    if (IsFree(turn, board[index + ((SIZE * 2) - 1)]) != -1)
     //                         moves.Add(board[index + ((SIZE * 2) - 1)]);
     //               }

     //               if (pos.X > 1)
     //               {
     //                    if (pos.Y > 0)
     //                    {
     //                         if (IsFree(turn, board[index - (SIZE + 2)]) != -1)
     //                              moves.Add(board[index - (SIZE + 2)]);
     //                    }

     //                    if (pos.Y < SIZE - 1)
     //                    {
     //                         if (IsFree(turn, board[index + (SIZE - 2)]) != -1)
     //                              moves.Add(board[index + (SIZE - 2)]);
     //                    }
     //               }
     //          }

     //          if(pos.X < SIZE - 1)
     //          {
     //               if (pos.Y > 1)
     //               {
     //                    if (IsFree(turn, board[index - ((SIZE * 2) - 1)]) != -1)
     //                         moves.Add(board[index - ((SIZE * 2) - 1)]);
     //               }

     //               if (pos.Y < (SIZE - 2))
     //               {
     //                    if (IsFree(turn, board[index + ((SIZE * 2) + 1)]) != -1)
     //                         moves.Add(board[index + ((SIZE * 2) + 1)]);
     //               }

     //               if (pos.X < SIZE - 2)
     //               {
     //                    if (pos.Y > 0)
     //                    {
     //                         if (IsFree(turn, board[index - (SIZE - 2)]) != -1)
     //                              moves.Add(board[index - (SIZE - 2)]);
     //                    }

     //                    if (pos.Y < SIZE - 1)
     //                    {
     //                         if (IsFree(turn, board[index + (SIZE + 2)]) != -1)
     //                              moves.Add(board[index + (SIZE + 2)]);
     //                    }
     //               }

     //          }
     //          return moves;
     //     }

     //     private static List<Cell> BishopMoves(ObservableCollection<Cell> board, Cell cell, int index)
     //     {
     //          List<Cell> moves = new List<Cell>();
     //          Point pos = cell.Pos;
     //          Cell nextCell;
     //          int count;
     //          int isFree;
     //          Player turn = cell.Piece.Player;

     //          //up-left diag is -9
     //          if (pos.X > 0 && pos.Y > 0)
     //          {
     //               count = index;
     //               do
     //               {
     //                    nextCell = board[count - 9];

     //                    isFree = IsFree(cell.Piece.Player, nextCell);
     //                    //break if there's a piece in the next cell, adding it if it can be captured
     //                    if (isFree != 0)
     //                    {
     //                         if(isFree == 1)
     //                         {
     //                              moves.Add(nextCell);
     //                              break;
     //                         }
     //                         break;
     //                    }

     //                    moves.Add(nextCell);
     //                    count -= 9;
     //               } while (nextCell.Pos.X > 0 && nextCell.Pos.Y > 0);

     //          }

     //          //up-right diag is -7
     //          if (pos.X < SIZE - 1 && pos.Y > 0)
     //          {
     //               count = index;
     //               do
     //               {
     //                    nextCell = board[count - 7];

     //                    isFree = IsFree(cell.Piece.Player, nextCell);
     //                    //break if there's a piece in the next cell, adding it if it can be captured
     //                    if (isFree != 0)
     //                    {
     //                         if (isFree == 1)
     //                         {
     //                              moves.Add(nextCell);
     //                              break;
     //                         }
     //                         break;
     //                    }

     //                    moves.Add(nextCell);
     //                    count -= 7;
     //               } while (nextCell.Pos.X < SIZE -1 && nextCell.Pos.Y > 0);

     //          }

     //          //down-left diag is +7
     //          if (pos.X > 0 && pos.Y < SIZE - 1)
     //          {
     //               count = index;
     //               do
     //               {
     //                    nextCell = board[count + 7];

     //                    isFree = IsFree(cell.Piece.Player, nextCell);
     //                    //break if there's a piece in the next cell, adding it if it can be captured
     //                    if (isFree != 0)
     //                    {
     //                         if (isFree == 1)
     //                         {
     //                              moves.Add(nextCell);
     //                              break;
     //                         }
     //                         break;
     //                    }

     //                    moves.Add(nextCell);
     //                    count += 7;
     //               } while (nextCell.Pos.X > 0 && nextCell.Pos.Y < SIZE - 1);

     //          }

     //          //down-right diag is +9
     //          if (pos.X < SIZE - 1 && pos.Y < SIZE - 1)
     //          {
     //               count = index;
     //               do
     //               {
     //                    nextCell = board[count + 9];

     //                    isFree = IsFree(cell.Piece.Player, nextCell);
     //                    //break if there's a piece in the next cell, adding it if it can be captured
     //                    if (isFree != 0)
     //                    {
     //                         if (isFree == 1)
     //                         {
     //                              moves.Add(nextCell);
     //                              break;
     //                         }
     //                         break;
     //                    }

     //                    moves.Add(nextCell);
     //                    count += 9;
     //               } while (nextCell.Pos.X < SIZE - 1 && nextCell.Pos.Y < SIZE - 1);

     //          }

     //          return moves;
     //     }

     //     private static List<Cell> RookMoves(ObservableCollection<Cell> board, Cell cell, int index)
     //     {
     //          List<Cell> moves = new List<Cell>();
     //          Point pos = cell.Pos;
     //          Cell nextCell;
     //          int isFree;
     //          int count;

     //          if(pos.X > 0)
     //          {
     //               count = index;
     //               do
     //               {
     //                    nextCell = board[count - 1];

     //                    isFree = IsFree(cell.Piece.Player, nextCell);
     //                    //break if there's a piece in the next cell, adding it if it can be captured
     //                    if (isFree != 0)
     //                    {
     //                         if (isFree == 1)
     //                         {
     //                              moves.Add(nextCell);
     //                              break;
     //                         }
     //                         break;
     //                    }

     //                    moves.Add(nextCell);
     //                    count--;

     //               } while (nextCell.Pos.X > 0);
     //          }

     //          if(pos.X < SIZE - 1)
     //          {
     //               count = index;
     //               do
     //               {
     //                    nextCell = board[count + 1];

     //                    isFree = IsFree(cell.Piece.Player, nextCell);
     //                    //break if there's a piece in the next cell, adding it if it can be captured
     //                    if (isFree != 0)
     //                    {
     //                         if (isFree == 1)
     //                         {
     //                              moves.Add(nextCell);
     //                              break;
     //                         }
     //                         break;
     //                    }

     //                    moves.Add(nextCell);
     //                    count++;

     //               } while (nextCell.Pos.X < SIZE - 1);

     //          }

     //          if(pos.Y > 0)
     //          {
     //               count = index;
     //               do
     //               {
     //                    nextCell = board[count - SIZE];

     //                    isFree = IsFree(cell.Piece.Player, nextCell);
     //                    //break if there's a piece in the next cell, adding it if it can be captured
     //                    if (isFree != 0)
     //                    {
     //                         if (isFree == 1)
     //                         {
     //                              moves.Add(nextCell);
     //                              break;
     //                         }
     //                         break;
     //                    }

     //                    moves.Add(nextCell);
     //                    count -= SIZE;

     //               } while (nextCell.Pos.Y > 0);

     //          }

     //          if(pos.Y < SIZE - 1)
     //          {
     //               count = index;
     //               do
     //               {
     //                    nextCell = board[count + SIZE];

     //                    isFree = IsFree(cell.Piece.Player, nextCell);
     //                    //break if there's a piece in the next cell, adding it if it can be captured
     //                    if (isFree != 0)
     //                    {
     //                         if (isFree == 1)
     //                         {
     //                              moves.Add(nextCell);
     //                              break;
     //                         }
     //                         break;
     //                    }

     //                    moves.Add(nextCell);
     //                    count += SIZE;

     //               } while (nextCell.Pos.Y < SIZE - 1);

     //          }

     //          return moves;
     //     }

     //     private static List<Cell> QueenMoves(ObservableCollection<Cell> board, Cell cell, int index)
     //     {
     //          List<Cell> moves = new List<Cell>();

     //          //queen moves are just bishop and rook moves combined
     //          moves.AddRange(BishopMoves(board, cell, index));
     //          moves.AddRange(RookMoves(board, cell, index));

     //          return moves;
     //     }

     //     private static List<Cell> KingMoves(ObservableCollection<Cell> board, Cell cell, int index)
     //     {
     //          List<Cell> moves = new List<Cell>();
     //          Point pos = cell.Pos;

     //          if (pos.X > 0)
     //          {
     //               //up left
     //               if(pos.Y > 0)
     //                    if (IsFree(cell.Piece.Player, board[index - (SIZE + 1)]) != -1)
     //                         moves.Add(board[index - (SIZE + 1)]);

     //               //left
     //               if (IsFree(cell.Piece.Player, board[index - 1]) != -1)
     //                    moves.Add(board[index - 1]);

     //               //down left
     //               if (pos.Y < SIZE - 1)
     //                    if (IsFree(cell.Piece.Player, board[index + (SIZE - 1)]) != -1)
     //                         moves.Add(board[index + (SIZE - 1)]);
     //          }

     //          if(pos.X < SIZE - 1)
     //          {
     //               //up right
     //               if (pos.Y > 0)
     //                    if (IsFree(cell.Piece.Player, board[index - (SIZE - 1)]) != -1)
     //                         moves.Add(board[index - (SIZE - 1)]);

     //               //right
     //               if (IsFree(cell.Piece.Player, board[index + 1]) != -1)
     //                    moves.Add(board[index + 1]);

     //               //down right
     //               if (pos.Y < SIZE - 1)
     //                    if (IsFree(cell.Piece.Player, board[index + (SIZE + 1)]) != -1)
     //                         moves.Add(board[index + (SIZE + 1)]);

     //          }

     //          //down
     //          if (pos.Y < SIZE - 1)
     //          {
     //               if (IsFree(cell.Piece.Player, board[index + SIZE]) != -1)
     //                    moves.Add(board[index + SIZE]);
     //          }

     //          //up
     //          if (pos.Y > 0)
     //          {
     //               if (IsFree(cell.Piece.Player, board[index - SIZE]) != -1)
     //                    moves.Add(board[index - SIZE]);
     //          }

     //          return moves;
     //     }

     //     private static bool PawnTake(Player turn, Cell cell)
     //     {
     //          bool canTake = false;

     //          //only pieces of opposite color can be captured
     //          if (cell.Piece.Player != turn)
     //               canTake = true;

     //          return canTake;
     //     }

     //     private static int IsFree(Player turn, Cell cell)
     //     {
     //          //return -1 if the point is not free
     //          int isFree = -1;

     //          //return 0 if the point is empty
     //          if (cell.Piece == null) 
     //               isFree = 0;
     //          //return 1 if the point is takable
     //           else if (cell.Piece.Player != turn)
     //               isFree = 1;

     //          return isFree;
     //     }
     //}
}
