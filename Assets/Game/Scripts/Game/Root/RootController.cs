using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lapis.Extension;
using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public enum PlayerState
{
    Prepare = 0,
    SetDirection = 1,
    SetScale = 2,
    Growing = 3,
    GameOver = 99,
}

public class RootController : MonoBehaviour, IRootController
{
    public event Action OnGrowAction;
    public event Action OnRootCrash;
    private PlayerState _currentPlayerState = PlayerState.Prepare;

    [SerializeField] private RootTop rootTop;
    [SerializeField] private Transform _nextGrowFinalNode;

    [Header("Scale")]
    [SerializeField] private GameObject _lengthIndicator;
    [SerializeField] private float _scaleChangeRate = 10f;
    [SerializeField] private float _lengthGrowDirection = 1;
    [SerializeField] private float _minlengthScale = 3f;
    [SerializeField] private float _maxlengthScale = 8f;
    private float _currentScaleAmount = 2f;


    [Header("Rotation")]
    [SerializeField] private float _rotateSpeed = 2;
    private float _currentRotateSpeed = 2;

    [Header("LineRenderer")]
    [SerializeField] private LineRenderer _lineRendererPrefab;
    [SerializeField] private LineRenderer _currentLineRenderer;

    [Header("Record")]
    private Stack<GameObject> _recordPointStack;
    [SerializeField] private GameObject _recordPointPrefab;
    [SerializeField] private GameObject _initRecordPoint;
    [SerializeField] private int recordCounter;
    [SerializeField] private int recordFrequency;

    // Start is called before the first frame update
    void Start()
    {
        _currentScaleAmount = _minlengthScale;
        _lengthGrowDirection = 1;

        _recordPointStack = new Stack<GameObject>();
        _recordPointStack.Push(_initRecordPoint);

        AssignNewLineRenderer(true);
    }

    // Update is called once per frame
    void Update()
    {
        StateMachine();

        if (Input.GetMouseButtonDown(0) || Input.GetButtonDown("Confirm"))
        {
            OnPlayerClick();
        }

        if (Input.GetMouseButtonUp(0) || Input.GetButtonUp("Confirm"))
        {
            OnPlayerClickEnd();
        }
    }

    public void StartGrow()
    {
        _currentRotateSpeed = 0f;
        DOTween.To(() => _currentRotateSpeed, x => _currentRotateSpeed = x, _rotateSpeed, 1);

        _currentPlayerState = PlayerState.SetDirection;
        rootTop.gameObject.SetActive(true);
    }

    public void StopGrow()
    {
        _currentPlayerState = PlayerState.GameOver;
        rootTop.gameObject.SetActive(false);
    }

    public void OnGameEnd()
    {
//        _currentPlayerState = PlayerState.GameOver;
//        rootTop.gameObject.SetActive(false);
    }

    public async UniTask SwitchBranch()
    {
        await rootTop.MoveTo(_recordPointStack.Peek().transform.position, false);

        await DeactivateCurrentLineRenderer();

        await UniTask.Delay(System.TimeSpan.FromSeconds(0.5f));

        await AssignNewLineRenderer();

        await UniTask.NextFrame();
    }

    private async UniTask DeactivateCurrentLineRenderer()
    {
        _currentLineRenderer.startColor = new Color(0, 87, 255);
        _currentLineRenderer.endColor = new Color(0, 87, 255);
        await UniTask.NextFrame();
    }

    private async UniTask AssignNewLineRenderer(bool isFirstTime = false)
    {
        _currentLineRenderer = Instantiate(_lineRendererPrefab, transform);
        rootTop.SetLineRenderer(_currentLineRenderer, isFirstTime);

        recordCounter = 0;
        await UniTask.NextFrame();
    }

    private void StateMachine()
    {
        switch (_currentPlayerState)
        {
            case PlayerState.SetDirection:
                var moveDirection = Input.GetAxisRaw("Horizontal");
//                var newRotation = rootTop.transform.eulerAngles.z + (moveDirection * _rotateSpeed);
//                rootTop.transform.rotation = Quaternion.Euler(new Vector3(0, 0, newRotation));
                rootTop.transform.Rotate(new Vector3(0, 0, _currentRotateSpeed * Time.deltaTime));
                //                rootTop.transform.SetRotationZ(moveDirection * _rotateSpeed);
                break;

            case PlayerState.SetScale:
                _currentScaleAmount += _scaleChangeRate * _lengthGrowDirection * UnityEngine.Time.deltaTime;
                _lengthIndicator.transform.SetScaleY(_currentScaleAmount);
                if (_currentScaleAmount >= _maxlengthScale || _currentScaleAmount <= _minlengthScale)
                    _lengthGrowDirection *= -1;
            break;
        }
    }

    private void OnPlayerClick()
    {
        switch (_currentPlayerState)
        {
            case PlayerState.SetDirection:
                _currentPlayerState = PlayerState.SetScale;
                break;
                /*
            case PlayerState.SetScale:
                StartCoroutine(GrowRootSequence());
                AudioManager.Instance.PlaySFX(ESoundEffectType.GrowRoot);
                recordCounter++;
                break;
                */
        }
    }

    private void OnPlayerClickEnd()
    {
        switch (_currentPlayerState)
        {
            case PlayerState.SetScale:
                StartCoroutine(GrowRootSequence());
                AudioManager.Instance.PlaySFX(ESoundEffectType.GrowRoot);
                recordCounter++;
                break;
        }
    }

    private IEnumerator GrowRootSequence()
    {
        OnGrowAction?.Invoke();
        StretchRootTopTo(_nextGrowFinalNode.position);
        _currentPlayerState = PlayerState.Growing;
        _lengthIndicator.SetActive(false);

        yield return new WaitForSeconds(Config.ROOT_GROW_PERFORM_TIME);

        if (recordCounter == recordFrequency)
        {
            var newRecordPoint = Instantiate(_recordPointPrefab, transform);
            newRecordPoint.transform.position = rootTop.transform.position;
            _recordPointStack.Push(newRecordPoint);
            recordCounter = 0;
        }

        _lengthIndicator.SetActive(true);

        // 讓角度往回推一點
        rootTop.transform.Rotate(new Vector3(0, 0, 5));

        _currentRotateSpeed = 0f;
        DOTween.To(() => _currentRotateSpeed, x => _currentRotateSpeed = x, _rotateSpeed, 1);
        _currentPlayerState = PlayerState.SetDirection;
    }

    private void StretchRootTopTo(Vector3 position)
    {
        rootTop.MoveTo(position);

        _currentScaleAmount = _minlengthScale;
        _lengthGrowDirection = 1;
        _lengthIndicator.transform.SetScaleY(_currentScaleAmount);
    }
}
