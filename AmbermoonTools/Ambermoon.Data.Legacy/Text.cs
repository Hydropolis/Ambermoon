﻿using System.Collections.Generic;

namespace Ambermoon.Data.Legacy
{
    public class Text : IText
    {
        public Text(byte[] glyphIndices)
        {
            GlyphIndices = glyphIndices;
            int currentLineSize = 0;
            LineCount = 1;

            for (int i = 0; i < glyphIndices.Length; ++i)
            {
                if (glyphIndices[i] == (byte)SpecialGlyph.NewLine)
                {
                    if (currentLineSize > MaxLineSize)
                        MaxLineSize = currentLineSize;

                    ++LineCount;
                    currentLineSize = 0;
                }
                else if (glyphIndices[i] >= (byte)SpecialGlyph.FirstColor)
                {
                    continue;
                }
                else
                {
                    ++currentLineSize;
                }
            }
        }

        public byte[] GlyphIndices { get; }
        public int LineCount { get; }
        public int MaxLineSize { get; }
    }

    public class TextProcessor : ITextProcessor
    {
        /// <summary>
        /// Main character name
        /// </summary>
        public string LeadName { get; set; }
        /// <summary>
        /// Active character name
        /// </summary>
        public string SelfName { get; set; }
        /// <summary>
        /// Current caster name
        /// </summary>
        public string CastName { get; set; }
        /// <summary>
        /// Current inventory owner name
        /// </summary>
        public string InvnName { get; set; }
        /// <summary>
        /// Current subject name
        /// </summary>
        public string SubjName { get; set; }
        /// <summary>
        /// Current sex-dependent 3rd person pronoun (e.g. 'he' or 'she')
        /// </summary>
        public string Sex1Name { get; set; }
        /// <summary>
        /// Current sex-dependent 3rd person possessive determiner (e.g. 'his' or 'her')
        /// </summary>
        public string Sex2Name { get; set; }

        static byte CharToGlyph(char ch, bool rune)
        {
            if (ch >= 'a' && ch <= 'z')
                return (byte)(ch - 'a' + (rune ? 64 : 0));
            else if (ch >= 'A' && ch <= 'Z')
                return (byte)(ch - 'A' + (rune ? 64 : 0));
            else if (ch == 'ä' || ch == 'Ä')
                return (byte)(rune ? 90 : 26);
            else if (ch == 'ü' || ch == 'Ü')
                return (byte)(rune ? 91 : 27);
            else if (ch == 'ö' || ch == 'Ö')
                return (byte)(rune ? 92 : 28);
            else if (ch == 'ß')
                return (byte)(rune ? 93 : 29);
            else if (ch == ';')
                return 30;
            else if (ch == ':')
                return 31;
            else if (ch == ',')
                return 32;
            else if (ch == '.')
                return 33;
            else if (ch == '\'' || ch == '´' || ch == '`')
                return 34;
            else if (ch == '"')
                return 35;
            else if (ch == '!')
                return 36;
            else if (ch == '?')
                return 37;
            else if (ch == '*')
                return 38;
            else if (ch == '_')
                return 39;
            else if (ch == '(')
                return 40;
            else if (ch == ')')
                return 41;
            else if (ch == '%')
                return 42;
            else if (ch == '/')
                return 43;
            else if (ch == '#')
                return 44;
            else if (ch == '-')
                return 45;
            else if (ch == '+')
                return 46;
            else if (ch == '=')
                return 47;
            else if (ch >= '0' && ch <= '9')
                return (byte)(ch - '0' + 48);
            else if (ch == '&')
                return 58;
            else if (ch == 'á' || ch == 'à' || ch == 'â' || ch == 'Á' || ch == 'À' || ch == 'Â')
                return 59;
            else if (ch == 'é' || ch == 'è' || ch == 'ê' || ch == 'É' || ch == 'È' || ch == 'Ê')
                return 60;
            else if (ch == '¢')
                return 61;
            else if (ch == 'û' || ch == 'Û')
                return 62;
            else if (ch == 'ô' || ch == 'Ô')
                return 63;
            else if (ch == ' ')
                return (byte)SpecialGlyph.SoftSpace;
            else if (ch == '$')
                return (byte)SpecialGlyph.HardSpace;
            else if (ch == '^')
                return (byte)SpecialGlyph.NewLine;
            else
                throw new AmbermoonException(ExceptionScope.Data, $"Unsupported text character '{ch}'.");
        }

        public IText ProcessText(string text, List<string> dictionary)
        {
            List<byte> glyphIndices = new List<byte>();

            text = text.Replace("~LEAD~", LeadName);
            text = text.Replace("~SELF~", SelfName);
            text = text.Replace("~CAST~", CastName);
            text = text.Replace("~INVN~", InvnName);
            text = text.Replace("~SUBJ~", SubjName);
            text = text.Replace("~SEX1~", LeadName);
            text = text.Replace("~SEX2~", LeadName);

            bool rune = false;
            int tagStart = -1;
            int dictRefStart = -1;

            void ProcessTag(string name)
            {
                if (name == "RUN1")
                    rune = true;
                else if (name == "NORM")
                    rune = false;
                else if (name.StartsWith("INK"))
                {
                    if (!int.TryParse(name.Substring(3), out int colorIndex) || colorIndex < 0 || colorIndex > 31)
                        throw new AmbermoonException(ExceptionScope.Data, $"Invalid ink tag: ~{name}~");

                    glyphIndices.Add((byte)(SpecialGlyph.FirstColor + colorIndex));
                }
                else
                    throw new AmbermoonException(ExceptionScope.Data, $"Unknown tag: ~{name}~");
            }

            for (int i = 0; i < text.Length; ++i)
            {
                if (text[i] == '~')
                {
                    if (dictRefStart != -1)
                        throw new AmbermoonException(ExceptionScope.Data, "Tag inside a dictionary reference.");

                    if (tagStart == -1)
                        tagStart = i + 1;
                    else
                    {
                        ProcessTag(text[tagStart..i]);
                        tagStart = -1;
                    }
                }
                else if (tagStart != -1)
                {
                    continue;
                }
                else if (text[i] == '>')
                {
                    if (dictRefStart != -1)
                        throw new AmbermoonException(ExceptionScope.Data, "A second dictionary reference started before the last was closed.");

                    dictRefStart = i + 1;
                }
                else if (text[i] == '<')
                {
                    if (dictRefStart == -1)
                        throw new AmbermoonException(ExceptionScope.Data, "Closing dictionary reference without starting one.");

                    dictionary.Add(text[dictRefStart..i].Trim());
                    dictRefStart = -1;
                }
                else
                {
                    glyphIndices.Add(CharToGlyph(text[i], rune));
                }
            }

            if (tagStart != -1)
                throw new AmbermoonException(ExceptionScope.Data, $"Not closed tag at position {tagStart - 1}.");
            if (dictRefStart != -1)
                throw new AmbermoonException(ExceptionScope.Data, $"Not closed dictionary reference at position {dictRefStart - 1}.");

            return new Text(glyphIndices.ToArray());
        }
    }
}
