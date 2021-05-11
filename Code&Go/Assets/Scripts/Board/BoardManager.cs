using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using AssetPackage;

public class BoardManager : Listener
{
    private int rows;
    private int columns;
    private int nReceivers = 0;
    private int nReceiversActive = 0;

    [SerializeField] private Transform cellsParent;
    [SerializeField] private Transform elementsParent;
    [SerializeField] private Transform limitsParent;
    [SerializeField] private Transform holesParent;
    [SerializeField] private Transform hintsParent;
    [SerializeField] private BoardCell[] cellPrefabs;
    [SerializeField] private BoardObject[] boardObjectPrefabs;
    [SerializeField] private GameObject limitsPrefab;
    [SerializeField] private BoardHint hintsPrefab;
    [SerializeField] private Button hintButton;
    [SerializeField] private bool boardModifiable = false;
    private bool objectsModifiable = false;
    private int hintsShown = 0;
    private int hintsPerUse = 0;

    // Hidden atributtes
    private BoardCell[,] board;
    private ArgumentLoader argLoader = null;
    public enum CellColors { NORMAL, GREEN, }

    private Dictionary<string, List<Vector2Int>> elementPositions;
    private List<List<BoardCell>> cells;

    private int currentSteps = 0;

    public GameObject gameOverPanel;
    public GameObject blackRect;

    private void Awake()
    {
        cells = new List<List<BoardCell>>();
        InitIDs();
        //currentPasos = 0;
    }

    public int GetCurrentSteps()
    {
        return currentSteps;
    }

    private void GenerateDefaultBoard()
    {
        for (int y = 1; y <= rows; y++)
            for (int x = 1; x <= columns; x++)
                AddBoardCell(0, x, y);
    }

    public void GenerateBoard(BoardCellState[] cells = null)
    {
        elementPositions = new Dictionary<string, List<Vector2Int>>();
        // If a board already exist, destroy it
        DestroyBoard();
        // Initialize board
        board = new BoardCell[columns + 2, rows + 2];
        // Instantiate cells
        if (cells != null)
            foreach (BoardCellState cellState in cells)
                AddBoardCell(cellState.id, cellState.x, cellState.y, cellState.args);
        else
            GenerateDefaultBoard();

        nReceivers = 0;
        nReceiversActive = 0;

        //Set focus point of the camera
        FocusPoint fPoint = GetComponent<FocusPoint>();
        if (fPoint != null)
            fPoint.offset = new Vector3(columns / 2.0f + 0.5f, 0.0f, rows / 2.0f + 0.5f);
    }

    public void GenerateLimits()
    {
        if (limitsPrefab == null) return;

        Vector2Int pos = new Vector2Int(-1, -1);
        Vector2Int[] edges = { new Vector2Int(-1, rows), new Vector2Int(columns, rows), new Vector2Int(columns, -1), new Vector2Int(-1, -1) };
        int dir = 0;
        for (int i = 0; i < 2 * columns + 2 * rows + 4; i++)
        {
            GameObject limit = Instantiate(limitsPrefab, limitsParent);
            limit.transform.localPosition = new Vector3(pos.x, 0, pos.y);

            if (dir % 2 == 0) pos.y += ((dir / 2 == 0) ? 1 : -1);
            else pos.x += ((dir / 2 == 0) ? 1 : -1);

            int j = 0;
            while (j < edges.Length && pos != edges[j])
                j++;

            if (j < edges.Length) dir++;
        }
    }

