using _Project.Scripts.Meta.MetaData;
using _Project.Scripts.Runtime.RuntimeData;

namespace _Project.Scripts.Managers
{
    public class ChronaManager
    {
        private readonly RunData _run;
        private readonly MetaProgressData _meta;

        public ChronaManager(RunData run, MetaProgressData meta)
        {
            _run = run;
            _meta = meta;
        }

        public void RescueChrona()
        {
            _run.IsChronaInParty = true;
            _meta.IsChronaRescued = true;
        }

        public bool IsFirstEncounter()
        {
            return !_meta.IsChronaRescued;
        }
    }
}