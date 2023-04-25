// See https://aka.ms/new-console-template for more information


//Skeleton Program code for the AQA A Level Paper 1 Summer 2023 examination
//this code should be used in conjunction with the Preliminary Material
//written by the AQA Programmer Team
//developed in the Visual Studio Community Edition programming environment



//namespace Dastan
//{
internal class Program
{
    private static void Main(string[] args)
    {
        var ThisGame = new Dastan(6, 6, 4);
        ThisGame.PlayGame();
        Console.WriteLine("Goodbye!");
        Console.ReadLine();
    }
}

internal class WeatherEvent
{
    private int Location;
    private int countdown;
    private bool IsAlive;

    public WeatherEvent()
    {
        countdown = 4;

        Console.WriteLine("The Weather event will occur in 4 turns ");
    }

    public bool CountDownComplete()
    {
        if (countdown == 0) return true;
        //if not 0 then rest of code executed
        countdown--;
        Console.WriteLine($"The Weather event will occur in {countdown} turns on column {(Location % 10)} ");
        return false;
    }

    public void SetWeatherLocation(int x)
    {
        Location = x;
    }

    public int GetWeatherLocation()
    {
        return Location;
    }
}

internal class Dastan
{
    protected List<Square> Board;
    protected int NoOfRows, NoOfColumns, MoveOptionOfferPosition;
    protected List<Player> Players = new();
    protected List<string> MoveOptionOffer = new();
    protected Player CurrentPlayer;
    protected Random RGen = new();
    protected WeatherEvent wether;

    public Dastan(int R, int C, int NoOfPieces)
    {
        Players.Add(new Player("Player One", 1));
        Players.Add(new Player("Player Two", -1));
        CreateMoveOptions();
        NoOfRows = R;
        NoOfColumns = C;
        MoveOptionOfferPosition = 0;
        CreateMoveOptionOffer();
        CreateBoard();
        CreatePieces(NoOfPieces);
        CurrentPlayer = Players[0];
    }

    private void DisplayBoard()
    {
        Console.Write(Environment.NewLine + "   ");
        for (var Column = 1; Column <= NoOfColumns; Column++) Console.Write(Column + "  ");
        Console.Write(Environment.NewLine + "  ");
        for (var Count = 1; Count <= NoOfColumns; Count++) Console.Write("---");
        Console.WriteLine("-");
        for (var Row = 1; Row <= NoOfRows; Row++)
        {
            Console.Write(Row + " ");
            for (var Column = 1; Column <= NoOfColumns; Column++)
            {
                var Index = GetIndexOfSquare(Row * 10 + Column);
                Console.Write("|" + Board[Index].GetSymbol());
                var PieceInSquare = Board[Index].GetPieceInSquare();
                if (PieceInSquare == null)
                    Console.Write(" ");
                else
                    Console.Write(PieceInSquare.GetSymbol());
            }

            Console.WriteLine("|");
        }

        Console.Write("  -");
        for (var Column = 1; Column <= NoOfColumns; Column++) Console.Write("---");
        Console.WriteLine();
        Console.WriteLine();
    }

    private void DisplayState()
    {
        DisplayBoard();
        Console.WriteLine("Move option offer: " + MoveOptionOffer[MoveOptionOfferPosition]);
        Console.WriteLine();
        Console.WriteLine(CurrentPlayer.GetPlayerStateAsString());
        Console.WriteLine("Turn: " + CurrentPlayer.GetName());
        Console.WriteLine();
    }

    private int GetIndexOfSquare(int SquareReference)
    {
        var Row = SquareReference / 10;
        var Col = SquareReference % 10;
        return (Row - 1) * NoOfColumns + (Col - 1);
    }

    private bool CheckSquareInBounds(int SquareReference)
    {
        var Row = SquareReference / 10;
        var Col = SquareReference % 10;
        if (Row < 1 || Row > NoOfRows)
            return false;
        if (Col < 1 || Col > NoOfColumns)
            return false;
        return true;
    }

    private bool CheckSquareIsValid(int SquareReference, bool StartSquare)
    {
        if (!CheckSquareInBounds(SquareReference)) return false;
        var PieceInSquare = Board[GetIndexOfSquare(SquareReference)].GetPieceInSquare();
        if (PieceInSquare == null)
        {
            if (StartSquare)
                return false;
            return true;
        }

        if (CurrentPlayer.SameAs(PieceInSquare.GetBelongsTo()))
        {
            if (StartSquare)
                return true;
            return false;
        }

        if (StartSquare)
            return false;
        return true;
    }

