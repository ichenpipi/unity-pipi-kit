using UnityEngine;
using UnityEngine.UI;

namespace ChenPipi.UI
{

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
