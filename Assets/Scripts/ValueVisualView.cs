using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValueVisualView : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer _pointVisual;
    [SerializeField]
    private SpriteRenderer _maxValueVisual;
    [SerializeField]
    private SpriteRenderer _minRequiredValueVisual;
    [SerializeField]
    private SpriteRenderer _minValueRequiredVisual;

    private IntValue _value;
    [SerializeField]
    private float _pointsDistance;

    private List<SpriteRenderer> _points;
    [SerializeField]
    private int _maxValue;

    [SerializeField]
    private Color _disableColor;
    [SerializeField]
    private Color _damagedColor;
    [SerializeField]
    private Color _activeColor;

    public bool awaken;

    private void Awake()
    {
        _points = new List<SpriteRenderer>();
        for (int i = 0; i < _maxValue; i++)
        {
            var point = Instantiate(_pointVisual, transform);
            point.transform.localPosition = _pointVisual.transform.localPosition + _pointsDistance * Vector3.up * i;
            point.gameObject.SetActive(true);
            point.color = _disableColor;
            _points.Add(point);
        }
        awaken = true;
    }

    public void Setup(int minValueRequired)
    {
        minValueRequired--;
        /*Debug.Log(minValueRequired);
       
        _minValueRequiredVisual.gameObject.SetActive(minValueRequired > 0);
        if (_points.Count <= 0) return;
        minValueRequired = minValueRequired < 0 ? 0 : minValueRequired < _points.Count ? minValueRequired : _points.Count -1;
        _minValueRequiredVisual.transform.localPosition = _points[minValueRequired].transform.localPosition +Vector3.up*0.5f;*/
        
        _minRequiredValueVisual.size = new Vector2(_minRequiredValueVisual.size.x, 0.75f + 0.5f * (minValueRequired));
    }
    
    public void UpdateVisualPoints(IntValue points)
    {
        _maxValueVisual.size = new Vector2(_maxValueVisual.size.x, 0.75f + 0.5f * (points.MaxValue-1));
        
        for (int i = 0; i <_points.Count; i++)
        {
            _points[i].color = i < points.CurrentValue ? _activeColor : i > points.MaxValue-1 ? _disableColor : _damagedColor;
        }
    }
}
