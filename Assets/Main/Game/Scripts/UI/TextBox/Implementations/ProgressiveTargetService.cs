using System.Collections.Generic;
using UnityEngine;

namespace TextBox
{
    public class ProgressiveTargetService : IProgressiveTargetService
    {
        private readonly Dictionary<ProgressiveTargetId, IProgressiveTarget> _targets;

        public ProgressiveTargetService(IEnumerable<IProgressiveTarget> targets)
        {
            _targets = new Dictionary<ProgressiveTargetId, IProgressiveTarget>();

            foreach (var target in targets)
            {
                if (target.Id == ProgressiveTargetId.None)
                {
                    Debug.LogWarning($"[{nameof(ProgressiveTargetService)}] Target {target.GetType().Name} has {nameof(ProgressiveTargetId.None)} and will be ignored.");
                    continue;
                }

                if (_targets.ContainsKey(target.Id))
                {
                    Debug.LogWarning($"[{nameof(ProgressiveTargetService)}] Duplicate target ID {target.Id}. Overwriting previous target.");
                }

                _targets[target.Id] = target;
            }
        }

        public IProgressiveTarget Get(ProgressiveTargetId id)
        {
            _targets.TryGetValue(id, out var target);
            return target;
        }
    }
}
