$(document).ready(function () {
    // add row
    $("#btnAddSection").on('click', function (event) {
        var row = {};
        var rowscount = $("#jqxgridSection").jqxGrid('getdatainformation').rowscount;
        rowscount++;
        row["rid"] = rowscount;
        row["ID"] = 0;
        row["Name"] = '';
        row["DisplayOrder"] = rowscount;

        $("#jqxgridSection").jqxGrid('addrow', null, row);
        setRowState(rowscount - 1, 'new');
    });
    // delete row.
    $("#btnDeleteSection").on('click', function () {
        var selectedrowindex = $("#jqxgridSection").jqxGrid('getselectedrowindex');
        var rowscount = $("#jqxgridSection").jqxGrid('getdatainformation').rowscount;
        if (selectedrowindex >= 0 && selectedrowindex < rowscount) {
            var id = $("#jqxgridSection").jqxGrid('getrowid', selectedrowindex);
            $("#jqxgridSection").jqxGrid('deleterow', id);
            setRowState(id, 'delete');
        }
    });

    // Attach a submit handler to the form
    $("#saveButton").on('click', function (event) {

        // Stop form from submitting normally
        event.preventDefault();

        // Get some values from elements on the page:
        var $form = $(this).closest("form"),
            url = $form.attr("action");

        if ($("#jqxgridSection").jqxGrid('getdatainformation')) {
            var rowscount = $("#jqxgridSection").jqxGrid('getdatainformation').rowscount;
            var rows = [];
            for (var i = 0; i < rowscount; i++) {
                var dataRecord = $("#jqxgridSection").jqxGrid('getrowdata', i);
                rows[i] = dataRecord;
            }
            var rowsSection = JSON.stringify(rows);
            $("#FieldSectionData").val(rowsSection);
        }

        // Send the data using post
        var posting = $.post(url, $form.serialize());

        // Put the results in a div
        posting.done(function (data) {
            BootstrapDialog.closeAll();
        });
    });


    var dataSection = PopulateDataSection();
    var sourceSection =
    {
        localdata: dataSection,
        datafields:
        [
            { name: 'ID', type: 'number' },
            { name: 'DisplayOrder', type: 'number' },
            { name: 'Name', type: 'string' },
        ],
        datatype: "array",
        updaterow: function (rowid, rowdata) {
            setRowState(rowid, 'updated');
        },
    };

    var dataAdapterSection = new $.jqx.dataAdapter(sourceSection);

    // initialize jqxGrid
    $("#jqxgridSection").jqxGrid(
    {
        theme: "energyblue",
        width: "100%",
        height: 150,
        source: dataAdapterSection,
        groupable: false,
        columnsresize: false,
        columnsreorder: false,
        editable: true,
        editmode: 'click',
        altrows: true,
        columns: [
            { text: '', datafield: 'ID', width: 25, editable: false, hidden: true },
            { text: 'Order', datafield: 'DisplayOrder', width: 50 },
            { text: 'Name', datafield: 'Name', width: 500 },
        ],
    });

});
