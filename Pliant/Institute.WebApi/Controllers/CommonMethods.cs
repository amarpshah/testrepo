using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Institute.WebApi.Controllers
{
    public class CommonMethods
    {
        public static string getStatus(int status)
        {
            switch (status)
            {
                case 1: return "Draft"; break;
                case 2: return "Ready"; break;
                case 3: return "Locked"; break;
                default: return "Draft"; break;
            }
        }
        public static string getTypeShort(int type)
        {
            switch (type)
            {
                case 0: return "NA"; break; // type: "" },
                case 1: return "DES"; break; //, type: "DES" },
                case 2: return "TOF"; break; //, type: "TOF" },
                case 3: return "MTF"; break; //, type: "MTF" },
                case 4: return "SCQ"; break; //, type: "SCQ" },
                case 5: return "MCQ"; break; //, type: "MCQ" },
                case 6: return "FTB"; break; //, type: "FTB" }
                default: return ""; break; // type: "" },
            }
        }
        
        public static string getType(int type)
        {
            switch (type)
            {
                case 0: return "Please select a question type"; break; // type: "" },
                case 1: return "Descriptive"; break; //, type: "DES" },
                case 2: return "True or False"; break; //, type: "TOF" },
                case 3: return "Match the following"; break; //, type: "MTF" },
                case 4: return "Single choice"; break; //, type: "SCQ" },
                case 5: return "Multiple choice"; break; //, type: "MCQ" },
                case 6: return "Fill in the blanks"; break; //, type: "FTB" }
                default: return "Please select a question type"; break; // type: "" },
            }
        }

        public static string getDifficultyLevel(int level)
        {
            switch (level)
            {
                case 0: return "Select difficulty"; break;
                case 1: return "Easy"; break;
                case 2: return "Medium"; break;
                case 3: return "Difficult"; break;
                default: return "Easy"; break;
            }
        }
      
    }
}