<%@ Page Title="HNet Images - Display" Language="C#" MasterPageFile="~/MasterPages/FrontEnd.Master" AutoEventWireup="true" CodeBehind="ImageDirDisplay.aspx.cs" Inherits="HNetPortal.ImageDirDisplay" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
   

    <style>
        #carousel-custom {
            margin: 20px auto;
            width: 80%;
        }

            #carousel-custom .carousel-indicators {
                margin: 10px 0 0;
                overflow: auto;
                position: static;
                text-align: left;
                white-space: nowrap;
                width: 100%;
            }

                #carousel-custom .carousel-indicators li {
                    background-color: transparent;
                    -webkit-border-radius: 0;
                    border-radius: 0;
                    display: inline-block;
                    height: auto;
                    margin: 0 !important;
                    width: auto;
                }

                    #carousel-custom .carousel-indicators li img {
                        display: block;
                        opacity: 0.5;
                    }

                    #carousel-custom .carousel-indicators li.active img {
                        opacity: 1;
                    }

                    #carousel-custom .carousel-indicators li:hover img {
                        opacity: 0.75;
                    }

            #carousel-custom .carousel-outer {
                position: relative;
            }

        .carousel-inner {
            width: 100%;        
        }

    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
    <h1><span class="glyphicon glyphicon-picture"></span>&nbsp;HNet Images - Display</h1>
    <b>
        <label id="dirLabel" runat="server"></label>
    </b>

    <div id="carousel-custom" class="carousel slide lazy-load" data-ride="carousel" data-interval="false">

       <ol class='carousel-indicators'>
            <asp:repeater id="Repeater1" runat="server" onitemdatabound="Repeater1_ItemDataBound">
                <ItemTemplate>
                    <li data-target='#carousel-custom' data-slide-to='<%# Eval("idx") %>' class='<%# Eval("CaroClass") %>'>
                        <asp:Image ID="imgThumb" Style="width: 50px; height: 50px" runat="server" />
                    </li>
                </ItemTemplate>
            </asp:repeater>
        </ol>

        <div class='carousel-outer'>
            <!-- Wrapper for slides -->
            <div class='carousel-inner'>
                <asp:repeater id="Repeater2" runat="server" onitemdatabound="Repeater2_ItemDataBound">
                    <ItemTemplate>
                     <div id="imgDiv" runat="server">
                        <asp:Image ID="imgFull" runat="server" Onclick="showImg(this);" />
                    </div>
                    </ItemTemplate>
                </asp:repeater>
            </div>

            <!-- Controls -->
            <a class='left carousel-control' href='#carousel-custom' data-slide='prev'>
                <span class='glyphicon glyphicon-chevron-left'></span>
            </a>
            <a class='right carousel-control' href='#carousel-custom' data-slide='next'>
                <span class='glyphicon glyphicon-chevron-right'></span>
            </a>
        </div>

        <!-- Indicators -->
    </div>


</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="belowFormPlaceHolder" runat="server">
    <script>
      
        $(function () {
            $('.carousel.lazy-load').bind('slide.bs.carousel', function (e) {
                var image = $(e.relatedTarget).find('img[data-src]');
                image.attr('src', image.data('src'));
                image.removeAttr('data-src');
            });
        });


        function showImg(which) {
            $("#dlgImg").attr("src", "/images/ajax-loader3.gif");
            var s = which.getAttribute('src');
            $("#dlgImg").attr("src", s);
            $("#myLargeModalLabel-1").html(s);
            $("#imgDlg").modal('show');
            return false;
        };

    </script>


    
    <div id="imgDlg" class="modal" tabindex="-1" role="dialog" aria-labelledby="myLargeModalLabel-1" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>
                    <h4 class="modal-title" id="myLargeModalLabel-1"></h4>
                </div>
                <div class="modal-body text-center">
                    <img id="dlgImg" src="/images/ajax-loader3.gif" class="img-responsive img-rounded center-block" alt="" /><br />
                </div>

            </div>
            <!-- /.modal-content -->
        </div>
        <!-- /.modal-dialog -->
    </div>
    <!-- /.modal mixer image -->

</asp:Content>
