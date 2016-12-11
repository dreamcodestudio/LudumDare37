using System;

public class FieldModel
{

    #region CONSTRUCTORS

    private FieldModel(bool[,] mines)
    {
        _mines = mines;
        _opened = new bool[Height, Width];
        _marked = new bool[Height, Width];
        _values = CalculateValues();
    }

    #endregion

    #region PUBLIC_VARIABLES

    public bool[,] Mines
    {
        get { return _mines; }
    }

    public bool[,] Opened
    {
        get { return _opened; }
    }

    public bool[,] Marked
    {
        get { return _marked; }
    }

    public int[,] Values
    {
        get { return _values; }
    }

    public int Width
    {
        get { return _mines.GetLength(1); }
    }

    public int Height
    {
        get { return _mines.GetLength(0); }
    }

    public EActionType ActionType;

    #endregion

    #region PRIVATE_VARIABLES

    private bool[,] _mines;
    private bool[,] _opened;
    private bool[,] _marked;
    private int[,] _values;

    #endregion

    public static FieldModel Create(int width, int height)
    {
        var field = new bool[height, width];
        var mineAmount = (int) (0.3 * height * width);
        var random = new Random();
        while (mineAmount > 0)
        {
            var x = random.Next(width);
            var y = random.Next(height);
            if (!field[y, x])
            {
                field[y, x] = true;
                mineAmount--;
            }
        }
        return new FieldModel(field);
    }

    public int RemainingMinesCount
    {
        get
        {
            var count = 0;
            CheckCell((i, j) =>
            {
                if (Mines[i, j] && !Marked[i, j])
                {
                    count++;
                }
            });
            return count;
        }
    }

    public void CheckCell(Action<int, int> action)
    {
        for (var i = 0; i < Height; i++)
        {
            for (var j = 0; j < Width; j++)
            {
                action(i, j);
            }
        }
    }

    public void CheckNeighborCell(int i, int j, Action<int, int> action)
    {
        for (var i1 = i - 1; i1 <= i + 1; i1++)
        {
            for (var j1 = j - 1; j1 <= j + 1; j1++)
            {
                if (InBounds(j1, i1) && !(i1 == i && j1 == j))
                {
                    action(i1, j1);
                }
            }
        }
    }

    public void Open(int i, int j)
    {
        if (!Opened[i, j])
        {
            if (Mines[i, j])
            {
                EventManager.Instance.CallCellExploded();
            }
            else
            {
                OpenCellsStartingFrom(i, j);
                EventManager.Instance.CallFieldUpdated();
                CheckCompletedStage();
            }
        }
    }

    public void Mark(int i, int j)
    {
        if (!Opened[i, j])
        {
            Marked[i, j] = true;
        }
        EventManager.Instance.CallFieldUpdated();
        CheckCompletedStage();
    }

    private bool InBounds(int x, int y)
    {
        return y >= 0 && y < Height && x >= 0 && x < Width;
    }

    private int[,] CalculateValues()
    {
        var values = new int[Height, Width];
        CheckCell((i, j) =>
        {
            var value = 0;
            CheckNeighborCell(i, j, (i1, j1) =>
            {
                if (Mines[i1, j1])
                    value++;
            });
            values[i, j] = value;
        });
        return values;
    }

    private void OpenCellsStartingFrom(int i, int j)
    {
        Opened[i, j] = true;
        CheckNeighborCell(i, j, (i1, j1) =>
        {
            if (!Mines[i1, j1] && !Opened[i1, j1] && !Marked[i1, j1])
            {
                OpenCellsStartingFrom(i1, j1);
            }
        });
    }

    private void CheckCompletedStage()
    {
        var notMarked = 0;
        var wrongMarked = 0;
        CheckCell((i, j) =>
        {
            if (Mines[i, j] && !Marked[i, j])
                notMarked++;
            if (!Mines[i, j] && Marked[i, j])
                wrongMarked++;
        });
        if (notMarked == 0 && wrongMarked == 0)
        {
            EventManager.Instance.CallCompletedStage();
        }
    }
}
