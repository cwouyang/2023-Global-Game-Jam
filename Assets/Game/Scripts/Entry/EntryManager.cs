using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class EntryManager : MonoBehaviour
{
    [SerializeField] private bool skipLaunchScreen;

    [SerializeField] private Image teamLogo;
    [SerializeField] private Text teamName;

    [SerializeField] private Color teamNameColor;

    private AudioManager audioManager;
    private ScreenEffectManager screenEffectManager;

    private Tester tester;

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_EDITOR
        SetupTester();
#else
        skipLaunchScreen = false;
#endif

        SetupSystemManagers();
        SetupTeamInfo();
        StartCoroutine(teamLogoPerform());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void SetupTester()
    {
        print("Setup Tester");
        tester = new GameObject("Tester").AddComponent<Tester>();
    }

    private void SetupSystemManagers()
    {
        audioManager = new GameObject("AudioManager").AddComponent<AudioManager>();
        screenEffectManager = new GameObject("ScreenEffectManager").AddComponent<ScreenEffectManager>();
    }

    private void SetupTeamInfo()
    {
        teamName.text = Config.teamName;
        teamLogo.color = Color.white;
        teamName.color = teamNameColor;
    }

    private IEnumerator teamLogoPerform()
    {
        if (skipLaunchScreen)
        {
            yield return null;
            SceneTransitionManager.Instance.SwitchScene(SceneType.Home);
        }
        else
        {
            teamLogo.DOFade(0, 0);
            teamName.DOFade(0, 0);
            yield return new WaitForSeconds(0.3f);
            teamLogo.DOFade(1, 2);
            teamName.DOFade(1, 2);

            yield return new WaitForSeconds(1f);

            AudioManager.Instance.PlaySFX(ESoundEffectType.ShowTeamInfo);

            yield return new WaitForSeconds(3f);

            SceneTransitionManager.Instance.SwitchScene(SceneType.Home);
        }
    }
}
