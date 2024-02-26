var rowsCoding = [];
var previewMode = 0;

function setRowState(id, state) {
    rowsCoding[id] = state;
}
function containsRow(id) {
    return typeof rowsCoding[id] !== 'undefined';
};

function OnPreview() {
    if (previewMode == 0) {
        previewMode = 1;
        //$("#previewFrame").attr("src", previewFrameSource);
        document.getElementById('previewFrame').contentWindow.location.reload(true);
        $("#btnPreview").text("Exit Preview");
        $("#editPanel").hide();
        $("#previewPanel").show();
    } else {
        previewMode = 0;
        $("#btnPreview").text("Preview");
        $("#editPanel").show();
        $("#previewPanel").hide();
    }
};

// Edit Question Handler - called by jquery.nestable.js
function onQuestionEdit(questionId) {

    // Stop form from submitting normally
    event.preventDefault();

    // Get some values from elements on the page:
    var projectId = $("#ProjectID").val();
    var url = "/Designer/PQSettingsEdit/" + projectId;

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

// Edit Question Handler - called by jquery.nestable.js
function onQuestionRemove(questionId, onCompletion) {

    // Stop form from submitting normally
    event.preventDefault();

    var projectId = $("#ProjectID").val();
    var url = "/Designer/PQSettingsDelete/" + projectId;

    // Send the data using post
    var posting = $.post(url, "fieldId=" + questionId);

    // Put the results in a div
    posting.done(function (data) {
        var resp = JSON.parse(data);
        if (resp.responseCode != "200") {
            BootstrapDialog.show({
                type: BootstrapDialog.TYPE_WARNING,
                title: 'Unable to Delete',
                message: resp.responseMessage,
                buttons: [{
                    label: 'Close',
                    action: function (dialogRef) {
                        dialogRef.close();
                        onCompletion(false);
                    }
                }]
            });
        } else {
            onCompletion(true);
        }
    });
    posting.fail(function (data) {
        onCompletion(false);
    });
    return false;
}

$(document).ready(function () {
    
    //SetQuestionType();

    function SetQuestionType()
    {
        var fldType = document.getElementById("FieldType").value;
        var element = document.getElementById('questionType');
        element.value = fldType;
    }

    function filter() {
        var statusVals = "";
        var keyword = $('#projectFilter_keyword').val();
        //var status = $("input[name='optionsRadios']:checked").val();
        $("input[name='optionsRadios']:checked").each(function () {
            statusVals += "'"+$(this).val()+"',";
        });
        var url = viewURL + keyword;
        
        //Show Overlay
        $(".overlay").css('display', 'block');
        $(".loading-img").css('display', 'block');

        // Send the data using post
        $.ajax({
            url: url,
            data: { "name": keyword, "status": statusVals.slice(0,-1) },
            type: "GET",
        })
            .always(function (data) {
                var DOM = $('<div>' + data + '</div>');
                var newData = DOM.find('.table');
                $('.table').html(newData);
                //Hide Overlay
                $(".overlay").css('display', 'none');
                $(".loading-img").css('display', 'none');
            });
    }

    $("#projectFilter_name").on('click', function (event) {
        
        event.preventDefault();
        filter();
    });

    $(".optionsRadios").on('click', function (event) {
        event.preventDefault();
        filter();
    });

    

    /*$("input[name='optionsRadios']:radio").change(function () {
        event.preventDefault();
        filter();
    });*/

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

        //Get Coding Data from jqxGrid
        if ($("#jqxgrid").jqxGrid('getdatainformation')) {
            var rowscount = $("#jqxgrid").jqxGrid('getdatainformation').rowscount;
            var rows = [];
            for (var i = 0; i < rowscount; i++) {
                var dataRecord = $("#jqxgrid").jqxGrid('getrowdata', i);
                rows[i] = dataRecord;
            }
            var rowsCoding = JSON.stringify(rows);
            $("#FieldCodingData").val(rowsCoding);
        }

        // Send the data using post
        var posting = $.post(url, $form.serialize());

        // Put the results in a div
        posting.done(function (data) {
            var content = $(data).find("#pfAll");
            $("#pfAll").empty().append(content.html());

            //Hide Overlay
            $(".overlay").css('display', 'none');
            $(".loading-img").css('display', 'none');
        });
    });

    //On Change event to manage ordering
    var updateOrder = function (e) {
        var list = e.length ? e : $(e.target);
        var fldOrder = new Array();
        var index = 1;

        if (list != null) {
            list.find(".dd-item").each(function () {
                var fld = new Object();
                fld.id = $(this).data("id");
                fld.order = index++;
                fldOrder.push(fld);
            });

            if (fldOrder.length > 0) {
                var projectId = $("#ProjectID").val();
                var url = "/Designer/PQSettingsSetOrder/" + projectId;

                //Show Overlay
                $(".overlay").css('display', 'block');
                $(".loading-img").css('display', 'block');

                // Send the data using post
                $.ajax({
                    url: url,
                    contentType: "application/json",
                    data: JSON.stringify(fldOrder),
                    type: "POST",
                })
                    .always(function (data) {
                        //Hide Overlay
                        $(".overlay").css('display', 'none');
                        $(".loading-img").css('display', 'none');
                    });
            }
        }
    };

    $('#pfAll').nestable({
        group: 1
    })
    .on('change', updateOrder);

});


