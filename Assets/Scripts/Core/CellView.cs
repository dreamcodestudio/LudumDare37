using System.Collections;
using System.Collections.Generic;
using Lean.Touch;
using UnityEngine;
using UnityEngine.UI;

public class CellView : MonoBehaviour
{

    [SerializeField] private Text _dangerLabel;

    [Header("Gameobject states")]
    [SerializeField]
    private GameObject _lockedGo;
    [SerializeField]
    private GameObject _openedGo;
    [SerializeField] private GameObject _failGo;
    [SerializeField]
    private GameObject _markedGo;

    public int i;
    public int j;

    private FieldModel _fieldModel;
    private Renderer _renderer;

    #region UNITY_EVENTS

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }

    private void Start ()
    {
        LeanTouch.OnFingerUp += OnFingerUp;
    }
	

    #endregion

    public void SetField(FieldModel field)
    {
        _fieldModel = field;
    }

    public void SetValue(int val)
    {
        _dangerLabel.text = val.ToString();
    }

    public void SetState(ECellState state)
    {
        switch (state)
        {
            case ECellState.Locked:
                _markedGo.SetActive(false);
                _lockedGo.SetActive(true);
                break;
            case ECellState.Opened:
                _lockedGo.SetActive(false);
                _markedGo.SetActive(false);
                _openedGo.SetActive(true);
                break;
            case ECellState.Marked:
                _lockedGo.SetActive(false);
                _markedGo.SetActive(true);
                break;
            case ECellState.Failed:
                _lockedGo.SetActive(false);
                _failGo.SetActive(true);
                break;
        }
    }

    public void OnFingerUp(LeanFinger finger)
    {
        var ray = finger.GetRay();
        var hit = default(RaycastHit);
        if (Physics.Raycast(ray, out hit, float.PositiveInfinity))
        {
            if (hit.collider.gameObject == gameObject)
            {
                //Debug.LogFormat(" I {0}  J {1}", i, j);
                if (_fieldModel.ActionType == EActionType.Open)
                {
                    _fieldModel.Open(i, j);
                }
                else if (_fieldModel.ActionType == EActionType.Mark)
                {
                    _fieldModel.Mark(i, j);
                }
            }
        }
    }
}
