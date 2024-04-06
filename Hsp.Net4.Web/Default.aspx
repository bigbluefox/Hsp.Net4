<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Hsp.Net4.Web._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <h1>ASP.NET</h1>
        <p class="lead">ASP.NET is a free web framework for building great Web sites and Web applications using HTML, CSS, and JavaScript.</p>
        <p><a href="http://www.asp.net" class="btn btn-primary btn-lg">Learn more &raquo;</a></p>
    </div>

    <div class="row">
        <div class="col-md-4">
            <h2>Getting started</h2>
            <p>
                ASP.NET Web Forms lets you build dynamic websites using a familiar drag-and-drop, event-driven model.
            A design surface and hundreds of controls and components let you rapidly build sophisticated, powerful UI-driven sites with data access.
            </p>
            <p>
                <a class="btn btn-default" href="https://go.microsoft.com/fwlink/?LinkId=301948">Learn more &raquo;</a>
            </p>
        </div>
        <div class="col-md-4">
            <h2>Get more libraries</h2>
            <p>
                NuGet is a free Visual Studio extension that makes it easy to add, remove, and update libraries and tools in Visual Studio projects.
            </p>
            <p>
                <a class="btn btn-default" href="https://go.microsoft.com/fwlink/?LinkId=301949">Learn more &raquo;</a>
            </p>
        </div>
        <div class="col-md-4">
            <h2>Web Hosting</h2>
            <p>
                You can easily find a web hosting company that offers the right mix of features and price for your applications.
            </p>
            <p>
                <a class="btn btn-default" href="https://go.microsoft.com/fwlink/?LinkId=301950">Learn more &raquo;</a>
            </p>
        </div>
    </div>

    <div class="row-fluid" style="padding: 15px 0;">
        <p>Session值为：<% = SessionValue %></p>
    </div>

    <div class="row-fluid" style="padding: 15px 0;">
        <button type="button" class="btn btn-default" data-toggle="tooltip" data-placement="left" title="Tooltip on left">Tooltip on left</button>

        <button type="button" class="btn btn-default" data-toggle="tooltip" data-placement="top" title="Tooltip on top">Tooltip on top</button>

        <button type="button" class="btn btn-default" data-toggle="tooltip" data-placement="bottom" title="Tooltip on bottom">Tooltip on bottom</button>

        <button type="button" class="btn btn-default" data-toggle="tooltip" data-placement="right" title="Tooltip on right">Tooltip on right</button>
    </div>

    <div class="row-fluid">
        <table id="table" class="table table-striped table-sm table-hover">
            <thead class="thead-dark">
                <tr>
                    <th data-field="id">ID</th>
                    <th data-field="name">Item Name</th>
                    <th data-field="price">Item Price</th>
                </tr>
            </thead>
        </table>
    </div>

    <script type="text/javascript">
        var $table = $('#table')

        $(function () {
            $('[data-toggle="popover"]').popover();
            $('[data-toggle="tooltip"]').tooltip();
        });

        $(function () {
            var data = [
                {
                    'id': 0,
                    'name': 'Item 0',
                    'price': '$0'
                },
                {
                    'id': 1,
                    'name': 'Item 1',
                    'price': '$1'
                },
                {
                    'id': 2,
                    'name': 'Item 2',
                    'price': '$2'
                },
                {
                    'id': 3,
                    'name': 'Item 3',
                    'price': '$3'
                },
                {
                    'id': 4,
                    'name': 'Item 4',
                    'price': '$4'
                },
                {
                    'id': 5,
                    'name': 'Item 5',
                    'price': '$5'
                }
            ]
            $table.bootstrapTable({ data: data })

            //$table.bootstrapTable('refreshOptions', {
            //    theadClasses: "thead-dark"
            //});
        })


    </script>




</asp:Content>
