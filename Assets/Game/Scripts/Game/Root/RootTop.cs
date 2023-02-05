using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using Cysharp.Threading.Tasks;

public class RootTop : MonoBehaviour
{

    [SerializeField] private Rigidbody2D _rigidbody2D;

    public event Action<float> OnPositionYChange;
    private float initPosY;
    private bool isMoving = false;

    [SerializeField] private LineRenderer _lineRenderer;


    // Start is called before the first frame update
    void Start()
    {
        initPosY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            var currentPointPos = new Vector3(transform.position.x, transform.position.y, 0);
            _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, currentPointPos);
        }
    }

    public void SetLineRenderer(LineRenderer lineRenderer, bool isFirstStart = false)
    {
        _lineRenderer = lineRenderer;
        _lineRenderer.positionCount = 2;
        var initLinePointPos = new Vector3(transform.position.x, transform.position.y, 0);
        _lineRenderer.SetPosition(0, initLinePointPos);
        _lineRenderer.SetPosition(1, initLinePointPos);
    }

    public async UniTask MoveTo(Vector3 position, bool needToDrawLine = true)
    {
        isMoving = true;

        if (position.y > initPosY)
            position.y = initPosY;

        if (position.x > 9f)
        {
            position.x = 9f;
        }
        else if (position.x < -9f)
        {
            position.x = -9f;
        }

        var posYChangeAmount = position.y - transform.position.y;
        OnPositionYChange?.Invoke(posYChangeAmount);
        transform.DOMove(position, Config.ROOT_GROW_PERFORM_TIME).SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                if (needToDrawLine)
                {
                    _lineRenderer.positionCount++;
                    var newLinePointPos = new Vector3(transform.position.x, transform.position.y, 0);
                    _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, newLinePointPos);
                }

                isMoving = false;
            });

        await UniTask.NextFrame();
    }
}