function RenderFieldSampleTable() {
    // prepare the data
    var data = PopulateData();

    var source =
    {
        localdata: data,
        datafields:
        [
            { name: 'ID', type: 'number' },
            { name: 'ParentSampleId', type: 'number' },
            { name: 'VariableName', type: 'string' },
            { name: 'Code', type: 'string' },
            { name: 'DisplayOrder', type: 'number' },
            { name: 'Title', type: 'string' },
            { name: 'Title_ar-SA', type: 'string' },
        ],
        datatype: "array",
        updaterow: function (rowid, rowdata) {
            setRowState(rowid, 'updated');
            // synchronize with the server - send update command
            //commit(true);
        },
        //addrow: function (rowid, rowdata) {
        //    setRowState(rowid, 'new');
        //    // synchronize with the server - send insert command
        //    //commit(true);
        //}
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
            { text: 'Order', datafield: 'DisplayOrder', width: 50 },
            { text: 'Code', datafield: 'Code', width: 50 },
            { text: 'Variable Name', datafield: 'VariableName', width: 100 },
            { text: 'Title [en]', datafield: 'Title', width: 150 },
            { text: 'عنوان [ar]', datafield: 'Title_ar-SA', width: 150 },
        ],
    });

    $("#btnAddCoding").on('click', function (event) {
        var row = {};
        var rowscount = $("#jqxgrid").jqxGrid('getdatainformation').rowscount;
        rowscount++;
        row["rid"] = rowscount;
        row["ID"] = 0;
        row["ParentSampleId"] = 0;
        row["Title"] = '';
        row["VariableName"] = '';
        row["Code"] = rowscount;
        row["DisplayOrder"] = rowscount;

        $("#jqxgrid").jqxGrid('addrow', null, row);
        //var selectedrowindex = $("#jqxgrid").jqxGrid('getselectedrowindex');
        //var id = $("#jqxgrid").jqxGrid('getrowid', rowscount);
        setRowState(rowscount-1, 'new');

        //$("#jqxgrid").jqxGrid('ensurerowvisible', selectedrowindex);
    });
    // delete row.
    $("#btnDeleteCoding").on('click', function () {
        var selectedrowindex = $("#jqxgrid").jqxGrid('getselectedrowindex');
        var rowscount = $("#jqxgrid").jqxGrid('getdatainformation').rowscount;
        if (selectedrowindex >= 0 && selectedrowindex < rowscount) {
            var id = $("#jqxgrid").jqxGrid('getrowid', selectedrowindex);
            $("#jqxgrid").jqxGrid('deleterow', id);
            setRowState(id, 'delete');
        }
    });

}

