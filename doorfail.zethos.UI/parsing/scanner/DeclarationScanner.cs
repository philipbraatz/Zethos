using System;
using System.Collections.Generic;
using System.Text;

namespace doorfail.zethos.UI.parsing.scanner
{
    public enum StatusDec
    {

        IDLE,
        ERROR,
        FOUND,
        FINISHED,
        DECLARE,
        NAME,
        START_PARAM,
        ADD_PARAM,
        CHECK_NEXT,
        DEFAULT_PARAM,
    }

    class DeclarationScanner : IScanner
    {
        private string key;
        private bool hasType;
        private string errorMsg;
        private SyntaxType syntax;
        private detectionString detection = new detectionString();
        private detectionString curParam = new detectionString();

        private EquationScanner defaultScanner = new EquationScanner();
        private int scannerRelativeDepth = 0;

        public StatusDec Status { get; private set; } = 0;
        int IScanner.Status { get => (int)Status; set => Status = (StatusDec)value; }
        string IScanner.errorMsg { get => errorMsg; set => errorMsg = value; }
        detectionString IScanner.detection { get => detection; set => detection = value; }
        public detectionString cur { get => curParam; set => curParam = value; }

        public bool isFinished() => Status == StatusDec.FINISHED;
        public bool isActive() => Status >= StatusDec.FOUND && !isFinished();
        public bool isFound() => Status == StatusDec.FOUND;

        public DeclarationScanner(string key, SyntaxType syntaxType, bool hasType)
        {
            this.key = key;
            this.hasType = hasType;
            this.syntax = syntaxType;
            detection.syntax = this.syntax;
        }

        public string GetError()
        {
            return errorMsg;
        }

        //call after FINISHED
        public detectionString GetDetection(int level, bool clearStatus = true)
        {
            detectionString ret = detection;
            ret.level = level;
            ret.errorMSG = errorMsg;
            if (clearStatus)
            {
                Status = StatusDec.IDLE;
                detection = new detectionString();
                curParam = new detectionString();
                errorMsg = String.Empty;

                detection.syntax = this.syntax;
            }
            return ret;
        }

        int IScanner.Next(String str)
        {
            return Next(str);
        }

        internal int Next(string str)
        {
            if (Status != StatusDec.FINISHED)
            {
                if (str == key)
                    Status = StatusDec.FOUND;
                else if (Status == StatusDec.FOUND)
                    if (str == ":")
                        Status = StatusDec.DECLARE;
                    else
                    {
                        Status = StatusDec.ERROR;
                        errorMsg = "Expected : but got " + str;
                    }
                else if (Status == StatusDec.DECLARE)
                {
                    Status = StatusDec.NAME;
                    detection.entry = str;
                }
                else if (Status == StatusDec.NAME)
                {
                    if (str == "(")
                        Status = StatusDec.START_PARAM;
                    else
                        Status = StatusDec.FINISHED;
                }
                else if (Status == StatusDec.START_PARAM || Status == StatusDec.ADD_PARAM || Status == StatusDec.CHECK_NEXT)
                {
                    curParam.raw += str + "\n";

                    if (str.Trim() == ")" && Status != StatusDec.DEFAULT_PARAM)
                        if (Status != StatusDec.ADD_PARAM)
                            Status = StatusDec.FINISHED;
                        else if (Status == StatusDec.DEFAULT_PARAM)
                        {
                            detection.varValue.Add(defaultScanner.GetDetection(1));
                        }
                        else
                        {
                            Status = StatusDec.ERROR;
                            errorMsg = "Expected another parameter but got )";
                        }
                    else if (str == ",")
                    {
                        if (Status == StatusDec.DEFAULT_PARAM)
                        {
                            detection.varValue.Add(defaultScanner.GetDetection(1));
                        }
                        else if (Status != StatusDec.START_PARAM)
                            Status = StatusDec.ADD_PARAM;
                        else
                        {
                            Status = StatusDec.ERROR;
                            errorMsg = "Expected another parameter but got ,";
                        }
                    }
                    else if (str == "=" && Status != StatusDec.FINISHED)
                    {
                        Status = StatusDec.DEFAULT_PARAM;
                        defaultScanner = new EquationScanner();

                        if (detection.varValue.Count > 0)
                        {
                            detectionString lastDS = detection.varValue[detection.varValue.Count - 1];
                            detection.varValue.Remove(lastDS);
                            defaultScanner.Next(lastDS.entry, 1);//grab var name
                        }
                        else//failed to parse, fallback to raw
                        {
                            int failIndex = detection.raw.LastIndexOf('\n');
                            string fallback = detection.raw.Substring(failIndex, detection.raw.Length - failIndex -1);
                            defaultScanner.Next(fallback, 1);
                        }
                        defaultScanner.Next(str, 1);//trigger equation
                        defaultScanner.SetInnerNest(1);//adjust level
                    }
                    else if (!(Status == StatusDec.CHECK_NEXT))
                    {
                        if (hasType && Status == StatusDec.START_PARAM)
                        {
                            try
                            {
                                String[] split = str.Split(' ', 2);
                                curParam.type = split[0];
                                curParam.entry = split[1];
                            }
                            catch (Exception)
                            {
                                throw new Exception("Parameter " + str + " is missing a type");
                            }

                            Status = StatusDec.ADD_PARAM;
                        }
                        else
                        {
                            //get param name/value
                            curParam.entry = str;
                            detection.varValue.Add(curParam);
                            curParam = new detectionString();

                            Status = StatusDec.CHECK_NEXT;
                        }
                    }
                    else
                    {
                        Status = StatusDec.ERROR;
                        errorMsg = "Expected to finish but got " + str;
                    }

                }
                else if (Status == StatusDec.DEFAULT_PARAM)
                {
                    if (str == ")")
                        scannerRelativeDepth--;
                    else if (str == "(")
                        scannerRelativeDepth++;
                    if (defaultScanner.Status != StatusEq.FINISHED)
                        if(scannerRelativeDepth > -1)
                            defaultScanner.Next(str, 1); 
                        else
                        {
                            detectionString detectionString = defaultScanner.GetDetection(1);
                            detectionString.type = curParam.type;
                            detection.varValue.Add(detectionString);
                            Status = StatusDec.FINISHED;
                        }
                    else//should never hit
                    {
                        detectionString detectionString = defaultScanner.GetDetection(1);
                        detectionString.type = curParam.type;
                        detection.varValue.Add(curParam);
                        Status = StatusDec.CHECK_NEXT;
                    }

                }
            }
            else
            {
                Status = StatusDec.ERROR;
                errorMsg = "Scanner Finished Task";
            }

            if ((int)Status > 2 && (int)Status != 3)
                detection.raw += str + "\n";

            return (int)Status;
        }
    }
}