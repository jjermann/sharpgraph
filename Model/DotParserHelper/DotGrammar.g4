grammar DotGrammar;

options {
	language=CSharp3;
}

@parser::header {#pragma warning disable 3021}
@lexer::header {#pragma warning disable 3021}

graph        :  STRICT? (GRAPH | DIGRAPH) id? '{' stmt_list '}' ;
stmt_list    :  ( stmt ';'? )* ;
stmt         :  node_stmt
		     |  edge_stmt
			 |  attr_stmt
			 |  id '=' id
			 |  subgraph
			 ;
attr_stmt    :  (GRAPH | NODE | EDGE) attr_list ;
attr_list    :  ('[' a_list? ']')+ ;
a_list       :  (sattr_stmt ','?)+ ;
sattr_stmt   :  id ('=' id)? ;
edge_stmt    :  edge_obj edgeRHS attr_list? ;
edge_obj     :  node_id
		     |  subgraph
			 ;
edgeRHS      :  ( edgeop edge_obj )+ ;
edgeop       :  '->' | '--' ;
node_stmt    :  node_id attr_list? ;
node_id      :  id port? ;
port         :  ':' id (':' id)? ;
subgraph     :  (SUBGRAPH id?)? '{' stmt_list '}' ;
id           :  ID
			 |  STRING
			 |  HTML_STRING
			 |  NUMBER
			 ;

STRICT       :  [Ss][Tt][Rr][Ii][Cc][Tt] ;
GRAPH        :  [Gg][Rr][Aa][Pp][Hh] ;
DIGRAPH      :  [Dd][Ii][Gg][Rr][Aa][Pp][Hh] ;
NODE         :  [Nn][Oo][Dd][Ee] ;
EDGE         :  [Ee][Dd][Gg][Ee] ;
SUBGRAPH     :  [Ss][Uu][Bb][Gg][Rr][Aa][Pp][Hh] ;
ID           :  LETTER (LETTER|DIGIT)* ;
fragment
LETTER       :  [a-zA-Z\u0080-\u00FF_] ;
NUMBER       :  '-'? ('.' DIGIT+ | DIGIT+ ('.' DIGIT*)? ) ;
fragment
DIGIT        :  [0-9] ;
STRING       :  '"' ('\\"'|.)*? '"' ;
HTML_STRING  :  '<' (TAG|~[<>])* '>' ;
fragment
TAG          :  '<' .*? '>' ;

PREPROC      :  ('#' | '//') .*? ('\r' | '\n') -> skip ;
BLOCKCOMMENT :  '/*' .*? '*/' -> skip ;
WS           :  (' ' | '\t' | '\r' | '\n')+ -> skip ;
