using _Project.Scripts.Core;
using _Project.Scripts.Runtime.Characters;
using UnityEngine;

namespace _Project.Scripts.Runtime.Environment
{
    public abstract class UnitSlot : MonoBehaviour
    {
        public SlotType SlotType => _slotType;

        [SerializeField] private SlotType _slotType;

        public UnitController OccupiedUnit { get; private set; }

        public void SetUnit(UnitController unit)
        {
            if (unit == null)
                return;

            OccupiedUnit = unit;
            unit.BindToSlot(this);

            unit.transform.SetParent(transform);
            unit.transform.localPosition = Vector3.zero;
            unit.transform.localRotation = Quaternion.identity;
        }

        public void ClearUnit()
        {
            if (OccupiedUnit != null)
                OccupiedUnit.Unbind();

            OccupiedUnit = null;
        }
    }
}