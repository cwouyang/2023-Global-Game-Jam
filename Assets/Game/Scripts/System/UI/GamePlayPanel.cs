using Cysharp.Threading.Tasks;

using DG.Tweening;

using UnityEngine;
using UnityEngine.UI;

public sealed class GamePlayPanel : MonoBehaviour
{
    public Timeer Timeer;
    public Water Water;
    public NutrientBar NutrientBar;
    public CrisBar CrisBar;

    [SerializeField] private CanvasGroup GameplayUi;
    [SerializeField] private Text _timeChangeHintText;
    [SerializeField] private Text _waterChangeHintText;
    [SerializeField] private Text _energyChangeHintText;
    [SerializeField] private Text _fertilizerChangeHintText;

    public bool Collided { get; set; }

    public void Init(ResourceTracker resourceTracker, GameSetting gameSetting)
    {
        GameplayUi.alpha = 0;
        Water.Init(resourceTracker, gameSetting);
        Timeer.Init(resourceTracker);
        NutrientBar.Init(resourceTracker, gameSetting);
        CrisBar.Init(resourceTracker, gameSetting);

        _waterChangeHintText.enabled = false;
        _timeChangeHintText.enabled = false;
        _energyChangeHintText.enabled = false;
        _fertilizerChangeHintText.enabled = false;

        resourceTracker.ResourceValueChanged += (sender, args) =>
        {
            if (!Collided)
            {
                return;
            }

            int diffValue;
            Text targetText;
            switch (args.NewValue)
            {
                case TimeResource w:
                    diffValue = (int)w.Value - (int)((TimeResource)args.OldValue).Value;
                    targetText = _timeChangeHintText;
                    break;
                case WaterResource w:
                    diffValue = (int)w.Value - (int)((WaterResource)args.OldValue).Value;
                    targetText = _waterChangeHintText;
                    break;
                case FertilizerResource f:
                    diffValue = (int)f.Value - (int)((FertilizerResource)args.OldValue).Value;
                    targetText = _fertilizerChangeHintText;
                    break;
                case EnergyResource e:
                    diffValue = (int)e.Value - (int)((EnergyResource)args.OldValue).Value;
                    targetText = _energyChangeHintText;
                    break;
                default:
                    return;
            }

            if (diffValue == 0)
            {
                return;
            }

            UniTask.Void
            (
                async () =>
                {
                    Transform gameObjectTransform = targetText.gameObject.transform;
                    Vector3 oldPosition = gameObjectTransform.localPosition;

                    char sign = diffValue > 0 ? '+' : ' ';
                    targetText.text = $"{sign}{diffValue}";
                    targetText.enabled = true;

                    float delta = 0;
                    while (delta < 2)
                    {
                        targetText.gameObject.transform.localPosition = new Vector2
                            (gameObjectTransform.localPosition.x, gameObjectTransform.localPosition.y + 0.05f);
                        targetText.color = new Color(targetText.color.r, targetText.color.g, targetText.color.b, 1 - delta / 3);
                        await UniTask.NextFrame();
                        delta += Time.deltaTime;
                    }

                    targetText.enabled = false;
                    targetText.gameObject.transform.localPosition = oldPosition;
                }
            );
        };
    }

    public void ShowPanel()
    {  
        DOTween.To(() => GameplayUi.alpha, x => GameplayUi.alpha = x, 1f, 0.5f);
    }

    public void HidePanel()
    {
        DOTween.To(() => GameplayUi.alpha, x => GameplayUi.alpha = x, 0f, 0.5f); 
    }
}