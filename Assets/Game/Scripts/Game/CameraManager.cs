using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Lapis.Extension;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    public event Action<float> OnPositionYChanged;

    [SerializeField] CameraEffectController _cameraEffectController;


    [SerializeField] private float gameStartPosY;
    [SerializeField] private float gameStartPosMoveTime;
    [SerializeField] private Ease  gameStartMoveEaseType;

    [SerializeField] private float backToInitPosMoveTime;
    [SerializeField] private Ease backToInitPosMoveEaseType;

    [SerializeField] private RootTop rootTop;

    [SerializeField] private GameObject _gameOverLayout;
    [SerializeField] private Image _gameOverImage;
    [SerializeField] private Text _gameOverText;

    [SerializeField] private Sprite _gameOverTimeSprite;
    [SerializeField] private Sprite _gameOverWaterSprite;
    [SerializeField] private Sprite _gameOverEnergySprite;
    [SerializeField] private Sprite _gameOverBranchSprite;

    [SerializeField] float duration = 0.5f;
    [SerializeField] float strengh = 1f;
    [SerializeField] int vibrato = 3;
    [SerializeField] float randomness = 0;


    private void Awake()
    {
        instance = this;
    }

    private float distanceBuffer;
    // Start is called before the first frame update
    void Start()
    {
        rootTop.OnPositionYChange += yChangeAmount =>
        {
            ChangeCameraPosY(yChangeAmount);
            OnPositionYChanged?.Invoke(yChangeAmount);
        };
    }

    // Update is called once per frame
    void Update()
    {

    }


    private void ChangeCameraPosY(float posYChangeAmount)
    {
        transform.DOMoveY(transform.position.y + posYChangeAmount, 0.3f).SetEase(Ease.Linear);
    }

    public async UniTask ScrollToInitPos()
    {
        transform.DOLocalMoveY(gameStartPosY, backToInitPosMoveTime).SetEase(backToInitPosMoveEaseType);
        await UniTask.Delay(TimeSpan.FromSeconds(backToInitPosMoveTime + 0.5f));
    }

    public async UniTask EnterStageCameraPerform()
    {
        transform.DOLocalMoveY(gameStartPosY, gameStartPosMoveTime).SetEase(gameStartMoveEaseType);

        await UniTask.Delay(TimeSpan.FromSeconds(gameStartPosMoveTime + 0.5f));
    }

    public async UniTask ShowGameOverText(GameOverReason reason)
    {
        switch (reason)
        {
            case GameOverReason.Time:
                _gameOverText.text = "時間到！";
                _gameOverImage.sprite = _gameOverTimeSprite;
                break;
            case GameOverReason.Water:
                _gameOverText.text = "水份乾枯！！";
                _gameOverImage.sprite = _gameOverWaterSprite;
                break;
            case GameOverReason.Energy:
                _gameOverText.text = "能量耗盡！";
                _gameOverImage.sprite = _gameOverEnergySprite;
                break;
            case GameOverReason.Branch:
                _gameOverText.text = "樹根用完了！";
                _gameOverImage.sprite = _gameOverBranchSprite;
                break;
        }

        _gameOverLayout.transform.localPosition = new Vector3(-1500, 0, 0);
        _gameOverLayout.transform.DOLocalMove(new Vector3(0, 0, 0), 1).SetEase(Ease.OutBack);
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        _gameOverLayout.transform.DOLocalMove(new Vector3(1500, 0, 0), 1).SetEase(Ease.InBack);
        await UniTask.Delay(TimeSpan.FromSeconds(1));
    }

    public void ShakeCamera()
    {
        transform.DOShakePosition(duration, strengh, vibrato, randomness);
    }
}
