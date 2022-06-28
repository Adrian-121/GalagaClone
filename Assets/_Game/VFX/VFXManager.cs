using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class VFXManager : MonoBehaviour, IGameControlled {

    private VFXObject.Factory _vfxObjectFactory;

    private List<VFXObject> _vfxObjectList = new List<VFXObject>();

    [Inject]
    public void Construct(VFXObject.Factory vfxObjectFactory) {
        _vfxObjectFactory = vfxObjectFactory;

        for (int i = 0; i < Constants.VFX_POOL_MAX; i++) {
            VFXObject newVFX = _vfxObjectFactory.Create();
            newVFX.Construct();
            newVFX.transform.SetParent(transform);
            _vfxObjectList.Add(newVFX);
        }
    }

    public void Deinitialize() {
    }

    public void OnUpdate() {
        foreach (VFXObject vfxObject in _vfxObjectList) {
            vfxObject.OnUpdate();
        }
    }

    /// <summary>
    /// Tries to spawn a new visual effect, if any available.
    /// </summary>
    public void TrySpawnVFX(VFXObject.TypeEnum type, Vector3 position) {
        VFXObject vfxToUse = null;

        foreach (VFXObject vfxObject in _vfxObjectList) {
            if (!vfxObject.IsAlive) {
                vfxToUse = vfxObject;
                break;
            }
        }

        if (vfxToUse == null) { return; }

        vfxToUse.Initialize(type, position);
    }
}