    private bool CheckIfGameOver()
    {
        var Player1HasMirza = false;
        var Player2HasMirza = false;
        foreach (var S in Board)
        {
            var PieceInSquare = S.GetPieceInSquare();
            if (PieceInSquare != null)
            {
                if (S.ContainsKotla() && PieceInSquare.GetTypeOfPiece() == "mirza" &&
                    !PieceInSquare.GetBelongsTo().SameAs(S.GetBelongsTo()))
                    return true;
                if (PieceInSquare.GetTypeOfPiece() == "mirza" && PieceInSquare.GetBelongsTo().SameAs(Players[0]))
                    Player1HasMirza = true;
                else if (PieceInSquare.GetTypeOfPiece() == "mirza" && PieceInSquare.GetBelongsTo().SameAs(Players[1]))
                    Player2HasMirza = true;
            }
        }

        return !(Player1HasMirza && Player2HasMirza);
    }

    private int GetSquareReference(string Description)
    {
        int SelectedSquare;
        Console.Write("Enter the square " + Description + " (row number followed by column number): ");
        SelectedSquare = Convert.ToInt32(Console.ReadLine());
        return SelectedSquare;
    }

    private void UseMoveOptionOffer()
    {
        int ReplaceChoice;
        Console.Write("Choose the move option from your queue to replace (1 to 5): ");
        ReplaceChoice = Convert.ToInt32(Console.ReadLine());
        CurrentPlayer.UpdateMoveOptionQueueWithOffer(ReplaceChoice - 1,
            CreateMoveOption(MoveOptionOffer[MoveOptionOfferPosition], CurrentPlayer.GetDirection()));
        CurrentPlayer.ChangeScore(-(10 - ReplaceChoice * 2));
        MoveOptionOfferPosition = RGen.Next(0, 5);
    }

    private int GetPointsForOccupancyByPlayer(Player CurrentPlayer)
    {
        var ScoreAdjustment = 0;
        foreach (var S in Board) ScoreAdjustment += S.GetPointsForOccupancy(CurrentPlayer);
        return ScoreAdjustment;
    }

    private void UpdatePlayerScore(int PointsForPieceCapture)
    {
        CurrentPlayer.ChangeScore(GetPointsForOccupancyByPlayer(CurrentPlayer) + PointsForPieceCapture);
    }

    private int CalculatePieceCapturePoints(int FinishSquareReference)
    {
        if (Board[GetIndexOfSquare(FinishSquareReference)].GetPieceInSquare() != null)
            return Board[GetIndexOfSquare(FinishSquareReference)].GetPieceInSquare().GetPointsIfCaptured();
        return 0;
    }

