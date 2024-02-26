var dataGridAns;
var dataGridTopics;

var rowsCoding = [];
var rowsCodingQ = [];
var previewMode = 0;
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

$(document).ready(function () {

    SetQuestionType();
    SetDefaultSectionName();
    SetProjectFieldOptions();
    InitMedia();
    InitEditors();

    // Attach a submit handler to the form
    $("#saveQuestion").on('click', function (event) {

        // Stop form from submitting normally
        event.preventDefault();

        // Get some values from elements on the page:
        var $form = $(this).closest("form"),
          url = $form.attr("action");

        //Encode certain fields
        $("textarea[name=questionTitle-en]").val(encodeURI($("textarea[name=questionTitle-en]").val()))
        $("textarea[name=questionTitle-ar]").val(encodeURI($("textarea[name=questionTitle-ar]").val()))

        //Set Editor contents
        $("#scriptOnEntry").val(OnEntryEditor.getValue());
        $("#scriptOnValidate").val(OnValidateEditor.getValue());
        $("#scriptOnExit").val(OnExitEditor.getValue());

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
        //Get Coding breakup Data from jqxGridQ
        if ($("#jqxgridQ").jqxGrid('getdatainformation')) {
            var rowscount = $("#jqxgridQ").jqxGrid('getdatainformation').rowscount;
            var rows = [];
            for (var i = 0; i < rowscount; i++) {
                var dataRecord = $("#jqxgridQ").jqxGrid('getrowdata', i);
                rows[i] = dataRecord;
            }
            var rowsCoding = JSON.stringify(rows);
            $("#FieldCodingDataQ").val(rowsCoding);
        }

        // Send the data using post
        var posting = $.post(url, $form.serialize());

        // Put the results in a div
        posting.done(function (data) {
            var content = $(data).find("#pfAllSections");
            $("#pfAllSections").empty().append(content.html());

            content = $(data).find("#fieldData");
            $("#fieldData").empty().append(content.html());

            SetDefaultSectionName();
            InitQuestionSelectors();

            $('#_tabScripts').hide();
            $('#_tabVariables').hide();
            $('#_tabOptions').hide();

            //Hide Overlay
            $(".overlay").css('display', 'none');
            $(".loading-img").css('display', 'none');
        });
    });

    InitQuestionSelectors();

});

function InitTabs() {

    
}
function InitQuestionSelectors() {
    $('.pfAll').nestable({
        group: 1
    })
    .on('change', updateOrder);
}

function urldecode(str) {
    str = str.replace(/[%!'()*]/g, function (c) {
        return '%' + c.charCodeAt(0).toString(16);
    });
    str = decodeURIComponent(str.replace(/\+/g, '%20'));
    try {
        str = decodeURIComponent(str);
    }
    catch (e) {
    }
    return str;
}

