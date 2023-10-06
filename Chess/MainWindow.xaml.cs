using System.Windows;
using Chess.BoardManager;
using Chess.Views;

namespace Chess
{
     /// <summary>
     /// Interaction logic for MainWindow.xaml
     /// </summary>
     public partial class MainWindow : Window
     {
          public MainWindow()
          {
               BitboardController.InitializeAttacks();
               VariableManager.Initialize();
               InitializeComponent();
          }
     }
}
