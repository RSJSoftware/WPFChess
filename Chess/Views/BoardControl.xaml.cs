using Chess.AI;
using System;
using System.Windows.Controls;

namespace Chess.Views
{
     /// <summary>
     /// Interaction logic for BoardControl.xaml
     /// </summary>
     public partial class BoardControl : UserControl
     {
          public BoardControl()
          {
               InitializeComponent();

               DataContext = VariableManager.Labels;
          }

          private void NewGame(object sender, System.Windows.RoutedEventArgs e)
          {
               VariableManager.NewGame();
          }

          private void SetBeginning(object sender, System.Windows.RoutedEventArgs e)
          {
               if (VariableManager.CurrentMove > 0)
                    VariableManager.SetBoardState(0);
          }

          private void SetBack(object sender, System.Windows.RoutedEventArgs e)
          {
               if (VariableManager.CurrentMove > 0)
                    VariableManager.SetBoardState(VariableManager.CurrentMove - 1);
          }

          private void SetForward(object sender, System.Windows.RoutedEventArgs e)
          {
               if (VariableManager.CurrentMove < (VariableManager.MoveHistory.Count - 1))
                    VariableManager.SetBoardState(VariableManager.CurrentMove + 1);
          }

          private void SetEnd(object sender, System.Windows.RoutedEventArgs e)
          {
               if (VariableManager.CurrentMove < (VariableManager.MoveHistory.Count - 1))
                    VariableManager.SetBoardState(VariableManager.MoveHistory.Count - 1);
          }

          private void FlipBoard(object sender, System.Windows.RoutedEventArgs e)
          {
               VariableManager.IsBoardFlipped = !VariableManager.IsBoardFlipped;

               //update board coordinate markers based on which way the board is flipped
               if (VariableManager.IsBoardFlipped)
               {
                    lb1.SetValue(Grid.RowProperty, 0);
                    lb2.SetValue(Grid.RowProperty, 1);
                    lb3.SetValue(Grid.RowProperty, 2);
                    lb4.SetValue(Grid.RowProperty, 3);
                    lb5.SetValue(Grid.RowProperty, 4);
                    lb6.SetValue(Grid.RowProperty, 5);
                    lb7.SetValue(Grid.RowProperty, 6);
                    lb8.SetValue(Grid.RowProperty, 7);

                    lba.SetValue(Grid.ColumnProperty, 8);
                    lbb.SetValue(Grid.ColumnProperty, 7);
                    lbc.SetValue(Grid.ColumnProperty, 6);
                    lbd.SetValue(Grid.ColumnProperty, 5);
                    lbe.SetValue(Grid.ColumnProperty, 4);
                    lbf.SetValue(Grid.ColumnProperty, 3);
                    lbg.SetValue(Grid.ColumnProperty, 2);
                    lbh.SetValue(Grid.ColumnProperty, 1);
               }
               else
               {
                    lb1.SetValue(Grid.RowProperty, 7);
                    lb2.SetValue(Grid.RowProperty, 6);
                    lb3.SetValue(Grid.RowProperty, 5);
                    lb4.SetValue(Grid.RowProperty, 4);
                    lb5.SetValue(Grid.RowProperty, 3);
                    lb6.SetValue(Grid.RowProperty, 2);
                    lb7.SetValue(Grid.RowProperty, 1);
                    lb8.SetValue(Grid.RowProperty, 0);

                    lba.SetValue(Grid.ColumnProperty, 1);
                    lbb.SetValue(Grid.ColumnProperty, 2);
                    lbc.SetValue(Grid.ColumnProperty, 3);
                    lbd.SetValue(Grid.ColumnProperty, 4);
                    lbe.SetValue(Grid.ColumnProperty, 5);
                    lbf.SetValue(Grid.ColumnProperty, 6);
                    lbg.SetValue(Grid.ColumnProperty, 7);
                    lbh.SetValue(Grid.ColumnProperty, 8);
               }

               VariableManager.FlipCells();
          }

          private void BitboardComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
          {
               VariableManager.UpdateAI(BitboardComboBox.SelectedIndex);
          }
     }
}
