using ChessChallenge.API;
using System;

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
        ulong[] psTables = { 10747914250, 16121871375, 13971237899, 12896446474, 12895397898, 10746864650, 8601480202, 10747914250, 29019367449, 32243740698, 33318533147, 34392274971, 34392274971, 33318533147, 32243740698, 29019367449, 36542908449, 37617699874, 38691441698, 38691441698, 38692490274, 38692491298, 37617699874, 36542908449, 53739571250, 54814362674, 53739571249, 53739571249, 53739571249, 53739571249, 53739571249, 53739571250, 95656436824, 96731228249, 96731228249, 96731228249, 96731228250, 96731228249, 96731228249, 95656436824, 1067267885024, 1068342676450, 1068343726052, 1069418517477, 1070493308902, 1072641842151, 1073716636650, 1074792476650, 10747914250, 19346245650, 16121871375, 13972288525, 12897497100, 11822705675, 11822705675, 10747914250, 29019367449, 32243740698, 33318533147, 34392274971, 34392274971, 33318533147, 32243740698, 29019367449, 36542908449, 37617699874, 38691441698, 38691441698, 38692490274, 38692491298, 37617699874, 36542908449, 53739571250, 54814362674, 53739571249, 53739571249, 53739571249, 53739571249, 53739571249, 53739571250, 95656436824, 96731228249, 96731228249, 96731228249, 96731228250, 96731228249, 96731228249, 95656436824, 1073716633574, 1074791424999, 1078014746599, 1079089536998, 1079089535974, 1076941003749, 1074791421925, 1071567050723 };

        readonly int[] phase_weight = { 0, 1, 1, 2, 4, 0 };

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
                        middlegame += getPS(0, piece, square);
                        endgame += getPS(1, piece, square);
                    }
                middlegame = -middlegame;
                endgame = -endgame;
            }
            return (middlegame * gamephase + endgame * (24 - gamephase)) / 24 * (board.IsWhiteToMove ? 1 : -1);
        }

        private int getPS(int mg, int piece, int square)
        {
            int index = mg * 48 + piece * 8 + square / 8;
            ulong value = psTables[index];
            index = square % 8;
            index = Math.Min(7 - index, index);
            return (int)((1023ul) & (value >> (10 * index))) * 10;
        }
    }
}