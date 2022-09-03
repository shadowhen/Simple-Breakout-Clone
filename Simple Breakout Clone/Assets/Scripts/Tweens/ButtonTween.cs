using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonTween : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float _maxScale;
    [SerializeField] private float _minScale;
    [SerializeField] private float _maxScaleDuration;
    [SerializeField] private float _minScaleDuration;

    private Tweener _tweener;

    public void ScaleUp()
    {
        // Kill the tween if active so we can stop the tween from running now
        if (_tweener.IsActive())
        {
            _tweener.Kill();
        }
        
        // Scale up the button and store it in a variable for checking active later
        _tweener = transform.DOScale(_maxScale, _maxScaleDuration).SetUpdate(true);
    }

    public void ScaleDown()
    {
        // Kill the tween if active so we can stop the tween from running now
        if (_tweener.IsActive())
        {
            _tweener.Kill();
        }

        // Scale up the button and store it in a variable for checking active later
        _tweener = transform.DOScale(_minScale, _minScaleDuration).SetUpdate(true);
    }

    // Implements method where the pointer enters the button and the button scales up
    public void OnPointerEnter(PointerEventData eventData)
    {
        ScaleUp();
    }

    // Implements method where the pointer exits the button and the button scales up
    public void OnPointerExit(PointerEventData eventData)
    {
        ScaleDown();
    }

    // When the button is visible, make sure that the button is scaled at 1.0 uniformly
    private void OnEnable()
    {
        transform.localScale = Vector3.one;
    }
}
