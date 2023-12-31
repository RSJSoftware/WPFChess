﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Chess.Views
{
     /// <summary>
     /// Interaction logic for PromotionDialog.xaml
     /// </summary>
     public partial class PromotionDialog : Window
     {
          public PromotionDialog()
          {
               InitializeComponent();
          }

          private void PromoteKnight(object sender, MouseButtonEventArgs e)
          {
               VariableManager.PromotionPiece = Piece.Knight;
               Close();
          }

          private void PromoteBishop(object sender, MouseButtonEventArgs e)
          {
               VariableManager.PromotionPiece = Piece.Bishop;
               Close();
          }

          private void PromoteRook(object sender, MouseButtonEventArgs e)
          {
               VariableManager.PromotionPiece = Piece.Rook;
               Close();
          }

          private void PromoteQueen(object sender, MouseButtonEventArgs e)
          {
               VariableManager.PromotionPiece = Piece.Queen;
               Close();
          }
     }
}
