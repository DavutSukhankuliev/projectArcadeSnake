using UnityEngine;
using UnityEngine.UI;

public class PlayerRay : MonoBehaviour
{
    private float _timer;
    private float _timerMax = 2f;

    private bool _isTimer;

    private bool _toggle;

    private void Start()
    {
        _timer = _timerMax;
    }

    void LateUpdate()
    {
        Ray ray = new Ray(transform.position, transform.forward);
       
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            _isTimer = true;
            _timer -= Time.deltaTime;
            if (_timer <= 0)
            {
                var componentButton = hit.collider.gameObject.GetComponent<Button>();
                if (!componentButton)
                {
                    var componentToggle = hit.collider.gameObject.GetComponent<Toggle>();
                    _toggle = componentToggle.isOn;
                    componentToggle.onValueChanged.Invoke(!_toggle);
                }
                else
                {
                    componentButton.onClick.Invoke();
                }
                _timer = _timerMax;
                _isTimer = false;
            }
        }
        else
        {
            _timer = _timerMax;
        }
        
    }
}
