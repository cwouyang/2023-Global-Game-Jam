using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

public enum GameOverReason
{
    Time = 0,
    Water,
    Energy,
    Branch,
    Cheat
}

public class GamePlayManager : MonoBehaviour
{
    public GameSetting GameSetting;
    public EncounterManager EncounterManager;
    public RootController RootController;
    public CameraManager CameraManager;
    public GamePlayPanel GamePlayPanel;
    public TreeGirl TreeGirl;
    public GameResultMenu GameResultMenu;

    private GameOverReason _gameOverReason;

    [SerializeField] private Life _lifeController;

    public ParticleSystem EvolveParticle;
    public LevelMapGenerator levelMapGenerator; 

    [SerializeField] private AudioSource _bgm;

    private ResourceTracker ResourceTracker;

    private string _loseReasonString = "";

    private TierComputer.Tier _assignedTier = TierComputer.Tier.None;

    public GameStatus Status;
    public EndType EndType;

    async void Start()
    { 
        Play();
    }

    private void Update()
    {
#if UNITY_EDITOR
        _DetectForceEndGameForDemo();
#endif
    }

    public async void Play()
    {
        ResourceTracker = new ResourceTracker(
              time: new ResourceSetting<ulong>(0, ulong.MaxValue, GameSetting.InitialLifeSeconds),
              energy: new ResourceSetting<uint>(0, GameSetting.EnergyLimit, GameSetting.InitialEnergy),
              water: new ResourceSetting<uint>(0, GameSetting.WaterLimit, GameSetting.InitialWater),
              fertilizer: new ResourceSetting<uint>(0, GameSetting.FertilizeLimit, GameSetting.InitialFertilizer),
              branches: new ResourceSetting<uint>(0, GameSetting.InitialUsableBranches, GameSetting.InitialUsableBranches));

        ResourceTracker.ResourceExhausted += _OnResourceExhausted;

        levelMapGenerator.Init(GameSetting);
        EncounterManager.PrepareAll(ResourceTracker, CameraManager, levelMapGenerator, GamePlayPanel);
        EncounterManager.OnRootCrash += _OnRootCrash;

        RootController.OnGrowAction += _OnRootAction;
        RootController.OnRootCrash += _OnRootCrash;

        GamePlayPanel.Init(ResourceTracker, GameSetting);

        await UniTask.WhenAll(GamePlayTask(), TimerTask());
    }

    private async UniTask TimerTask()
    {
        float time = 0;

        while (true)
        {
            time += UnityEngine.Time.deltaTime;

            if (time > 1)
            {
                time -= 1;
                if (Status == GameStatus.Grow)
                {
                    _ResourceUpdatePerSecond();
                }
            }

            await UniTask.NextFrame();
        }
    }

    private async UniTask GamePlayTask()
    {
        Status = GameStatus.Begin;
        EndType = EndType.None;

        await DisplayBegin();

        await GamePlayMain();

        await DisplayEnd();
    }

    #region GameStatus
    private async UniTask DisplayBegin()
    {
        await CameraManager.EnterStageCameraPerform();
        Status = GameStatus.Grow;

        GamePlayPanel.ShowPanel();
        await UniTask.Delay(System.TimeSpan.FromSeconds(0.5f));
    }

    private async UniTask GamePlayMain()
    {
        RootController.StartGrow();
        while (Status != GameStatus.End)
        {
            if (Status == GameStatus.Grow)
            {
            }
            else if (Status == GameStatus.Crash)
            {
                _lifeController.LoseLife((int)ResourceTracker.Branch);
                await RootController.SwitchBranch();
                if (ResourceTracker.Branch > 0)
                    Status = GameStatus.Grow;
                else
                    Status = GameStatus.End;
            }

            await UniTask.NextFrame();
        }
        RootController.StopGrow();
    }

