using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparencyCast : MonoBehaviour
{

    [SerializeField]
    private Shader _transparentShader;
    [SerializeField]
    private float _fadeSpeed = 3f;

    private List<Transform> _obstructingTransforms = new List<Transform>();
    private List<Transform> _lastObstructingTransforms = new List<Transform>();

    private Dictionary<Transform, List<Shader>> _defaultShaders = new Dictionary<Transform, List<Shader>>();

    // Update is called once per frame
    void Update()
    {
        GetObstructingObjects();

        //Change shaders back to originals
        for (int i = 0; i < _lastObstructingTransforms.Count; i++)
        {
            if (_lastObstructingTransforms[i] != null && _lastObstructingTransforms[i].GetComponent<Renderer>() != null && !_obstructingTransforms.Contains(_lastObstructingTransforms[i]))
            {
                Material[] materials = _lastObstructingTransforms[i].GetComponent<Renderer>().materials;
                for (int j = 0; j < materials.Length; j++)
                {
                    materials[j].SetColor("_OutlineColor", new Color(0, 0, 0, 1));
                    materials[j].SetFloat("_AlphaMultiplier", 1f);
                    materials[j].shader = _defaultShaders[_lastObstructingTransforms[i]][j];
                }
            }
        }

        //Change shader to transparent shader
        for (int i = 0; i < _obstructingTransforms.Count; i++)
        {
            if (_obstructingTransforms[i] != GameController.player.transform)
            {
                if (_obstructingTransforms[i] != null)
                {
                    if (_obstructingTransforms[i].GetComponent<Renderer>().material.shader.name != _transparentShader.name && _obstructingTransforms[i].GetComponent<Renderer>().bounds.size.y > 2.5f)
                    {
                        Material[] materials = _obstructingTransforms[i].GetComponent<Renderer>().materials;
                        foreach (Material material in materials)
                        {
                            material.shader = _transparentShader;
                            material.SetFloat("_AlphaMultiplier", 0.8f);
                            material.SetColor("_OutlineColor", new Color(0, 0, 0, 0));
                        }
                    }
                }
            }
        }

        foreach (Transform _transform in _obstructingTransforms)
        {
            if (_transform.GetComponent<Renderer>() != null && _transform.GetComponent<Renderer>().bounds.size.y > 2.5f)
            {
                Material[] materials = _transform.GetComponent<Renderer>().materials;
                foreach (Material material in materials)
                {
                    material.SetColor("_OutlineColor", new Color(0, 0, 0, 0));
                    material.SetFloat("_AlphaMultiplier", Mathf.Lerp(_transform.GetComponent<Renderer>().material.GetFloat("_AlphaMultiplier"), 0.1f, Time.deltaTime * _fadeSpeed));
                }
            }
        }
    }

    private void GetObstructingObjects()
    {
        Ray ray = new Ray(this.transform.position, (GameController.player.transform.position - this.transform.position).normalized);
        RaycastHit[] raycasts = Physics.RaycastAll(ray, (this.transform.position - GameController.player.transform.position).magnitude);
        _lastObstructingTransforms.Clear();
        foreach (Transform _transform in _obstructingTransforms)
        {
            _lastObstructingTransforms.Add(_transform);
        }
        _obstructingTransforms.Clear();
        foreach (RaycastHit raycastHit in raycasts)
        {
            _obstructingTransforms.Add(raycastHit.transform);
        }


        foreach (Transform _transform in _obstructingTransforms)
        {
            if (!_defaultShaders.ContainsKey(_transform))
            {
                if (_transform != GameController.player.transform && _transform.GetComponent<Renderer>() != null)
                {
                    _defaultShaders[_transform] = new List<Shader>();
                    foreach (Material material in _transform.GetComponent<Renderer>().materials)
                    {
                        _defaultShaders[_transform].Add(material.shader);
                    }
                }
            }
        }
    }
}
