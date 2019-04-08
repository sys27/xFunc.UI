// Copyright 2012-2019 Dmitry Kischenko
//
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, 
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either 
// express or implied. 
// See the License for the specific language governing permissions and 
// limitations under the License.
using System;
using System.Collections.Generic;
using xFunc.Maths.Tokenization.Tokens;

namespace xFunc.Maths.Tokenization.PostProcessing
{

    /// <summary>
    /// The base class for lexer post processor.
    /// </summary>
    public abstract class LexerPostProcessorBase : ILexerPostProcessor
    {

        /// <summary>
        /// The method for post processing of tokens.
        /// </summary>
        /// <param name="tokens">The list of tokens.</param>
        public abstract void Process(IList<IToken> tokens);

        /// <summary>
        /// Gets previous token based on specified index.
        /// </summary>
        /// <param name="tokens">The list of tokens.</param>
        /// <param name="index">The index of currenct token.</param>
        /// <returns>The previous token.</returns>
        protected IToken GetPreviousToken(IList<IToken> tokens, int index)
        {
            var previousIndex = --index;
            if (previousIndex < 0)
                return null;

            return tokens[previousIndex];
        }

        /// <summary>
        /// Gets next token based on specified index.
        /// </summary>
        /// <param name="tokens">The list of tokens.</param>
        /// <param name="index">The index of currenct token.</param>
        /// <returns>The next token.</returns>
        protected IToken GetNextToken(IList<IToken> tokens, int index)
        {
            var previousIndex = ++index;
            if (previousIndex >= tokens.Count)
                return null;

            return tokens[previousIndex];
        }

    }

}