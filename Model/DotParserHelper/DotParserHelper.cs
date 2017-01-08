using System;
using System.IO;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using DotParser;

namespace SharpGraph {
    [CLSCompliant(false)]
    public static class DotParserHelper {
        public static IParseTree GetParseTree(TextReader reader, IDotGrammarListener listener = null) {
            var input = new AntlrInputStream(reader);
            var lexer = new DotGrammarLexer(input);
            var tokens = new CommonTokenStream(lexer);
            var parser = new DotGrammarParser(tokens);
            var tree = parser.graph();
            if (listener != null) {
                tree.EnterRule(listener);
            }
            return tree;
        }
    }
}