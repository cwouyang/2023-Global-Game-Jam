using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

public class HomeManager : MonoBehaviour
{

    [SerializeField] private Button enterGameButton;

    // Start is called before the first frame update
    void Start()
    {
        enterGameButton.onClick.AddListener(() => EnterGame());
    }

    private void EnterGame()
    {
        StartCoroutine(EnterScenePerform());
    }

    private IEnumerator EnterScenePerform()
    {
        CameraEffectController.Instance.StartFadeOut(1f);
        yield return new WaitForSeconds(1);
        SceneTransitionManager.Instance.SwitchScene(SceneType.Game);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
