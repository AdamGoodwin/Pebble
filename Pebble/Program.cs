using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;

namespace Pebble
{
    public enum CastlingRights
    {
        castleNone = 0,
        castleKingside = 1,
        castleQueenside = 2,
        castleBoth = 3
    };
   
    public enum Pieces
    {
        Empty = 0,
        Pawn = 1,
        Knight = 2,
        King = 3,
        Bishop = 5,
        Rook = 6,
        Queen = 7,
        White = 8,
        Black = 16
    };

    public enum MoveType
    {
        Regular = 0,
        Capture,
        EnPassant,
        KingsideCastle,
        QueensideCastle,
        PromoteQueen,
        PromoteRook,
        PromoteBishop,
        PromoteKnight,
        PromoteCaptureQueen,
        PromoteCaptureRook,
        PromoteCaptureBishop,
        PromoteCaptureKnight
    };

    public class Move
    {
        public int moveType;        //Type of move...Regular, Capture, En Passant, Kingside Castle, Queenside Castle, Promotion to Queen - Rook - Bishop - Knight
        public int start;           //Starting square
        public int destination;     //Ending square
        public int capture;         //What was captured?

        public Move(int moveType, int start, int destination, int capture)
        {
            this.moveType = moveType;
            this.start = start;
            this.destination = destination;
            this.capture = capture;
        }
    }

    public class Board
    {
        int[] board;
        int sideToMove;
        int enPassantSquare;
        int whiteCastlingRights;
        int blackCastlingRights;
        int halfMoves;
        int moves;

        public int[] kingDelta = { 15, 16, 17, -1, 1, -17, -16, -15 };
        public int[] knightDelta = { 14, 31, 33, 18, -14, -31, -33, -18 };
        public int[] bishopDelta = { 15, 17, -17, -15, 0, 0, 0, 0 };
        public int[] rookDelta = { 16, -1, 1, -16, 0, 0, 0, 0 };
        public int[] queenDelta = { 15, 16, 17, -1, 1, -17, -16, -15 };
        public int[] whitePawnDelta = { 16, 32, 15, 17, 0, 0, 0, 0 };
        public int[] blackPawnDelta = { -16, -32, -15, -17, 0, 0, 0, 0 };

        public Board()
        {
            board = new int[128];
            sideToMove = 0; // White will be 1.  Black will be -1.
            enPassantSquare = -1;
            halfMoves = 0;
            moves = 0;
        }

