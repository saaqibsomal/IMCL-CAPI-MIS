$(document).ready(function () {
    $('#recCopy').on('click', function (e) {
        SelectAnotherProject(projectId);
    });

    $('#recApprove').on('click', function (e) {
        UpdateStatus(1);
    });

    $('#recReject').on('click', function (e) {
        UpdateStatus(0);
    });

    $('#recTest').on('click', function (e) {
        UpdateStatus(2);
    });

    //var district = getParameterByName("district");
    //if (district.length > 0) {
    //    var fg = initialStringGridFilter();
    //    $("#jqxgrid").jqxGrid('addfilter', 'District', fg);
    //    $("#jqxgrid").jqxGrid('applyfilters');
    //}
});

function SelectAnotherProject(projectId) {
    var rowindexes = $('#jqxgrid').jqxGrid('getselectedrowindexes');
    if (rowindexes.length == 0) {
        alert('Please select records first by using the checkbox in the first column.');
    } else {
        var targetProject = 0;

        //Select Target Project
        BootstrapDialog.show({
            title: 'Select Project',
            message: $('<div></div>').load('/Designer/SelectProject'),
            onhide: function (dialogRef) {
                targetProject = dialogRef.getModalBody().find('#SelectedProject').val();

                CopyToAnotherProject(projectId, targetProject);
                //alert("Target Project: " + targetProject + ", Selected Rows: " + rowindexes.length);
            },
        });
    }
}

function CopyToAnotherProject(projectId, targetProject) {
    var rowindexes = $('#jqxgrid').jqxGrid('getselectedrowindexes');
    if (rowindexes.length == 0) {
        alert('Please select records first by using the checkbox in the first column.');
    } else {
        var boundrows = $('#jqxgrid').jqxGrid('getboundrows');
        var selectedIDs = new Array();
        for (var i = 0; i < rowindexes.length; i++) {
            var row = boundrows[rowindexes[i]];
            selectedIDs.push(row['sbjnum']);
            console.log(row['sbjnum']);
        }
        var request = $.ajax({
            url: "/Survey/CopySurveys/" + projectId,
            type: "POST",
            data: { "TargetProject": targetProject, "SurveyIDs": selectedIDs.toString() },
            dataType: "text",
            timeout: 60 * 15 * 1000
        });
        request.done(function (msg) {
            BootstrapDialog.alert('Surveys successfully copied');
        });
        request.fail(function (jqXHR, textStatus) {
            alert("Request failed: " + textStatus);
        });
    }
}

function UpdateStatus(status) {
    var rowindexes = $('#jqxgrid').jqxGrid('getselectedrowindexes');
    if (rowindexes.length == 0) {
        alert('Please select records first by using the checkbox in the first column.');
    } else {
        var boundrows = $('#jqxgrid').jqxGrid('getboundrows');
        var selectedIDs = new Array();
        for (var i = 0; i < rowindexes.length; i++) {
            var row = boundrows[rowindexes[i]];
            selectedIDs.push(row['sbjnum']);
            console.log(row['sbjnum']);
        }
        var request = $.ajax({
            url: "/Survey/SurveyApprovalApprove/" + projectId,
            type: "POST",
            data: { "OpStatus": status, "SurveyIDs": selectedIDs.toString() },
            dataType: "text"
        });
        request.done(function (msg) {
            var boundrows = $('#jqxgrid').jqxGrid('getboundrows');
            for (var i = 0; i < rowindexes.length; i++) {
                dataGrid[rowindexes[i]]['OpStatus'] = (status == 1 ? 'Yes' : 'No');
            }
            $('#jqxgrid').jqxGrid('updatebounddata');
            initialGridFilter();

        });
        request.fail(function (jqXHR, textStatus) {
            alert("Request failed: " + textStatus);
        });
    }
}

function urldecode(str) {
    str = decodeURI(str);
    return decodeURIComponent(str.replace(/\+/g, '%20'));
}

function initialGridFilter() {
    var filtergroup = new $.jqx.filter();
    var filter_or_operator = 1;
    var filtervalue = 'Unchecked';
    var filtercondition = 'contains';
    var filter = filtergroup.createfilter('stringfilter', filtervalue, filtercondition);
    filtergroup.addfilter(filter_or_operator, filter);
    return filtergroup;
}

function initialStringGridFilter(filtervalue) {
    var filtergroup = new $.jqx.filter();
    var filter_or_operator = 1;
    var filtercondition = 'contains';
    var filter = filtergroup.createfilter('stringfilter', filtervalue, filtercondition);
    filtergroup.addfilter(filter_or_operator, filter);
    return filtergroup;
}

function download(filename, content) {
    contentType = 'application/octet-stream';
    var a = document.createElement('a');
    var blob = new Blob([content], { 'type': contentType });
    a.href = window.URL.createObjectURL(blob);
    a.download = filename;
    a.click();
}

function ShowPOIMap(pid, sid) {
    window.open('/Survey/POIMap/' + pid + '?sid=' + sid);
    return;
    BootstrapDialog.show({
        type: BootstrapDialog.TYPE_DEFAULT,
        title: 'POI Position Audit',
        message: function (dialog) {
            var $message = $('<div></div>');
            var pageToLoad = dialog.getData('pageToLoad');
            $message.load(pageToLoad);

            return $message;
        },
        data: {
            'pageToLoad': '/Survey/POIMap/' + pid + '?sid=' + sid
        },
        buttons: [{
            label: 'Close',
            action: function (dialogRef) {
                dialogRef.close();
            }
        }]
    });
}

