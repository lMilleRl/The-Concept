using System;
using UnityEngine;

namespace Choices
{
    public sealed class ChoiceNavigationController
    {
        private readonly ChoiceOptionData[] _options;
        private readonly int _columns;

        public int SelectedIndex { get; private set; }
        public ChoiceOptionData SelectedOption => _options[SelectedIndex];

        public ChoiceNavigationController(ChoiceOptionData[] options, int columns)
        {
            _options = options ?? Array.Empty<ChoiceOptionData>();
            _columns = Mathf.Max(1, columns);
            SelectedIndex = 0;
        }

        public bool Move(Vector2Int direction)
        {
            if (_options.Length < 2 || direction == Vector2Int.zero)
                return false;

            Vector2Int currentPosition = GetGridPosition(SelectedIndex);
            int candidateIndex = -1;
            int bestPrimaryDistance = int.MaxValue;
            int bestCrossDistance = int.MaxValue;
            int bestDistanceSquared = int.MaxValue;

            for (int i = 0; i < _options.Length; i++)
            {
                if (i == SelectedIndex)
                    continue;

                Vector2Int offset = GetGridPosition(i) - currentPosition;
                int primaryDistance = offset.x * direction.x + offset.y * direction.y;
                if (primaryDistance <= 0)
                    continue;

                int crossDistance = Math.Abs(offset.x * direction.y - offset.y * direction.x);
                int distanceSquared = offset.sqrMagnitude;

                if (primaryDistance > bestPrimaryDistance ||
                    (primaryDistance == bestPrimaryDistance && crossDistance > bestCrossDistance) ||
                    (primaryDistance == bestPrimaryDistance && crossDistance == bestCrossDistance && distanceSquared >= bestDistanceSquared))
                    continue;

                candidateIndex = i;
                bestPrimaryDistance = primaryDistance;
                bestCrossDistance = crossDistance;
                bestDistanceSquared = distanceSquared;
            }

            if (candidateIndex < 0)
                return false;

            SelectedIndex = candidateIndex;
            return true;
        }

        private Vector2Int GetGridPosition(int index)
        {
            return new Vector2Int(index % _columns, -(index / _columns));
        }
    }
}