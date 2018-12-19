$(function () {
    // State dictionary to help rendering for AJAX requests.
    var states = {};
    $.ajax({
        url: "/Default.aspx/GetStates",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            var result = response.d;
            if (result.Succeeded) {
                for (var i = 0; i < result.Data.length; i++) {
                    var state = result.Data[i];
                    states[state.StateId.toString()] = state.StateCode;
                }
            }
        }
    });

    function formatDate(date) {
        var month = (date.getMonth() + 1).toString();
        var day = date.getDate().toString();
        var year = date.getFullYear().toString();

        if (month.length < 2) {
            month = '0' + month;
        }
        if (day.length < 2) {
            day = '0' + day;
        }

        return [month, day, year].join('/');
    }

    // Validate date of MM/dd/yyyy format.
    function validateDate(dateString) {
        var regExp = /^(\d{2,2})(\/)(\d{2,2})\2(\d{4}|\d{4})$/;

        var matchResult = dateString.match(regExp);
        if (matchResult === null) {
            return null;
        }

        var month = matchResult[1]; // parse date into variables
        var day = matchResult[3];
        var year = matchResult[4];
        if (month < 1 || month > 12) { // check month range
            return null;
        }
        if (day < 1 || day > 31) {
            return null;
        }
        if ((month === 4 || month === 6 || month === 9 || month === 11) && day === 31) {
            return null;
        }
        if (month === 2) {
            var isLeap = year % 4 === 0 && (year % 100 !== 0 || year % 400 === 0);
            if (day > 29 || (day === 29 && !isLeap)) {
                return null;
            }
        }
        // JavaScript month is 0-based.
        return new Date(year, month - 1, day, 0, 0, 0, 0);
    }

    function validate() {
        var valid = true;
        $("#input-first-name, #input-last-name, #select-state, #select-gender, #input-dob").each(function () {
            var $this = $(this);
            var $control = $this.parent();
            if ($.trim($this.val()) === "") {
                valid = false;
                $control.addClass("has-error");
            } else {
                $control.removeClass("has-error");
            }
        });
        var $inputDob = $("#input-dob");
        var $inputGen = $("#select-gender");
        if (validateDate($inputDob.val()) === null) {
            valid = false;
            $inputDob.parent().addClass("has-error");
        }

        if ($inputGen.val() === "A")
        {
            valid = false;
            $inputGen.parent().addClass("has-error");
        }

        return valid;
    }

    function renderPerson(person) {
        var $row = $("<tr>", {
            "class": "row-person",
            "data-person-id": person.PersonId
        });
        
        // First Name
        $row.append($("<td>", {
            "text": person.FirstName
        }));
        // Last Name
        $row.append($("<td>", {
            "text": person.LastName
        }));
        // State
        $row.append($("<td>", {
            "data-state-id": person.StateId,
            "text": states[person.StateId]
        }));
        // Gender
        $row.append($("<td>", {
            "data-gender": person.Gender,
            "text": person.Gender
        }));
        // Date Of Birth
        var dateOfBirth = new Date(parseInt(person.DateOfBirth.replace(/[^0-9]+/g, "")));
        $row.append($("<td>", {
            "text": formatDate(dateOfBirth)
        }));

        var $command = $("<td>");
        $command.append($("<a>", {
            "class": "edit-person",
            "data-person-id": person.PersonId,
            "text": "Edit"
        }));
        
        $row.append($command);

        $row.appendTo($("#table-persons tbody"));
    }

    $("#datepicker-dob").datepicker({ format: "mm/dd/yyyy" }).datepicker("setDate", "0");
    $("#datepicker-search-dob").datepicker({ format: "mm/dd/yyyy" });

    $("#button-create-person").click(function () {
        // Reset all form fields.
        $("#input-person-id").val("0");
        $("#input-first-name, #input-last-name, #input-dob").each(function () {
            var $this = $(this);
            $this.val("");
            $this.parent().removeClass("has-error");
        });
        $("#select-state option:first-child, #select-gender option:first-child").attr("selected", "selected");
        $("#error-upsert-person").addClass("hidden").text("");
    });

    $("#button-upsert-person").click(function () {
        if (!validate())
            return;
        var data = {
            personId: $("#input-person-id").val(),
            firstName: $("#input-first-name").val(),
            lastName: $("#input-last-name").val(),
            stateId: $("#select-state").val(),
            gender: $("#select-gender").val(),
            dateOfBirth: validateDate($("#input-dob").val())
        };
        $.ajax({
            url: "/Default.aspx/UpsertPerson",
            type: "POST",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(data),
            success: function (response) {
                var result = response.d;
                if (result.Succeeded) {
                    $("#modal-upsert-person").modal("hide");
                    var person = result.Data;
                    var $tds = $("tr[data-person-id='" + person.PersonId + "'] td");
                    if ($tds.length === 0) {
                        renderPerson(person);
                    } else {
                        $tds[1].innerText = person.FirstName;
                        $tds[2].innerText = person.LastName;
                        $tds[3].innerText = states[person.StateId];
                        $($tds[3]).attr("data-state-id", person.StateId);
                        $tds[4].innerText = person.Gender;
                        $($tds[4]).attr("data-gender", person.Gender);
                        var dateOfBirth = new Date(parseInt(person.DateOfBirth.replace(/[^0-9]+/g, "")));
                        $tds[5].innerText = formatDate(dateOfBirth);
                    }
                } else {
                    $("#error-upsert-person").removeClass("hidden").text(result.Message);
                }
            }
        });
    });

    $("#table-persons").on("click", "a.edit-person", function () {
        $("#input-first-name, #input-last-name, #input-dob").each(function () {
            $(this).parent().removeClass("has-error");
        });

        $("#input-person-id").val(this.dataset.personId);
        var $tds = $($(this).parent().parent()).children();
        $("#input-first-name").val($.trim($tds[1].innerText));
        $("#input-last-name").val($.trim($tds[2].innerText));
        $("#select-state").val($.trim($tds[3].dataset.stateId));
        $("#select-gender").val($.trim($tds[4].dataset.gender));
        $("#input-dob").val($.trim($tds[5].innerText));
        $("#error-upsert-person").addClass("hidden").text("");
        $("#modal-upsert-person").modal({
            backdrop: 'static',
            keyboard: false
        });
    });
});