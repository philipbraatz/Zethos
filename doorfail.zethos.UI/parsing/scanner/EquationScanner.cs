using doorfail.zethos.UI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace doorfail.zethos.UI.parsing.scanner
{
    public enum StatusEq
    {
        IDLE,
        ERROR,
        FOUND,
        FINISHED,
        START_VALUE,
        CHECK_OPERATOR,
        NEXT_VALUE,
        CHECK_GETTER,
        APPEND_TILL_END,
    }

    class EquationScanner : IScanner
    {
        private string varName;
        private string varNamePrev;
        private string errorMsg;
        private detectionString detection = new detectionString();
        private detectionString curParam = new detectionString();

        bool Declaration = false;
        bool waitForEndline = false;
        int startLevel = 0;
        int innerLevel = 0;
        int curLevel = 0;

        public StatusEq Status { get; private set; } = 0;
        public detectionString cur { get => detection; set => detection = value; }
        int IScanner.Status { get => (int)Status; set => Status =(StatusEq)value; }
        string IScanner.errorMsg { get => errorMsg; set => errorMsg = value; }
        detectionString IScanner.detection { get => detection; set => detection = value; }

        public bool isFinished() => Status == StatusEq.FINISHED;
        public bool isActive() => Status >= StatusEq.FOUND && !isFinished();
        public bool isFound() => Status == StatusEq.FOUND;
        public EquationScanner()
        {
        }

        public string GetError()
        {
            return errorMsg;
        }

        public void SetInnerNest(int level)
        {
            innerLevel = level;
        }

        //call after FINISHED
        public detectionString GetDetection(int level,bool clearStatus = true)
        {
            detectionString ret = detection;
            ret.level = level;
            ret.errorMSG = errorMsg;
            if (clearStatus)
            {
                Status = StatusEq.IDLE;
                detection = new detectionString();
                curParam = new detectionString();
                errorMsg = String.Empty;

                detection.syntax = SyntaxType.EQUATION;
            }
            return ret;
        }

        //send a \n to declare the end of the operator
        //each piece of the equation is in its own detectionString
        public StatusEq Next(String str,int curLevel)
        {
            this.curLevel = curLevel;

            if(waitForEndline)
            {
                if (str.EndsWith("\n"))
                    waitForEndline = false;
                Status = StatusEq.IDLE;
                return Status;
            }

            if (!this.isActive() &&
                (str == "=" ||
                (str == ":")
            ))
            {
                if (IdlePass(str))
                    return Status;
            }
            else if (Status == StatusEq.FOUND || Status == StatusEq.NEXT_VALUE)
            {
                if (curLevel < startLevel)
                {
                    Status = StatusEq.FINISHED;
                    return Status;
                }

                FoundPass();

                if (str != "\n")
                    NextValuePass(str);
                else if (detection.syntax == SyntaxType.DECLARATION)//getter setter
                    Status = StatusEq.CHECK_GETTER;
                else if (detection.syntax == SyntaxType.EQUATION)
                {
                    Status = StatusEq.FINISHED;
                    return Status;
                }
                else//invalid syntax type
                {
                    Status = StatusEq.ERROR;
                    errorMsg = "Syntax "+ detection.syntax.ToString()+ " is invalid, cannot Finish detection";
                    return Status;
                }

                if (detection.varValue.Count > 0 && detection.varValue.Last().operand == Operator.NEST_IN)
                    innerLevel++;
                else if (detection.varValue.Count > 0 && detection.varValue.Last().operand == Operator.NEST_OUT)
                {
                    innerLevel--;
                    if (innerLevel == -1)
                    {
                        Status = StatusEq.ERROR;
                        errorMsg = "Too many closing paratheses";
                    }
                }
            }
            else if (Status == StatusEq.CHECK_OPERATOR)
            {
                if (curLevel < startLevel || str == "\n")
                {
                    Status = StatusEq.FINISHED;
                    return Status;
                }
                CheckOpPass(str);
            }
            else if (Status == StatusEq.CHECK_GETTER)
            {
                if (CheckGetterPass(str))
                    return Status;
            }
            else if (Status == StatusEq.APPEND_TILL_END)
                AppendPass();

            if (Status == StatusEq.IDLE)
            {
                varNamePrev = varName;
                varName = str;
            }
            else
                detection.raw += str;
            return Status;
        }

        int IScanner.Next(string str)
        {
            throw new NotImplementedException();
        }

        private bool IdlePass(string str)
        {
            if (str == ":")
                if (varName != "space" &&
                    varName != "template" &&
                    varName != "class" &&
                    varName != "func")//refactor to check all structure keywords
                    detection.syntax = SyntaxType.DECLARATION;
                else
                {
                    waitForEndline = true;
                    Status = StatusEq.IDLE;
                    return true;
                }
            Status = StatusEq.FOUND;
            return false;
        }

        private void FoundPass()
        {
            if (Status == StatusEq.FOUND)
            {
                Operand op = OperatorConversion.OperatorValues.Where(op => op.Key == varName).FirstOrDefault();
                if (op.Op == Operator.NON_OPERAND)
                    detection.entry = varName;
                else
                {
                    detection.entry = varNamePrev;
                    detection.operand = op.Op;
                }
                startLevel = curLevel;
                detection.operand = Operator.VARIBLE;
            }
        }

        private void CheckOpPass(string str)
        {
            Operand op = OperatorConversion.OperatorValues.Where(op => op.Key == str).FirstOrDefault();
            if (op.Op != Operator.NON_OPERAND)
            {
                detection.varValue.Add(new detectionString { operand = op.Op, entry = op.Key, raw = str + "\n" });//operand only
                if (op.Loc == Placement.BEFORE || op.Loc == Placement.BETWEEN || op.Loc == Placement.ANY)
                    Status = StatusEq.NEXT_VALUE;
                else if (str.EndsWith("\n") || startLevel != curLevel)
                    Status = StatusEq.FINISHED;
                else
                    Status = StatusEq.CHECK_OPERATOR;
            }
            else
            {
                Status = StatusEq.ERROR;
                errorMsg = str + " is not a operand";
            }
        }
        private void NextValuePass(string str)
        {
            if (str != "(" && str != "-")
                detection.varValue.Add(new detectionString { entry = str, raw = str + "\n" });//value only
            else
                CheckOpPass(str);

            if (!str.EndsWith("\n"))
            {
                if (str != "(" && str != "-")
                    Status = StatusEq.CHECK_OPERATOR;
                else
                    Status = StatusEq.NEXT_VALUE;
            }
            else
                Status = StatusEq.FINISHED;
        }
        private bool CheckGetterPass(string str)
        {
            if (str == "get")
                detection.syntax = SyntaxType.GETTER;
            else if (str == "set")
                detection.syntax = SyntaxType.SETTER;
            else
            {
                Status = StatusEq.FINISHED;
                return true;
            }
            startLevel = curLevel;
            Status = StatusEq.APPEND_TILL_END;
            return false;
        }
        private void AppendPass()
        {
            if (curLevel == startLevel)
                Status = StatusEq.CHECK_GETTER;
        }
    }
}
