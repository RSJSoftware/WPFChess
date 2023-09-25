using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Chess.BoardManager;
using Chess.Perft;
using Chess.Views;

namespace Chess
{
     /// <summary>
     /// Interaction logic for MainWindow.xaml
     /// </summary>
     public partial class MainWindow : Window
     {
          private LabelManager lbman;

          public MainWindow()
          {
               BitboardController.InitializeAttacks();
               VariableManager.Initialize();
               InitializeComponent();

               lbman = VariableManager.GetLabelManager();
               lbman.Debug = "Hello there";
               DataContext = lbman;
          }

          private void Button_Click(object sender, RoutedEventArgs e)
          {
               TextInput.Text = VariableManager.GetBoard().ToString();
               Console.Write(TextInput.Text);
          }


     }
}
