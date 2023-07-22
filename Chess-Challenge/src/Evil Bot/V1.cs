using System;
using System.Collections.Generic;
using System.Linq;
using ChessChallenge.API;

public class V1 : IChessBot
{
    int[] values = new[] { 0, 1, 3, 3, 4, 9, 10 };
    public Move Think(Board board, Timer timer)
    {
        var best = GetBestMoveRecursive(board);
        return best.move;
    }

    private int GetMoveValue(Board board, Move move, int depth, int curScore = 0)
    {
        if (MoveIsCheckmate(board, move))
        {
            return 200;
        }

        Piece capturedPiece = board.GetPiece(move.TargetSquare);
        curScore += values[(int)capturedPiece.PieceType];

        board.MakeMove(move);
        var enemyMove = GetBestMoveRecursive(board,depth+1);
        curScore -= enemyMove.value;
        board.UndoMove(move);



        return curScore;
    }

    private (Move move,int value) GetBestMoveRecursive(Board board, int depth = 0)
    {
        Move[] moves = board.GetLegalMoves();
        if (depth == 2)
        {
            return (Move.NullMove, 0);
        }

        if (moves.Length == 0)
        {
            return (Move.NullMove, -200);
        }

        var bestMoves = moves.ToList();
        int highestValueMove = int.MinValue;

        foreach (Move move in moves)
        {
            // Always play checkmate in one
            if (MoveIsCheckmate(board, move))
            {
                bestMoves = new List<Move>{move};
                break;
            }

            var moveValue = GetMoveValue(board, move, depth);

            if (moveValue > highestValueMove)
            {
                highestValueMove = moveValue;
                bestMoves = new List<Move>{move};
            }
            else if (moveValue == highestValueMove)
            {
                bestMoves.Add(move);
            }
        }
        return (bestMoves[new Random().Next(bestMoves.Count)],highestValueMove);
    }


    // Test if this move gives checkmate
    private bool MoveIsCheckmate(Board board, Move move)
    {
        board.MakeMove(move);
        bool isMate = board.IsInCheckmate();
        board.UndoMove(move);
        return isMate;
    }
}