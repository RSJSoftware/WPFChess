using GalaSoft.MvvmLight;

namespace Chess.Views
{
     public class LabelManager : ViewModelBase
     {
          private string debug;
          public string Debug
          {
               get { return this.debug; }
               set { this.debug = value; RaisePropertyChanged(() => this.Debug); }
          }


          private string perftStats;
          public string PerftStats
          {
               get { return this.perftStats; }
               set { this.perftStats = value; RaisePropertyChanged(() => this.PerftStats); }
          }

          private string perftMoves;
          public string PerftMoves
          {
               get { return this.perftMoves; }
               set { this.perftMoves = value; RaisePropertyChanged(() => this.PerftMoves); }
          }

          private string currentMove;
          public string CurrentMove
          {
               get { return this.currentMove; }
               set { this.currentMove = value; RaisePropertyChanged(() => this.CurrentMove); }
          }

          private string gameWinner;
          public string GameWinner
          {
               get { return this.gameWinner; }
               set { this.gameWinner = value; RaisePropertyChanged(() => this.GameWinner); }
          }
     }
}