    private async UniTask DisplayEnd()
    {
        GamePlayPanel.HidePanel();

        TierComputer.Tier tier = TierComputer.Run(ResourceTracker);
        Debug.Log($"Final tier {tier.ToString()}");

        if (_assignedTier != TierComputer.Tier.None)
            tier = _assignedTier;

        await UniTask.Delay(System.TimeSpan.FromSeconds(1));

        _bgm.Stop();

        RootController.OnGameEnd();

        await CameraManager.ShowGameOverText(_gameOverReason);

        await CameraManager.ScrollToInitPos();
        EvolveParticle.Play();
        AudioManager.Instance.PlaySFX(ESoundEffectType.Evolve);
        await UniTask.Delay(System.TimeSpan.FromSeconds(1));

        await TreeGirl.SetFinalAppearance(tier);

        await GameResultMenu.ShowMask();

        EvolveParticle.Stop();
        EvolveParticle.Clear();

        await GameResultMenu.HideMask();
        await GameResultMenu.OpenContent(tier);
    }
    #endregion

    #region GameEvent
    private void _ResourceUpdatePerSecond()
    {
        ResourceTracker.DecreaseTime(1); 

        ResourceTracker.DecreaseWater(GameSetting.ConsumeWaterPerSecond); 

        if (ResourceTracker.Fertilizer > 0)
        {
            ResourceTracker.DecreaseFertilizer(GameSetting.ConsumeFertilizerPerSecond);
            ResourceTracker.IncreaseEnergy(GameSetting.BonusRecoverEnergyPerSecond);
        }

        ResourceTracker.IncreaseEnergy(GameSetting.RecoverEnergyPerSecond);

        Debug.Log($"GameStatus[{Status}] " +
            $"Time[{ResourceTracker.Time}] " +
            $"Water[{ResourceTracker.Water}] " +
            $"Energy[{ResourceTracker.Energy}] " +
            $"Fertilizer[{ResourceTracker.Fertilizer}]" +
            $"Branch[{ResourceTracker.Branch}]");
    }

    private void _OnRootAction()
    {
        ResourceTracker.DecreaseEnergy(GameSetting.ConsumeEnergyPerTime);
    }

    private void _OnRootCrash()
    {
        ResourceTracker.DecreaseBranch(1);
        if (ResourceTracker.Branch > 0) 
        {
            Status = GameStatus.Crash;
            Debug.Log("Status => Crash ");
        }
    }

    private void _DetectForceEndGameForDemo()
    {
        if (Status == GameStatus.Grow)
        {
            if (Input.GetKeyDown(KeyCode.Y))
                SetTierAndEndGame(TierComputer.Tier.F);
            else if (Input.GetKeyDown(KeyCode.U))
                SetTierAndEndGame(TierComputer.Tier.C);
            else if (Input.GetKeyDown(KeyCode.I))
                SetTierAndEndGame(TierComputer.Tier.B);
            else if (Input.GetKeyDown(KeyCode.O))
                SetTierAndEndGame(TierComputer.Tier.A);
            else if (Input.GetKeyDown(KeyCode.P))
                SetTierAndEndGame(TierComputer.Tier.S);
        }
    }

    private void SetTierAndEndGame(TierComputer.Tier tier)
    {
        _assignedTier = tier;
        Status = GameStatus.End;
        _gameOverReason = GameOverReason.Cheat;
    }

    private void _OnResourceExhausted(object sender, ResourceExhaustedEventArgs args)
    {
        switch (args.Resource)
        {
            case TimeResource time:
                Status = GameStatus.End;
                EndType = EndType.TimeOut;
                Debug.Log("Status => End : TimeOut");

                _gameOverReason = GameOverReason.Time;
                break;
            case WaterResource water:
                Status = GameStatus.End;
                EndType = EndType.WaterOut;
                Debug.Log("Status => End : WaterOut");

                _gameOverReason = GameOverReason.Water;
                break;
            case EnergyResource energy:
                Status = GameStatus.End;
                EndType = EndType.EnergyOut;
                Debug.Log("Status => End : EnergyOut");
                _gameOverReason = GameOverReason.Energy;
                break;
            case BranchResource branch:
                Status = GameStatus.End;
                EndType = EndType.BranchOut;
                Debug.Log("Status => End : BranchOut");
                _gameOverReason = GameOverReason.Branch;
                break;
        }
    }
    #endregion
}
