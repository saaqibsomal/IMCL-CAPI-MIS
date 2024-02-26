<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SpeedTest.aspx.cs" Inherits="DNA_CAPI_MIS.SpeedTest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="http://ajax.googleapis.com/ajax/libs/jquery/1.9.1/jquery.js"></script>
    <script type="text/javascript">
        var timerStart = Date.now();
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <b><p>Speed Test</p></b>
        <p><div id="t1"></div></p>
        <p><div id="t2"></div></p>
        <p><div id="t3"></div></p>
    </div>
    </form>
    <script type="text/javascript">
        $(document).ready(function () {
            document.getElementById('t1').innerHTML = "Time until DOM ready: " + (Date.now() - timerStart);
        });
        $(window).load(function () {
            document.getElementById('t2').innerHTML = "Page Loading Time: " + (Date.now() - timerStart);
        });
        var loadTime = window.performance.timing.domContentLoadedEventEnd - window.performance.timing.navigationStart;
        //document.getElementById('t3').innerHTML = "Content Loading Time: " + loadTime;
    </script>
</body>
</html>