        public List<Move> generateMoves()
        {
            List<Move> movesList = new List<Move>();
            int colorToMove = 0;
            int kingSquare = -1;
            int knightSquare = -1;
            int bishopSquare = -1;
            int rookSquare = -1;
            int queenSquare = -1;
            int whitePawnSquare = -1;
            int blackPawnSquare = -1;

            if (sideToMove == 1)
            {
                colorToMove = (int)Pieces.White;
            }
            else
            {
                colorToMove = (int)Pieces.Black;
            }

            //Pawns for each side
            if ((colorToMove & (int)Pieces.White) != 0)
            {//White Pawns
                for (int squareIndex = 16; squareIndex < 104; squareIndex++)
                {
                    if (board[squareIndex] == (int)Pieces.Pawn + (int)Pieces.White)
                    {
                        whitePawnSquare = squareIndex;
                        int rank = getRank(squareIndex);
                        if (rank == 2)
                        {
                            //Regular Pawn Moves
                            for (int i = 0; i <= 1; i++)
                            {
                                Move move;
                                int pawnDestination = whitePawnSquare + whitePawnDelta[i];
                                if (board[pawnDestination] == (int)Pieces.Empty)
                                {
                                    move = new Move((int)MoveType.Regular, whitePawnSquare, pawnDestination, 0);
                                    movesList.Add(move);
                                }
                                else //There must be a piece in the way
                                {
                                    break;
                                }
                            }

                            //Captures
                            for(int i = 2; i <= 3; i++)
                            {
                                Move move;
                                int pawnDestination = whitePawnSquare + whitePawnDelta[i];
                                if ((board[pawnDestination] & 0x88) == 0 && (board[pawnDestination] & (int)Pieces.Black) != 0)
                                {
                                    int capturedPiece = 0;
                                    switch (board[pawnDestination])
                                    {
                                        case (int)Pieces.Black + (int)Pieces.Pawn:
                                            capturedPiece = (int)Pieces.Black + (int)Pieces.Pawn;
                                            break;
                                        case (int)Pieces.Black + (int)Pieces.Knight:
                                            capturedPiece = (int)Pieces.Black + (int)Pieces.Knight;
                                            break;
                                        case (int)Pieces.Black + (int)Pieces.Bishop:
                                            capturedPiece = (int)Pieces.Black + (int)Pieces.Bishop;
                                            break;
                                        case (int)Pieces.Black + (int)Pieces.Rook:
                                            capturedPiece = (int)Pieces.Black + (int)Pieces.Rook;
                                            break;
                                        case (int)Pieces.White + (int)Pieces.Queen:
                                            capturedPiece = (int)Pieces.Black + (int)Pieces.Queen;
                                            break;
                                    }
                                    move = new Move((int)MoveType.Capture, whitePawnSquare, pawnDestination, capturedPiece);
                                    movesList.Add(move);
                                }
                            }
                        }
                        else if(rank >= 3 && rank <= 6)
                        {
                            //Regular Pawn Move
                            Move move;
                            int pawnDestination = whitePawnSquare + whitePawnDelta[0];
                            if (board[pawnDestination] == (int)Pieces.Empty)
                            {
                                move = new Move((int)MoveType.Regular, whitePawnSquare, pawnDestination, 0);
                                movesList.Add(move);
                            }
                            else //There must be a piece in the way
                            {
                            }

                            //Captures
                            for (int i = 2; i <= 3; i++)
                            {
                                pawnDestination = whitePawnSquare + whitePawnDelta[i];
                                if ((board[pawnDestination] & 0x88) == 0 && (board[pawnDestination] & (int)Pieces.Black) != 0)
                                {
                                    int capturedPiece = 0;
                                    switch (board[pawnDestination])
                                    {
                                        case (int)Pieces.Black + (int)Pieces.Pawn:
                                            capturedPiece = (int)Pieces.Black + (int)Pieces.Pawn;
                                            break;
                                        case (int)Pieces.Black + (int)Pieces.Knight:
                                            capturedPiece = (int)Pieces.Black + (int)Pieces.Knight;
                                            break;
                                        case (int)Pieces.Black + (int)Pieces.Bishop:
                                            capturedPiece = (int)Pieces.Black + (int)Pieces.Bishop;
                                            break;
                                        case (int)Pieces.Black + (int)Pieces.Rook:
                                            capturedPiece = (int)Pieces.Black + (int)Pieces.Rook;
                                            break;
                                        case (int)Pieces.White + (int)Pieces.Queen:
                                            capturedPiece = (int)Pieces.Black + (int)Pieces.Queen;
                                            break;
                                    }
                                    move = new Move((int)MoveType.Capture, whitePawnSquare, pawnDestination, capturedPiece);
                                    movesList.Add(move);
                                }
                            }
                        }
                        else if(rank == 7) //Promotion is next rank
                        {
                            //Reg Pawn Moves
                            Move move;
                            int pawnDestination = whitePawnSquare + whitePawnDelta[0];
                            if (board[pawnDestination] == (int)Pieces.Empty)
                            {
                                move = new Move((int)MoveType.PromoteQueen, whitePawnSquare, pawnDestination, 0);
                                movesList.Add(move);
                                move = new Move((int)MoveType.PromoteRook, whitePawnSquare, pawnDestination, 0);
                                movesList.Add(move);
                                move = new Move((int)MoveType.PromoteBishop, whitePawnSquare, pawnDestination, 0);
                                movesList.Add(move);
                                move = new Move((int)MoveType.PromoteKnight, whitePawnSquare, pawnDestination, 0);
                                movesList.Add(move);
                            }

                            //Captures
                            for (int i = 2; i <= 3; i++)
                            {
                                pawnDestination = whitePawnSquare + whitePawnDelta[i];
                                if ((board[pawnDestination] & 0x88) == 0 && (board[pawnDestination] & (int)Pieces.Black) != 0)
                                {
                                    int capturedPiece = 0;
                                    switch (board[pawnDestination])
                                    {
                                        case (int)Pieces.Black + (int)Pieces.Knight:
                                            capturedPiece = (int)Pieces.Black + (int)Pieces.Knight;
                                            break;
                                        case (int)Pieces.Black + (int)Pieces.Bishop:
                                            capturedPiece = (int)Pieces.Black + (int)Pieces.Bishop;
                                            break;
                                        case (int)Pieces.Black + (int)Pieces.Rook:
                                            capturedPiece = (int)Pieces.Black + (int)Pieces.Rook;
                                            break;
                                        case (int)Pieces.White + (int)Pieces.Queen:
                                            capturedPiece = (int)Pieces.Black + (int)Pieces.Queen;
                                            break;
                                    }
                                    move = new Move((int)MoveType.PromoteCaptureQueen, whitePawnSquare, pawnDestination, capturedPiece);
                                    movesList.Add(move);
                                    move = new Move((int)MoveType.PromoteCaptureRook, whitePawnSquare, pawnDestination, capturedPiece);
                                    movesList.Add(move);
                                    move = new Move((int)MoveType.PromoteCaptureBishop, whitePawnSquare, pawnDestination, capturedPiece);
                                    movesList.Add(move);
                                    move = new Move((int)MoveType.PromoteCaptureKnight, whitePawnSquare, pawnDestination, capturedPiece);
                                    movesList.Add(move);
                                }
                            }
                        }
                        if (rank == 5)
                        { //En Passant
                            if (enPassantSquare - whitePawnSquare == whitePawnDelta[2])
                            {
                                int pawnDestination = whitePawnSquare + whitePawnDelta[2];
                                int capturedPiece = (int)Pieces.Black + (int)Pieces.Pawn;
                                Move move = new Move((int)MoveType.EnPassant, whitePawnSquare, pawnDestination, capturedPiece);
                                movesList.Add(move);
                            }
                            if (enPassantSquare - whitePawnSquare == whitePawnDelta[3])
                            {
                                int pawnDestination = whitePawnSquare + whitePawnDelta[3];
                                int capturedPiece = (int)Pieces.Black + (int)Pieces.Pawn;
                                Move move = new Move((int)MoveType.EnPassant, whitePawnSquare, pawnDestination, capturedPiece);
                                movesList.Add(move);
                            }
                        }
                    }
                }
            }
            else
            {//Black Pawns
                for (int squareIndex = 16; squareIndex < 104; squareIndex++)
                {
                    if (board[squareIndex] == (int)Pieces.Pawn + (int)Pieces.Black)
                    {
                        blackPawnSquare = squareIndex;
                        int rank = getRank(squareIndex);
                        if (rank == 7)
                        {
                            for (int i = 0; i <= 1; i++)
                            {
                                int pawnDestination = blackPawnSquare + blackPawnDelta[i];
                                if(board[pawnDestination] == (int)Pieces.Empty)
                                {
                                    Move move = new Move(0, blackPawnSquare, pawnDestination, 0);
                                    movesList.Add(move);
                                }
                                else  //Must have hit a piece in the way
                                {
                                    break;
                                }
                            }
                            //Captures
                            for (int i = 2; i <= 3; i++)
                            {
                                Move move;
                                int pawnDestination = blackPawnSquare + blackPawnDelta[i];
                                if ((board[pawnDestination] & 0x88) == 0 && (board[pawnDestination] & (int)Pieces.White) != 0)
                                {
                                    move = new Move((int)MoveType.Capture, blackPawnSquare, pawnDestination, 0);
                                    movesList.Add(move);
                                }
                            }
                        }
                        else if(rank <= 6 && rank >= 3)
                        {
                            //Regular Pawn Moves
                            int pawnDestination = blackPawnSquare + blackPawnDelta[0];
                            if (board[pawnDestination] == (int)Pieces.Empty)
                            {
                                Move move = new Move(0, blackPawnSquare, pawnDestination, 0);
                                movesList.Add(move);
                            }
                            else
                            {
                            }

                            //Captures
                            for (int i = 2; i <= 3; i++)
                            {
                                Move move;
                                pawnDestination = blackPawnSquare + blackPawnDelta[i];
                                if ((board[pawnDestination] & 0x88) == 0 && (board[pawnDestination] & (int)Pieces.White) != 0)
                                {
                                    move = new Move((int)MoveType.Capture, blackPawnSquare, pawnDestination, 0);
                                    movesList.Add(move);
                                }
                            }
                        }
                        else if (rank == 2) //Promotion is next rank
                        {
                            //Reg Pawn Moves
                            Move move;
                            int pawnDestination = blackPawnSquare + blackPawnDelta[0];
                            if (board[pawnDestination] == (int)Pieces.Empty)
                            {
                                move = new Move((int)MoveType.PromoteQueen, blackPawnSquare, pawnDestination, 0);
                                movesList.Add(move);
                                move = new Move((int)MoveType.PromoteRook, blackPawnSquare, pawnDestination, 0);
                                movesList.Add(move);
                                move = new Move((int)MoveType.PromoteBishop, blackPawnSquare, pawnDestination, 0);
                                movesList.Add(move);
                                move = new Move((int)MoveType.PromoteKnight, blackPawnSquare, pawnDestination, 0);
                                movesList.Add(move);
                            }

                            //Captures
                            for (int i = 2; i <= 3; i++)
                            {
                                pawnDestination = blackPawnSquare + blackPawnDelta[i];
                                if ((board[pawnDestination] & 0x88) == 0 && (board[pawnDestination] & (int)Pieces.White) != 0)
                                {
                                    int capturedPiece = 0;
                                    switch (board[pawnDestination])
                                    {
                                        case (int)Pieces.White + (int)Pieces.Knight:
                                            capturedPiece = (int)Pieces.White + (int)Pieces.Knight;
                                            break;
                                        case (int)Pieces.White + (int)Pieces.Bishop:
                                            capturedPiece = (int)Pieces.White + (int)Pieces.Bishop;
                                            break;
                                        case (int)Pieces.White + (int)Pieces.Rook:
                                            capturedPiece = (int)Pieces.White + (int)Pieces.Rook;
                                            break;
                                        case (int)Pieces.White + (int)Pieces.Queen:
                                            capturedPiece = (int)Pieces.White + (int)Pieces.Queen;
                                            break;
                                    }
                                    move = new Move((int)MoveType.PromoteCaptureQueen, blackPawnSquare, pawnDestination, capturedPiece);
                                    movesList.Add(move);
                                    move = new Move((int)MoveType.PromoteCaptureRook, blackPawnSquare, pawnDestination, capturedPiece);
                                    movesList.Add(move);
                                    move = new Move((int)MoveType.PromoteCaptureBishop, blackPawnSquare, pawnDestination, capturedPiece);
                                    movesList.Add(move);
                                    move = new Move((int)MoveType.PromoteCaptureKnight, blackPawnSquare, pawnDestination, capturedPiece);
                                    movesList.Add(move);
                                }
                            }
                        }
                        if (rank == 4)
                        {
                            if (enPassantSquare - blackPawnSquare == -15)
                            {
                                int pawnDestination = blackPawnSquare + blackPawnDelta[2];
                                int capturedPiece = (int)Pieces.White + (int)Pieces.Pawn;
                                Move move = new Move((int)MoveType.EnPassant, blackPawnSquare, pawnDestination, capturedPiece);
                                movesList.Add(move);
                            }
                            if (enPassantSquare - blackPawnSquare == -17)
                            {
                                int pawnDestination = blackPawnSquare + blackPawnDelta[3];
                                int capturedPiece = (int)Pieces.White + (int)Pieces.Pawn;
                                Move move = new Move((int)MoveType.EnPassant, blackPawnSquare, pawnDestination, capturedPiece);
                                movesList.Add(move);
                            }
                        }
                    }
                }
            }

            //Queen for each side
            if ((colorToMove & (int)Pieces.White) != 0)
            {//White Queen
                for (int squareIndex = 0; squareIndex < 120; squareIndex++)
                {
                    if (board[squareIndex] == (int)Pieces.Queen + (int)Pieces.White)
                    {
                        queenSquare = squareIndex;

                        for (int i = 0; i < 8; i++)
                        {
                            int queenDestination = queenSquare + queenDelta[i];
                            while ((queenDestination & 0x88) == 0)
                            {
                                Move move;
                                if ((board[queenDestination] != (int)Pieces.Empty))
                                {
                                    if ((board[queenDestination] & (int)Pieces.Black) != 0)
                                    {
                                        int capturedPiece = 0;
                                        switch (board[queenDestination])
                                        {
                                            case (int)Pieces.Black + (int)Pieces.Pawn:
                                                capturedPiece = (int)Pieces.Black + (int)Pieces.Pawn;
                                                break;
                                            case (int)Pieces.Black + (int)Pieces.Knight:
                                                capturedPiece = (int)Pieces.Black + (int)Pieces.Knight;
                                                break;
                                            case (int)Pieces.Black + (int)Pieces.Bishop:
                                                capturedPiece = (int)Pieces.Black + (int)Pieces.Bishop;
                                                break;
                                            case (int)Pieces.Black + (int)Pieces.Rook:
                                                capturedPiece = (int)Pieces.Black + (int)Pieces.Rook;
                                                break;
                                            case (int)Pieces.Black + (int)Pieces.Queen:
                                                capturedPiece = (int)Pieces.Black + (int)Pieces.Queen;
                                                break;
                                        }
                                        move = new Move((int)MoveType.Capture, queenSquare, queenDestination, capturedPiece);
                                        movesList.Add(move);
                                        break;
                                    }
                                    else
                                    {
                                        break; //We've hit a piece of the same color
                                    }
                                }
                                else //Square is empty
                                {
                                    move = new Move((int)MoveType.Regular, queenSquare, queenDestination, 0);
                                    movesList.Add(move);
                                    queenDestination += queenDelta[i];
                                }
                            }
                        }
                    }
                }
            }
            else
            {//Black Queen
                for (int squareIndex = 0; squareIndex < 120; squareIndex++)
                {
                    if (board[squareIndex] == (int)Pieces.Queen + (int)Pieces.Black)
                    {
                        queenSquare = squareIndex;

                        for (int i = 0; i < 8; i++)
                        {
                            int queenDestination = queenSquare + queenDelta[i];
                            while ((queenDestination & 0x88) == 0)
                            {
                                Move move;
                                if ((board[queenDestination] != (int)Pieces.Empty))
                                {
                                    if ((board[queenDestination] & (int)Pieces.White) != 0)
                                    {
                                        int capturedPiece = 0;
                                        switch (board[queenDestination])
                                        {
                                            case (int)Pieces.White + (int)Pieces.Pawn:
                                                capturedPiece = (int)Pieces.White + (int)Pieces.Pawn;
                                                break;
                                            case (int)Pieces.White + (int)Pieces.Knight:
                                                capturedPiece = (int)Pieces.White + (int)Pieces.Knight;
                                                break;
                                            case (int)Pieces.White + (int)Pieces.Bishop:
                                                capturedPiece = (int)Pieces.White + (int)Pieces.Bishop;
                                                break;
                                            case (int)Pieces.White + (int)Pieces.Rook:
                                                capturedPiece = (int)Pieces.White + (int)Pieces.Rook;
                                                break;
                                            case (int)Pieces.White + (int)Pieces.Queen:
                                                capturedPiece = (int)Pieces.White + (int)Pieces.Queen;
                                                break;
                                        }
                                        move = new Move((int)MoveType.Capture, queenSquare, queenDestination, capturedPiece);
                                        movesList.Add(move);
                                        break;
                                    }
                                    else
                                    {
                                        break; //We've hit a piece of the same color
                                    }
                                }
                                else //Square is empty
                                {
                                    move = new Move((int)MoveType.Regular, queenSquare, queenDestination, 0);
                                    movesList.Add(move);
                                    queenDestination += queenDelta[i];
                                }
                            }
                        }
                    }
                }
            }

            //Rooks for each side
            if ((colorToMove & (int)Pieces.White) != 0)
            {//White Rooks
                for (int squareIndex = 0; squareIndex < 120; squareIndex++)
                {
                    if (board[squareIndex] == (int)Pieces.Rook + (int)Pieces.White)
                    {
                        rookSquare = squareIndex;

                        for (int i = 0; i < 4; i++)
                        {
                            int rookDestination = rookSquare + rookDelta[i];
                            while ((rookDestination & 0x88) == 0)
                            {
                                Move move;
                                if ((board[rookDestination] != (int)Pieces.Empty))
                                {
                                    if ((board[rookDestination] & (int)Pieces.Black) != 0)
                                    {
                                        int capturedPiece = 0;
                                        switch (board[rookDestination])
                                        {
                                            case (int)Pieces.Black + (int)Pieces.Pawn:
                                                capturedPiece = (int)Pieces.Black + (int)Pieces.Pawn;
                                                break;
                                            case (int)Pieces.Black + (int)Pieces.Knight:
                                                capturedPiece = (int)Pieces.Black + (int)Pieces.Knight;
                                                break;
                                            case (int)Pieces.Black + (int)Pieces.Bishop:
                                                capturedPiece = (int)Pieces.Black + (int)Pieces.Bishop;
                                                break;
                                            case (int)Pieces.Black + (int)Pieces.Rook:
                                                capturedPiece = (int)Pieces.Black + (int)Pieces.Rook;
                                                break;
                                            case (int)Pieces.Black + (int)Pieces.Queen:
                                                capturedPiece = (int)Pieces.Black + (int)Pieces.Queen;
                                                break;
                                        }
                                        move = new Move((int)MoveType.Capture, rookSquare, rookDestination, capturedPiece);
                                        movesList.Add(move);
                                        break;
                                    }
                                    else
                                    {
                                        break; //We've hit a piece of the same color
                                    }
                                }
                                else //Square is empty
                                {
                                    move = new Move((int)MoveType.Regular, rookSquare, rookDestination, 0);
                                    movesList.Add(move);
                                    rookDestination += rookDelta[i];
                                }
                            }
                        }
                    }
                }
            }
            else
            {//Black Rooks
                for (int squareIndex = 0; squareIndex < 120; squareIndex++)
                {
                    if (board[squareIndex] == (int)Pieces.Rook + (int)Pieces.Black)
                    {
                        rookSquare = squareIndex;

                        for (int i = 0; i < 8; i++)
                        {
                            int rookDestination = rookSquare + rookDelta[i];
                            while ((rookDestination & 0x88) == 0)
                            {
                                Move move;
                                if ((board[rookDestination] != (int)Pieces.Empty))
                                {
                                    if ((board[rookDestination] & (int)Pieces.White) != 0)
                                    {
                                        int capturedPiece = 0;
                                        switch (board[rookDestination])
                                        {
                                            case (int)Pieces.White + (int)Pieces.Pawn:
                                                capturedPiece = (int)Pieces.White + (int)Pieces.Pawn;
                                                break;
                                            case (int)Pieces.White + (int)Pieces.Knight:
                                                capturedPiece = (int)Pieces.White + (int)Pieces.Knight;
                                                break;
                                            case (int)Pieces.White + (int)Pieces.Bishop:
                                                capturedPiece = (int)Pieces.White + (int)Pieces.Bishop;
                                                break;
                                            case (int)Pieces.White + (int)Pieces.Rook:
                                                capturedPiece = (int)Pieces.White + (int)Pieces.Rook;
                                                break;
                                            case (int)Pieces.White + (int)Pieces.Queen:
                                                capturedPiece = (int)Pieces.White + (int)Pieces.Queen;
                                                break;
                                        }
                                        move = new Move((int)MoveType.Capture, rookSquare, rookDestination, capturedPiece);
                                        movesList.Add(move);
                                        break;
                                    }
                                    else
                                    {
                                        break; //We've hit a piece of the same color
                                    }
                                }
                                else //Square is empty
                                {
                                    move = new Move((int)MoveType.Regular, rookSquare, rookDestination, 0);
                                    movesList.Add(move);
                                    rookDestination += rookDelta[i];
                                }
                            }
                        }
                    }
                }
            }

            //Bishops for each side
            if ((colorToMove & (int)Pieces.White) != 0)
            {//White Bishops
                for (int squareIndex = 0; squareIndex < 120; squareIndex++)
                {
                    if (board[squareIndex] == (int)Pieces.Bishop + (int)Pieces.White)
                    {
                        bishopSquare = squareIndex;

                        for (int i = 0; i < 4; i++)
                        {
                            int bishopDestination = bishopSquare + bishopDelta[i];
                            while ((bishopDestination & 0x88) == 0)
                            {
                                Move move;
                                if ((board[bishopDestination] != (int)Pieces.Empty))
                                {
                                    if ((board[bishopDestination] & (int)Pieces.Black) != 0)
                                    {
                                        int capturedPiece = 0;
                                        switch (board[bishopDestination])
                                        {
                                            case (int)Pieces.Black + (int)Pieces.Pawn:
                                                capturedPiece = (int)Pieces.Black + (int)Pieces.Pawn;
                                                break;
                                            case (int)Pieces.Black + (int)Pieces.Knight:
                                                capturedPiece = (int)Pieces.Black + (int)Pieces.Knight;
                                                break;
                                            case (int)Pieces.Black + (int)Pieces.Bishop:
                                                capturedPiece = (int)Pieces.Black + (int)Pieces.Bishop;
                                                break;
                                            case (int)Pieces.Black + (int)Pieces.Rook:
                                                capturedPiece = (int)Pieces.Black + (int)Pieces.Rook;
                                                break;
                                            case (int)Pieces.Black + (int)Pieces.Queen:
                                                capturedPiece = (int)Pieces.Black + (int)Pieces.Queen;
                                                break;
                                        }
                                        move = new Move((int)MoveType.Capture, bishopSquare, bishopDestination, capturedPiece);
                                        movesList.Add(move);
                                        break;
                                    }
                                    else
                                    {
                                        break; //We've hit a piece of the same color
                                    }
                                }
                                else //Square is empty
                                {
                                    move = new Move((int)MoveType.Regular, bishopSquare, bishopDestination, 0);
                                    movesList.Add(move);
                                    bishopDestination += bishopDelta[i];
                                }
                            }
                        }
                    }
                }
            }
            else
            {//Black Bishops
                for (int squareIndex = 0; squareIndex < 120; squareIndex++)
                {
                    if (board[squareIndex] == (int)Pieces.Bishop + (int)Pieces.Black)
                    {
                        bishopSquare = squareIndex;

                        for (int i = 0; i < 8; i++)
                        {
                            int bishopDestination = bishopSquare + bishopDelta[i];
                            while ((bishopDestination & 0x88) == 0)
                            {
                                Move move;
                                if ((board[bishopDestination] != (int)Pieces.Empty))
                                {
                                    if ((board[bishopDestination] & (int)Pieces.White) != 0)
                                    {
                                        int capturedPiece = 0;
                                        switch (board[bishopDestination])
                                        {
                                            case (int)Pieces.White + (int)Pieces.Pawn:
                                                capturedPiece = (int)Pieces.White + (int)Pieces.Pawn;
                                                break;
                                            case (int)Pieces.White + (int)Pieces.Knight:
                                                capturedPiece = (int)Pieces.White + (int)Pieces.Knight;
                                                break;
                                            case (int)Pieces.White + (int)Pieces.Bishop:
                                                capturedPiece = (int)Pieces.White + (int)Pieces.Bishop;
                                                break;
                                            case (int)Pieces.White + (int)Pieces.Rook:
                                                capturedPiece = (int)Pieces.White + (int)Pieces.Rook;
                                                break;
                                            case (int)Pieces.White + (int)Pieces.Queen:
                                                capturedPiece = (int)Pieces.White + (int)Pieces.Queen;
                                                break;
                                        }
                                        move = new Move((int)MoveType.Capture, bishopSquare, bishopDestination, capturedPiece);
                                        movesList.Add(move);
                                        break;
                                    }
                                    else
                                    {
                                        break; //We've hit a piece of the same color
                                    }
                                }
                                else //Square is empty
                                {
                                    move = new Move((int)MoveType.Regular, bishopSquare, bishopDestination, 0);
                                    movesList.Add(move);
                                    bishopDestination += bishopDelta[i];
                                }
                            }
                        }
                    }
                }
            }

            //Knights for each side
            if ((colorToMove & (int)Pieces.White) != 0)
            {//White
                for (int squareIndex = 0; squareIndex < 120; squareIndex++)
                {
                    if (board[squareIndex] == (int)Pieces.Knight + (int)Pieces.White)
                    {
                        knightSquare = squareIndex;

                        for (int i = 0; i < 8; i++)
                        {
                            int knightDestination = knightSquare + knightDelta[i];
                            if ((knightDestination & 0x88) == 0)
                            {
                                Move move;
                                if (board[knightDestination] != (int)Pieces.Empty)
                                {
                                    if ((board[knightDestination] & (int)Pieces.Black) != 0)
                                    {
                                        int capturedPiece = 0;
                                        switch (board[knightDestination])
                                        {
                                            case (int)Pieces.Black + (int)Pieces.Pawn:
                                                capturedPiece = (int)Pieces.Black + (int)Pieces.Pawn;
                                                break;
                                            case (int)Pieces.Black + (int)Pieces.Knight:
                                                capturedPiece = (int)Pieces.Black + (int)Pieces.Knight;
                                                break;
                                            case (int)Pieces.Black + (int)Pieces.Bishop:
                                                capturedPiece = (int)Pieces.Black + (int)Pieces.Bishop;
                                                break;
                                            case (int)Pieces.Black + (int)Pieces.Rook:
                                                capturedPiece = (int)Pieces.Black + (int)Pieces.Rook;
                                                break;
                                            case (int)Pieces.White + (int)Pieces.Queen:
                                                capturedPiece = (int)Pieces.Black + (int)Pieces.Queen;
                                                break;
                                        }
                                        move = new Move((int)MoveType.Capture, knightSquare, knightDestination, capturedPiece);
                                        movesList.Add(move);
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }
                                else
                                {
                                    move = new Move((int)MoveType.Regular, knightSquare, knightDestination, 0);
                                    movesList.Add(move);
                                }
                            }
                        }
                    }
                }
            }
            else
            {//Black
                for (int squareIndex = 0; squareIndex < 120; squareIndex++)
                {
                    if (board[squareIndex] == (int)Pieces.Knight + (int)Pieces.Black)
                    {
                        knightSquare = squareIndex;

                        for (int i = 0; i < 8; i++)
                        {
                            int knightDestination = knightSquare + knightDelta[i];
                            if ((knightDestination & 0x88) == 0)
                            {
                                Move move;
                                if (board[knightDestination] != (int)Pieces.Empty)
                                {
                                    if ((board[knightDestination] & (int)Pieces.White) != 0)
                                    {
                                        int capturedPiece = 0;
                                        switch (board[knightDestination])
                                        {
                                            case (int)Pieces.White + (int)Pieces.Pawn:
                                                capturedPiece = (int)Pieces.White + (int)Pieces.Pawn;
                                                break;
                                            case (int)Pieces.White + (int)Pieces.Knight:
                                                capturedPiece = (int)Pieces.White + (int)Pieces.Knight;
                                                break;
                                            case (int)Pieces.White + (int)Pieces.Bishop:
                                                capturedPiece = (int)Pieces.White + (int)Pieces.Bishop;
                                                break;
                                            case (int)Pieces.White + (int)Pieces.Rook:
                                                capturedPiece = (int)Pieces.White + (int)Pieces.Rook;
                                                break;
                                            case (int)Pieces.White + (int)Pieces.Queen:
                                                capturedPiece = (int)Pieces.White + (int)Pieces.Queen;
                                                break;
                                        }
                                        move = new Move((int)MoveType.Capture, knightSquare, knightDestination, capturedPiece);
                                        movesList.Add(move);
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }
                                else
                                {
                                    move = new Move((int)MoveType.Regular, knightSquare, knightDestination, 0);
                                    movesList.Add(move);
                                }
                            }
                        }
                    }
                }
            }

            //Kings for each side
            if ((colorToMove & (int)Pieces.White) != 0)
            {//Looking for the White King
                for (int squareIndex = 0; squareIndex < 120; squareIndex++)
                {
                    if (board[squareIndex] == (int)Pieces.King + (int)Pieces.White)
                    {
                        kingSquare = squareIndex;
                        break;
                    }
                }

                for(int i = 0; i < 8; i++)
                {
                    int kingDestination = kingSquare + kingDelta[i];
                    if ((kingDestination & 0x88) == 0)
                    {
                        Move move;
                        if (board[kingDestination] != (int)Pieces.Empty)
                        {
                            if ((board[kingDestination] & (int)Pieces.Black) != 0)
                            {
                                int capturedPiece = 0;
                                switch (board[kingDestination])
                                {
                                    case (int)Pieces.Black + (int)Pieces.Pawn:
                                        capturedPiece = (int)Pieces.Black + (int)Pieces.Pawn;
                                        break;
                                    case (int)Pieces.Black + (int)Pieces.Knight:
                                        capturedPiece = (int)Pieces.Black + (int)Pieces.Knight;
                                        break;
                                    case (int)Pieces.Black + (int)Pieces.Bishop:
                                        capturedPiece = (int)Pieces.Black + (int)Pieces.Bishop;
                                        break;
                                    case (int)Pieces.Black + (int)Pieces.Rook:
                                        capturedPiece = (int)Pieces.Black + (int)Pieces.Rook;
                                        break;
                                    case (int)Pieces.Black + (int)Pieces.Queen:
                                        capturedPiece = (int)Pieces.Black + (int)Pieces.Queen;
                                        break;
                                }
                                move = new Move((int)MoveType.Capture, kingSquare, kingDestination, capturedPiece);
                                movesList.Add(move);
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            move = new Move((int)MoveType.Regular, kingSquare, kingDestination, 0);
                            movesList.Add(move);
                        }
                    }
                }
            }
            else
            {//Looking for the Black King
                for (int squareIndex = 0; squareIndex < 120; squareIndex++)
                {
                    if (board[squareIndex] == (int)Pieces.King + (int)Pieces.Black)
                    {
                        kingSquare = squareIndex;
                        break;
                    }
                }

                for (int i = 0; i < 8; i++)
                {
                    int kingDestination = kingSquare + kingDelta[i];
                    if ((kingDestination & 0x88) == 0)
                    {
                        Move move;
                        if (board[kingDestination] != (int)Pieces.Empty)
                        {
                            if ((board[kingDestination] & (int)Pieces.White) != 0)
                            {
                                int capturedPiece = 0;
                                switch (board[kingDestination])
                                {
                                    case (int)Pieces.White + (int)Pieces.Pawn:
                                        capturedPiece = (int)Pieces.White + (int)Pieces.Pawn;
                                        break;
                                    case (int)Pieces.White + (int)Pieces.Knight:
                                        capturedPiece = (int)Pieces.White + (int)Pieces.Knight;
                                        break;
                                    case (int)Pieces.White + (int)Pieces.Bishop:
                                        capturedPiece = (int)Pieces.White + (int)Pieces.Bishop;
                                        break;
                                    case (int)Pieces.White + (int)Pieces.Rook:
                                        capturedPiece = (int)Pieces.White + (int)Pieces.Rook;
                                        break;
                                    case (int)Pieces.White + (int)Pieces.Queen:
                                        capturedPiece = (int)Pieces.White + (int)Pieces.Queen;
                                        break;
                                }
                                move = new Move((int)MoveType.Capture, kingSquare, kingDestination, capturedPiece);
                                movesList.Add(move);
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            move = new Move((int)MoveType.Regular, kingSquare, kingDestination, 0);
                            movesList.Add(move);
                        }
                    }
                }
            }
            
            return movesList; 
        }

        public int getFile(int squareIndex)
        {
            if ((squareIndex & 0x88) != 0)
            {
                return -1;  //Not on Board
            }
            return (squareIndex & 7) + 1;
        }

        public int getRank(int squareIndex)
        {
            if ((squareIndex & 0x88) != 0)
            {
                return -1;  //Not on Board
            }
            return (squareIndex >> 4) + 1;
        }

        public string getFEN()
        {
            string position = "";
            int piece = 0;
            int numberEmpty = 0;

            for (int squareIndex = 112; squareIndex <= 120; squareIndex++)
            {
                if (squareIndex == 8)
                {
                    break;
                }

                if ((squareIndex & 0x88) != 0)
                {
                    if (numberEmpty != 0)
                    {
                        position += numberEmpty;
                        numberEmpty = 0;
                    }

                    position += "/";
                    squareIndex -= 25;
                    continue;
                }

                if (board[squareIndex] != 0)
                {
                    int color = board[squareIndex] & 8;
                    if (color == (int)Pieces.White)
                    {
                        piece = board[squareIndex] - (int)Pieces.White;

                        if (numberEmpty != 0)
                        {
                            position += numberEmpty;
                        }

                        switch (piece)
                        {
                            case 1:
                                position += "P";
                                numberEmpty = 0;
                                break;
                            case 2:
                                position += "N";
                                numberEmpty = 0;
                                break;
                            case 3:
                                position += "K";
                                numberEmpty = 0;
                                break;
                            case 5:
                                position += "B";
                                numberEmpty = 0;
                                break;
                            case 6:
                                position += "R";
                                numberEmpty = 0;
                                break;
                            case 7:
                                position += "Q";
                                numberEmpty = 0;
                                break;
                        }
                    }
                    else
                    {
                        piece = board[squareIndex] - (int)Pieces.Black;

                        if (numberEmpty != 0)
                        {
                            position += numberEmpty;
                        }
                            
                        switch (piece)
                        {
                            case 1:
                                position += "p";
                                numberEmpty = 0;
                                break;
                            case 2:
                                position += "n";
                                numberEmpty = 0;
                                break;
                            case 3:
                                position += "k";
                                numberEmpty = 0;
                                break;
                            case 5:
                                position += "b";
                                numberEmpty = 0;
                                break;
                            case 6:
                                position += "r";
                                numberEmpty = 0;
                                break;
                            case 7:
                                position += "q";
                                numberEmpty = 0;
                                break;
                        }
                    }
                }
                else
                {
                    numberEmpty++;
                }

            }

            return position;
        }

        public void setBoard(string position)
        {
            int squareIndex = 112;

            string[] pos = position.Split(' ');
            string currentPos = pos[0];

            for (int stringPosition = 0; stringPosition < pos[0].Count(); stringPosition++)
            {
                switch (position[stringPosition])
                {
                    case 'p':
                        board[squareIndex] = (int)Pieces.Pawn + (int)Pieces.Black;
                        squareIndex++;
                        break;
                    case 'n':
                        board[squareIndex] = (int)Pieces.Knight + (int)Pieces.Black;
                        squareIndex++;
                        break;
                    case 'b':
                        board[squareIndex] = (int)Pieces.Bishop + (int)Pieces.Black;
                        squareIndex++;
                        break;
                    case 'r':
                        board[squareIndex] = (int)Pieces.Rook + (int)Pieces.Black;
                        squareIndex++;
                        break;
                    case 'q':
                        board[squareIndex] = (int)Pieces.Queen + (int)Pieces.Black;
                        squareIndex++;
                        break;
                    case 'k':
                        board[squareIndex] = (int)Pieces.King + (int)Pieces.Black;
                        squareIndex++;
                        break;
                    case 'P':
                        board[squareIndex] = (int)Pieces.Pawn + (int)Pieces.White;
                        squareIndex++;
                        break;
                    case 'N':
                        board[squareIndex] = (int)Pieces.Knight + (int)Pieces.White;
                        squareIndex++;
                        break;
                    case 'B':
                        board[squareIndex] = (int)Pieces.Bishop + (int)Pieces.White;
                        squareIndex++;
                        break;
                    case 'R':
                        board[squareIndex] = (int)Pieces.Rook + (int)Pieces.White;
                        squareIndex++;
                        break;
                    case 'Q':
                        board[squareIndex] = (int)Pieces.Queen + (int)Pieces.White;
                        squareIndex++;
                        break;
                    case 'K':
                        board[squareIndex] = (int)Pieces.King + (int)Pieces.White;
                        squareIndex++;
                        break;
                    case '/':
                        squareIndex -= 24;
                        break;
                    default: //Should be a number
                        int emptySquares = currentPos[stringPosition] - '0';
                        squareIndex += emptySquares;
                        break;
                }

            }

            //Side to Move
            if (pos[1].Equals('w'))
            {
                int sideToMove = 1; //White to Move
            }
            else
            {
                int sideToMove = -1; //Black to Move
            }

            //Castling Right go here
            string castle = pos[2];
            switch (castle)
            {
                case "KQkq":
                    whiteCastlingRights = (int)CastlingRights.castleBoth;
                    blackCastlingRights = (int)CastlingRights.castleBoth;
                    break;
                case "Kkq":
                    whiteCastlingRights = (int)CastlingRights.castleKingside;
                    blackCastlingRights = (int)CastlingRights.castleBoth;
                    break;
                case "Qkq":
                    whiteCastlingRights = (int)CastlingRights.castleQueenside;
                    blackCastlingRights = (int)CastlingRights.castleBoth;
                    break;
                case "kq":
                    whiteCastlingRights = (int)CastlingRights.castleNone;
                    blackCastlingRights = (int)CastlingRights.castleBoth;
                    break;
                case "KQk":
                    whiteCastlingRights = (int)CastlingRights.castleBoth;
                    blackCastlingRights = (int)CastlingRights.castleKingside;
                    break;
                case "KQq":
                    whiteCastlingRights = (int)CastlingRights.castleBoth;
                    blackCastlingRights = (int)CastlingRights.castleQueenside;
                    break;
                case "KQ":
                    whiteCastlingRights = (int)CastlingRights.castleBoth;
                    blackCastlingRights = (int)CastlingRights.castleNone;
                    break;
                case "K":
                    whiteCastlingRights = (int)CastlingRights.castleKingside;
                    blackCastlingRights = (int)CastlingRights.castleNone;
                    break;
                case "Q":
                    whiteCastlingRights = (int)CastlingRights.castleQueenside;
                    blackCastlingRights = (int)CastlingRights.castleNone;
                    break;
                case "k":
                    whiteCastlingRights = (int)CastlingRights.castleNone;
                    blackCastlingRights = (int)CastlingRights.castleKingside;
                    break;
                case "q":
                    whiteCastlingRights = (int)CastlingRights.castleNone;
                    blackCastlingRights = (int)CastlingRights.castleQueenside;
                    break;
                case "Kk":
                    whiteCastlingRights = (int)CastlingRights.castleKingside;
                    blackCastlingRights = (int)CastlingRights.castleKingside;
                    break;
                case "Kq":
                    whiteCastlingRights = (int)CastlingRights.castleKingside;
                    blackCastlingRights = (int)CastlingRights.castleQueenside;
                    break;
                case "Qk":
                    whiteCastlingRights = (int)CastlingRights.castleQueenside;
                    blackCastlingRights = (int)CastlingRights.castleKingside;
                    break;
                case "Qq":
                    whiteCastlingRights = (int)CastlingRights.castleQueenside;
                    blackCastlingRights = (int)CastlingRights.castleQueenside;
                    break;
                default:
                    whiteCastlingRights = (int)CastlingRights.castleNone;
                    blackCastlingRights = (int)CastlingRights.castleNone;
                    break;
            }

            //En Passant
            string enPassant = pos[3];
            switch(enPassant[0])
            {
                case 'a':
                    if (sideToMove == 1)
                        enPassantSquare = 80;
                    else
                        enPassantSquare = 32;
                    break;
                case 'b':
                    if (sideToMove == 1)
                        enPassantSquare = 81;
                    else
                        enPassantSquare = 33;
                    break;
                case 'c':
                    if (sideToMove == 1)
                        enPassantSquare = 82;
                    else
                        enPassantSquare = 34;
                    break;
                case 'd':
                    if (sideToMove == 1)
                        enPassantSquare = 83;
                    else
                        enPassantSquare = 35;
                    break;
                case 'e':
                    if (sideToMove == 1)
                        enPassantSquare = 84;
                    else
                        enPassantSquare = 36;
                    break;
                case 'f':
                    if (sideToMove == 1)
                        enPassantSquare = 85;
                    else
                        enPassantSquare = 37;
                    break;
                case 'g':
                    if (sideToMove == 1)
                        enPassantSquare = 86;
                    else
                        enPassantSquare = 38;
                    break;
                case 'h':
                    if (sideToMove == 1)
                        enPassantSquare = 87;
                    else
                        enPassantSquare = 39;
                    break;
                default:
                    enPassantSquare = -1;
                    break;
            }

            //Half Move Clock
            int halfMoves = Int32.Parse(pos[4]);

            //Number of Full moves
            int moves = Int32.Parse(pos[5]);
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            string startingPosition = "2r3k1/1q1nbppp/r3p3/3pP3/pPpP4/P1Q2N2/2RN1PPP/2R4K b - b3 0 23";
            List<Move> moveList = new List<Move>();
            Console.WriteLine("What position do you want to set up to play?");

            Board newGame = new Board();
            newGame.setBoard(startingPosition);

            moveList = newGame.generateMoves();

            for (int i = 0; i < 36; i++)
            {
                Console.WriteLine(moveList[i].start);
                Console.WriteLine(moveList[i].destination);
            }
            
            
            Console.ReadKey();
        }
    }
}