    public void GenerateHoles()
    {
        if (cellPrefabs[1] == null) return;

        Vector2Int pos = new Vector2Int(0, 0);
        Vector2Int[] edges = { new Vector2Int(0, rows + 1), new Vector2Int(columns + 1, rows + 1), new Vector2Int(columns + 1, 0), new Vector2Int(0, 0) };
        int dir = 0;
        for (int i = 0; i < 2 * columns + 2 * rows + 4; i++)
        {
            BoardCell hole = Instantiate(cellPrefabs[1], holesParent);
            hole.transform.localPosition = new Vector3(pos.x, 0, pos.y);
            hole.SetPosition(pos.x, pos.y);
            hole.SetBoardManager(this);
            board[pos.x, pos.y] = hole;

            if (dir % 2 == 0) pos.y += ((dir / 2 == 0) ? 1 : -1);
            else pos.x += ((dir / 2 == 0) ? 1 : -1);

            int j = 0;
            while (j < edges.Length && pos != edges[j])
                j++;

            if (j < edges.Length) dir++;
        }
        rows += 2;
        columns += 2;
    }

    public void ClearHoles()
    {
        foreach (Transform child in holesParent)
        {
            BoardCell hole = child.gameObject.GetComponent<BoardCell>();
            board[hole.GetPosition().x, hole.GetPosition().y] = null;
            Destroy(child.gameObject);
        }
    }

