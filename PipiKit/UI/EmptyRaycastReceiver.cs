using UnityEngine;
using UnityEngine.UI;

namespace ChenPipi.PipiKit.UI
{

    [AddComponentMenu("UI/PipiKit/EmptyRaycastReceiver")]
    public class EmptyRaycastReceiver : MaskableGraphic
    {

        protected EmptyRaycastReceiver()
        {
            useLegacyMeshGeneration = false;
        }

        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            toFill.Clear();
        }

    }

}
