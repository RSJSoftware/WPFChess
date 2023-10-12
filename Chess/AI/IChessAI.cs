using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.AI
{
     public interface IChessAI
     {
          int Think();
          Piece Move(int moveIndex);
     }
}
