using System;

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

    public class Board
    {
        int[] board;
        int sideToMove;
        int enPassantSquare;
        int halfMoves;
        int moves;

        public Board()
        {
            board = new int[128];
            sideToMove = 1; // White will be 1.  Black will be -1.
            enPassantSquare = -1;
            halfMoves = 0;
            moves = 0;
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
            int squareIndex = 0;
            int rank = 0;
            Console.WriteLine("What position do you want to set up to play?");

            Board newGame = new Board();
            Console.WriteLine("Input a square to see what rank it's on");
            squareIndex = Int32.Parse(Console.ReadLine());
            rank = newGame.getFile(squareIndex);
            Console.WriteLine(rank);


            Console.ReadKey();
        }
    }
}