    public void PlayGame()
    {
        bool weathering = false;
        var GameOver = false;
        while (!GameOver)
        {
            DisplayState();
            var SquareIsValid = false;
            int Choice;
            do
            {
                Console.Write("Choose move option to use from queue (1 to 3) or 9 to take the offer: ");
                Choice = Convert.ToInt32(Console.ReadLine());
                if (Choice == 9)
                {
                    UseMoveOptionOffer();
                    DisplayState();
                }
            } while (Choice < 1 || Choice > 3);

            var StartSquareReference = 0;
            while (!SquareIsValid)
            {
                StartSquareReference = GetSquareReference("containing the piece to move");
                SquareIsValid = CheckSquareIsValid(StartSquareReference, true);
            }

// separate validation for end and stat square, personally would have added together
            var FinishSquareReference = 0;
            SquareIsValid = false;
            while (!SquareIsValid)
            {
                FinishSquareReference = GetSquareReference("to move to");
                SquareIsValid = CheckSquareIsValid(FinishSquareReference, false);
            }

            var MoveLegal = CurrentPlayer.CheckPlayerMove(Choice, StartSquareReference, FinishSquareReference);
            if (MoveLegal)
            {
                var PointsForPieceCapture = CalculatePieceCapturePoints(FinishSquareReference);
                CurrentPlayer.ChangeScore(-(Choice + 2 * (Choice - 1)));
                CurrentPlayer.UpdateQueueAfterMove(Choice);
                UpdateBoard(StartSquareReference, FinishSquareReference);
                UpdatePlayerScore(PointsForPieceCapture);
                Console.WriteLine("New score: " + CurrentPlayer.GetScore() + Environment.NewLine);
            }


            // 50/50
            if (!weathering)
            {
                if (WeatherEventOccurs() )
                {
                    //whilst invalid
                    var ValidSqr = false;
                    while (!ValidSqr)
                    {
                        var Index = RGen.Next(0, Board.Count() - 1);
                        if (Board[Index].GetPieceInSquare() == null)
                        {
                            ValidSqr = true;
                            var row = Index / NoOfRows + 1;
                            var col = Index % NoOfColumns + 1;
                            var end = Convert.ToString(row) + Convert.ToString(col);
                            var loc = Convert.ToInt32(end);
                        
                            wether = new WeatherEvent();
                            wether.SetWeatherLocation(loc);
                            weathering = true;
                        }
                    }
                }
            }
           
            //protects against null error until new WeatherEvent();
            //decrement 0 from above exec
            if (wether != null)
            {
                if (wether.CountDownComplete())
                {
                
                    var location = wether.GetWeatherLocation();
                    var col = location % 10;
                    Console.WriteLine($"weather event on column {col} ");
                    for (var i = 1; i < NoOfRows+1; i++)
                    {
                        var end = Convert.ToString(i) + Convert.ToString(col);
                        var newloc = Convert.ToInt32(end);
                        if (Board[GetIndexOfSquare(newloc)].GetPieceInSquare() != null) Board[GetIndexOfSquare(newloc)].RemovePiece();
                        if (Board[GetIndexOfSquare(newloc)].ContainsKotla()) Board[GetIndexOfSquare(newloc)] = new Square();
                    }
                    //event ended
                    weathering = false;
                }
            }

            if (CurrentPlayer.SameAs(Players[0]))
                CurrentPlayer = Players[1];
            else
                CurrentPlayer = Players[0];
            GameOver = CheckIfGameOver();
        }

        DisplayState();
        DisplayFinalResult();
    }

    private bool WeatherEventOccurs()
    {
        var weather = RGen.Next(0, 2);
        if (weather == 0) return true;
        return false;
    }


    private void UpdateBoard(int StartSquareReference, int FinishSquareReference)
    {
        Board[GetIndexOfSquare(FinishSquareReference)]
            .SetPiece(Board[GetIndexOfSquare(StartSquareReference)].RemovePiece());
    }

    private void DisplayFinalResult()
    {
        if (Players[0].GetScore() == Players[1].GetScore())
            Console.WriteLine("Draw!");
        else if (Players[0].GetScore() > Players[1].GetScore())
            Console.WriteLine(Players[0].GetName() + " is the winner!");
        else
            Console.WriteLine(Players[1].GetName() + " is the winner!");
    }

    private void CreateBoard()
    {
        Square S;
        Board = new List<Square>();
        for (var Row = 1; Row <= NoOfRows; Row++)
        for (var Column = 1; Column <= NoOfColumns; Column++)
        {
            if (Row == 1 && Column == NoOfColumns / 2)
                S = new Kotla(Players[0], "K");
            else if (Row == NoOfRows && Column == NoOfColumns / 2 + 1)
                S = new Kotla(Players[1], "k");
            else
                S = new Square();
            Board.Add(S);
        }
    }

    private void CreatePieces(int NoOfPieces)
    {
        Piece CurrentPiece;
        for (var Count = 1; Count <= NoOfPieces; Count++)
        {
            CurrentPiece = new Piece("piece", Players[0], 1, "!");
            Board[GetIndexOfSquare(2 * 10 + Count + 1)].SetPiece(CurrentPiece);
        }

        CurrentPiece = new Piece("mirza", Players[0], 5, "1");
        Board[GetIndexOfSquare(10 + NoOfColumns / 2)].SetPiece(CurrentPiece);
        for (var Count = 1; Count <= NoOfPieces; Count++)
        {
            CurrentPiece = new Piece("piece", Players[1], 1, "\"");
            Board[GetIndexOfSquare((NoOfRows - 1) * 10 + Count + 1)].SetPiece(CurrentPiece);
        }

        CurrentPiece = new Piece("mirza", Players[1], 5, "2");
        Board[GetIndexOfSquare(NoOfRows * 10 + NoOfColumns / 2 + 1)].SetPiece(CurrentPiece);
    }

