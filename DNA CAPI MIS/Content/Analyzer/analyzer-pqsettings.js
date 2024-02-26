// Edit Question Handler - called by jquery.nestable.js
function onQuestionEdit(questionId) {

    // Stop form from submitting normally
    event.preventDefault();

    // Get some values from elements on the page:
    var $form = $("form"),
      url = $form.attr("action");
    url = url.replace('PQSettings', 'PQSettingsEdit');

    //Show Overlay
    $(".overlay").css('display', 'block');
    $(".loading-img").css('display', 'block');

    // Send the data using post
    var posting = $.post(url, "fieldId="+questionId);

    // Put the results in a div
    posting.done(function (data) {
        var content = $(data).find("#fieldData");
        $("#fieldData").empty().append(content.html());

        //Trigger OnDocumentReady functions
        var ftype = $("#FieldType").val();
        $("#questionType").val(ftype);
        RenderFieldSampleTable();

        //Hide Overlay
        $(".overlay").css('display', 'none');
        $(".loading-img").css('display', 'none');

        //Scroll to top
        $("html, body").animate({ scrollTop: 0 }, "slow");
    });
}

$(document).ready(function () {

    // Attach a submit handler to the form
    $("#saveQuestion").on('click', function (event) {

        // Stop form from submitting normally
        event.preventDefault();

        // Get some values from elements on the page:
        var $form = $(this).closest("form"),
          url = $form.attr("action");

        //Show Overlay
        $(".overlay").css('display', 'block');
        $(".loading-img").css('display', 'block');

        // Send the data using post
        var posting = $.post(url, $form.serialize());

        // Put the results in a div
        posting.done(function (data) {
            var content = data; //$(data).find(".content");
            //$("#tabulation").empty().append(content);

            //Hide Overlay
            $(".overlay").css('display', 'none');
            $(".loading-img").css('display', 'none');
        });
    });
});


function RenderFieldSampleTable() {
    // prepare the data
    var data = PopulateData();

    var source =
    {
        localdata: data,
        datafields:
        [
            { name: 'SampleId', type: 'number' },
            { name: 'ParentSampleId', type: 'number' },
            { name: 'Title', type: 'string' },
            { name: 'VariableName', type: 'string' },
            { name: 'Code', type: 'string' },
            { name: 'DisplayOrder', type: 'number' },
        ],
        datatype: "array",
        updaterow: function (rowid, rowdata) {
            // synchronize with the server - send update command
        }
    };

    var dataAdapter = new $.jqx.dataAdapter(source);

    var toThemeProperty = function (className) {
        return className + " " + className + "-" + theme;
    }

    // initialize jqxGrid
    $("#jqxgrid").jqxGrid(
    {
        theme: "energyblue",
        width: "100%",
        height: 250,
        source: dataAdapter,
        groupable: false,
        columnsresize: false,
        columnsreorder: false,
        editable: true,
        editmode: 'click',
        altrows: true,
        columns: [
            { text: 'Title', datafield: 'Title', width: 150 },
            { text: 'Variable Name', datafield: 'VariableName', width: 100 },
            { text: 'Code', datafield: 'Code', width: 50 },
            { text: 'Order', datafield: 'DisplayOrder', width: 50 },
        ],
    });
}
