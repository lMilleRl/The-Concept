using System.Collections.Generic;

namespace TextBox
{
    public static class TextBoxCompositionFactory
    {
        public static ITextEffect[] CreateEffects(TextEffectEntry[] entries, ICharProgressProvider progressProvider)
        {
            var effects = new ITextEffect[entries.Length];

            for (int i = 0; i < entries.Length; i++)
            {
                effects[i] = entries[i].Type switch
                {
                    TextBoxCommandType.Wave => new WaveEffect(entries[i].Params, progressProvider),
                    TextBoxCommandType.Shake => new ShakeEffect(entries[i].Params, progressProvider),
                    TextBoxCommandType.Distortion => new DistortionEffect(entries[i].Params, progressProvider),
                    _ => null
                };
            }

            return effects;
        }

        public static ITextBoxCommand[] CreateCommands(TextCommandEntry[] entries,
            ITypeRunner typeRunner, ITextBoxVoiceSpeaker voiceSpeaker, ITextFormChanger textFormChanger,
            IProgressiveTargetService progressiveTargetService, IDebugWriter debugWriter, IEventRegistry eventRegistry)
        {
            var commands = new List<ITextBoxCommand>(entries.Length);

            for (int i = 0; i < entries.Length; i++)
            {
                switch (entries[i].Type)
                {
                    case TextBoxCommandType.Progressive:
                    case TextBoxCommandType.ReplaceText:
                    case TextBoxCommandType.Mute:
                        continue;
                }

                ITextBoxCommand commandToAdd = entries[i].Type switch
                {
                    TextBoxCommandType.Pause => new PauseCommand(typeRunner, entries[i].DefaultValue),
                    TextBoxCommandType.Speed => new SpeedCommand(typeRunner, entries[i].DefaultValue),
                    TextBoxCommandType.EaseSpeed => new EaseSpeedCommand(typeRunner, debugWriter, entries[i].DefaultValue),
                    TextBoxCommandType.Event => new EventCommand(eventRegistry, debugWriter),
                    TextBoxCommandType.Wave or
                    TextBoxCommandType.Shake or
                    TextBoxCommandType.Distortion => new EffectCommand(entries[i].Type, textFormChanger),
                    _ => null
                };

                commands.Add(commandToAdd);
            }

            return commands.ToArray();
        }

        public static void RegisterFacadeCommands(ICommandCoordinator coordinator, TextCommandEntry[] entries,
            ITypeRunner typeRunner, ITextBoxFacade facade, IProgressiveTargetService progressiveTargetService,
            TextRegistry textRegistry, ITextBoxVoiceSpeaker voiceSpeaker, IDebugWriter debugWriter,
            out ProgressiveCommand progressiveCommand, out MuteCommand muteCommand)
        {
            var progressiveEntry = System.Array.Find(entries, e => e.Type == TextBoxCommandType.Progressive);
            var progressiveDefaultId = progressiveEntry.Type == TextBoxCommandType.Progressive
                ? (ProgressiveTargetId)(int)progressiveEntry.DefaultValue
                : ProgressiveTargetId.None;

            progressiveCommand = new ProgressiveCommand(typeRunner, facade, progressiveTargetService, debugWriter, progressiveDefaultId);
            coordinator.Register(progressiveCommand);
            coordinator.Register(new ReplaceTextCommand(facade, textRegistry, debugWriter));

            muteCommand = new MuteCommand(typeRunner, voiceSpeaker, facade);
            coordinator.Register(muteCommand);
        }
    }
}