    private void CreateMoveOptionOffer()
    {
        MoveOptionOffer.Add("jazair");
        MoveOptionOffer.Add("chowkidar");
        MoveOptionOffer.Add("cuirassier");
        MoveOptionOffer.Add("ryott");
        MoveOptionOffer.Add("faujdar");
    }

    private MoveOption CreateRyottMoveOption(int Direction)
    {
        var NewMoveOption = new MoveOption("ryott");
        var NewMove = new Move(0, 1 * Direction);
        NewMoveOption.AddToPossibleMoves(NewMove);
        NewMove = new Move(0, -1 * Direction);
        NewMoveOption.AddToPossibleMoves(NewMove);
        NewMove = new Move(1 * Direction, 0);
        NewMoveOption.AddToPossibleMoves(NewMove);
        NewMove = new Move(-1 * Direction, 0);
        NewMoveOption.AddToPossibleMoves(NewMove);
        return NewMoveOption;
    }

    private MoveOption CreateFaujdarMoveOption(int Direction)
    {
        var NewMoveOption = new MoveOption("faujdar");
        var NewMove = new Move(0, -1 * Direction);
        NewMoveOption.AddToPossibleMoves(NewMove);
        NewMove = new Move(0, 1 * Direction);
        NewMoveOption.AddToPossibleMoves(NewMove);
        NewMove = new Move(0, 2 * Direction);
        NewMoveOption.AddToPossibleMoves(NewMove);
        NewMove = new Move(0, -2 * Direction);
        NewMoveOption.AddToPossibleMoves(NewMove);
        return NewMoveOption;
    }

    private MoveOption CreateJazairMoveOption(int Direction)
    {
        var NewMoveOption = new MoveOption("jazair");
        var NewMove = new Move(2 * Direction, 0);
        NewMoveOption.AddToPossibleMoves(NewMove);
        NewMove = new Move(2 * Direction, -2 * Direction);
        NewMoveOption.AddToPossibleMoves(NewMove);
        NewMove = new Move(2 * Direction, 2 * Direction);
        NewMoveOption.AddToPossibleMoves(NewMove);
        NewMove = new Move(0, 2 * Direction);
        NewMoveOption.AddToPossibleMoves(NewMove);
        NewMove = new Move(0, -2 * Direction);
        NewMoveOption.AddToPossibleMoves(NewMove);
        NewMove = new Move(-1 * Direction, -1 * Direction);
        NewMoveOption.AddToPossibleMoves(NewMove);
        NewMove = new Move(-1 * Direction, 1 * Direction);
        NewMoveOption.AddToPossibleMoves(NewMove);
        return NewMoveOption;
    }

    private MoveOption CreateCuirassierMoveOption(int Direction)
    {
        var NewMoveOption = new MoveOption("cuirassier");
        var NewMove = new Move(1 * Direction, 0);
        NewMoveOption.AddToPossibleMoves(NewMove);
        NewMove = new Move(2 * Direction, 0);
        NewMoveOption.AddToPossibleMoves(NewMove);
        NewMove = new Move(1 * Direction, -2 * Direction);
        NewMoveOption.AddToPossibleMoves(NewMove);
        NewMove = new Move(1 * Direction, 2 * Direction);
        NewMoveOption.AddToPossibleMoves(NewMove);
        return NewMoveOption;
    }

    private MoveOption CreateChowkidarMoveOption(int Direction)
    {
        var NewMoveOption = new MoveOption("chowkidar");
        var NewMove = new Move(1 * Direction, 1 * Direction);
        NewMoveOption.AddToPossibleMoves(NewMove);
        NewMove = new Move(1 * Direction, -1 * Direction);
        NewMoveOption.AddToPossibleMoves(NewMove);
        NewMove = new Move(-1 * Direction, 1 * Direction);
        NewMoveOption.AddToPossibleMoves(NewMove);
        NewMove = new Move(-1 * Direction, -1 * Direction);
        NewMoveOption.AddToPossibleMoves(NewMove);
        NewMove = new Move(0, 2 * Direction);
        NewMoveOption.AddToPossibleMoves(NewMove);
        NewMove = new Move(0, -2 * Direction);
        NewMoveOption.AddToPossibleMoves(NewMove);
        return NewMoveOption;
    }

