using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
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

		BoardState board;
		Cell selectedCell;
		List<Cell> highlightedCells;
		//LabelManager lbman;

		Point initialPos;
		int index;

		public MainBoard()
		{
			InitializeComponent();

			//placeholder board initializer
			board = VariableManager.GetBoard();
			ChessBoard.ItemsSource = VariableManager.GetCells();

			//get scale size defined in xaml
			ScaleTransform st = mainGrid.LayoutTransform as ScaleTransform;
			scaleSize = st.ScaleX;

			highlightedCells = new List<Cell>();
		}

		private void MouseButtonDown(object sender, MouseButtonEventArgs e)
		{
			//if left button is pressed, set the current position of the mouse and get the piece it was clicked on
			if (e.LeftButton == MouseButtonState.Pressed && sender is FrameworkElement frameworkElement)
               {
				if (selectedCell != null && highlightedCells.Contains(frameworkElement.DataContext))
				{
                         Point newPos = e.MouseDevice.GetPosition(ChessBoard);
                         newPos.X -= (newPos.X % cellSize);
                         newPos.Y -= (newPos.Y % cellSize);

                         Console.WriteLine("Mouse clicked on position: " + newPos);

                         MovePiece(newPos);
					return;
				}

                    //handle selection in the handles selection class
                    SelectionHandler(frameworkElement.DataContext as Cell);

				Console.WriteLine("Mouse clicked on cell: " + frameworkElement.DataContext);
			}

		}

		private void MouseMoveHandle(object sender, MouseEventArgs e)
		{
			var selectedElement = e.Source as UIElement;
			var selectedPresenter = VisualTreeHelper.GetParent(selectedElement) as ContentPresenter;
			if (e.LeftButton == MouseButtonState.Pressed && sender is FrameworkElement frameworkElement)
			{

				DragDrop.DoDragDrop(frameworkElement, new DataObject(DataFormats.Serializable, frameworkElement.DataContext), DragDropEffects.Move);
			}
		}

		private void DropHandler(object sender, DragEventArgs e)
		{
			if (selectedCell == null)
				return;

			Point newPos = e.GetPosition(this);
			//fix the dragging point since it takes into consideration the whole canvas
			if (newPos.X > 8 || newPos.Y > 8)
			{
				newPos.X /= scaleSize;
				newPos.Y /= scaleSize;
			}

			Console.WriteLine("Dropping at newPos: " + newPos);
			if (newPos != selectedCell.Pos)
				MovePiece(newPos);


			Console.WriteLine("Dropped at newPos: " + newPos);
		}

		private void DragOverHandler(object sender, DragEventArgs e)
		{
			if (selectedCell == null)
				return;

			//put position of image to the center of the mouse
			Point newPos = e.GetPosition(this);

			double shift = (double)cellSize / 2;
			newPos.X = (newPos.X / scaleSize) - (shift);
			newPos.Y = (newPos.Y / scaleSize) - (shift);

			selectedCell.Pos = newPos;

			//set style to drag
			selectedCell.IsDragging = true;

		}

		private void SelectionHandler(Cell cell)
		{
			//deselect if selected cell is the cell being passed
			if (selectedCell != null && cell.Equals(selectedCell))
			{
				selectedCell = null;
				cell.IsSelected = false;
				cell.IsDragging = false;
				DehighlightBorders();
				Console.Write("deselecting 1 ");

				return;
			}
			//return if attempting to select a cell without a piece
			if (cell.ChessPiece == null)
				return;

			//if selected cell is not null and the new cell is an enemy piece, move
			if (selectedCell != null && cell.ChessPiece.Player != selectedCell.ChessPiece.Player)
			{
				MovePiece(cell.Pos);
				Console.Write("Taking ");
			}
			//if selected cell is not null and the new cell is an ally piece, select the ally piece
			else if (selectedCell != null && cell.ChessPiece.Player == selectedCell.ChessPiece.Player)
			{
				//move cell to its original position in the observable collection
				VariableManager.MoveCell(VariableManager.GetCells().Count - 1, index);
				SelectionHandler(selectedCell);
				selectedCell = cell;
				HighlightBorders();
				initialPos = cell.Pos;
				index = (int)cell.Name;
				//move new cell to the back of the observable collection so that it renders in front
				VariableManager.MoveCell(index, VariableManager.GetCells().Count - 1);
				cell.IsSelected = true;
				Console.Write("reselecting ");
			}
			//if selected cell is null, select new cell if it is an ally piece
			else
			{
				if (cell.ChessPiece.Player == board.Turn)
				{
					selectedCell = cell;
					HighlightBorders();
					initialPos = cell.Pos;
					index = (int)cell.Name;
					//move cell to the back of the observable collection so that it renders in front
					VariableManager.MoveCell(index, VariableManager.GetCells().Count - 1);
					cell.IsSelected = true;
					Console.Write("selecting ");
				}
			}

			Console.WriteLine(cell.ToString());
		}

		private void HighlightBorders()
		{
			highlightedCells.Clear();
			if (!board.LegalMoves.TryGetValue(selectedCell.Name, out List<Move> legalMoves))
				return;

			board.TestMoves(selectedCell.Name);

			foreach(Move move in legalMoves)
               {
                    highlightedCells.Add(VariableManager.GetCells().Where(x => x.Name == move.End).FirstOrDefault());

               }

			foreach (Cell c in highlightedCells)
			{
				c.IsLegal = true;
			}
		}

		private void DehighlightBorders()
		{
			foreach (Cell c in highlightedCells)
			{
				c.IsLegal = false;
			}
		}

		private void MovePiece(Point pos)
		{
			//return in case somehow this was called without a selected piece
			if (selectedCell == null)
				return;

			//center the image to the cell
			pos.X -= (pos.X % cellSize);
			pos.Y -= (pos.Y % cellSize);

			Console.WriteLine("Starting pos: " + initialPos + " " + selectedCell.Name);

			Cell end = VariableManager.GetCells().Where(x => x.Pos == pos).FirstOrDefault();

			//move piece back to original index before any calculations begin
			bool moved = false;
			VariableManager.MoveCell(VariableManager.GetCells().Count - 1, index);

			if (end != null)
			{
				if (selectedCell.ChessPiece.Type == Piece.Pawn && (end.Pos.Y == 0 || end.Pos.Y == 7))
				{
                         PromotionDialog promotionPieces = new PromotionDialog();
                         promotionPieces.ShowDialog();
                    }

				if (!selectedCell.Equals(end))
					moved = board.MovePiece(selectedCell.Name, end.Name, VariableManager.GetPromotionPiece());

                    VariableManager.SetPromotionPiece(Piece.None);
               }

			//if pieces didn't move, move back to the starting position and deselct
			if (!moved)
			{
				selectedCell.Pos = initialPos;
				SelectionHandler(selectedCell);

				return;
			}


			//update visuals
			VariableManager.UpdateCells();

			//move a piece to new position, change turn, and deselect piece and button
			selectedCell.Pos = initialPos;
			SelectionHandler(selectedCell);
		}
     }
}
