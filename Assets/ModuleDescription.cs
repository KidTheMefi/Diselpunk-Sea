using System;
using TMPro;
using UnityEngine;

public class ModuleDescription : MonoBehaviour
{
    [SerializeField]
    private GameObject _descriptionView;
    [SerializeField]
    private TextMeshPro _descriptionTextMeshPro;

    
    public void SetDescriptionText(string text)
    {
        _descriptionTextMeshPro.text = text;
    }
    
    private void OnMouseDown()
    {
        _descriptionView.SetActive(!_descriptionView.activeSelf);
    }
}
