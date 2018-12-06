using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuNavigation : MonoBehaviour
{
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _controlsMenu;
    [SerializeField] private GameObject _skillsAndConsumables;
    [SerializeField] private GameObject _objectiveText;
    [SerializeField] private Transform[] _cameraPoints;

    private CameraFollowPlayer _camScript;

    private Quaternion _originalCamTransform;
    private Vector3 _originalCamOffset;

    public void Start()
    {
        _camScript = GameController.camera.GetComponent<CameraFollowPlayer>();

        _originalCamTransform = GameController.camera.transform.rotation;
        _originalCamOffset = _camScript.Offset;

        _camScript.Target = _cameraPoints[0];
        _camScript.TargetRotation = _cameraPoints[0].rotation;

        _camScript.Offset = Vector3.zero;

        _skillsAndConsumables.SetActive(false);
        _objectiveText.SetActive(false);
    }

    public void StartGame()
    {
        _camScript.Target = GameController.player.transform;
        _camScript.TargetRotation = _originalCamTransform;
        _camScript.Offset = _originalCamOffset;

        GameController.gameHasStarted = true;
        GameController.player.GetComponent<PlayerMovement>().enabled = true;
        GameController.player.GetComponent<PlayerAttacks>().enabled = true;

        _mainMenu.SetActive(false);
        _skillsAndConsumables.SetActive(true);
        _objectiveText.SetActive(true);
    }

    public void ControlsMenu()
    {
        _camScript.Target = _cameraPoints[1];
        _camScript.TargetRotation = _cameraPoints[1].rotation;

        _mainMenu.SetActive(false);
        _controlsMenu.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void Back()
    {
        _camScript.Target = _cameraPoints[0];
        _camScript.TargetRotation = _cameraPoints[0].rotation;

        _mainMenu.SetActive(true);
        _controlsMenu.SetActive(false);
    }
}