    private void DestroyBoard()
    {
        foreach (Transform child in cellsParent)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in holesParent)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in elementsParent)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in limitsParent)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in hintsParent)
        {
            Destroy(child.gameObject);
        }

        foreach (List<BoardCell> cellsList in cells)
        {
            cellsList.Clear();
        }
    }

    private void InitIDs()
    {
        foreach (BoardObject element in boardObjectPrefabs)
            element.GetObjectID();
        foreach (BoardCell element in cellPrefabs)
        {
            element.GetObjectID();
            cells.Add(new List<BoardCell>());
        }
    }

    private bool IsInBoardBounds(int x, int y)
    {
        return y >= 0 && y < board.GetLength(1) && x >= 0 && x < board.GetLength(0);
    }
    private bool IsInBoardBounds(Vector2Int position)
    {
        return IsInBoardBounds(position.x, position.y);
    }

    public void SetModifiable(bool modifiable)
    {
        this.objectsModifiable = modifiable;
        foreach (Transform child in cellsParent)
        {
            ModifiableBoardCell modCell = child.GetComponent<ModifiableBoardCell>();
            if (modCell != null) modCell.SetModifiable(modifiable);
        }

        foreach (Transform child in elementsParent)
        {
            DraggableObject drag = child.GetComponent<DraggableObject>();
            if (drag != null) drag.SetModifiable(modifiable);
        }
    }

    public void SetArgLoader(ArgumentLoader argumentLoader)
    {
        argLoader = argumentLoader;
    }

    public void LoadBoard(BoardState state, bool limits = false)
    {
        if (state == null) return;

        rows = state.rows;
        columns = state.columns;

        // Generate board cells
        GenerateBoard(state.cells);

        //Create Hints
        if (state.boardHints != null)
            foreach (BoardHintState hintState in state.boardHints)
                AddBoardHint(hintState);
        hintsPerUse = (hintsParent.childCount / 3.0f >= 1) ? Mathf.CeilToInt(hintsParent.childCount / 3.0f) : ((hintsParent.childCount / 2.0f >= 1) ? Mathf.CeilToInt(hintsParent.childCount / 2.0f) : hintsParent.childCount);
        if (hintsParent.childCount == 0) DeactivateHintButton();
        //Generate limits and holes around the board
        GenerateHoles();
        if (limits) GenerateLimits();

        //Generate the board elements
        GenerateBoardElements(state);
    }

    public void DeleteBoardElements()
    {
        foreach (List<Vector2Int> item in elementPositions.Values)
            foreach (Vector2Int pos in item)
                RemoveBoardObject(pos.x, pos.y, true, false);
    }

    public void GenerateBoardElements(BoardState state)
    {
        // Generate board elements
        foreach (BoardObjectState objectState in state.boardElements)
            AddBoardObject(objectState.id, objectState.x, objectState.y, objectState.orientation, objectState.args);
    }

    public void RegisterReceiver()
    {
        nReceivers++;
    }

    public void Reset()
    {
        nReceivers = 0;
        nReceiversActive = 0;
        DeleteBoardElements();
        elementPositions.Clear();
        //currentSteps = 0;
    }

    public bool HasHint(Vector2Int pos)
    {
        return IsInBoardBounds(pos.x, pos.y) && board[pos.x, pos.y].HasHint();
    }

    public void ReceiverActivated()
    {
        nReceiversActive++;
    }

    public void ReceiverDeactivated()
    {
        nReceiversActive--;
    }

    public bool BoardCompleted()
    {
        return nReceivers > 0 && nReceiversActive >= nReceivers;
    }

    public int GetNReceivers()
    {
        return nReceivers;
    }

    public int GetNEmitters()
    {
        if (!elementPositions.ContainsKey("laser")) return 0;
        return elementPositions["laser"].Count;
    }

    public void SetRows(int rows)
    {
        this.rows = rows;
    }

    public void SetColumns(int columns)
    {
        this.columns = columns;
    }

    public int GetRows()
    {
        return rows;
    }

    public int GetColumns()
    {
        return columns;
    }

    public BoardCell GetBoardCell(int x, int y)
    {
        return IsInBoardBounds(x, y) ? board[x, y] : null;
    }

    public int GetBoardCellType(int x, int y)
    {
        return IsInBoardBounds(x, y) ? board[x, y].GetObjectID() : -1;
    }

    public BoardState GetBoardState()
    {
        BoardState state = new BoardState(rows - 2, columns - 2, elementsParent.childCount, hintsParent.childCount);
        int i = 0;
        //Iterate the cells of the current board
        foreach (BoardCell cell in board)
        {
            Vector2Int pos = cell.GetPosition();
            //If it's a valid position and not a hole
            if (cell == null || pos.x == 0 || pos.y == 0 || pos.x == columns - 1 || pos.y == rows - 1) continue;
            state.SetBoardCell(cell);
            //Process the object in the cell if there's any
            if (i < elementsParent.childCount && cell.GetState() == BoardCell.BoardCellState.OCUPPIED)
                state.SetBoardObject(i++, cell);
        }

        //Get hints of the current board
        i = 0;
        foreach (Transform hintT in hintsParent.transform)
        {
            BoardHint hint = hintT.GetComponent<BoardHint>();
            if (hint != null)
                state.SetBoardHint(i++, hint);
        }

        return state;
    }

    public string GetBoardStateAsString()
    {
        return GetBoardState().ToJson();
    }

    public void AddBoardHint(Vector2Int pos, int amount, int id)
    {
        if (!IsInBoardBounds(pos)) return;

        BoardHint hint = Instantiate(hintsPrefab, hintsParent);
        hint.SetDirection((BoardObject.Direction)0);
        hint.SetAmount(amount);
        hint.SetPosition(pos.x, pos.y);
        hint.SetHintID(id);
        hint.gameObject.SetActive(true);

        board[pos.x, pos.y].SetHint(hint);
    }

    public void AddBoardHint(BoardHintState hintState)
    {
        if (!IsInBoardBounds(hintState.x, hintState.y)) return;

        BoardHint hint = Instantiate(hintsPrefab, hintsParent);
        hint.SetDirection((BoardObject.Direction)hintState.orientation);
        hint.SetAmount(hintState.amount);
        hint.SetPosition(hintState.x, hintState.y);
        hint.SetHintID(hintState.id);

        board[hintState.x, hintState.y].SetHint(hint);
    }

    public void RotateHint(Vector2Int pos, int dir, int index)
    {
        if (!IsInBoardBounds(pos) || index >= board[pos.x, pos.y].GetNHints()) return;
        board[pos.x, pos.y].GetHint(index).Rotate(2 * dir);
    }

    public void DeleteHint(Vector2Int pos, int index)
    {
        if (!IsInBoardBounds(pos) || !board[pos.x, pos.y].HasHint()) return;
        board[pos.x, pos.y].RemoveHint(index);
    }

    public BoardCell AddBoardCell(int id, int x, int y, string[] args = null)
    {
        if (id >= cellPrefabs.Length || !IsInBoardBounds(x, y)) return null;

        BoardCell cell = Instantiate(cellPrefabs[id], cellsParent);
        cell.SetPosition(x, y);
        cell.SetBoardManager(this);
        board[x, y] = cell;

        if (boardModifiable)
        {
            ModifiableBoardCell modCell = cell.gameObject.AddComponent<ModifiableBoardCell>();
            modCell.SetBoardManager(this);
            modCell.SetModifiable(objectsModifiable);
        }

        cells[id].Add(cell);

        return cell;
    }
    public bool AddBoardObject(int id, int x, int y, int orientation = 0, string[] additionalArgs = null)
    {
        if (id >= boardObjectPrefabs.Length || !IsInBoardBounds(x, y)) return false;

        BoardObject bObject = Instantiate(boardObjectPrefabs[id], elementsParent);
        bObject.LoadArgs(additionalArgs);
        bObject.SetBoard(this);
        bObject.SetDirection((BoardObject.Direction)orientation);

        return AddBoardObject(x, y, bObject);
    }

    public bool AddBoardObject(int x, int y, BoardObject boardObject)
    {
        if (IsInBoardBounds(x, y) && boardObject != null)
        {
            bool placed = board[x, y].PlaceObject(boardObject);
            if (!placed) return false;

            if (boardObject.transform.parent != elementsParent)
            {
                boardObject.transform.SetParent(elementsParent);
                boardObject.SetBoard(this);
            }
            if (elementPositions != null)
            {
                if (!elementPositions.ContainsKey(boardObject.GetNameAsLower()))
                    elementPositions[boardObject.GetNameAsLower()] = new List<Vector2Int>();
                elementPositions[boardObject.GetNameAsLower()].Add(new Vector2Int(x, y));

                FollowingText text = boardObject.GetComponent<FollowingText>();
                if (text != null)
                    text.SetName(boardObject.GetName() + " " + elementPositions[boardObject.GetNameAsLower()].Count.ToString());
            }

            if (boardModifiable)
            {
                DraggableObject drag = boardObject.gameObject.GetComponent<DraggableObject>();
                if (drag == null)
                {
                    drag = boardObject.gameObject.AddComponent<DraggableObject>();
                    drag.SetBoard(this);
                    drag.SetArgumentLoader(argLoader);
                    drag.SetLastPos(new Vector2Int(x, y));
                }
                drag.SetModifiable(objectsModifiable);
            }
            return placed;
        }
        return false;
    }

    public void RemoveBoardObject(int x, int y, bool delete = true, bool updatePositions = true)
    {
        if (IsInBoardBounds(x, y) && board[x, y].GetPlacedObject() != null)
        {
            string name = board[x, y].GetPlacedObject().GetNameAsLower();
            board[x, y].RemoveObject(delete);
            if (updatePositions && elementPositions.ContainsKey(name)) elementPositions[name].Remove(new Vector2Int(x, y));
            RefreshNames(name);
        }
    }

    private void RefreshNames(string name)
    {
        if (!elementPositions.ContainsKey(name)) return;
        foreach (Vector2Int pos in elementPositions[name])
        {
            if (board[pos.x, pos.y].GetPlacedObject() == null) continue;

            FollowingText text = board[pos.x, pos.y].GetPlacedObject().GetComponent<FollowingText>();
            if (text != null)
                text.SetName(name + " " + (elementPositions[name].IndexOf(pos) + 1).ToString());
        }
    }

    public bool MoveBoardObject(Vector2Int from, Vector2Int to)
    {
        if (IsInBoardBounds(from) && IsInBoardBounds(to) && from != to)
        {
            BoardCell cell = board[to.x, to.y];
            if (cell.GetState() != BoardCell.BoardCellState.FREE) return false;


            BoardObject bObject = board[from.x, from.y].GetPlacedObject();
            if (bObject == null) return false;

            board[from.x, from.y].RemoveObject(false);
            board[to.x, to.y].PlaceObject(bObject);
            return true;
        }
        return false;
    }

    public void RotateBoardObject(Vector2Int position, int direction)
    {
        if (IsInBoardBounds(position))
        {
            BoardObject bObject = board[position.x, position.y].GetPlacedObject();
            if (bObject == null) return;

            bObject.Rotate(direction);
        }
    }

    public void SetBoardObjectDirection(Vector2Int position, BoardObject.Direction direction)
    {
        if (IsInBoardBounds(position))
        {
            BoardObject bObject = board[position.x, position.y].GetPlacedObject();
            if (bObject == null) return;

            bObject.SetDirection(direction);
        }
    }

    public void ReplaceCell(int id, int x, int y)
    {
        if (!IsInBoardBounds(x, y) || id == board[x, y].GetObjectID()) return;
        BoardCell currentCell = board[x, y];
        BoardObject boardObject = currentCell.GetPlacedObject();
        currentCell.RemoveObject(false);

        BoardCell cell = AddBoardCell(id, x, y);

        if (boardObject != null) cell.PlaceObject(boardObject);

        if (cells[id].Contains(currentCell)) cells[id].Remove(currentCell);
        Destroy(currentCell.gameObject);
    }

    //Moves an object given its name and index called by the blocks
    public IEnumerator MoveObject(string name, int index, Vector2Int direction, int amount, float time)
    {
        name = name.ToLower();
        if (elementPositions.ContainsKey(name) && index < elementPositions[name].Count)
        {
            int i = 0;
            while (i++ < amount && MoveObject(elementPositions[name][index], direction, time))
            {
                elementPositions[name][index] += direction;
                yield return new WaitForSeconds(time + 0.05f);
            }
        }
        yield return null;
    }

    public IEnumerator RotateObject(string name, int index, int direction, int amount, float time)
    {
        name = name.ToLower();
        if (elementPositions.ContainsKey(name) && index < elementPositions[name].Count)
        {
            currentSteps += amount / 2;

            for (int i = 0; i < amount % 8; i++)
            {
                RotateObject(elementPositions[name][index], direction, time);
                yield return new WaitForSeconds(time + 0.05f);
            }
        }
        yield return null;
    }

    // Smooth interpolation, this code must be called by blocks
    public bool MoveObject(Vector2Int origin, Vector2Int direction, float time)
    {
        // Check valid direction
        if (!(direction.magnitude == 1.0f && (Mathf.Abs(direction.x) == 1 || Mathf.Abs(direction.y) == 1)))
            return false;

        // Check if origin object exists
        if (!IsInBoardBounds(origin)) return false;

        BoardObject bObject = board[origin.x, origin.y].GetPlacedObject();
        if (bObject == null) return false; // No object to move

        // Check if direction in valid
        if (IsInBoardBounds(origin + direction))
        {
            // A valid direction
            BoardCell fromCell = board[origin.x, origin.y];
            BoardCell toCell = board[origin.x + direction.x, origin.y + direction.y];

            if (toCell.GetPlacedObject() != null)
            {
                // TODO: hacer que se choque o haga algo
                if (bObject.GetAnimator() != null)
                    bObject.GetAnimator().Play("Collision");
                return false;
            }

            currentSteps++;

            StartCoroutine(InternalMoveObject(fromCell, toCell, time));
            return true;
        }
        else
        {
            // Not a valid direction
            return false;
        }
    }

    public void RotateObject(Vector2Int origin, int direction, float time)
    {
        // Check direction
        if (direction == 0) return;
        direction = direction < 0 ? -1 : 1;

        // Check if origin object exists
        if (!IsInBoardBounds(origin)) return;

        BoardObject bObject = board[origin.x, origin.y].GetPlacedObject();
        if (bObject == null) return; // No to rotate

        // You cannot rotate an object that is already rotating
        if (bObject.GetDirection() == BoardObject.Direction.PARTIAL) return;

        StartCoroutine(InternalRotateObject(bObject, direction, time));
    }

    public Vector3 GetLocalPosition(Vector3 position)
    {
        return transform.InverseTransformPoint(position);
    }

    public bool IsCellOccupied(int x, int y)
    {
        return IsInBoardBounds(x, y) && board[x, y].GetPlacedObject() != null;
    }

    // We assume, all are valid arguments
    private IEnumerator InternalMoveObject(BoardCell from, BoardCell to, float time)
    {
        // All cells to PARTIALLY_OCUPPIED
        from.SetState(BoardCell.BoardCellState.PARTIALLY_OCUPPIED);
        to.SetState(BoardCell.BoardCellState.PARTIALLY_OCUPPIED);

        BoardObject bObject = from.GetPlacedObject();

        Vector3 fromPos = bObject.transform.position;
        Vector3 toPos = to.transform.position;

        // Interpolate movement
        float auxTimer = 0.0f;
        while (auxTimer < time)
        {
            Vector3 lerpPos = Vector3.Lerp(fromPos, toPos, auxTimer / time);
            bObject.transform.position = lerpPos;
            auxTimer += Time.deltaTime;
            yield return null;
        }

        // Replace
        from.RemoveObject(false);
        to.PlaceObject(bObject);

        // All cells to correct state
        from.SetState(BoardCell.BoardCellState.FREE);
        to.SetState(BoardCell.BoardCellState.OCUPPIED);
    }

    private IEnumerator InternalRotateObject(BoardObject bObject, int direction, float time)
    {
        // Get direction
        BoardObject.Direction currentDirection = bObject.GetDirection();
        BoardObject.Direction targetDirection = (BoardObject.Direction)((((int)currentDirection + direction) + 8) % 8);

        // Froze direction
        bObject.SetDirection(BoardObject.Direction.PARTIAL, false);

        Vector3 fromAngles = bObject.transform.localEulerAngles;
        fromAngles.y = (int)currentDirection * 45.0f;
        Vector3 toAngles = bObject.transform.localEulerAngles;
        toAngles.y = ((int)currentDirection + direction) * 45.0f;

        // Interpolate movement
        float auxTimer = 0.0f;
        while (auxTimer < time)
        {
            Vector3 lerpAngles = Vector3.Lerp(fromAngles, toAngles, auxTimer / time);
            bObject.transform.localEulerAngles = lerpAngles;
            auxTimer += Time.deltaTime;
            yield return null;
        }

        // Set final rotation (defensive code)
        bObject.SetDirection(targetDirection);
    }

    private Vector2Int GetDirectionFromString(string dir)
    {
        dir = dir.ToLower();
        switch (dir)
        {
            case ("down"):
                return Vector2Int.down;
            case ("right"):
                return Vector2Int.right;
            case ("up"):
                return Vector2Int.up;
            case ("left"):
                return Vector2Int.left;
            default:
                return Vector2Int.zero;
        }
    }

    private int GetRotationFromString(string rot)
    {
        rot = rot.ToLower();
        return rot == "clockwise" ? 1 : (rot == "anti_clockwise" ? -1 : 0);
    }

    public override void ReceiveMessage(string msg, MSG_TYPE type)
    {
        try
        {
            string[] args = msg.Split(' ');
            int amount = 0, index = -1, rot = 0;
            float intensity = 0.0f, time = 0.5f;
            bool active;
            Vector2Int dir;

            switch (type)
            {
                case MSG_TYPE.MOVE_LASER:
                    amount = int.Parse(args[0]);
                    dir = GetDirectionFromString(args[1]);
                    time = Mathf.Min(UBlockly.Times.instructionWaitTime / (amount + 1), 0.5f);
                    StartCoroutine(MoveObject("Laser", 0, dir, amount, time));
                    break;
                case MSG_TYPE.MOVE:
                    index = int.Parse(args[1]);
                    amount = int.Parse(args[2]);
                    dir = GetDirectionFromString(args[3]);
                    time = Mathf.Min(UBlockly.Times.instructionWaitTime / (amount + 1), 0.5f);
                    StartCoroutine(MoveObject(args[0], index - 1, dir, amount, time));
                    break;
                case MSG_TYPE.ROTATE_LASER:
                    amount = int.Parse(args[0]) * 2;
                    rot = GetRotationFromString(args[1]);
                    time = Mathf.Min(UBlockly.Times.instructionWaitTime / (amount + 1), 0.5f);
                    StartCoroutine(RotateObject("Laser", 0, rot, amount, time));
                    break;
                case MSG_TYPE.ROTATE:
                    index = int.Parse(args[1]);
                    amount = int.Parse(args[2]) * 2;
                    rot = GetRotationFromString(args[3]);
                    time = Mathf.Min(UBlockly.Times.instructionWaitTime / (amount + 1), 0.5f);
                    StartCoroutine(RotateObject(args[0], index - 1, rot, amount, time));
                    break;
                case MSG_TYPE.CHANGE_INTENSITY:
                    index = int.Parse(args[0]);
                    intensity = float.Parse(args[1]);
                    ChangeLaserIntensity(index - 1, intensity);
                    break;
                case MSG_TYPE.ACTIVATE_DOOR:
                    active = bool.Parse(args[2]);
                    index = int.Parse(args[1]);
                    ActivateDoor(args[0], index - 1, active);
                    break;
            }
        }
        catch
        {
            UBlockly.CSharp.Interpreter.Stop();
            gameOverPanel.SetActive(true);
            blackRect.SetActive(true);
        }
    }

    private void ChangeLaserIntensity(int index, float newIntensity)
    {
        if (elementPositions.ContainsKey("laser") && index < elementPositions["laser"].Count)
        {
            Vector2Int pos = elementPositions["laser"][index];
            LaserEmitter laser = (LaserEmitter)board[pos.x, pos.y].GetPlacedObject();
            if (laser != null)
            {
                laser.ChangeIntensity(newIntensity);
                currentSteps++;
            }
        }
    }

    private void ActivateDoor(string name, int index, bool active)
    {
        name = name.ToLower();
        if (name == "puerta" && elementPositions.ContainsKey(name) && index < elementPositions[name].Count)
        {
            Vector2Int pos = elementPositions[name][index];
            Door door = (Door)board[pos.x, pos.y].GetPlacedObject();
            if (door != null)
            {
                door.SetActive(active);
                currentSteps++;
            }
        }
    }

    public bool CellsOccupied(int id)
    {
        if (id >= cells.Count || cells[id].Count == 0) return false;

        int i = 0;
        while (i < cells[id].Count && cells[id][i].GetState() == BoardCell.BoardCellState.OCUPPIED)
            i++;

        return i >= cells[id].Count;
    }

    public override bool ReceiveBoolMessage(string msg, MSG_TYPE type)
    {
        string[] args = msg.Split(' ');
        switch (type)
        {
            case MSG_TYPE.CELL_OCCUPIED:
                return CellsOccupied(int.Parse(args[0]));
            default:
                return false;
        }
    }

    private void DeactivateHintButton()
    {
        if (hintButton == null) return;
        hintButton.GetComponent<Image>().color = Color.grey;
        hintButton.enabled = false;
    }

    public void UseHint()
    {
        if (hintsShown >= hintsParent.childCount /*|| ProgressManager.Instance.GetHintsRemaining() == 0*/)
        {
            DeactivateHintButton();
            return;
        }

        int i = 0;
        while (i < hintsPerUse && hintsShown + i < hintsParent.childCount)
        {
            hintsParent.transform.GetChild(hintsShown + i).gameObject.SetActive(true);
            i++;
        }
        hintsShown += i;
        //ProgressManager.Instance.AddHints(-1);

        if (hintsShown >= hintsParent.childCount /*|| ProgressManager.Instance.GetHintsRemaining() == 0*/)
            DeactivateHintButton();

        TrackerAsset.Instance.setVar("remaining_hints", ProgressManager.Instance.GetHintsRemaining());
        TrackerAsset.Instance.GameObject.Used("hint_used");
    }
}
