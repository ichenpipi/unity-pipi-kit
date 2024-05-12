using UnityEngine;
using UnityEngine.UI;

namespace ChenPipi.PipiKit.UI
{

    [AddComponentMenu("UI/PipiKit/GraphicsRaycasterLite")]
    public class GraphicsRaycasterLite : GraphicRaycaster
    {

        [SerializeField]
        public Camera targetCamera;

        public override Camera eventCamera
        {
            get
            {
                if (targetCamera == null)
                {
                    targetCamera = base.eventCamera;
                }
                return targetCamera;
            }
        }

    }

}
