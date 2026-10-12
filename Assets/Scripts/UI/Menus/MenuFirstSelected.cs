using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

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

        if (IsGamepadConnected())
        {
            SelectFirst();
        }
    }

    private void Update()
    {
        var es = EventSystem.current;
        if (es == null) return;

        GameObject current = es.currentSelectedGameObject;
        if ((current == null || !current.activeInHierarchy) && IsGamepadConnected())
        {
            SelectFirst();
        }
    }

    private bool IsGamepadConnected()
    {
        if (Gamepad.current != null) return true;

        foreach (var device in InputSystem.devices)
        {
            if (device is Gamepad)
            {
                return true;
            }
        }

        return false;
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