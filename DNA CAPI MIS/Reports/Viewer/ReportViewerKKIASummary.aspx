<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportViewerKKIASummary.aspx.cs" Inherits="DNA_CAPI_MIS.Reports.Viewer.ReportViewerKKIASummary" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="http://ajax.googleapis.com/ajax/libs/jquery/2.0.2/jquery.min.js"></script>
    <script>
        function WebkitReportFix() {
            // Start timer to make sure overflow is set to visible
            setInterval(function () {
                var div = $('table[id*=_fixedTable] > tbody > tr:last > td:last > div')

                div.css('overflow', 'visible');
            }, 1000);
        }

        WebkitReportFix();
	</script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager ID="CAPIScriptManager" runat="server"></asp:ScriptManager>
        <rsweb:ReportViewer ID="CAPIReportViewer" runat="server" Font-Names="Verdana" 
            Font-Size="8pt" InteractiveDeviceInfos="(Collection)" ProcessingMode="Local" 
            WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt" Height="100%" 
            Width="100%">

            <LocalReport  />
        </rsweb:ReportViewer>
    </div>
    </form>
</body>
</html>
