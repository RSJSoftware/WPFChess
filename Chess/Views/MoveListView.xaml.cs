using Chess.BoardManager;
using Chess.ViewModels;
using System.Runtime.CompilerServices;
using System.Windows;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Chess.Views
{
     /// <summary>
     /// Interaction logic for MoveListView.xaml
     /// </summary>
     public partial class MoveListView : UserControl
     {
          LabelManager lbman;

          public MoveListView()
          {
               InitializeComponent();
               //MoveListGrid.ItemsSource = VariableManager.MoveList;
               MoveDataGrid.ItemsSource = VariableManager.MoveList;
               lbman = VariableManager.Labels;
               DataContext = lbman;
          }

          private void ViewMove(object sender, MouseEventArgs e)
          {
               if (e.LeftButton == MouseButtonState.Pressed && sender is FrameworkElement frameworkElement)
               {
                    //get the cell clicked on, return if it somehow wasn't a cell
                    DependencyObject clickedObj = e.OriginalSource as DependencyObject;
                    while ((clickedObj != null) && !(clickedObj is DataGridCell) && !(clickedObj is DataGridColumnHeader))
                         clickedObj = VisualTreeHelper.GetParent(clickedObj);

                    if (clickedObj == null || !(clickedObj is DataGridCell)) return;

                    DataGridCell cell = clickedObj as DataGridCell;
                    MoveVM move = frameworkElement.DataContext as MoveVM;

                    //get column from selected cell, subtract by two to get the correct board state without further manipulating the number
                    int column = cell.Column.DisplayIndex - 2;

                    //return if there has not been a black move in the clicked cell
                    if (column == 0 && move.BlackMove == "")
                         return;

                    Console.WriteLine("Clicked on move number: " + move.MoveNum + " in column: " + column + " setting to move number: " + (move.MoveNum * 2 + column));

                    VariableManager.SetBoardState(move.MoveNum * 2 + column);
               }
          }
     }
}
