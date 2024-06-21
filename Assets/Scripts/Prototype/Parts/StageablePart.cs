using Unity.Entities;
using UnityEngine;

namespace Kosmos.Prototype.Parts
{
    public class StageablePart : PartBase
    {
        [SerializeField]
        protected int StageIndex;

        public int GetStageIndex()
        {
            return StageIndex;
        }
    }
}
