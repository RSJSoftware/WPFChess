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


          private string frontPlayer;
          public string FrontPlayer
          {
               get { return this.frontPlayer; }
               set { this.frontPlayer = value; RaisePropertyChanged(() => this.FrontPlayer); }
          }


          private string frontPlayerTakes;
          public string FrontPlayerTakes
          {
               get { return this.frontPlayerTakes; }
               set { this.frontPlayerTakes = value; RaisePropertyChanged(() => this.FrontPlayerTakes); }
          }


          private string backPlayer;
          public string BackPlayer
          {
               get { return this.backPlayer; }
               set { this.backPlayer = value; RaisePropertyChanged(() => this.BackPlayer); }
          }

          private string backPlayerTakes;
          public string BackPlayerTakes
          {
               get { return this.backPlayerTakes; }
               set { this.backPlayerTakes = value; RaisePropertyChanged(() => this.BackPlayerTakes); }
          }


          private int selectedAI;
          public int SelectedAI
          {
               get { return this.selectedAI; }
               set { this.selectedAI = value; RaisePropertyChanged(() => this.SelectedAI); }
          }

          private int setupSelectedAI;
          public int SetupSelectedAI
          {
               get { return this.setupSelectedAI; }
               set { this.setupSelectedAI = value; RaisePropertyChanged(() => this.SetupSelectedAI); }
          }
     }
}
