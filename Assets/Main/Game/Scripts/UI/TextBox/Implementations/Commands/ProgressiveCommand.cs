using System;
using System.Collections.Generic;
using UnityEngine;

namespace TextBox
{
    public class ProgressiveCommand : ITextBoxCommand, IDisposable
    {
        private readonly ITypeRunner _typeRunner;
        private readonly ITextBoxFacade _facade;
        private readonly IProgressiveTargetService _targetService;
        private readonly IDebugWriter _debugWriter;
        private readonly ProgressiveTargetId _defaultTargetId;

        private readonly List<IProgressiveTarget> _activeTargets = new();
        private TextBoxCommandContext _activeContext;

        public TextBoxCommandType Type => TextBoxCommandType.Progressive;

        public ProgressiveCommand(ITypeRunner typeRunner, ITextBoxFacade facade, IProgressiveTargetService targetService, IDebugWriter debugWriter, ProgressiveTargetId defaultTargetId)
        {
            _typeRunner = typeRunner;
            _facade = facade;
            _targetService = targetService;
            _debugWriter = debugWriter;
            _defaultTargetId = defaultTargetId;
        }

        public void Execute(TextBoxCommandContext context)
        {
            _activeTargets.Clear();
            CollectTargets(context);

            if (_activeTargets.Count == 0)
            {
                _debugWriter.LogWarning($"[{nameof(ProgressiveCommand)}] No progressive targets found.");
                return;
            }

            _activeContext = context;
            _typeRunner.OnCharRevealed += HandleCharRevealed;
            _facade.OnCurrentTextEnded += HandleCurrentTextEnded;

            SetProgress(0f);
        }

        private void CollectTargets(TextBoxCommandContext context)
        {
            if (context.Params.Length == 0)
            {
                AddTarget(_defaultTargetId);
                return;
            }

            for (int i = 0; i < context.Params.Length; i++)
            {
                AddTarget((ProgressiveTargetId)(int)context.Params[i]);
            }
        }

        private void AddTarget(ProgressiveTargetId id)
        {
            var target = _targetService.Get(id);

            if (target == null)
            {
                _debugWriter.LogWarning($"[{nameof(ProgressiveCommand)}] No target found for ID {id}.");
                return;
            }

            _activeTargets.Add(target);
        }

        private void HandleCharRevealed(int charIndex)
        {
            if (charIndex < _activeContext.StartCharIndex)
                return;

            float progress = Mathf.Clamp01(
                (float)(charIndex - _activeContext.StartCharIndex + 1) / _activeContext.CharLength);

            SetProgress(progress);

            if (progress >= 1f)
                Unsubscribe();
        }

        private void HandleCurrentTextEnded()
        {
            SetProgress(1f);
            Unsubscribe();
        }

        private void SetProgress(float progress)
        {
            for (int i = 0; i < _activeTargets.Count; i++)
                _activeTargets[i].SetProgress(progress);
        }

        public void Dispose()
        {
            Unsubscribe();
        }

        private void Unsubscribe()
        {
            _typeRunner.OnCharRevealed -= HandleCharRevealed;
            _facade.OnCurrentTextEnded -= HandleCurrentTextEnded;
            _activeTargets.Clear();
        }
    }
}
