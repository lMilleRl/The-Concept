using UnityEngine;

public class ClimbingTransition : StateTransition
{
    private ITriggerDetector _ladderDetector;

    private bool _isLadder;

    public ClimbingTransition(BaseStateTransitionData baseData, ITriggerDetector ladderDetector) : base(baseData)
    {
        _ladderDetector = ladderDetector;

        _ladderDetector.Triggered += HandleCollision;
    }

    protected override bool TryThrowRequestToEnterState()
    {
        if (_isLadder)
        {
            ThrowThatConditionComplete();
            _isLadder = false;
            return true;
        }

        return false;
    }

    private void HandleCollision(Collider2D collision)
    {
        if (!IsCurrentStateThatForTransActive()) return;

        if (collision.TryGetComponent<LadderTrigger>(out LadderTrigger ladderTrigger))
        {
            _isLadder = true;
        }
    }

    ~ClimbingTransition()
    {
        _ladderDetector.Triggered -= HandleCollision;
    }
}