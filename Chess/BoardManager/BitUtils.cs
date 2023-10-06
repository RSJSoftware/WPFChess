using Chess.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.BoardManager
{
     public static class BitUtils
     {
          public static void HighlightBits(ulong bitboard)
          {
               foreach(Cell c in VariableManager.CellList)
                    c.IsChecking = false;

               while (bitboard != 0)
               {
                    int lsbit = BitboardController.GetLSBitIndex(bitboard);

                    Cell cell = VariableManager.CellList.Where(x => x.Name == (Sq)lsbit).FirstOrDefault();
                    cell.IsChecking = true;

                    bitboard = BitboardController.PopBit(bitboard, lsbit);
               }
          }

          public static string PrintBits(ulong bitboard)
          {
               string output = "";

               for (int i = 0; i < 64; i++)
               {
                    if ((bitboard & (1UL << i)) == 0)
                         output += "0";
                    else
                         output += "1";
               }

               return output;
          }
     }
}
