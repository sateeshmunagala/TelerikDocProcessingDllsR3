
/* 
    WebAii's Script Logging Support.
    
    - Include this file on all pages that you would like to perform script logging from.

    All Rights Reserved. ArtOfTest, Inc. 2007    
*/

/*
 Javascript enum mapping to ArtOfTest.WebAii.Core.LogType 
*/
function LogTypeDefinition()
{
    // Simply log information to the log file.
    this.Information = 0;
    // Log information to the log file and fail the test
    this.Error = 1; 
    // Log information to the log file and to the System.Diagnostics.Trace.
    this.Trace = 2;
    // Log information to the log file and to the eventlog.
    this.EventLog = 3;
}

var LogType = new LogTypeDefinition();

// Perform a log operation.
// type: The type of logging. (Trace,Error ... etc). [Defined by LogTypeDefinition() above]
// message: The message to log.
//
// Example:
//    WebAiiLog(LogType.Information,"Inside function foo()");
//
function WebAiiLog(type,message)
{
  try
  {
        var element = document.getElementById("__webaii_logger");
        
        if (element)
        {
            // Set the type of the logging.
            element.setAttribute("logType",type);
            
            if (this.IsFireFox())
            {
                element.datasources = message;
                var ev = document.createEvent("Events"); 
                ev.initEvent("WebAiiLogEvent", true, false); 
                element.dispatchEvent(ev); 
                
            }
            else
            {
                element.innerText = message;
                element.fireEvent("oncellchange");
            }
        }
        
  }
  catch(err)
  {
	    alert(err);
  }
}


// Detect FireFox.
function IsFireFox()
{
    
    if (navigator.userAgent.indexOf("Firefox") != -1)
        return true;
    else
        return false;
}

/*
    END OF FILE
*/