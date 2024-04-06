<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm2.aspx.cs" Inherits="Hsp.Net4.EasyUI.Web.Models.WebForm2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>bootstrap模态框远程加载网页的方法</title>

    <link href="../Content/bootstrap.css" rel="stylesheet" />
    <script src="../Scripts/jquery-easyui/jquery.min.js"></script>
    <script src="../Scripts/bootstrap.js"></script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h2>模态框页面调用</h2>
            <p>调用后页面加载到本页面中，由此，不能加载相同的JS包等，限制比较多</p>
            <a data-toggle="modal" href="/Models/WebForm3.aspx" data-target="#modal">Click me</a>

            <div class="modal" id="modal">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <!--这里是远程加载过过来的内容区-->
                    </div>
                </div>
            </div>
            <br /><br />
            <a href="javascript:void(0)" onclick="showDialog();">showDialog</a>
            <br /><br />
            <input type="button" value="打开窗口" onclick="openWin()" />


            <script type="text/javascript">
                //$('#dlg').dialog({ closed: true });

                $(document).ready(function () {
                    $("#modal").on("hidden.bs.modal", function () {
                        $(this).removeData("bs.modal");
                    });
                })

                function showDialog() {
                    var url = "/Models/WebForm3.aspx";
                    if (IEVersion() == -1 || IEVersion() == "edge") {
                        window.showModalDialog = window.open(url, "_blank",
                            "width=" + (screen.availWidth / 2) + ",height=" + screen.availHeight + ",scroll=0,");
                    } else {
                        window.showModalDialog(url, "_blank",
                            "width=" + (screen.availWidth/2) + ",height=" + screen.availHeight + ",scroll=0,");
                    }
                }




                function IEVersion() {
                    var userAgent = navigator.userAgent; //取得浏览器的userAgent字符串  
                    var isIE = userAgent.indexOf("compatible") > -1 && userAgent.indexOf("MSIE") > -1; //判断是否IE<11浏览器  
                    var isEdge = userAgent.indexOf("Edge") > -1 && !isIE; //判断是否IE的Edge浏览器  
                    var isIE11 = userAgent.indexOf('Trident') > -1 && userAgent.indexOf("rv:11.0") > -1;
                    if (isIE) {
                        var reIE = new RegExp("MSIE (\\d+\\.\\d+);");
                        reIE.test(userAgent);
                        var fIEVersion = parseFloat(RegExp["$1"]);
                        if (fIEVersion == 7) {
                            return 7;
                        } else if (fIEVersion == 8) {
                            return 8;
                        } else if (fIEVersion == 9) {
                            return 9;
                        } else if (fIEVersion == 10) {
                            return 10;
                        } else {
                            return 6;//IE版本<=7
                        }
                    } else if (isEdge) {
                        return 'edge';//edge
                    } else if (isIE11) {
                        return 11; //IE11  
                    } else {
                        return -1;//不是ie浏览器
                    }
                }

                function open_win() {
                    window.open("http://www.w3cschool.cn", "", "width=" + (screen.availWidth / 2) + ",height=" + screen.availHeight + ",scroll=0,");
                }

                function openWin() {
                   var myWindow = window.open('', '', 'width=200,height=100');
                    myWindow.document.write("<p>这是'我的窗口'</p>");
                    myWindow.focus();
                }

                function ShowCusmerDialog(url, name, width, height) {
                    var url = "http//:www.baidu.com/"
                    var name = "百度";
                    var iWidth = width == undefined ? 1100 : width;//弹窗宽度
                    var iHeight = height == undefined ? 700 : height; //弹窗高度
                    var iTop = (window.screen.availHeight - 30 - iHeight) / 2;
                    var iLeft = (window.screen.availWidth - 10 - iWidth) / 2;
                    window.open(url, name, 'height=' + iHeight + ',innerHeight=' + iHeight + ',width=' + iWidth + ',innerWidth=' + iWidth + ',top=' + iTop + ', left = '+ iLeft + ', toolbar = no, menubar = no, scrollbars = yes, resizeable = no, location = no, status = no');
                }




            </script>

        </div>
    </form>
</body>
</html>
