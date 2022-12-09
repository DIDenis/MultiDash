using UnityEngine;
using Mirror;
using System.Collections;

public class ColorChanger : NetworkBehaviour
{
    [SerializeField] Color hitColor = new Color(1, 0, 0);
    [SerializeField] float colorChangeDuration = 3f;
    SkinnedMeshRenderer meshRenderer;
    bool isInvincible;

    void Awake()
    {
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    [ServerCallback]
    public bool Hit()
    {
        if (isInvincible)
            return false;
        else
        {
            StartCoroutine(HitAwait());
            return true;
        }
    }

    [Server]
    public void SetData(HitData newValue)
    {
        isInvincible = newValue.boolValue;
        RpcSetData(newValue);
    }

    [ClientRpc]
    public void RpcSetData(HitData newValue)
    {
        var propertyBlock = new MaterialPropertyBlock();
        propertyBlock.SetColor("_Color", newValue.colorValue);
        meshRenderer.SetPropertyBlock(propertyBlock);
    }

    IEnumerator HitAwait()
    {
        yield return new WaitForFixedUpdate();
        Color temp = meshRenderer.material.color;
        HitData hitData = new HitData(hitColor, true);
        SetData(hitData);
        yield return new WaitForSeconds(colorChangeDuration);
        hitData.colorValue = temp;
        hitData.boolValue = false;
        SetData(hitData);
    }
}

public struct HitData
{
    public Color colorValue;
    public bool boolValue;

    public HitData(Color colorValue, bool boolValue)
    {
        this.colorValue = colorValue;
        this.boolValue = boolValue;
    }
}
