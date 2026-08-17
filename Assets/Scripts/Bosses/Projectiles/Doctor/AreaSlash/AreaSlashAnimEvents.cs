using System;
using UnityEngine;

public class AreaSlashAnimEvents : MonoBehaviour
{
    private AreaSlash slash;

    private void Start()
    {
        slash =  GetComponentInParent<AreaSlash>();
    }

    public void Activate()
    {
        slash.activated = true;
    }

    public void Deactivate()
    {
        slash.activated = false;
        Destroy(slash.gameObject);
    }
}