    private MoveOption CreateMoveOption(string Name, int Direction)
    {
        if (Name == "chowkidar")
            return CreateChowkidarMoveOption(Direction);
        if (Name == "ryott")
            return CreateRyottMoveOption(Direction);
        if (Name == "faujdar")
            return CreateFaujdarMoveOption(Direction);
        if (Name == "jazair")
            return CreateJazairMoveOption(Direction);
        return CreateCuirassierMoveOption(Direction);
    }

    private void CreateMoveOptions()
    {
        Players[0].AddToMoveOptionQueue(CreateMoveOption("ryott", 1));
        Players[0].AddToMoveOptionQueue(CreateMoveOption("chowkidar", 1));
        Players[0].AddToMoveOptionQueue(CreateMoveOption("cuirassier", 1));
        Players[0].AddToMoveOptionQueue(CreateMoveOption("faujdar", 1));
        Players[0].AddToMoveOptionQueue(CreateMoveOption("jazair", 1));
        Players[1].AddToMoveOptionQueue(CreateMoveOption("ryott", -1));
        Players[1].AddToMoveOptionQueue(CreateMoveOption("chowkidar", -1));
        Players[1].AddToMoveOptionQueue(CreateMoveOption("jazair", -1));
        Players[1].AddToMoveOptionQueue(CreateMoveOption("faujdar", -1));
        Players[1].AddToMoveOptionQueue(CreateMoveOption("cuirassier", -1));
    }
}

internal class Piece
{
    protected string TypeOfPiece, Symbol;
    protected int PointsIfCaptured;
    protected Player BelongsTo;

    public Piece(string T, Player B, int P, string S)
    {
        TypeOfPiece = T;
        BelongsTo = B;
        PointsIfCaptured = P;
        Symbol = S;
    }

    public string GetSymbol()
    {
        return Symbol;
    }

    public string GetTypeOfPiece()
    {
        return TypeOfPiece;
    }

    public Player GetBelongsTo()
    {
        return BelongsTo;
    }

    public int GetPointsIfCaptured()
    {
        return PointsIfCaptured;
    }
}

internal class Square
{
    protected string Symbol;
    protected Piece PieceInSquare;
    protected Player BelongsTo;

    public Square()
    {
        PieceInSquare = null;
        BelongsTo = null;
        Symbol = " ";
    }

    public virtual void SetPiece(Piece P)
    {
        PieceInSquare = P;
    }

    public virtual Piece RemovePiece()
    {
        var PieceToReturn = PieceInSquare;
        PieceInSquare = null;
        return PieceToReturn;
    }

    public virtual Piece GetPieceInSquare()
    {
        return PieceInSquare;
    }

    public virtual string GetSymbol()
    {
        return Symbol;
    }

    public virtual int GetPointsForOccupancy(Player CurrentPlayer)
    {
        return 0;
    }

    public virtual Player GetBelongsTo()
    {
        return BelongsTo;
    }

    public virtual bool ContainsKotla()
    {
        if (Symbol == "K" || Symbol == "k")
            return true;
        return false;
    }
}

internal class Kotla : Square
{
    public Kotla(Player P, string S)
    {
        BelongsTo = P;
        Symbol = S;
    }

    public override int GetPointsForOccupancy(Player CurrentPlayer)
    {
        if (PieceInSquare == null) return 0;

        if (BelongsTo.SameAs(CurrentPlayer))
        {
            if (CurrentPlayer.SameAs(PieceInSquare.GetBelongsTo()) && (PieceInSquare.GetTypeOfPiece() == "piece" ||
                                                                       PieceInSquare.GetTypeOfPiece() == "mirza"))
                return 5;
            return 0;
        }

        if (CurrentPlayer.SameAs(PieceInSquare.GetBelongsTo()) &&
            (PieceInSquare.GetTypeOfPiece() == "piece" || PieceInSquare.GetTypeOfPiece() == "mirza"))
            return 1;
        return 0;
    }
}

internal class MoveOption
{
    protected string Name;
    protected List<Move> PossibleMoves;

    public MoveOption(string N)
    {
        Name = N;
        PossibleMoves = new List<Move>();
    }

    public void AddToPossibleMoves(Move M)
    {
        PossibleMoves.Add(M);
    }

    public string GetName()
    {
        return Name;
    }

