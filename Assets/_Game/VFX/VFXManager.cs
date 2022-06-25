using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class VFXManager : MonoBehaviour {

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

    public void SpawnVFX(VFXObject.TypeEnum type, Vector3 position) {
        VFXObject vfxToUse = null;

        foreach (VFXObject vfObject in _vfxObjectList) {
            if (!vfObject.IsAlive) {
                vfxToUse = vfObject;
                break;
            }
        }

        if (vfxToUse == null) { return; }

        vfxToUse.Initialize(type, position);
    }
}