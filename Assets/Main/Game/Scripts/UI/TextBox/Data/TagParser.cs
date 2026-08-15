using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace TextBox
{
    public class TagParser : ITagParser
    {
        private const int NoId = -1;

        public ParseResult Parse(string rawText)
        {
            if (string.IsNullOrEmpty(rawText))
                return new ParseResult { CleanText = string.Empty, Commands = Array.Empty<TextBoxCommandContext>() };

            var cleanText = new StringBuilder(rawText.Length);
            var openTags = new List<OpenTag>();
            var commands = new List<TextBoxCommandContext>();
            int visibleIndex = 0;

            int i = 0;
            while (i < rawText.Length)
            {
                if (rawText[i] == '[')
                {
                    if (TryParseCustomTag(rawText, i, out var tag, out int tagEnd))
                    {
                        ProcessTag(tag, ref visibleIndex, openTags, commands);
                        i = tagEnd + 1;
                        continue;
                    }

                    cleanText.Append(rawText[i]);
                    visibleIndex++;
                    i++;
                }
                else if (rawText[i] == '<' && TryFindRichTagEnd(rawText, i, out int gt))
                {
                    cleanText.Append(rawText, i, gt - i + 1);
                    i = gt + 1;
                }
                else
                {
                    cleanText.Append(rawText[i]);
                    visibleIndex++;
                    i++;
                }
            }

            for (int j = 0; j < openTags.Count; j++)
            {
                var open = openTags[j];
                commands.Add(new TextBoxCommandContext
                {
                    CommandType = open.CommandType,
                    StartCharIndex = open.VisibleStart,
                    CharLength = visibleIndex - open.VisibleStart,
                    Params = open.Params
                });
            }

            commands.Sort((a, b) =>
            {
                int cmp = a.StartCharIndex.CompareTo(b.StartCharIndex);
                if (cmp == 0)
                    return b.CharLength.CompareTo(a.CharLength);
                return cmp;
            });

            return new ParseResult
            {
                CleanText = cleanText.ToString(),
                Commands = commands.ToArray()
            };
        }

        private void ProcessTag(
            in TagParseResult tag, ref int visibleIndex,
            List<OpenTag> openTags, List<TextBoxCommandContext> commands)
        {
            if (tag.IsClosing)
            {
                int matchIdx = FindMatchingOpen(openTags, tag);

                if (matchIdx < 0)
                    return;

                var open = openTags[matchIdx];
                commands.Add(new TextBoxCommandContext
                {
                    CommandType = open.CommandType,
                    StartCharIndex = open.VisibleStart,
                    CharLength = visibleIndex - open.VisibleStart,
                    Params = open.Params
                });
                openTags.RemoveAt(matchIdx);
            }
            else
            {
                openTags.Add(new OpenTag
                {
                    Id = tag.Id,
                    HasId = tag.HasId,
                    CommandType = tag.CommandType,
                    VisibleStart = visibleIndex,
                    Params = tag.Params
                });
            }
        }

        private static bool TryFindRichTagEnd(string text, int startIndex, out int tagEnd)
        {
            tagEnd = -1;

            int i = startIndex + 1;
            if (i >= text.Length)
                return false;

            if (text[i] == '/')
                i++;

            if (i >= text.Length || !char.IsLetter(text[i]))
                return false;

            while (i < text.Length && text[i] != '=' && text[i] != '>' && text[i] != '<')
            {
                if (char.IsWhiteSpace(text[i]))
                    return false;
                i++;
            }

            if (i >= text.Length || text[i] == '<')
                return false;

            int gt = text.IndexOf('>', i);
            if (gt < 0)
                return false;

            tagEnd = gt;
            return true;
        }

        private static int FindMatchingOpen(List<OpenTag> openTags, in TagParseResult closingTag)
        {
            for (int j = openTags.Count - 1; j >= 0; j--)
            {
                if (closingTag.HasId)
                {
                    if (openTags[j].HasId && openTags[j].Id == closingTag.Id)
                        return j;
                }
                else
                {
                    if (!openTags[j].HasId && openTags[j].CommandType == closingTag.CommandType)
                        return j;
                }
            }

            return -1;
        }

        private bool TryParseCustomTag(
            string text, int startIndex,
            out TagParseResult tag, out int tagEnd)
        {
            tag = default;
            tagEnd = -1;

            int i = startIndex + 1;
            if (i >= text.Length)
                return false;

            bool isClosing = false;
            if (text[i] == '\\')
            {
                isClosing = true;
                i++;
                if (i >= text.Length)
                    return false;
            }

            int idStart = i;
            while (i < text.Length && char.IsDigit(text[i]))
                i++;

            bool hasId = i > idStart;
            int id = hasId
                ? int.Parse(text.Substring(idStart, i - idStart), CultureInfo.InvariantCulture)
                : NoId;

            if (hasId)
            {
                if (i >= text.Length || text[i] != ';')
                    return false;
                i++;
            }

            int nameStart = i;
            while (i < text.Length && text[i] != '=' && text[i] != ']')
                i++;

            if (i == nameStart)
                return false;

            string name = text.Substring(nameStart, i - nameStart).ToLowerInvariant();

            float[] parameters = Array.Empty<float>();
            if (i < text.Length && text[i] == '=')
            {
                i++;
                int paramStart = i;
                while (i < text.Length && text[i] != ']')
                    i++;

                if (i > paramStart)
                    parameters = ParseParams(text, paramStart, i - paramStart);
            }

            if (i >= text.Length || text[i] != ']')
                return false;

            tagEnd = i;

            if (!Enum.TryParse<TextBoxCommandType>(name, true, out var cmdType))
                cmdType = TextBoxCommandType.Unknown;

            tag = new TagParseResult
            {
                Id = id,
                HasId = hasId,
                CommandType = cmdType,
                IsClosing = isClosing,
                Params = parameters
            };

            return true;
        }

        private static float[] ParseParams(string text, int start, int length)
        {
            string block = text.Substring(start, length);
            string[] parts = block.Split(':');
            float[] result = new float[parts.Length];

            for (int i = 0; i < parts.Length; i++)
                float.TryParse(parts[i], NumberStyles.Float, CultureInfo.InvariantCulture, out result[i]);

            return result;
        }

        private struct TagParseResult
        {
            public int Id;
            public bool HasId;
            public TextBoxCommandType CommandType;
            public bool IsClosing;
            public float[] Params;
        }

        private struct OpenTag
        {
            public int Id;
            public bool HasId;
            public TextBoxCommandType CommandType;
            public int VisibleStart;
            public float[] Params;
        }
    }
}
