using System.Collections;
using System.Collections.Generic;
using IndieYP.Utils;
using Lean.Touch;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameThreadManager : MonoBehaviour
{

    [Header("UI")] [SerializeField] private Text _remainingMines;
    [SerializeField]
    private Button _openBtn;
    [SerializeField]
    private Button _markBtn;
    [SerializeField]
    private Button _restartBtn;

    [SerializeField] private Text _openLabel;
    [SerializeField] private Text _markLabel;
    [SerializeField] private Color _selectedActionColor;

    [Header("Prefabs")] [SerializeField] private GameObject _cellPrefab;

    [Header("Gameplay")] [SerializeField] private Transform _gameFieldTransform;


    IDictionary<string , CellView> _cells = new Dictionary<string, CellView>(); 


    private FieldModel _fieldModel;
    private const float CELL_OFFSET = 1.2f;

    #region UNITY_EVENTS

    private void Start()
    {
        Debug.Log("Start level");

        _fieldModel = FieldModel.Create(8, 8);

        EventManager.OnCellExploded += OnCellExploded;
        EventManager.OnFieldUpdated += OnFieldUpdated;
        EventManager.OnCompletedStage += OnCompletedStage;
        
        _openBtn.onClick.AddListener(OnOpenBtnClick);
        _markBtn.onClick.AddListener(OnMarkBtnClick);
        _restartBtn.onClick.AddListener(OnRestartBtnClick);

        _remainingMines.text = "Remaining cores " + _fieldModel.RemainingMinesCount;
        OnOpenBtnClick();

        _fieldModel.CheckCell((i, j) =>
        {
            var cellView =
                Instantiate(_cellPrefab,
                    new Vector3(j*_cellPrefab.transform.localScale.x * CELL_OFFSET, 0f, i*_cellPrefab.transform.localScale.z * CELL_OFFSET),
                    Quaternion.identity).GetComponent<CellView>();
            cellView.i = i;
            cellView.j = j;
            cellView.SetField(_fieldModel);
            //cellView.SetColor(Color.gray);
            cellView.transform.parent = _gameFieldTransform;
            var key = i.ToString() + j.ToString();
            _cells.Add(key,cellView);


        });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        _fieldModel.CheckCell((i, j) =>
        {
            var cellView = _cells[i.ToString() + j.ToString()];
            if (_fieldModel.Opened[i, j])
            {
                cellView.SetState(ECellState.Opened);
                if (_fieldModel.Values[i, j] > 0)
                {
                    cellView.SetValue(_fieldModel.Values[i, j]);
                }
            }
            else
            {
                //cellView.SetColor(Color.gray);
                if (_fieldModel.Marked[i, j])
                {
                    cellView.SetState(ECellState.Marked);
                }
            }
        });
    }

    private void OnDestroy()
    {
        EventManager.OnCellExploded -= OnCellExploded;
        EventManager.OnFieldUpdated -= OnFieldUpdated;
        EventManager.OnCompletedStage -= OnCompletedStage;

        _openBtn.onClick.RemoveListener(OnOpenBtnClick);
        _markBtn.onClick.RemoveListener(OnMarkBtnClick);
        _restartBtn.onClick.RemoveListener(OnRestartBtnClick);

        LeanTouch.OnFingerUp = null;
    }

    #endregion

    private void OnCompletedStage()
    {
        _remainingMines.text = "You win";
        _restartBtn.gameObject.SetActive(true);
    }

    private void OnCellExploded()
    {
        Debug.Log("You lose");

        _fieldModel.CheckCell((i, j) =>
        {
            var cellView = _cells[i.ToString() + j.ToString()];
            if (_fieldModel.Mines[i, j])
            {
                cellView.SetState(ECellState.Failed);
            }
        });

        _remainingMines.text = "You lose";
        _restartBtn.gameObject.SetActive(true);
    }

    private void OnFieldUpdated()
    {
        _remainingMines.text = "Remaining cores " + _fieldModel.RemainingMinesCount;
    }

    private void OnOpenBtnClick()
    {
        _fieldModel.ActionType = EActionType.Open;
        _markLabel.color = Color.white;
        _openLabel.color = _selectedActionColor;
    }

    private void OnMarkBtnClick()
    {
        _fieldModel.ActionType = EActionType.Mark;
        _openLabel.color = Color.white;
        _markLabel.color = _selectedActionColor;
    }

    private void OnRestartBtnClick()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
