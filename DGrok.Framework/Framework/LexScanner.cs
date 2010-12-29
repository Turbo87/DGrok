// Copyright 2007, 2008 Joe White
//
// This file is part of DGrok <http://www.excastle.com/dgrok/>.
//
// DGrok is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// DGrok is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with DGrok.  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DGrok.Framework
{
    internal class LexScanner
    {
        private class Match
        {
            private int _length;
            private string _parsedText;
            private TokenType _tokenType;

            public Match(TokenType tokenType, int length)
                : this(tokenType, length, "") { }
            public Match(TokenType tokenType, int length, string parsedText)
            {
                _tokenType = tokenType;
                _length = length;
                _parsedText = parsedText;
            }

            public int Length
            {
                get { return _length; }
            }
            public string ParsedText
            {
                get { return _parsedText; }
            }
            public TokenType TokenType
            {
                get { return _tokenType; }
            }
        }

        private string _fileName;
        private int _index;
        private string _source;
        private Dictionary<string, TokenType> _wordTypes;

        public LexScanner(string source, string fileName)
        {
            _source = source;
            _fileName = fileName;
            _wordTypes = new Dictionary<string, TokenType>(StringComparer.InvariantCultureIgnoreCase);
            AddWordTypes(TokenSets.Semikeyword, "Semikeyword".Length);
            AddWordTypes(TokenSets.Keyword, "Keyword".Length);
        }

        private Location Location
        {
            get { return new Location(_fileName, _source, _index); }
        }

        /// <summary>
        /// Add contents of the given TokenSet to the _wordTypes dictionary
        /// </summary>
        /// <param name="tokenTypes">The TokenSet that should be added</param>
        /// <param name="suffixLength">If the token is "If" and the enum name is "IfKeyword", 
        /// then suffixLength should be the length of the string "Keyword"</param>
        private void AddWordTypes(IEnumerable<TokenType> tokenTypes, int suffixLength)
        {
            // Iterate through all tokens of the TokenSet
            foreach (TokenType tokenType in tokenTypes)
            {
                // Example: IfKeyword
                string tokenString = tokenType.ToString();
                // Strips the suffix. Example: IfKeyword -> If
                string baseWord = tokenString.Substring(0, tokenString.Length - suffixLength);
                // Add (key)word to dictionary
                _wordTypes[baseWord.ToLowerInvariant()] = tokenType;
            }
        }
        /// <summary>
        /// Checks whether there is an AmpersandIdentifier at the current reading position
        /// </summary>
        /// <returns>null or a Match instance describing the length</returns>
        private Match AmpersandIdentifier()
        {
            // First character is not an Ampersand (&)
            // or the second character is not a valid identifier beginning (A-Z or _)
            if (Peek(0) != '&' || !IsWordLeadChar(Peek(1)))
                // Current reading position is not an AmpersandIdentifier
                return null;

            // We already know that the first two characters are valid, so let's skip these
            int length = 2;
            // While the next characters are valid characters for an identifier, go on counting
            while (IsWordContinuationChar(Peek(length)))
                ++length;

            // We've found an AmpersandIdentifier at the current reading position with the measured length
            return new Match(TokenType.Identifier, length);
        }
        private Match BareWord()
        {
            if (!IsWordLeadChar(Peek(0)))
                return null;
            int offset = 1;
            while (IsWordContinuationChar(Peek(offset)))
                ++offset;
            string word = _source.Substring(_index, offset);
            TokenType tokenType;
            if (_wordTypes.ContainsKey(word))
                tokenType = _wordTypes[word];
            else
                tokenType = TokenType.Identifier;
            return new Match(tokenType, offset);
        }
        /// <summary>
        /// Makes sure the requested position is still inside the string
        /// </summary>
        /// <param name="offset">Offset from the current reading position</param>
        /// <returns>True if valid position</returns>
        private bool CanRead(int offset)
        {
            return _index + offset < _source.Length;
        }
        /// <summary>
        /// Checks whether there is a CurlyBraceComment (or CompilerDirective) at the current reading position
        /// </summary>
        /// <returns>null or a Match instance describing the length</returns>
        private Match CurlyBraceComment()
        {
            // If the first character is not an opening curly brace this is not a match
            if (Peek(0) != '{')
                return null;

            // Skip first curly brace that we already checked
            int offset = 1;

            // Read and count until the next closing curly brace or the end of the string
            while (CanRead(offset) && Peek(offset) != '}')
                ++offset;

            // Add the closing curly brace to the parsed string
            ++offset;

            // {$...} is a compiler directive not a comment
            if (Peek(1) == '$')
            {
                // Save the text inside of the directive without leading {$ and trailing whitespace and }
                string parsedText = _source.Substring(_index + 2, offset - 3).TrimEnd();
                // Return CompilerDirective match
                return new Match(TokenType.CompilerDirective, offset, parsedText);
            }
            else
                // Return CurlyBraceComment match
                return new Match(TokenType.CurlyBraceComment, offset);
        }
        /// <summary>
        /// Checks whether there are two dots at the current reading position
        /// </summary>
        /// <returns>null or a Match instance</returns>
        private Match DotDot()
        {
            // Make sure there are two dots
            if (Peek(0) == '.' && Peek(1) == '.')
                // and return Match instance
                return new Match(TokenType.DotDot, 2);

            return null;
        }
        private Match DoubleQuotedApostrophe()
        {
            if (Peek(0) == '"' && Peek(1) == '\'' && Peek(2) == '"')
                return new Match(TokenType.StringLiteral, 3);
            return null;
        }
        /// <summary>
        /// Checks whether there is a EqualityOrAssignmentOperator at the current reading position
        /// 
        /// Test for each of the following operators := < <= <> = > >=
        /// </summary>
        /// <returns>null or a corresponding Match instance</returns>
        private Match EqualityOrAssignmentOperator()
        {
            switch (Peek(0))
            {
                case ':':
                    if (Peek(1) == '=')
                        return new Match(TokenType.ColonEquals, 2);
                    break;
                case '<':
                    switch (Peek(1))
                    {
                        case '=': return new Match(TokenType.LessOrEqual, 2);
                        case '>': return new Match(TokenType.NotEqual, 2);
                    }
                    return new Match(TokenType.LessThan, 1);
                case '=':
                    return new Match(TokenType.EqualSign, 1);
                case '>':
                    if (Peek(1) == '=')
                        return new Match(TokenType.GreaterOrEqual, 2);
                    return new Match(TokenType.GreaterThan, 1);
            }
            return null;
        }
        /// <summary>
        /// Checks whether there is a HexNumber at the current reading position
        /// </summary>
        /// <returns>null or a corresponding Match instance</returns>
        private Match HexNumber()
        {
            // Hex numbers begin with a $ character
            if (Peek(0) != '$')
                return null;

            // Skip the $ character
            int offset = 1;
            // Read and count up to the last valid HexDigit
            while (IsHexDigit(Peek(offset)))
                ++offset;

            // Return Match instance
            return new Match(TokenType.Number, offset);
        }
        /// <summary>
        /// Checks whether the given character is a valid hexadecimal character
        /// </summary>
        /// <returns>True if the given character is a valid hexadecimal character</returns>
        private bool IsHexDigit(char ch)
        {
            return
                (ch >= '0' && ch <= '9') ||
                (ch >= 'A' && ch <= 'F') ||
                (ch >= 'a' && ch <= 'f');
        }
        /// <summary>
        /// Checks whether the given character is a valid character inside of an identifier
        /// </summary>
        /// <returns>True if the given character is a valid character inside of an identifier</returns>
        private bool IsWordContinuationChar(char ch)
        {
            return (Char.IsLetterOrDigit(ch) || ch == '_');
        }
        /// <summary>
        /// Checks whether the given character is a valid character to start an identifier
        /// </summary>
        /// <returns>True if the given character is a valid character to start an identifier</returns>
        private bool IsWordLeadChar(char ch)
        {
            return (Char.IsLetter(ch) || ch == '_');
        }
        private Match NextMatch()
        {
            return
                BareWord() ??
                EqualityOrAssignmentOperator() ??
                Number() ??
                StringLiteral() ??
                SingleLineComment() ??
                CurlyBraceComment() ??
                ParenStarComment() ??
                DotDot() ??
                SingleCharacter() ??
                HexNumber() ??
                AmpersandIdentifier() ??
                DoubleQuotedApostrophe();
        }
        public Token NextToken()
        {
            while (_index < _source.Length && Char.IsWhiteSpace(_source[_index]))
                ++_index;
            if (_index >= _source.Length)
                return null;

            Match match = NextMatch();
            if (match == null)
                throw new LexException("Unrecognized character '" + _source[_index] + "'", Location);
            string text = _source.Substring(_index, match.Length);
            Token result = new Token(match.TokenType, Location, text, match.ParsedText);
            _index += match.Length;
            return result;
        }
        private Match Number()
        {
            if (!Char.IsNumber(Peek(0)))
                return null;
            int offset = 1;
            while (Char.IsNumber(Peek(offset)))
                ++offset;
            if (Peek(offset) == '.' && Peek(offset + 1) != '.')
            {
                ++offset;
                while (Char.IsNumber(Peek(offset)))
                    ++offset;
            }
            if (Peek(offset) == 'e' || Peek(offset) == 'E')
            {
                ++offset;
                if (Peek(offset) == '+' || Peek(offset) == '-')
                    ++offset;
                while (Char.IsNumber(Peek(offset)))
                    ++offset;
            }
            return new Match(TokenType.Number, offset);
        }
        private Match ParenStarComment()
        {
            if (Peek(0) != '(' || Peek(1) != '*')
                return null;
            int offset = 2;
            while (CanRead(offset) && !(Read(offset) == '*' && Peek(offset + 1) == ')'))
                ++offset;
            offset += 2;
            if (Peek(2) == '$')
            {
                string parsedText = _source.Substring(_index + 3, offset - 5).TrimEnd();
                return new Match(TokenType.CompilerDirective, offset, parsedText);
            }
            else
                return new Match(TokenType.ParenStarComment, offset);
        }
        /// <summary>
        /// Returns the character at the given offset from the current reading position
        /// </summary>
        /// <param name="offset">Offset from the current reading position</param>
        /// <returns>Character at the given offset from the current reading position or 
        /// Char.MaxValue if the requested position is outside of the bounds</returns>
        private char Peek(int offset)
        {
            if (CanRead(offset))
                return Read(offset);
            return Char.MaxValue;
        }
        /// <summary>
        /// Returns the character at the given offset from the current reading position
        /// 
        /// This function reads without checking if the reading position is inside the bounds. Use Peek() instead!
        /// </summary>
        /// <param name="offset">Offset from the current reading position</param>
        /// <returns>Character at the given offset from the current reading position</returns>
        private char Read(int offset)
        {
            return _source[_index + offset];
        }
        /// <summary>
        /// Checks whether there is a valid single character token at the current reading position
        /// </summary>
        /// <returns>null or a corresponding Match instance</returns>
        private Match SingleCharacter()
        {
            char ch = _source[_index];
            switch (ch)
            {
                case '(': return new Match(TokenType.OpenParenthesis, 1);
                case ')': return new Match(TokenType.CloseParenthesis, 1);
                case '*': return new Match(TokenType.TimesSign, 1);
                case '+': return new Match(TokenType.PlusSign, 1);
                case ',': return new Match(TokenType.Comma, 1);
                case '-': return new Match(TokenType.MinusSign, 1);
                case '.': return new Match(TokenType.Dot, 1);
                case '/': return new Match(TokenType.DivideBySign, 1);
                case ':': return new Match(TokenType.Colon, 1);
                case ';': return new Match(TokenType.Semicolon, 1);
                case '@': return new Match(TokenType.AtSign, 1);
                case '[': return new Match(TokenType.OpenBracket, 1);
                case ']': return new Match(TokenType.CloseBracket, 1);
                case '^': return new Match(TokenType.Caret, 1);
            }
            return null;
        }
        /// <summary>
        /// Checks whether there is a single line comment starting at the current reading position
        /// </summary>
        /// <returns>null or a corresponding Match instance</returns>
        private Match SingleLineComment()
        {
            // Check for // at the beginning of the string
            if (Peek(0) != '/' || Peek(1) != '/')
                return null;
            
            // Skip already checked //
            int offset = 2;
            // Read and count until line break
            /// TODO include trailing \n after \r ?!
            while (CanRead(offset) && Read(offset) != '\r' && Read(offset) != '\n')
                ++offset;

            // Return Match instance
            return new Match(TokenType.SingleLineComment, offset);
        }
        private Match StringLiteral()
        {
            char firstChar = Read(0);
            if (firstChar != '\'' && firstChar != '#')
                return null;
            int offset = 0;
            while (Peek(offset) == '\'' || Peek(offset) == '#')
            {
                if (Peek(offset) == '\'')
                {
                    ++offset;
                    while (Read(offset) != '\'')
                        ++offset;
                    ++offset;
                }
                else
                {
                    ++offset;
                    if (Read(offset) == '$')
                        ++offset;
                    while (Char.IsLetterOrDigit(Peek(offset)))
                        ++offset;
                }
            }
            return new Match(TokenType.StringLiteral, offset);
        }
    }
}
