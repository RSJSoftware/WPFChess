using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
     }
}
