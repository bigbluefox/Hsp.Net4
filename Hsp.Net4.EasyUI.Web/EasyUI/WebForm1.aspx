<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="Hsp.Net4.EasyUI.Web.EasyUI.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>jQuery EasyUI Demo</title>
    <link href="../Content/bootstrap.css" rel="stylesheet" />
    <link href="../Scripts/jquery-easyui/themes/bootstrap/easyui.css" rel="stylesheet" />
    <link href="../Scripts/jquery-easyui/themes/icon.css" rel="stylesheet" />
    <script src="../Scripts/jquery-easyui/jquery.min.js"></script>
    <script src="../Scripts/bootstrap.js"></script>
    <script src="../Scripts/jquery-easyui/jquery.easyui.min.js"></script>
    <script src="../Scripts/jquery-easyui/locale/easyui-lang-zh_CN.js"></script>

    <style type="text/css">
        .easyui-dialog {
            display: none;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h2>Basic Dialog</h2>
            <p>Click below button to open or close dialog. 可加载内容页面</p>
            <div style="margin: 20px 0;">
                <a href="javascript:void(0)" class="easyui-linkbutton" onclick="$('#dlg').dialog('open')">Open</a>
                <a href="javascript:void(0)" class="easyui-linkbutton" onclick="$('#dlg').dialog('close')">Close</a>
            </div>
            <div id="dlg" class="easyui-dialog" title="Basic Dialog" data-options="iconCls:'icon-save',closed: true" style="width: 600px; height: 300px; overflow: hidden; padding: 0px;">
                <iframe name="MyFrame" frameborder="no" border="0" marginwidth="0" marginheight="0" id="prodcutDetailSrc" scrolling="yes" width="100%" height="100%" src="/Contact"></iframe>
            </div>


            <script type="text/javascript">
                //$('#dlg').dialog({ closed: true });

            </script>

        </div>
    </form>
</body>
</html>