    public bool CheckIfThereIsAMoveToSquare(int StartSquareReference, int FinishSquareReference)
    {
        var StartRow = StartSquareReference / 10;
        var StartColumn = StartSquareReference % 10;
        var FinishRow = FinishSquareReference / 10;
        var FinishColumn = FinishSquareReference % 10;
        foreach (var M in PossibleMoves)
            if (StartRow + M.GetRowChange() == FinishRow && StartColumn + M.GetColumnChange() == FinishColumn)
                return true;
        return false;
    }
}

internal class Move
{
    protected int RowChange, ColumnChange;

    public Move(int R, int C)
    {
        RowChange = R;
        ColumnChange = C;
    }

    public int GetRowChange()
    {
        return RowChange;
    }

    public int GetColumnChange()
    {
        return ColumnChange;
    }
}

internal class MoveOptionQueue
{
    private readonly List<MoveOption> Queue = new();

    public string GetQueueAsString()
    {
        var QueueAsString = "";
        var Count = 1;
        foreach (var M in Queue)
        {
            QueueAsString += Count + ". " + M.GetName() + "   ";
            Count += 1;
        }

        return QueueAsString;
    }

    public void Add(MoveOption NewMoveOption)
    {
        Queue.Add(NewMoveOption);
    }

    public void Replace(int Position, MoveOption NewMoveOption)
    {
        Queue[Position] = NewMoveOption;
    }

    public void MoveItemToBack(int Position)
    {
        var Temp = Queue[Position];
        Queue.RemoveAt(Position);
        Queue.Add(Temp);
    }

    public MoveOption GetMoveOptionInPosition(int Pos)
    {
        return Queue[Pos];
    }
}

internal class Player
{
    private readonly string Name;
    private readonly int Direction;
    private int Score;
    private readonly MoveOptionQueue Queue = new();

    public Player(string N, int D)
    {
        Score = 100;
        Name = N;
        Direction = D;
    }

    public bool SameAs(Player APlayer)
    {
        if (APlayer == null)
            return false;
        if (APlayer.GetName() == Name)
            return true;
        return false;
    }

    public string GetPlayerStateAsString()
    {
        return Name + Environment.NewLine + "Score: " + Score + Environment.NewLine + "Move option queue: " +
               Queue.GetQueueAsString() + Environment.NewLine;
    }

    public void AddToMoveOptionQueue(MoveOption NewMoveOption)
    {
        Queue.Add(NewMoveOption);
    }

    public void UpdateQueueAfterMove(int Position)
    {
        Queue.MoveItemToBack(Position - 1);
    }

    public void UpdateMoveOptionQueueWithOffer(int Position, MoveOption NewMoveOption)
    {
        Queue.Replace(Position, NewMoveOption);
    }

    public int GetScore()
    {
        return Score;
    }

    public string GetName()
    {
        return Name;
    }

    public int GetDirection()
    {
        return Direction;
    }

    public void ChangeScore(int Amount)
    {
        Score += Amount;
    }

    public bool CheckPlayerMove(int Pos, int StartSquareReference, int FinishSquareReference)
    {
        var Temp = Queue.GetMoveOptionInPosition(Pos - 1);
        return Temp.CheckIfThereIsAMoveToSquare(StartSquareReference, FinishSquareReference);
    }
}
/* 1  2  3  4  5  6
  -------------------
1 |  |  |K1|  |  |  |
2 |  |  |  | !| !|  |
3 |  | !|  | !|  | "|
4 |  | "|  |  |  |  |
5 | "|  |  |  | "|  |
6 |  |  |  |k2|  |  |
  -------------------

Move option offer: jazair

Player One
Score: 108
Move option queue: 1. cuirassier   2. faujdar   3. jazair   4. ryott   5. chowkidar

Turn: Player One

Choose move option to use from queue (1 to 3) or 9 to take the offer: 2
Enter the square containing the piece to move (row number followed by column number): 32
Enter the square to move to (row number followed by column number): 31
New score: 109

weather event on column 1

   1  2  3  4  5  6
  -------------------
1 |  |  |K1|  |  |  |
2 |  |  |  | !| !|  |
3 |  |  |  | !|  | "|
4 |  | "|  |  |  |  |
5 |  |  |  |  | "|  |
6 |  |  |  |k2|  |  |
  -------------------
*/