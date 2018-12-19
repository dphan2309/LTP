using System;
using System.Collections.Generic;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using LTP.BusinessLogic;
using LTP.DataModels;

namespace LTP.Application
{
    public partial class _Default : Page
    {
        readonly StateBusiness _stateBusiness = new StateBusiness();
        private int PageSize = 10;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }

            var getStateResult = _stateBusiness.GetAll();
            if (getStateResult.Succeeded)
            {
                var list = getStateResult.Data;
                stateDropDown.DataSource = list;
                stateDropDown.DataBind();

                ddlState.DataSource = list;
                ddlState.DataBind();

                ddlState.Items.Insert(0, new ListItem("Any"));
            }
            LoadDefault(1);

        }
        protected void PersonGrid_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var childRepeater = (Repeater)e.Item.FindControl("stateSearchDropDown");
            if (childRepeater != null)
            {
                var getStateResult = _stateBusiness.GetAll();
                if (getStateResult.Succeeded)
                {
                    childRepeater.DataSource = getStateResult.Data;
                    childRepeater.DataBind();
                }
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static ExecuteResult<List<State>> GetStates()
        {
            var stateBusiness = new StateBusiness();
            var getResult = stateBusiness.GetAll();
            return getResult;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static ExecuteResult<Person> UpsertPerson(int personId, string firstName, string lastName, int stateId, char gender, DateTime dateOfBirth)
        {
            var person = new Person
            {
                PersonId = personId,
                FirstName = firstName,
                LastName = lastName,
                StateId = stateId,
                Gender = gender,
                DateOfBirth = dateOfBirth
            };
            var personBusiness = new PersonBusiness();
            var saveResult = personBusiness.Save(person);
            return saveResult;
        }

        private void LoadDefault(int pageIndex)
        {
            var personBusiness = new PersonBusiness();
            var searchCriteria = new PersonSearchCriteria();
            var recordCount = 0;
            searchCriteria.pageIndex = pageIndex;
            searchCriteria.pageSize = PageSize;

            if (!String.IsNullOrEmpty(txtFirstName.Text)) { searchCriteria.FirstName = txtFirstName.Text; }
            if (!String.IsNullOrEmpty(txtLastName.Text)) { searchCriteria.LastName = txtLastName.Text; }
            if (!String.IsNullOrEmpty(ddlGender.Text)) { searchCriteria.Gender = Convert.ToChar(ddlGender.SelectedValue); }
            if (!String.IsNullOrEmpty(txtDOB.Text)) { searchCriteria.DateOfBirth = Convert.ToDateTime(txtDOB.Text); }
            if(!String.IsNullOrEmpty(ddlState.Text)) {
                if(ddlState.SelectedValue != "Any")
                {
                    searchCriteria.StateId = Convert.ToInt16(ddlState.SelectedValue);
                }
            }

            var searchResult = personBusiness.Search(searchCriteria, ref recordCount);
            if (searchResult.Succeeded)
            {
                personGrid.DataSource = searchResult.Data;
                personGrid.DataBind();
                PopulatePager(recordCount, searchCriteria.pageIndex);
            }
        }
        private void PopulatePager(int recordCount, int currentPage)
        {
            double dblPageCount = (double)((decimal)recordCount / Convert.ToDecimal(PageSize));
            int pageCount = (int)Math.Ceiling(dblPageCount);
            List<ListItem> pages = new List<ListItem>();
            if (pageCount > 0)
            {
                for (int i = 1; i <= pageCount; i++)
                {
                    pages.Add(new ListItem(i.ToString(), i.ToString(), i != currentPage));
                }
            }
            rptPager.DataSource = pages;
            rptPager.DataBind();
        }
        protected void Page_Changed(object sender, EventArgs e)
        {
            int pageIndex = int.Parse((sender as LinkButton).CommandArgument);
            LoadDefault(pageIndex);
        }
        protected void btnSearchPerson_Click(object sender, EventArgs e)
        {
            LoadDefault(1);
        }
    }
}