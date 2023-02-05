using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class GameResultMenu : MonoBehaviour
{

    [SerializeField] private CanvasGroup _contentGroup;
    [SerializeField] private CanvasGroup _maskGroup;

    [SerializeField] private Button _replayButton;
    [SerializeField] private Button _homeButton;

    [SerializeField] private Image _treeGirlImage;
    [SerializeField] private List<Sprite> treeGirlSpriteList;
    [SerializeField] private Text _rankTierText;

    // Start is called before the first frame update
    void Start()
    {
        _homeButton.onClick.AddListener(() =>
        {
            SceneTransitionManager.Instance.SwitchScene(SceneType.Home);
            AudioManager.Instance.PlaySFX(ESoundEffectType.Click);
        });

        _replayButton.onClick.AddListener(() => {
            SceneTransitionManager.Instance.SwitchScene(SceneType.Game);
            AudioManager.Instance.PlaySFX(ESoundEffectType.Click);
        });

        _contentGroup.alpha = 0f;
        _maskGroup.alpha = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public async UniTask ShowMask()
    {
        DOTween.To(() => _maskGroup.alpha, x => _maskGroup.alpha = x, 1f, 2f);
        await UniTask.Delay(System.TimeSpan.FromSeconds(2f));
    }

    public async UniTask HideMask()
    {
        DOTween.To(() => _maskGroup.alpha, x => _maskGroup.alpha = x, 0f, 2f);
        await UniTask.NextFrame();
    }

    public async UniTask OpenContent(TierComputer.Tier tier)
    {
        _contentGroup.alpha = 0f;
        _treeGirlImage.sprite = treeGirlSpriteList[(int)tier];
        _rankTierText.text = tier.ToString();

        DOTween.To(() => _contentGroup.alpha, x => _contentGroup.alpha = x, 1f, 1f);
        await UniTask.NextFrame();
    }
}
