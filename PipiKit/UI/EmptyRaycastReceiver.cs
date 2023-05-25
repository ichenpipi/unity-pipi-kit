using UnityEngine.UI;

namespace ChenPipi.UI
{

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
