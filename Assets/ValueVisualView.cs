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
    private void Awake()
    {
        _points = new List<SpriteRenderer>();

        for (int i = 0; i < 10; i++)
        {
            var point = Instantiate(_pointVisual, transform);
            point.transform.localPosition = _pointVisual.transform.localPosition + _pointsDistance * Vector3.up * i;
            point.gameObject.SetActive(true);
            point.color = _disableColor;
            _points.Add(point);
        }

        var value = new IntValue(7);
        value.SetValueTo(5);
        SetMinRequiredPoints(3);
        UpdateVisualPoints(value);
    }

    public void SetMinRequiredPoints(int value)
    {
        if (_points.Count <= 0) return;
        value = value < 0 ? 0 : value < _points.Count ? value : _points.Count -1;
        _minValueRequiredVisual.transform.position = _points[value].transform.position;

    }
    public void UpdateVisualPoints(IntValue points)
    {
        _maxValueVisual.size = new Vector2(_maxValueVisual.size.x, 0.75f + 0.5f * points.MaxValue);
        
        for (int i = 0; i <_points.Count; i++)
        {
            _points[i].color = i < points.CurrentValue ? _activeColor : i > points.MaxValue ? _disableColor : _damagedColor;
        }
    }
}
