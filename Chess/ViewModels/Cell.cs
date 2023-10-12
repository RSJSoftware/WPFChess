using GalaSoft.MvvmLight;
using System.Windows;

namespace Chess.BoardManager
{
     public class Cell : ViewModelBase
     {
          private Point pos;
          public Point Pos
          {
               get { return this.pos; }
               set { this.pos = value; RaisePropertyChanged(() => this.Pos); }
          }

          private ChessPieceVM chessPiece;
          public ChessPieceVM ChessPiece
          {
               get { return this.chessPiece; }
               set { this.chessPiece = value; RaisePropertyChanged(() => this.ChessPiece); }
          }

          private Sq name;
          public Sq Name
          {
               get { return this.name; }
               set { this.name = value; RaisePropertyChanged(() => this.Name); }
          }

          //activated when cell is selected with left click
          private bool isSelected;
          public bool IsSelected
          {
               get { return this.isSelected; }
               set { this.isSelected = value; RaisePropertyChanged(() => this.IsSelected); }
          }

          //activated when cell is a legal move for selected cell
          private bool isLegal;
          public bool IsLegal
          {
               get { return this.isLegal; }
               set { this.isLegal = value; RaisePropertyChanged(() => this.IsLegal); }
          }

          //activated when dragging cell
          private bool isDragging;
          public bool IsDragging
          {
               get { return this.isDragging; }
               set { this.isDragging = value; RaisePropertyChanged(() => this.IsDragging); }
          }

          //activated when king is in check
          private bool isCheck;
          public bool IsCheck
          {
               get { return this.isCheck; }
               set { this.isCheck = value; RaisePropertyChanged(() => this.IsCheck); }
          }

          //activated
          private bool isChecking;
          public bool IsChecking
          {
               get { return this.isChecking; }
               set { this.isChecking = value; RaisePropertyChanged(() => this.IsChecking); }
          }

          //activated when cell is highlighted with right click
          private bool isHighlighted;
          public bool IsHighlighted
          {
               get { return this.isHighlighted; }
               set { this.isHighlighted = value; RaisePropertyChanged(() => this.IsHighlighted); }
          }

          //activated
          private bool isActivated;
          public bool IsActivated
          {
               get { return this.isActivated; }
               set { this.isActivated = value; RaisePropertyChanged(() => this.IsActivated); }
          }

          //activated when in setup mode
          private bool isSetUp;
          public bool IsSetUp
          {
               get { return this.isSetUp; }
               set { this.isSetUp = value; RaisePropertyChanged(() => this.IsSetUp); }
          }


          override
          public string ToString()
          {
               if (chessPiece == null)
                    return name + " at " + pos;
               return name + " " + chessPiece.ToString() + " at " + pos;
          }
     }
}
