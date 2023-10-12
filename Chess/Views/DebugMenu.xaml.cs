using Chess.BoardManager;
using Chess.Perft;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Chess.Views
{
     /// <summary>
     /// Interaction logic for UserControl1.xaml
     /// </summary>
     public partial class DebugMenu : UserControl
     {
          readonly bool[] highlights = new bool[12];
          public DebugMenu()
          {
               InitializeComponent();
               DataContext = VariableManager.Labels;
          }

          private void HighlightBitboard(object sender, RoutedEventArgs e)
          {
               highlights[BitboardComboBox.SelectedIndex] = true;
               HandleHighlights();
          }

          private void DehighlightBitboard(object sender, RoutedEventArgs e)
          {
               highlights[BitboardComboBox.SelectedIndex] = false;
               HandleHighlights();
          }

          private void UpdateBitboard(object sender, RoutedEventArgs e)
          {
               HandleHighlights();
          }

          private void Load(object sender, RoutedEventArgs e)
          {
               //r2q3r/ppp1kBpp/2n5/4PQN1/8/2P5/P1PP2PP/R1B2RK1
               VariableManager.Board.ReadFen(FenTextBox.Text);
               VariableManager.UpdateCells();
          }

          private void LoadInitialPos(object sender, RoutedEventArgs e)
          {
               VariableManager.Board.ReadFen("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
               VariableManager.UpdateCells();
          }


          private void PerftTest(object sender, RoutedEventArgs e)
          {
               VariableManager.Labels.Debug = "Performing PERFT test: Depth " + PerftText.Text;
               
               if (Int32.TryParse(PerftText.Text, out int depth))
               {
                    PerftErrorLabel.Visibility = Visibility.Hidden;
                    Driver.PerftDriver(depth, VariableManager.Board);
                    VariableManager.Labels.PerftStats = $"mov: {Driver.nodes}\ncap: {Driver.captures}\nep: {Driver.enPassants}\n" +
                         $"cas: {Driver.castles}\npro: {Driver.promotions}\nche: {Driver.checks}\nmat: {Driver.checkmates}\ntime: {Driver.stopwatch.ElapsedMilliseconds}ms ";

                    VariableManager.Labels.PerftMoves = Driver.moveDebug;
                    VariableManager.Labels.Debug = "Performing PERFT test at Depth " + PerftText.Text + " completed!";
               }
               else
               {
                    PerftErrorLabel.Visibility = Visibility.Visible;
               }
          }

          private void HandleHighlights()
          {
               foreach (Cell c in VariableManager.CellList)
                    c.IsChecking = false;

               for (int i = 0; i < highlights.Length; i++)
               {
                    if (!highlights[i])
                         continue;

                    int piece = i % 6;
                    int player = i / 6;
                    ulong bitboard = VariableManager.Board.Board.GetBitboard((Piece)piece, (Player)player);

                    while (bitboard != 0)
                    {
                         int lsbit = BitboardController.GetLSBitIndex(bitboard);

                         Cell cell = VariableManager.CellList.Where(x => x.Name == (Sq)lsbit).FirstOrDefault();
                         cell.IsChecking = true;

                         bitboard = BitboardController.PopBit(bitboard, lsbit);
                    }
               }
          }
     }
}
