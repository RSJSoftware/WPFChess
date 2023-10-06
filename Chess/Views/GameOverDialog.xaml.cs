using System;
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
     /// Interaction logic for GameOverDialog.xaml
     /// </summary>
     public partial class GameOverDialog : Window
     {
          LabelManager lbman;

          public GameOverDialog()
          {
               InitializeComponent();

               lbman = VariableManager.Labels;
               DataContext = lbman;
          }

          private void Cancel(object sender, RoutedEventArgs e)
          {
               Close();
          }

          private void NewGame(object sender, RoutedEventArgs e)
          {
               VariableManager.NewGame();
               Close();
          }
     }
}
