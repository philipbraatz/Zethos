using System;
using System.Collections.Generic;
using System.Text;

namespace doorfail.zethos.UI.Utils
{
    public enum Placement
    {
        BEFORE,
        AFTER,
        BETWEEN,
        ANY
    }
    public enum Operator
    {
        NON_OPERAND,
        VARIBLE,
        EQUAL,
        ADD,
        SUBTRACT_NEG,
        MULTIPLY,
        DIVIDE,
        MODULOSE,
        NEST_IN,
        NEST_OUT,
        OR,
        AND,
        NOT,
        XOR,
        NEW,
        POWER,
        ROOT,
        SHIFTLEFT,
        SHIFTRIGHT,
        INCREMENT,
        DECREMENT,
        CAST,
        FUNCTION,
        TERINARY, 
        ELSE
    }
    public struct Operand
    {
        public string Key;
        public Operator Op;
        public Placement Loc;
    }

    public static class OperatorConversion
    {
        public static List<Operand> OperatorValues = new List<Operand> {
            new Operand{Key =  "",Op = Operator.VARIBLE     ,Loc =Placement.ANY},
            new Operand{Key =  "==",Op = Operator.EQUAL     ,Loc =Placement.BETWEEN},
            new Operand{Key =  "+" ,Op = Operator.ADD       ,Loc =Placement.BETWEEN} ,
            new Operand{Key =  "-" ,Op = Operator.SUBTRACT_NEG  ,Loc =Placement.BEFORE},
            new Operand{Key =  "*" ,Op = Operator.MULTIPLY  ,Loc =Placement.BETWEEN},
            new Operand{Key =  "/" ,Op = Operator.DIVIDE    ,Loc =Placement.BETWEEN},
            new Operand{Key =  "%" ,Op = Operator.MODULOSE  ,Loc =Placement.BETWEEN},
            new Operand{Key =  "(" ,Op = Operator.NEST_IN   ,Loc =Placement.ANY},
            new Operand{Key =  ")" ,Op = Operator.NEST_OUT  ,Loc =Placement.ANY},
            new Operand{Key =  "||",Op = Operator.OR        ,Loc =Placement.BETWEEN},
            new Operand{Key =  "&&",Op = Operator.AND       ,Loc =Placement.BETWEEN},
            new Operand{Key =  "!!",Op = Operator.NOT       ,Loc =Placement.BETWEEN},
            new Operand{Key =  "^^",Op = Operator.AND       ,Loc =Placement.BETWEEN},
            new Operand{Key =  "new",Op =Operator.NEW       ,Loc =Placement.BEFORE},
            new Operand{Key =  "^" ,Op = Operator.POWER     ,Loc =Placement.BETWEEN},
            new Operand{Key =  "^/",Op = Operator.ROOT      ,Loc =Placement.BETWEEN},
            new Operand{Key =  "<<",Op = Operator.SHIFTLEFT ,Loc =Placement.BETWEEN},
            new Operand{Key =  ">>",Op = Operator.SHIFTRIGHT,Loc =Placement.BETWEEN},
            new Operand{Key =  "++",Op = Operator.INCREMENT ,Loc =Placement.AFTER},
            new Operand{Key =  "--",Op = Operator.DECREMENT ,Loc =Placement.AFTER},
            new Operand{Key =  ":", Op = Operator.CAST      ,Loc =Placement.BETWEEN},
            new Operand{Key =  "",  Op = Operator.FUNCTION  ,Loc =Placement.ANY},
            new Operand{Key =  "?", Op = Operator.TERINARY  ,Loc =Placement.BETWEEN},
            new Operand{Key =  "|", Op = Operator.ELSE      ,Loc =Placement.BETWEEN},
        };
    }
}
