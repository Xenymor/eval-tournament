using ChessChallenge.API;

namespace Chess_Challenge.Example
{

    /// <summary>
    /// This is the evaluation function used by the EvilBot.
    /// EvilBot uses the same search implementation as MyBot,
    /// so if you want to compare eval functions, simply
    /// copy the other eval function here.
    /// </summary>
    public class EvilBotEvaluator : IEvaluator
    {
        int[,] psTables = {
        //Pawn
        {
             0,   0,   0,   0,   0,   0,   0,   0,
            50,  50,  50,  50,  50,  50,  50,  50,
            10,  10,  20,  30,  30,  20,  10,  10,
             5,   5,  10,  25,  25,  10,   5,   5,
             0,   0,   0,  20,  20,   0,   0,   0,
             5,  -5, -10,   0,   0, -10,  -5,   5,
             5,  10,  10, -20, -20,  10,  10,   5,
             0,   0,   0,   0,   0,   0,   0,   0
        },
        //Pawn End
        /*{
             0,   0,   0,   0,   0,   0,   0,   0,
            80,  80,  80,  80,  80,  80,  80,  80,
            50,  50,  50,  50,  50,  50,  50,  50,
            30,  30,  30,  30,  30,  30,  30,  30,
            20,  20,  20,  20,  20,  20,  20,  20,
            10,  10,  10,  10,  10,  10,  10,  10,
            10,  10,  10,  10,  10,  10,  10,  10,
             0,   0,   0,   0,   0,   0,   0,   0
        },*/
        //Knights
        {
            -50,-40,-30,-30,-30,-30,-40,-50,
            -40,-20,  0,  0,  0,  0,-20,-40,
            -30,  0, 10, 15, 15, 10,  0,-30,
            -30,  5, 15, 20, 20, 15,  5,-30,
            -30,  0, 15, 20, 20, 15,  0,-30,
            -30,  5, 10, 15, 15, 10,  5,-30,
            -40,-20,  0,  5,  5,  0,-20,-40,
            -50,-40,-30,-30,-30,-30,-40,-50,
        },
        //Bishops
        {
            -20,-10,-10,-10,-10,-10,-10,-20,
            -10,  0,  0,  0,  0,  0,  0,-10,
            -10,  0,  5, 10, 10,  5,  0,-10,
            -10,  5,  5, 10, 10,  5,  5,-10,
            -10,  0, 10, 10, 10, 10,  0,-10,
            -10, 10, 10, 10, 10, 10, 10,-10,
            -10,  5,  0,  0,  0,  0,  5,-10,
            -20,-10,-10,-10,-10,-10,-10,-20,
        },
        //Rooks
        {
            0,  0,  0,  0,  0,  0,  0,  0,
            5, 10, 10, 10, 10, 10, 10,  5,
            -5,  0,  0,  0,  0,  0,  0, -5,
            -5,  0,  0,  0,  0,  0,  0, -5,
            -5,  0,  0,  0,  0,  0,  0, -5,
            -5,  0,  0,  0,  0,  0,  0, -5,
            -5,  0,  0,  0,  0,  0,  0, -5,
            0,  0,  0,  5,  5,  0,  0,  0
        },
        //Queens
        {
            -20,-10,-10, -5, -5,-10,-10,-20,
            -10,  0,  0,  0,  0,  0,  0,-10,
            -10,  0,  5,  5,  5,  5,  0,-10,
            -5,  0,  5,  5,  5,  5,  0, -5,
            0,  0,  5,  5,  5,  5,  0, -5,
            -10,  5,  5,  5,  5,  5,  0,-10,
            -10,  0,  5,  0,  0,  0,  0,-10,
            -20,-10,-10, -5, -5,-10,-10,-20
        },
        //King
        { -80, -70, -70, -70, -70, -70, -70, -80,
            -60, -60, -60, -60, -60, -60, -60, -60,
            -40, -50, -50, -60, -60, -50, -50, -40,
            -30, -40, -40, -50, -50, -40, -40, -30,
            -20, -30, -30, -40, -40, -30, -30, -20,
            -10, -20, -20, -20, -20, -20, -20, -10,
            20, 20, -5, -5, -5, -5, 20, 20,
            20, 30, 10, 0, 0, 10, 30, 20
        },
        //King End
        /*{ -20, -10, -10, -10, -10, -10, -10, -20,
            -5, 0, 5, 5, 5, 5, 0, -5,
            -10, -5, 20, 30, 30, 20, -5, -10,
            -15, -10, 35, 45, 45, 35, -10, -15,
            -20, -15, 30, 40, 40, 30, -15, -20,
            -25, -20, 20, 25, 25, 20, -20, -25,
            -30, -25, 0, 0, 0, 0, -25, -30,
            -50, -30, -30, -30, -30, -30, -30, -50 
        }*/
};

        readonly int[] phase_weight = { 0, 1, 1, 2, 4, 0 };
        readonly int[] pieceValues = { 100, 300, 350, 500, 900, 10_000 };

        public int Evaluate(Board board, Timer timer)
        {
            int middlegame = 0, endgame = 0, gamephase = 0, sideToMove = 2;
            for (; --sideToMove >= 0;)
            {
                for (int piece = -1, square; ++piece < 6;)
                    for (ulong mask = board.GetPieceBitboard((PieceType)piece + 1, sideToMove > 0); mask != 0;)
                    {
                        // Gamephase, middlegame -> endgame
                        gamephase += phase_weight[piece];

                        // Material and square evaluation
                        square = BitboardHelper.ClearAndGetIndexOfLSB(ref mask) ^ 56 * sideToMove;
                        middlegame += psTables[piece, square] + pieceValues[piece];
                        endgame += psTables[piece, square] + pieceValues[piece];
                    }
                middlegame = -middlegame;
                endgame = -endgame;
            }
            return (middlegame * gamephase + endgame * (24 - gamephase)) / 24 * (board.IsWhiteToMove ? 1 : -1);
        }
    }
}