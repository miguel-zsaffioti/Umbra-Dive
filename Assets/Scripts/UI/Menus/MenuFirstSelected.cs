using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuFirstSelected : MonoBehaviour
{
    [Tooltip("Em ordem de prioridade. Usa o primeiro que estiver ativo e interagível.")]
    public Selectable[] candidates;

    private void OnEnable()
    {
        StartCoroutine(SelectNextFrame());
    }

    private IEnumerator SelectNextFrame()
    {
        yield return null;
        SelectFirst();
    }

    private void Update()
    {
        var es = EventSystem.current;
        if (es == null) return;

        GameObject current = es.currentSelectedGameObject;
        if (current == null || !current.activeInHierarchy)
        {
            SelectFirst();
        }
    }

    private void SelectFirst()
    {
        var es = EventSystem.current;
        if (es == null) return;

        foreach (var selectable in candidates)
        {
            if (selectable != null && selectable.gameObject.activeInHierarchy && selectable.IsInteractable())
            {
                es.SetSelectedGameObject(selectable.gameObject);
                return;
            }
        }
    }
}