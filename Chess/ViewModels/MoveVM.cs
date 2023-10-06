using GalaSoft.MvvmLight;

namespace Chess.ViewModels
{
     public class MoveVM : ViewModelBase
     {

          private int moveNum;
          public int MoveNum
          {
               get { return this.moveNum; }
               set { this.moveNum = value; RaisePropertyChanged(() => this.MoveNum); }
          }

          private string whiteMove;
          public string WhiteMove
          {
               get { return this.whiteMove; }
               set { this.whiteMove = value; RaisePropertyChanged(() => this.WhiteMove); }
          }

          private string blackMove;
          public string BlackMove
          {
               get { return this.blackMove; }
               set { this.blackMove = value; RaisePropertyChanged(() => this.BlackMove); }
          }
     }
}
