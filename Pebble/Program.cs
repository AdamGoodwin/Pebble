namespace Pebble
{
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

    public class Move
    {
        public int moveType;        //Type of move...Ordinary, Capture, En Passant, Kingside Castle, Queenside Castle, Promotion to Queen - Rook - Bishop - Knight
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
        int halfMoves;
        int moves;

        public int[] kingDelta = { 15, 16, 17, -1, 1, -17, -16, -15 };
        public int[] knightDelta = { 14, 31, 33, 18, -14, -31, -33, -18 };
        public int[] bishopDelta = { 15, 17, -17, -15, 0, 0, 0, 0 };
        public int[] rookDelta = { 16, -1, 1, -16, 0, 0, 0, 0 };

        public Board()
        {
            board = new int[128];
            sideToMove = 1; // White will be 1.  Black will be -1.
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

            if (sideToMove == 1)
            {
                colorToMove = (int)Pieces.White;
            }
            else
            {
                colorToMove = (int)Pieces.Black;
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
                                Move move = new Move(0, bishopSquare, bishopDestination, 0);
                                movesList.Add(move);
                                bishopDestination += bishopDelta[i];
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
                                Move move = new Move(0, bishopSquare, bishopDestination, 0);
                                movesList.Add(move);
                                bishopDestination += bishopDelta[i];
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
                                Move move = new Move(0, knightSquare, knightDestination, 0);
                                movesList.Add(move);
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
                                Move move = new Move(0, knightSquare, knightDestination, 0);
                                movesList.Add(move);
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
                        Move move = new Move(0, kingSquare, kingDestination, 0);
                        movesList.Add(move);
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
                        Move move = new Move(0, kingSquare, kingDestination, 0);
                        movesList.Add(move);
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

            for (int stringPosition = 0; stringPosition < position.Count(); stringPosition++)
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
                        int emptySquares = position[stringPosition] - '0';
                        squareIndex += emptySquares;
                        break;
                }

            }
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            string startingPosition = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";
            List<Move> moveList = new List<Move>();
            Console.WriteLine("What position do you want to set up to play?");

            Board newGame = new Board();
            newGame.setBoard(startingPosition);

            moveList = newGame.generateMoves();

            Console.WriteLine(moveList[0].start);
            Console.WriteLine(moveList[0].destination);

            Console.WriteLine(moveList[1].start);
            Console.WriteLine(moveList[1].destination);
            
            Console.WriteLine(moveList[2].start);
            Console.WriteLine(moveList[2].destination);
            
            Console.WriteLine(moveList[3].start);
            Console.WriteLine(moveList[3].destination);

            Console.WriteLine(moveList[4].start);
            Console.WriteLine(moveList[4].destination);
            
            Console.WriteLine(moveList[5].start);
            Console.WriteLine(moveList[5].destination);
            
            Console.WriteLine(moveList[6].start);
            Console.WriteLine(moveList[6].destination);
            
            Console.WriteLine(moveList[7].start);
            Console.WriteLine(moveList[7].destination);
            
            Console.WriteLine(moveList[8].start);
            Console.WriteLine(moveList[8].destination);
            
            Console.WriteLine(moveList[9].start);
            Console.WriteLine(moveList[9].destination);
            
            Console.WriteLine(moveList[10].start);
            Console.WriteLine(moveList[10].destination);
            
            Console.WriteLine(moveList[11].start);
            Console.WriteLine(moveList[11].destination);
            
            Console.WriteLine(moveList[12].start);
            Console.WriteLine(moveList[12].destination);
            
            Console.WriteLine(moveList[13].start);
            Console.WriteLine(moveList[13].destination);
            
            Console.ReadKey();
        }
    }
}