function setRowState(id, state) {
    rowsCoding[id] = state;
}
function containsRow(id) {
    return typeof rowsCoding[id] !== 'undefined';
};
function setRowStateQ(id, state) {
    rowsCodingQ[id] = state;
}
function containsRowQ(id) {
    return typeof rowsCodingQ[id] !== 'undefined';
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

        //Decode certain fields
        $("textarea[name=questionTitle-en]").val(urldecode($("textarea[name=questionTitle-en]").val()))
        $("textarea[name=questionTitle-ar]").val(urldecode($("textarea[name=questionTitle-ar]").val()))

        //Trigger OnDocumentReady functions
        SetQuestionType();
        SetDefaultSectionName();
        SetProjectFieldOptions();
        RenderFieldSampleTable();
        InitMedia();
        InitEditors();

        $('#_tabScripts').show();
        $('#_tabVariables').show();
        $('#_tabOptions').show();

        //Hide Overlay
        $(".overlay").css('display', 'none');
        $(".loading-img").css('display', 'none');

        InitCheckboxControl();

        //Scroll to top
        $("#_tabQuestion").click();
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


var btnAnswerAdvSettingsClick = function (grid) {
    var gridName = "";
    var gridData;
    if (grid == 1) {
        gridName = "#jqxgrid";
        gridData = dataGridAns;
    } else if (grid == 2) {
        gridName = "#jqxgridQ";
        gridData = dataGridTopics;
    }
    //Advanced - add/edit - Opens dialog to enter more information
    var selectedrowindex = $(gridName).jqxGrid('getselectedrowindex');
    var rowscount = $(gridName).jqxGrid('getdatainformation').rowscount;
    var vname = $("input[name=variableName]").val();
    var id = 0;
    if (selectedrowindex >= 0 && selectedrowindex < rowscount) {
        id = gridData[selectedrowindex]['ID'];
    }
    BootstrapDialog.show({
        title: (grid == 1 ? 'Advanced Answer Settings' : 'Advanced Topics Settings'),
        message: $('<div></div>').load('/Designer/PQAnswer/' + id + '?count=' + rowscount + '&varName=' + vname + '&grid=' + grid),
        onhide: function (dialogRef) {
            var fieldId = dialogRef.getModalBody().find('#ProjectFieldSampleID').val();
            var sampleTitle = dialogRef.getModalBody().find('#PFSTitle').val();
            var sampleTitle_ar = dialogRef.getModalBody().find('#PFSTitle_ar').val();
            var sampleVName = dialogRef.getModalBody().find('#PFSVariableName').val();
            var sampleCode = dialogRef.getModalBody().find('#PFSCode').val();
            var sampleOrder = dialogRef.getModalBody().find('#PFSDisplayOrder').val();

            if (fieldId > 0) {
                if (selectedrowindex < 0 || id == 0) {
                    var row = {};
                    var rowscount = $(gridName).jqxGrid('getdatainformation').rowscount;
                    rowscount++;
                    row["rid"] = rowscount;
                    row["ID"] = fieldId;
                    row["ParentSampleId"] = 0;
                    row["Title"] = sampleTitle;
                    row["VariableName"] = sampleVName;
                    row["Code"] = sampleCode;
                    row["DisplayOrder"] = sampleOrder;
                    row['Title_ar-SA'] = sampleTitle_ar;
                    $(gridName).jqxGrid('addrow', null, row);
                    setRowState(rowscount - 1, 'new');
                }
                else {
                    var row = gridData[selectedrowindex];
                    row["Title"] = sampleTitle;
                    row["VariableName"] = sampleVName;
                    row["Code"] = sampleCode;
                    row["DisplayOrder"] = sampleOrder;
                    $(gridName).jqxGrid('updatebounddata');
                }
            }
        },
    });
}

function RenderFieldSampleTable() {
    // prepare the data
    dataGridAns = PopulateData();
    dataGridTopics = PopulateDataQ();

    var source =
    {
        localdata: dataGridAns,
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
    var sourceQ =
    {
        localdata: dataGridTopics,
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
            setRowStateQ(rowid, 'updated');
            // synchronize with the server - send update command
            //commit(true);
        },
        //addrow: function (rowid, rowdata) {
        //    setRowState(rowid, 'new');
        //    // synchronize with the server - send insert command
        //    //commit(true);
        //}
    };
    var rendererAnswerSettingsButton = function (id) {
        return '<a href="#" class="btnAdvCoding" onClick="btnAnswerAdvSettingsClick(1)" style="padding:4px;"><img style="margin-top:3px" src="/Content/jqwidgets/images/settings.png" /></a>'
    }
    var rendererTopicSettingsButton = function (id) {
        return '<a href="#" class="btnAdvCoding" onClick="btnAnswerAdvSettingsClick(2)" style="padding:4px;"><img style="margin-top:3px" src="/Content/jqwidgets/images/settings.png" /></a>'
    }

    var dataAdapter = new $.jqx.dataAdapter(source);
    var dataAdapterQ = new $.jqx.dataAdapter(sourceQ);

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
            { text: '', datafield: 'ID', width: 25, cellsrenderer: rendererAnswerSettingsButton, editable: false },
            { text: 'Order', datafield: 'DisplayOrder', width: 50 },
            { text: 'Code', datafield: 'Code', width: 50 },
            { text: 'Variable Name', datafield: 'VariableName', width: 100 },
            { text: 'Title [en]', datafield: 'Title', width: 150 },
            { text: 'عنوان [ar]', datafield: 'Title_ar-SA', width: 150 },
        ],
    });
    $("#jqxgridQ").jqxGrid(
    {
        theme: "energyblue",
        width: "100%",
        height: 250,
        source: dataAdapterQ,
        groupable: false,
        columnsresize: false,
        columnsreorder: false,
        editable: true,
        editmode: 'click',
        altrows: true,
        columns: [
            { text: '', datafield: 'ID', width: 25, cellsrenderer: rendererTopicSettingsButton, editable: false },
            { text: 'Order', datafield: 'DisplayOrder', width: 50 },
            { text: 'Code', datafield: 'Code', width: 50 },
            { text: 'Variable Name', datafield: 'VariableName', width: 100 },
            { text: 'Title [en]', datafield: 'Title', width: 150 },
            { text: 'عنوان [ar]', datafield: 'Title_ar-SA', width: 150 },
        ],
    });

    $("a[name=btnCopyCoding]").on('click', function (event) {
        var fieldId = $(event.target).attr('data-id');
        var vname = $("input[name=variableName]").val();
        var rowscount = $("#jqxgrid").jqxGrid('getdatainformation').rowscount;

        var surveyform = $.get("/Designer/GetProjectFieldSamplesJSON/" + fieldId)
        .done(function (data) {
            var pfs = JSON.parse(data)
            $.each(pfs, function (key, value) {
                var row = {};
                rowscount++;
                row["rid"] = rowscount;
                row["ID"] = 0;
                row["ParentSampleId"] = 0;
                row["Title"] = value['Title'];
                row["Title_ar-SA"] = value['Title_ar'];
                row["VariableName"] = vname + '_' + rowscount;
                row["Code"] = rowscount;
                row["DisplayOrder"] = rowscount;

                $("#jqxgrid").jqxGrid('addrow', null, row);
                //var selectedrowindex = $("#jqxgrid").jqxGrid('getselectedrowindex');
                //var id = $("#jqxgrid").jqxGrid('getrowid', rowscount);
                setRowState(rowscount - 1, 'new');
            });
        });
    });

    function BulkImportOptions(gridId, data) {
        //Parse text and import options
        var lines = data.split('\n');
        for (var l = 0; l < lines.length; l++) {
            var values = lines[l].split(',');
            if (values.length >= 2) {
                var code = values[0];
                var title_en = values[1];
                var title_ar = "";
                if (values.length > 2) {
                    title_ar = values[2];
                }
                AddOptionRow(gridId, code, title_en, title_ar);
            }
        }
    }

    function AddOptionRow(gridId, code, title_en, title_ar) {
        var vname = $("input[name=variableName]").val();

        var row = {};
        var rowscount = $(gridId).jqxGrid('getdatainformation').rowscount;
        rowscount++;
        row["rid"] = rowscount;
        row["ID"] = 0;
        row["ParentSampleId"] = 0;
        row["Title"] = title_en;
        row["Title_ar-SA"] = title_ar;
        row["VariableName"] = vname + '_' + rowscount;
        row["Code"] = (code == 0 ? rowscount : code);
        row["DisplayOrder"] = rowscount;

        $(gridId).jqxGrid('addrow', null, row);
        //var selectedrowindex = $("#jqxgrid").jqxGrid('getselectedrowindex');
        //var id = $("#jqxgrid").jqxGrid('getrowid', rowscount);
        //$("#jqxgrid").jqxGrid('ensurerowvisible', selectedrowindex);
    }

    $("#btnAddCoding").on('click', function (event) {
        AddOptionRow('#jqxgrid', 0, '', '')
        setRowState(rowscount - 1, 'new');
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

    $("#btnImportCoding").on('click', function () {
        BootstrapDialog.show({
            title: 'Import Answers',
            message: $('<div></div>').load('/Designer/ImportOptions/'),
            onhide: function (dialogRef) {
                                       
            },
            buttons: [{
                label: 'Import',
                action: function (dialogRef) {
                    dialogRef.close();
                    var newOptions = dialogRef.getModalBody().find('#bulkOptions').val();
                    BulkImportOptions("#jqxgrid", newOptions);
                }
            }]

        });
    });

    $("#btnQImportCoding").on('click', function () {
        BootstrapDialog.show({
            title: 'Import Topics',
            message: $('<div></div>').load('/Designer/ImportOptions/'),
            onhide: function (dialogRef) {

            },
            buttons: [{
                label: 'Import',
                action: function (dialogRef) {
                    dialogRef.close();
                    var newOptions = dialogRef.getModalBody().find('#bulkOptions').val();
                    BulkImportOptions("#jqxgridQ", newOptions);
                }
            }]

        });
    });

    $("a[name=btnQCopyCoding]").on('click', function (event) {
        var fieldId = $(event.target).attr('data-id');
        var vname = $("input[name=variableName]").val();
        var rowscount = $("#jqxgridQ").jqxGrid('getdatainformation').rowscount;

        var surveyform = $.get("/Designer/GetProjectFieldSamplesQJSON/" + fieldId)
        .done(function (data) {
            var pfs = JSON.parse(data)
            $.each(pfs, function (key, value) {
                var row = {};
                rowscount++;
                row["rid"] = rowscount;
                row["ID"] = 0;
                row["ParentSampleId"] = 0;
                row["Title"] = value['Title'];
                row["Title_ar-SA"] = value['Title_ar'];
                row["VariableName"] = vname + '_' + rowscount;
                row["Code"] = rowscount;
                row["DisplayOrder"] = rowscount;

                $("#jqxgridQ").jqxGrid('addrow', null, row);
                //var selectedrowindex = $("#jqxgrid").jqxGrid('getselectedrowindex');
                //var id = $("#jqxgrid").jqxGrid('getrowid', rowscount);
                setRowStateQ(rowscount - 1, 'new');
            });
        });
    });
    $("#btnQAddCoding").on('click', function (event) {
        AddOptionRow('#jqxgridQ', 0, '', '')
        setRowStateQ(rowscount - 1, 'new');
    });

    // delete row.
    $("#btnQDeleteCoding").on('click', function () {
        var selectedrowindex = $("#jqxgridQ").jqxGrid('getselectedrowindex');
        var rowscount = $("#jqxgridQ").jqxGrid('getdatainformation').rowscount;
        if (selectedrowindex >= 0 && selectedrowindex < rowscount) {
            var id = $("#jqxgridQ").jqxGrid('getrowid', selectedrowindex);
            $("#jqxgridQ").jqxGrid('deleterow', id);
            setRowStateQ(id, 'delete');
        }
    });

}

function SetCurrentSection(sectionName) {
    if (sectionName.length > 0) {
        $("#CurrentSectionName").html(sectionName)
    } else {
        $("#CurrentSectionName").html("&nbsp;");
    }
    $('#fieldSection').val(sectionName);
}

var newProjectFieldSectionName = "";

function AddNewSection(projectId) {
    BootstrapDialog.show({
        title: 'Create new section',
        message: $('<div></div>').load('/ProjectFieldSections/Create/' + projectId),
        onhide: function (dialogRef) {
            var newName = dialogRef.getModalBody().find('.col-md-9 input').val();
            var pfs = $("#ProjectFieldSection").prepend('<li><a tabindex="-1" href="#" onclick="SetCurrentSection(\'' + newName + '\')">' + newName + '</a></li>');
            SetCurrentSection(newName);
        },
    });
}

function OpenSectionManager() {
    var projectId = $("#ProjectID").val();

    BootstrapDialog.show({
        title: 'Manage Sections',
        message: $('<div></div>').load('/ProjectFieldSections/Index/' + projectId),
        onhide: function (dialogRef) {
            location.reload();
        },
    });

}

function SetDefaultSectionName() {
    var fldId = document.getElementById("FieldID").value;
    if (fldId.length > 0) {
        var fldSection = document.getElementById("fieldSection").value;
        SetCurrentSection(fldSection);
    } else {
        SetCurrentSection("");
    }
}

function SetProjectFieldOptions() {
    var oid = document.getElementById("Orientation")
    try {
        if (oid.value == "V") {
            document.getElementById("rdoOrientationV").checked = true;
        } else if (oid.value == "H") {
            document.getElementById("rdoOrientationH").checked = true;
        }
    }
    catch (e) { }
}

function SetQuestionType() {
    var fldType = document.getElementById("FieldType").value;
    var element = document.getElementById('questionType');
    element.value = fldType;
}

function SetupPFMediaFileUploader(value, filePath) {
    var muUrl = pqSettingsUploadURL + value;

    //alert(value + "," + filePath);
    var mfOption;
    if (value == 'Image') {
        mfOption = {
            previewFileType: "image",
            browseClass: "btn btn-success",
            browseLabel: "Pick Image",
            browseIcon: '<i class="glyphicon glyphicon-picture"></i>',
            removeClass: "btn btn-danger",
            removeLabel: "Delete",
            removeIcon: '<i class="glyphicon glyphicon-trash"></i>',
            uploadClass: "btn btn-info",
            uploadLabel: "Upload",
            uploadIcon: '<i class="glyphicon glyphicon-upload"></i>',
            uploadUrl: muUrl, // server upload action
            uploadAsync: false,
            maxFileCount: 1,
            maxFileSize: 1000
        };
        if (filePath != null) {
            pqSettingsInitialPreview = [
                "<img src='" + filePath + "' class='file-preview-image' />"
            ];
            mfOption.initialPreview = pqSettingsInitialPreview;
            mfOption.overwriteInitial = true;
        }
        $("#PFMediaFile").fileinput(mfOption);

        $("#PFMediaFileControl").show();
    } else if (value == 'Audio') {
        mfOption = {
            previewFileType: "audio",
            browseLabel: "Pick Audio",
            allowedFileTypes: ["audio"],
            uploadUrl: muUrl, // server upload action
            uploadAsync: false,
            maxFileCount: 1,
            maxFileSize: 5000
        };
        if (filePath != null) {
            pqSettingsInitialPreview = [
                "<audio controls=''><source src='" + filePath + "' type='audio/mp3'></audio>"
            ];
            mfOption.initialPreview = pqSettingsInitialPreview;
            mfOption.overwriteInitial = true;
        }

        $("#PFMediaFile").fileinput(mfOption);
        $("#PFMediaFileControl").show();
    } else if (value == 'Video') {
        mfOption = {
            previewFileType: "video",
            browseLabel: "Pick Video",
            allowedFileTypes: ["video", "*.MOV"],
            uploadUrl: muUrl, // server upload action
            uploadAsync: false,
            maxFileCount: 1,
            maxFileSize: 10000
        };
        if (filePath != null) {
            pqSettingsInitialPreview = [
                "<video width='213px' height='160px' controls=''><source src='" + filePath + "'  type='video/mp4'></video>"
            ];
            mfOption.initialPreview = pqSettingsInitialPreview;
            mfOption.overwriteInitial = true;
        }
        $("#PFMediaFile").fileinput(mfOption);
        $("#PFMediaFileControl").show();
    } else {
        $("#PFMediaFileControl").hide();
    }
}

function fixedEncodeURIComponent(str) {
    return encodeURIComponent(str).replace(/[!'()*]/g, function (c) {
        return '%' + c.charCodeAt(0).toString(16);
    });
}
