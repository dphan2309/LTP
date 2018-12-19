<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="LTP.Application._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="col-md-12">
            <h2>List of Persons</h2>
            <p>
                <button type="button" id="button-create-person" class="btn btn-success" data-toggle="modal" data-target="#modal-upsert-person">Create New Person</button>
            </p>
        </div>
    </div>
    <div class="row">
        <div class="col-md-12">
            <div class="panel panel-default">
                <div class="panel-heading">Search Information</div>
                <div class="panel-body">
                    <table class="table table-bordered">
                        <tr>
                            <td>
                                <label for="txtFirstName">First Name</label>
                                <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" MaxLength="50" PlaceHolder="First Name" />
                            </td>
                            <td>
                                <label for="txtLastName">Last Name</label>
                                <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" MaxLength="50" PlaceHolder="Last Name" />
                            </td>
                            <td>
                                <label for="ddlState">State</label>
                                <asp:DropDownList ID="ddlState" runat="server" CssClass="form-control" PlaceHolder="State" DataTextField="StateCode" DataValueField="StateId" />

                            </td>
                            <td>
                                <label for="ddlGender">Gender</label>
                                <asp:DropDownList ID="ddlGender" runat="server" CssClass="form-control" PlaceHolder="Gender">
                                    <asp:ListItem Value="A" Text="Any" />
                                    <asp:ListItem Value="F" Text="F" />
                                    <asp:ListItem Value="M" Text="M" />
                                </asp:DropDownList>
                            </td>
                            <td>
                                <label for="txtDOB">Date of Birth</label>
                                <div class="input-group date" id="datepicker-search-dob">
                                    <asp:TextBox ID="txtDOB" runat="server" CssClass="form-control" PlaceHolder="MM/dd/yyyy" MaxLength="10" />
                                    <span class="input-group-addon">
                                        <span class="glyphicon glyphicon-calendar"></span>
                                    </span>
                                </div>
                            </td>
                            <td>
                                <br />
                                <asp:Button ID="btnSearchPerson" CssClass="btn btn-primary" runat="server" Text="Search" OnClick="btnSearchPerson_Click" />
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
            <div class="panel panel-default">
                <div class="panel-heading">Results</div>
                <div class="panel-body">
                    <asp:Repeater runat="server" ID="personGrid" OnItemDataBound="PersonGrid_OnItemDataBound">
                        <HeaderTemplate>
                            <table class="table table-bordered" id="table-persons">
                                <thead>
                                    <tr>
                                        <th>First Name</th>
                                        <th>Last Name</th>
                                        <th>State</th>
                                        <th>Gender</th>
                                        <th>Date of Birth</th>
                                        <th></th>
                                    </tr>
                                </thead>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr class="row-person" data-person-id="<%# DataBinder.Eval(Container.DataItem, "PersonId") %>">
                                <td>
                                    <%# DataBinder.Eval(Container.DataItem, "FirstName") %> 
                                </td>
                                <td>
                                    <%# DataBinder.Eval(Container.DataItem, "LastName") %> 
                                </td>
                                <td data-state-id="<%# DataBinder.Eval(Container.DataItem, "StateId") %>">
                                    <%# DataBinder.Eval(Container.DataItem, "StateCode") %> 
                                </td>
                                <td data-gender="<%# DataBinder.Eval(Container.DataItem, "Gender") %>">
                                    <%# DataBinder.Eval(Container.DataItem, "Gender") %> 
                                </td>
                                <td>
                                    <%# DataBinder.Eval(Container.DataItem, "DateOfBirth", "{0:MM/dd/yyyy}") %> 
                                </td>
                                <td>
                                    <a class="edit-person" data-person-id="<%# DataBinder.Eval(Container.DataItem, "PersonId") %>">Edit</a>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                    <asp:Repeater ID="rptPager" runat="server">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkPage" runat="server" Text='<%#Eval("Text") %>' CommandArgument='<%# Eval("Value") %>'
                                CssClass='<%# Convert.ToBoolean(Eval("Enabled")) ? "page_enabled" : "page_disabled" %>'
                                OnClick="Page_Changed" OnClientClick='<%# !Convert.ToBoolean(Eval("Enabled")) ? "return false;" : "" %>'></asp:LinkButton>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>

        </div>
    </div>

    <div class="modal fade" id="modal-upsert-person" tabindex="-1" role="dialog" aria-labelledby="modal-title-upsert-person" aria-hidden="true" data-backdrop="static">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close"
                        data-dismiss="modal">
                        <span aria-hidden="true">&times;</span>
                        <span class="sr-only">Close</span>
                    </button>
                    <h4 class="modal-title" id="modal-title-upsert-person">Create New Person</h4>
                </div>
                <div class="modal-body">
                    <div role="form">
                        <input type="hidden" id="input-person-id" />
                        <div class="form-group">
                            <label for="input-first-name">First Name</label>
                            <input type="text" class="form-control" id="input-first-name" placeholder="First Name" maxlength="50" />
                        </div>
                        <div class="form-group">
                            <label for="input-last-name">Last Name</label>
                            <input type="text" class="form-control" id="input-last-name" placeholder="Last Name" maxlength="50" />
                        </div>
                        <div class="form-group">
                            <label for="select-state">State</label>
                            <asp:Repeater runat="server" ID="stateDropDown">
                                <HeaderTemplate>
                                    <select class="form-control" id="select-state">
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <option value="<%# DataBinder.Eval(Container.DataItem, "StateId") %>">
                                        <%# DataBinder.Eval(Container.DataItem, "StateCode") %> 
                                    </option>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </select>
                                </FooterTemplate>
                            </asp:Repeater>
                        </div>
                        <div class="form-group">
                            <label for="select-gender">Gender</label>
                            <select class="form-control" id="select-gender">
                                <option value="A">Any</option>
                                <option value="F">F</option>
                                <option value="M">M</option>
                            </select>
                        </div>
                        <div class="form-group">
                            <label for="input-dob">Date Of Birth</label>
                            <div class="input-group date" id="datepicker-dob">
                                <input type="text" id="input-dob" class="form-control" placeholder="MM/dd/yyyy" maxlength="10" />
                                <span class="input-group-addon">
                                    <span class="glyphicon glyphicon-calendar"></span>
                                </span>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" id="button-upsert-person-cancel" class="btn btn-default" data-dismiss="modal">
                        Cancel
                    </button>
                    <button type="button" id="button-upsert-person" class="btn btn-primary">
                        Save
                    </button>
                    <p id="error-upsert-person" class="error-message text-left hidden"></p>
                </div>
            </div>
        </div>
    </div>

    <script src="Scripts/bootstrap-datepicker.min.js"></script>
    <script src="Scripts/default.aspx.js"></script>
</asp:Content>
