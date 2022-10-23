using UnityEngine;

namespace AzuMarketplaceSigns;

public class SignHover : MonoBehaviour, Hoverable
{
    public string mSignName = "";
    public string mText = "";

    public string GetHoverText() => Localization.instance.Localize(mText);

    public string GetHoverName() => Localization.instance.Localize(mText);
}