using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField]
    private bool GoToHubPortal = false;
    [Header("Disappear variables")]
    [SerializeField]
    private bool _disappear = false;
    [SerializeField]
    private float _disappearSpeed = 1;
    [SerializeField]
    private int _timeBeforeDisappearInMs = 10000;
    [SerializeField]
    private Shader _transparentShader;

    private DateTime _disappearTime;
    private bool _isDisappearing = false;
    private List<Material> _materials = new List<Material>();
    private Renderer[] _renderers;

    private void Start()
    {
        if (_disappear)
        {
            _disappearTime = DateTime.Now.AddMilliseconds(_timeBeforeDisappearInMs);
        }
    }

    private void Update()
    {
        if (!_isDisappearing)
        {
            TeleportPlayer();
            if (_disappear)
            {
                if (DateTime.Now > _disappearTime)
                {
                    _isDisappearing = true;
                    _renderers = GetComponentsInChildren<Renderer>();

                    //Disable colliders
                    Collider[] _colliders = GetComponentsInChildren<Collider>();
                    foreach (Collider collider in _colliders)
                    {
                        collider.enabled = false;
                    }

                    //Change shaders and get materials
                    foreach (Renderer renderer in _renderers)
                    {
                        if (renderer != GetComponent<Renderer>())
                        {
                            if (renderer.gameObject.name != "PortalFX" && renderer.gameObject.name != "Wobble" && renderer.gameObject.name != "Mist" && renderer.gameObject.name != "Shading")
                            {
                                foreach (Material material in renderer.materials)
                                {
                                    _materials.Add(material);
                                    material.shader = _transparentShader;
                                }
                            }
                            else
                            {
                                if (renderer.gameObject.GetComponent<ParticleSystem>() != null)
                                {
                                    renderer.gameObject.GetComponent<ParticleSystem>().Stop();
                                }
                            }
                        }
                    }
                }
            }
        }
        else
        {
            foreach (Material material in _materials)
            {
                material.SetColor("_OutlineColor", Color.Lerp(material.GetColor("_OutlineColor"), new Color(0, 0, 0, 0), Time.deltaTime * _disappearSpeed));
                material.SetFloat("_AlphaMultiplier", Mathf.Lerp(material.GetFloat("_AlphaMultiplier"), 0, Time.deltaTime * _disappearSpeed));

                if (material.GetFloat("_AlphaMultiplier") > 0.1f)
                {
                    break;
                }
            }
            foreach (Material material in _materials)
            {
                if (material.GetFloat("_AlphaMultiplier") > 0.1f)
                {
                    return;
                }
            }
            foreach (Renderer renderer in _renderers)
            {
                if (renderer != GetComponent<Renderer>())
                {
                    renderer.gameObject.SetActive(false);
                }
            }
        }

        //temp keybind;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (GoToHubPortal)
            {
                GameController.GoToHub();
            }
            else
            {
                GameController.GoToLevel();
            }
        }
    }


    private void TeleportPlayer()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit raycastHit = new RaycastHit();
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out raycastHit);
            if (raycastHit.transform.gameObject == this.gameObject)
            {
                if (GoToHubPortal)
                {
                    if (OlChapBehaviour.GetQuestProgression() == 2 || OlChapBehaviour.GetQuestProgression() == 4)
                    {
                        OlChapBehaviour.ContinuePorgression();
                        ObjectiveText.SetObjectiveText("- Go back to the old man");
                    }
                    GameController.GoToHub();
                }
                else
                {
                    switch (OlChapBehaviour.GetQuestProgression())
                    {
                        case 2:
                            ObjectiveText.SetObjectiveText("- Kill and enemy and find an item");
                            break;
                        case 4:
                            ObjectiveText.SetObjectiveText("- Kill a radish");
                            break;
                        case 5:
                            ObjectiveText.SetObjectiveText("- Find and loot a chest");
                            break;
                        case 6:
                            ObjectiveText.SetObjectiveText("- Find keys (" + GameController.questProgress + "/5)");
                            break;
                    }
                    GameController.GoToLevel();
                }
            }
        }
    }
}
