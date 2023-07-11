using DentedPixel;
using UnityEngine;

public class Walker : MonoBehaviour
{
    [SerializeField, Header("Settings")]
    private float _speed = 0.1f;
    [SerializeField]
    private float _time = 1f;

    [SerializeField, Header("Path")]
    private CircleCreator _path;

    private bool _toggle;
    private bool _isRunningForward;
    private bool _isRunningBackward;
    private float _currJourneyPercent;
    private float _journeyFractionTime;
    private int? _moveTwid;

    // TODO: should this be put on the PathCreatir/CircleCreator ?
    private int _index;

    private void Update()
    {
        if (_path.EvenPoints == null || _path.EvenPoints.Length == 0) { return; }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            _toggle = !_toggle;

            if (_toggle)
            {
                startRunForward();
            }
            else
            {
                startRunBackward();
            }
        }
    }

    private void startRunForward()
    {
        if (_isRunningForward) { return; }

        if (_isRunningBackward == true)
        {
            _index--;
            _journeyFractionTime = _journeyFractionTime - Find(_currJourneyPercent * 100, _of: _journeyFractionTime);
        }
        else
        {
            _currJourneyPercent = 0f;
            _journeyFractionTime = _time / _path.EvenPoints.Length;
        }
        _isRunningForward = true;
        _isRunningBackward = false;

        // Debug.Log("_currJourneyPercent: " + _currJourneyPercent);

        runForward();
    }

    private void runForward()
    {
        if (_moveTwid.HasValue)
        {
            LeanTween.cancel(_moveTwid.Value, callOnComplete: false);
            _moveTwid = null;
        }

        _moveTwid = LeanTween.value(_currJourneyPercent, 1f, _journeyFractionTime).id;

        LeanTween.descr(_moveTwid.Value)
            .setOnUpdate(((float value) =>
            {
                _currJourneyPercent = value;
                int nextI = _index + 1;
                var isLastPoint = nextI == _path.EvenPoints.Length;
                if (isLastPoint)
                {
                    LeanTween.cancel(_moveTwid.Value, callOnComplete: false);
                    _moveTwid = null;

                    _index = 0;
                    _currJourneyPercent = 0f;
                    _journeyFractionTime = _time / _path.EvenPoints.Length;
                    runForward();
                    return;
                }
                // Debug.Log("from " + _index + " to " + nextI);
                transform.position = Vector3.Lerp(_path.EvenPoints[_index], _path.EvenPoints[nextI], _currJourneyPercent);
            }))
            .setOnComplete(() =>
            {
                _index++;
                _currJourneyPercent = 0f;
                _journeyFractionTime = _time / _path.EvenPoints.Length;
                runForward();
            });
    }

    private void startRunBackward()
    {
        if (_isRunningBackward) { return; }

        if (_isRunningForward == true)
        {
            _index++;
            _journeyFractionTime = Find(_currJourneyPercent * 100, _of: _journeyFractionTime);
        }
        else
        {
            _currJourneyPercent = 1f;
            _journeyFractionTime = _time / _path.EvenPoints.Length;
        }
        _isRunningForward = false;
        _isRunningBackward = true;

        runBackward();
    }

    private void runBackward()
    {
        if (_moveTwid.HasValue)
        {
            LeanTween.cancel(_moveTwid.Value, callOnComplete: false);
            _moveTwid = null;
        }

        _moveTwid = LeanTween.value(_currJourneyPercent, 0f, _journeyFractionTime).id;

        LeanTween.descr(_moveTwid.Value)
            .setOnUpdate((float value) =>
            {
                _currJourneyPercent = value;
                int nextI = _index - 1;
                var isLastPoint = nextI == -1;
                if (isLastPoint)
                {
                    LeanTween.cancel(_moveTwid.Value, callOnComplete: false);
                    _moveTwid = null;

                    _index = _path.EvenPoints.Length - 1;
                    _currJourneyPercent = 1f;
                    _journeyFractionTime = _time / _path.EvenPoints.Length;
                    runBackward();
                    return;
                }
                // Debug.Log("from " + nextI + " to " + _index);
                transform.position = Vector3.Lerp(_path.EvenPoints[nextI], _path.EvenPoints[_index], _currJourneyPercent);
            })
            .setOnComplete(() =>
            {
                _index--;
                _currJourneyPercent = 1f;
                _journeyFractionTime = _time / _path.EvenPoints.Length;
                runBackward();
            });
    }

    public static float Find(float _percent, float _of)
    {
        return (_of / 100f) * _percent;
    }
}
