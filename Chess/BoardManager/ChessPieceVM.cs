using Chess.BoardManager;
using GalaSoft.MvvmLight;

namespace Chess
{
     public class ChessPieceVM : ViewModelBase
     {
          private Piece type;
          public Piece Type
          {
               get { return this.type; }
               set { this.type = value; RaisePropertyChanged(() => this.Type); }
          }

          private Player player;
          public Player Player
          {
               get { return this.player; }
               set { this.player = value; RaisePropertyChanged(() => this.Player); }
          }

          override
          public string ToString()
          {
               return player + " " + type;
          }
     }


}
