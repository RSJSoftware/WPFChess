using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Chess.BoardManager;

namespace Chess.Views
{
	/// <summary>
	/// Interaction logic for MainBoard.xaml
	/// </summary>
	public partial class MainBoard : UserControl
	{
		private int cellSize = 1;
		private double scaleSize;

		int index;

		public MainBoard()
		{
			InitializeComponent();

			//placeholder board initializer
			ChessBoard.ItemsSource = VariableManager.CellList;

			//get scale size defined in xaml
			ScaleTransform st = mainGrid.LayoutTransform as ScaleTransform;
			scaleSize = st.ScaleX;
		}

		private void MouseButtonDown(object sender, MouseButtonEventArgs e)
		{
			//if left button is pressed, set the current position of the mouse and get the piece it was clicked on
			if (e.LeftButton == MouseButtonState.Pressed && sender is FrameworkElement frameworkElement)
			{
				if (VariableManager.SelectedCell != null && VariableManager.HighlightedCells.Contains(frameworkElement.DataContext))
				{
					Point newPos = e.MouseDevice.GetPosition(ChessBoard);
					newPos.X -= newPos.X % cellSize;
					newPos.Y -= newPos.Y % cellSize;

					Console.WriteLine("Mouse clicked on position: " + newPos);

					MovePiece(newPos);
					return;
				}

				//if in setup mode with a setup piece selected, simply set the piece and return
				if (VariableManager.IsInSetupMode && (VariableManager.SetupPiece != Piece.None || VariableManager.IsTrash))
				{
					VariableManager.SetCell(frameworkElement.DataContext as Cell);
					return;
				}

				//handle selection in the handles selection class
				SelectionHandler(frameworkElement.DataContext as Cell);

				Console.WriteLine("Mouse clicked on cell: " + frameworkElement.DataContext);
			}
			else if (e.RightButton == MouseButtonState.Pressed && sender is FrameworkElement frameworkElement2 && !VariableManager.IsInSetupMode)
			{
				//right click will highlight the cell
				Cell clickedCell = frameworkElement2.DataContext as Cell;
                    clickedCell.IsHighlighted = !clickedCell.IsHighlighted;

               }
			

		}

		private void MouseMoveHandle(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed && sender is FrameworkElement frameworkElement)
				DragDrop.DoDragDrop(frameworkElement, new DataObject(DataFormats.Serializable, frameworkElement.DataContext), DragDropEffects.Move);
		}

		private void DropHandler(object sender, DragEventArgs e)
		{
			if (VariableManager.SelectedCell == null || (VariableManager.IsInSetupMode && (VariableManager.SetupPiece != Piece.None || VariableManager.IsTrash)))
				return;

			Point newPos = e.GetPosition(this);

			//fix the dragging point since it takes into consideration the whole canvas
			if (newPos.X > 8 || newPos.Y > 8)
			{
				newPos.X /= scaleSize;
				newPos.Y /= scaleSize;

                    //center the image to the cell
                    newPos.X -= newPos.X % cellSize;
                    newPos.Y -= newPos.Y % cellSize;
               }

			Console.WriteLine("Dropping at newPos: " + newPos);

			MovePiece(newPos);

			Console.WriteLine("Dropped at newPos: " + newPos);
		}

		private void DragOverHandler(object sender, DragEventArgs e)
		{
			if (VariableManager.SelectedCell == null)
				return;

			//put position of image to the center of the mouse
			Point newPos = e.GetPosition(this);

			//reset position if the mouse leaves the board while dragging
			if (newPos.X < 0 || newPos.Y < 0 || newPos.X > Width || newPos.Y > Height)
			{
				Console.WriteLine("Fixing pos");
				VariableManager.SelectedCell.IsDragging = false;
				VariableManager.SelectedCell.Pos = VariableManager.InitialPos;
				return;
			}

			double shift = (double)cellSize / 2;
			newPos.X = (newPos.X / scaleSize) - shift;
			newPos.Y = (newPos.Y / scaleSize) - shift;

			VariableManager.SelectedCell.Pos = newPos;

			//set style to drag
			VariableManager.SelectedCell.IsDragging = true;

		}

		private void SelectionHandler(Cell cell)
		{
			//don't allow selection if there is no piece in cell, browsing past moves, or the gamestate is not playing
			if (cell.ChessPiece == null || VariableManager.CurrentMove != VariableManager.MoveHistory.Count - 1 || VariableManager.State != GameState.Playing)
				return;

               //if in setup mode select any piece in order to allow drag and drop
               if (VariableManager.IsInSetupMode)
			{
				if(VariableManager.SelectedCell != null)
                         VariableManager.MoveCell(VariableManager.CellList.Count - 1, index);
				VariableManager.SelectedCell = cell;
                    VariableManager.InitialPos = cell.Pos;
                    index = (int)cell.Name;
                    VariableManager.MoveCell(index, VariableManager.CellList.Count - 1);
                    return;
			}

               //deselect if selected cell is the cell being passed
               if (VariableManager.SelectedCell != null && cell.Equals(VariableManager.SelectedCell))
			{
				VariableManager.SelectedCell = null;
				cell.IsSelected = false;
				cell.IsDragging = false;
				VariableManager.DehighlightBorders();
				Console.Write("deselecting 1 ");

				return;
			}

			//if selected cell is not null and the new cell is an enemy piece, move
			if (VariableManager.SelectedCell != null && cell.ChessPiece.Player != VariableManager.SelectedCell.ChessPiece.Player)
			{
				MovePiece(cell.Pos);
				Console.Write("Taking ");
			}
			//if selected cell is not null and the new cell is an ally piece, select the ally piece
			else if (VariableManager.SelectedCell != null && cell.ChessPiece.Player == VariableManager.SelectedCell.ChessPiece.Player)
			{
				//move cell to its original position in the observable collection
				VariableManager.MoveCell(VariableManager.CellList.Count - 1, index);
				SelectionHandler(VariableManager.SelectedCell);
				VariableManager.SelectedCell = cell;
				VariableManager.HighlightBorders();
				VariableManager.InitialPos = cell.Pos;
				index = (int)cell.Name;
				//move new cell to the back of the observable collection so that it renders in front
				VariableManager.MoveCell(index, VariableManager.CellList.Count - 1);
				cell.IsSelected = true;
				Console.Write("reselecting ");
			}
			//if selected cell is null, select new cell if it is an ally piece
			else
			{
				if (cell.ChessPiece.Player == VariableManager.Board.Turn)
				{
					VariableManager.SelectedCell = cell;
					VariableManager.HighlightBorders();
					VariableManager.InitialPos = cell.Pos;
					index = (int)cell.Name;
					//move cell to the back of the observable collection so that it renders in front
					VariableManager.MoveCell(index, VariableManager.CellList.Count - 1);
					cell.IsSelected = true;
					Console.Write("selecting ");
				}
			}

			Console.WriteLine(cell.ToString());
		}

