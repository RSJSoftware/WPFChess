using Chess.AI;
using Chess.BoardManager;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Chess.Views
{
     /// <summary>
     /// Interaction logic for BoardSetupControl.xaml
     /// </summary>
     public partial class BoardSetupControl : UserControl
     {
          ToggleButton selectedButton;

          public BoardSetupControl()
          {
               InitializeComponent();
               DataContext = VariableManager.Labels;
               selectedButton = null;
          }

          private void Load(object sender, RoutedEventArgs e)
          {
               VariableManager.Board.ReadFen(FenTextBox.Text);
               VariableManager.UpdateCells();
          }

          private void LoadInitialPos(object sender, RoutedEventArgs e)
          {
               VariableManager.Board.ReadFen(BoardState.INITIALPOS);
               VariableManager.UpdateCells();
          }

          private void HandleSelection(object sender, RoutedEventArgs e)
          {
               if (selectedButton != null)
                    selectedButton.IsChecked = false;

               selectedButton = sender as ToggleButton;
               VariableManager.IsTrash = false;

               if (!VariableManager.IsInSetupMode)
               {
                    VariableManager.ToggleSetUpMode();
                    SetupToggleBtn.IsChecked = true;
               }

               switch (selectedButton.Name)
               {
                    case "whitePawn": VariableManager.SetupPiece = Piece.Pawn; VariableManager.SetupColor = Player.White; break;
                    case "whiteKnight": VariableManager.SetupPiece = Piece.Knight; VariableManager.SetupColor = Player.White; break;
                    case "whiteBishop": VariableManager.SetupPiece = Piece.Bishop; VariableManager.SetupColor = Player.White; break;
                    case "whiteRook": VariableManager.SetupPiece = Piece.Rook; VariableManager.SetupColor = Player.White; break;
                    case "whiteQueen": VariableManager.SetupPiece = Piece.Queen; VariableManager.SetupColor = Player.White; break;
                    case "whiteKing": VariableManager.SetupPiece = Piece.King; VariableManager.SetupColor = Player.White; break;
                    case "blackPawn": VariableManager.SetupPiece = Piece.Pawn; VariableManager.SetupColor = Player.Black; break;
                    case "blackKnight": VariableManager.SetupPiece = Piece.Knight; VariableManager.SetupColor = Player.Black; break;
                    case "blackBishop": VariableManager.SetupPiece = Piece.Bishop; VariableManager.SetupColor = Player.Black; break;
                    case "blackRook": VariableManager.SetupPiece = Piece.Rook; VariableManager.SetupColor = Player.Black; break;
                    case "blackQueen": VariableManager.SetupPiece = Piece.Queen; VariableManager.SetupColor = Player.Black; break;
                    case "blackKing": VariableManager.SetupPiece = Piece.King; VariableManager.SetupColor = Player.Black; break;
                    case "trash": VariableManager.SetupPiece = Piece.None; VariableManager.SetupColor = Player.White; VariableManager.IsTrash = true; break;
                    default: Console.WriteLine("togglebutton name incorrect."); break;
               }
          }

          private void HandleDeselection(object sender, RoutedEventArgs e)
          {
               selectedButton = null;
               VariableManager.SetupPiece = Piece.None;
          }

          private void Start(object sender, RoutedEventArgs e)
          {
               if (!VariableManager.IsInSetupMode)
                    return;

               SetupToggleBtn.IsChecked = false;
               if (selectedButton != null)
               {
                    selectedButton.IsChecked = false;
                    selectedButton = null;
               }

               VariableManager.ToggleSetUpMode();

               //make sure all cells are in order
               for (int i = 0; i < 64; i++)
               {
                    if (VariableManager.CellList[i].Name != (Sq)i)
                    {
                         Console.WriteLine("Cells out of order, sorting...");
                         VariableManager.SortCells();
                    }
               }

               Console.WriteLine("Setting up fen");

               string fen = "";
               
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

                    Piece toCheck = VariableManager.CellList[square].ChessPiece.Type;
                    Player toCheckPlayer = VariableManager.CellList[square].ChessPiece.Player;

                    if (toCheck == Piece.None)
                    {
                         spaces++;
                         continue;
                    }

                    if (spaces > 0)
                    {
                         fen += spaces + "";
                         spaces = 0;
                    }

                    switch (toCheck)
                    {
                         case Piece.Pawn: fen += (toCheckPlayer == Player.White) ? "P" : "p"; break;
                         case Piece.Knight: fen += (toCheckPlayer == Player.White) ? "N" : "n"; break;
                         case Piece.Bishop: fen += (toCheckPlayer == Player.White) ? "B" : "b"; break;
                         case Piece.Rook: fen += (toCheckPlayer == Player.White) ? "R" : "r"; break;
                         case Piece.Queen: fen += (toCheckPlayer == Player.White) ? "Q" : "q"; break;
                         case Piece.King: fen += (toCheckPlayer == Player.White) ? "K" : "k"; break;

                    }
               }

               fen += (btnWhite.IsChecked == true) ? " w " : " b ";

               bool castleExist = false;
               if (VariableManager.CellList[(int)Sq.e1].ChessPiece.Type == Piece.King && 
                    VariableManager.CellList[(int)Sq.e1].ChessPiece.Player == Player.White)
               {
                    if (VariableManager.CellList[(int)Sq.a1].ChessPiece.Type == Piece.Rook &&
                    VariableManager.CellList[(int)Sq.a1].ChessPiece.Player == Player.White)
                    {
                         castleExist = true;
                         fen += "K";
                    }
                    if (VariableManager.CellList[(int)Sq.h1].ChessPiece.Type == Piece.Rook &&
                    VariableManager.CellList[(int)Sq.h1].ChessPiece.Player == Player.White)
                    {
                         castleExist = true;
                         fen += "Q";
                    }
               }

               if (VariableManager.CellList[(int)Sq.e8].ChessPiece.Type == Piece.King &&
                    VariableManager.CellList[(int)Sq.e8].ChessPiece.Player == Player.Black)
               {
                    if (VariableManager.CellList[(int)Sq.a8].ChessPiece.Type == Piece.Rook &&
                    VariableManager.CellList[(int)Sq.a8].ChessPiece.Player == Player.Black)
                    {
                         castleExist = true;
                         fen += "k";
                    }
                    if (VariableManager.CellList[(int)Sq.h8].ChessPiece.Type == Piece.Rook &&
                    VariableManager.CellList[(int)Sq.h8].ChessPiece.Player == Player.Black)
                    {
                         castleExist = true;
                         fen += "q";
                    }
               }

               if (!castleExist)
                    fen += "-";

               fen += " - 0 1";

               Console.WriteLine("Fen completed: " + fen);

               VariableManager.NewGame(fen);

               if (btnBlack.IsChecked == true)
                    VariableManager.FlipCells();

               VariableManager.UpdateAI(BitboardComboBox.SelectedIndex);
          }

          private void SetupModeToggle(object sender, RoutedEventArgs e)
          {
               VariableManager.ToggleSetUpMode();
               if (SetupToggleBtn.IsChecked != true)
               {
                    if (selectedButton != null)
                    {
                         selectedButton.IsChecked = false;
                         selectedButton = null;
                    }

                    VariableManager.UpdateCells();
               }
          }
     }
}