		private void MovePiece(Point pos)
		{
			//return in case somehow this was called without a selected piece
			if (VariableManager.SelectedCell == null)
				return;


			Console.WriteLine("Starting pos: " + VariableManager.InitialPos + " " + VariableManager.SelectedCell.Name + " to " + pos);

			Cell end = VariableManager.CellList.Where(x => x.Pos == pos).FirstOrDefault();

			Piece takePiece = Piece.None;

			//move piece back to original index before any calculations begin
			bool moved = false;
			VariableManager.MoveCell(VariableManager.CellList.Count - 1, index);

               //if in setup mode move the piece to any position without legality checks
               if (VariableManager.IsInSetupMode)
			{
				if (end != null)
                    {
                         end.ChessPiece.Type = VariableManager.SelectedCell.ChessPiece.Type;
                         end.ChessPiece.Player = VariableManager.SelectedCell.ChessPiece.Player;
                         VariableManager.SelectedCell.ChessPiece.Type = Piece.None;
                    }

                    VariableManager.SelectedCell.Pos = VariableManager.InitialPos;
                    VariableManager.SelectedCell.IsDragging = false;
                    return;
               }

			Player mover = VariableManager.Board.Turn;

			if (end != null)
			{
				takePiece = end.ChessPiece.Type;

                    ChessPieceVM selectedPiece = VariableManager.SelectedCell.ChessPiece;

                    if (selectedPiece.Type == Piece.Pawn && 
					(((VariableManager.IsBoardFlipped && selectedPiece.Player == Player.Black) || 
						(!VariableManager.IsBoardFlipped && selectedPiece.Player == Player.White)) &&
						end.Pos.Y == 0 && VariableManager.InitialPos.Y == 1) || 
					(((VariableManager.IsBoardFlipped && selectedPiece.Player == Player.White) || 
						(!VariableManager.IsBoardFlipped && selectedPiece.Player == Player.Black)) &&
						end.Pos.Y == 7 && VariableManager.InitialPos.Y == 6))
				{
					PromotionDialog promotionPieces = new PromotionDialog();
					promotionPieces.ShowDialog();
				}

				if (!VariableManager.SelectedCell.Equals(end))
					moved = VariableManager.Board.MovePiece(VariableManager.SelectedCell.Name, end.Name, VariableManager.PromotionPiece);

				VariableManager.PromotionPiece = Piece.None;
			}

               //deselect piece and move cell back to initial position
               VariableManager.SelectedCell.Pos = VariableManager.InitialPos;
               SelectionHandler(VariableManager.SelectedCell);

               //if pieces didn't move, return
               if (!moved) 
				return;


			//update visuals and history
			VariableManager.HandleMoveUpdates(takePiece);

			//check to see if the game is over
			if (VariableManager.State != GameState.Playing)
			{
				if(VariableManager.State == GameState.Checkmate)
					VariableManager.Labels.GameWinner = mover + " wins by checkmate!";
				else
				{
					string draw = "";
					switch(VariableManager.State)
					{
						case GameState.Stalemate: 
						case GameState.Repetition: draw = VariableManager.State + ""; break;
						case GameState.Insufficient: draw = "insufficient material";  break;
						case GameState.MoveCount: draw = "fifty-move rule"; break;
						default: draw = "gamestate not accounted for in mainboard " + VariableManager.State; break;

                         }

                         VariableManager.Labels.GameWinner = "Draw by " + draw + ".";
                    }

                    GameOverDialog gameOver = new GameOverDialog();
                    gameOver.ShowDialog();
				return;
               }

			mover = VariableManager.Board.Turn;

               //make the AI move if one is activated
               if (VariableManager.AI != null)
				VariableManager.ActivateAI();

               //check to see if the game is over again
               if (VariableManager.State != GameState.Playing)
               {
                    if (VariableManager.State == GameState.Checkmate)
                         VariableManager.Labels.GameWinner = mover + " wins by checkmate!";
                    else
                         VariableManager.Labels.GameWinner = "Draw by " + VariableManager.State;

                    GameOverDialog gameOver = new GameOverDialog();
                    gameOver.ShowDialog();
                    return;
               }
          }
	}
}
