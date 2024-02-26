// On Focus out from every input and select field
function onChange() {
    var i;
    var _groupType = parseInt($("#groupType").val());
    var _incidence = parseInt($("#incidencePercent").val());
    var _incidenceLevelId = 0;
    var _incidenceLevelName = "";

    var _surveyType = parseInt($("#surveyType").val());
    var _transportation = parseFloat($("#transportation").val());
    var _others = parseFloat($("#others").val());
    var _baseSize = parseInt($("#noofGroups").val());

    for (i = incidenceLevel.length-1; i >= 0; i--) {
        if (_incidence >= incidenceLevel[i][0]) {
            _incidenceLevelId = i;
            _incidenceLevelName = incidenceLevel[i][1];
        }
    }

    var _slabRate = slabRate[_groupType][_incidenceLevelId];
    var costingRescruiting = recruiting[_groupType][_incidenceLevelId] * _slabRate;
    var costingGifts = gifts[_groupType][_incidenceLevelId] * _slabRate;
    var costingVenue = venue[_groupType][_incidenceLevelId] * 1;
    var costingRefreshments = refreshments[_groupType][_incidenceLevelId] * _slabRate;
    var costingModeration = moderation[_groupType][_incidenceLevelId] * 1;
    var costingTranscripts = transcripts[_groupType][_incidenceLevelId] * 1;
    var costingTranslation = translation[_groupType][_incidenceLevelId] * 1;
    var costingContentAnalysis = contentanalysis[_groupType][_incidenceLevelId] * 1;
    var costingReporting = reporting[_groupType][_incidenceLevelId] * 1;
    var costingPresentation = presentation[_groupType][_incidenceLevelId] * 1;

    var output_TotalCost = 0;
    var output_TotalCost1 = costingRescruiting + costingGifts + costingVenue + costingRefreshments + costingModeration;
    var output_TotalCost2 = output_TotalCost1 + costingTranscripts + costingTranslation + costingContentAnalysis + costingReporting + costingPresentation;
    if (_surveyType == 0) {
        output_TotalCost = output_TotalCost1;
    } else if (_surveyType == 1) {
        output_TotalCost = output_TotalCost1 + costingTranscripts;
    } else if (_surveyType == 2) {
        output_TotalCost = output_TotalCost2 - costingTranslation;
    } else if (_surveyType == 3) {
        output_TotalCost = output_TotalCost2;
    }
    output_TotalCost = output_TotalCost * _baseSize;

    //Discount calculations
    var _incidenceLevelIdBase = 1;

    if (_baseSize < 4) {
        _incidenceLevelIdBase = 0;
    } else if (_baseSize >= 5 && _baseSize < 9) {
        _incidenceLevelIdBase = 1;
    } else if (_baseSize >= 9 && _baseSize < 14) {
        _incidenceLevelIdBase = 2;
    } else if (_baseSize > 13) {
        _incidenceLevelIdBase = 3;
    }

    var _priority = parseInt($("#priority").val());
    var _heavyClientDisc = 0;
    var _Discount = false;
    if ($("#heavyClientDisc").val()) {
        _Discount = true;
        _heavyClientDisc = parseInt($("#heavyClientDisc").val());
    }
    var output_costingDiscount = 0;
    if (_Discount) {
        output_costingDiscount = (lbd[_incidenceLevelIdBase][_incidenceLevelId] / 100) + (hcd[_heavyClientDisc][_incidenceLevelId] / 100);
    }

    var output_Additional = _transportation + _others;                  //This is in US$
    var output_TotalCostUSD = output_TotalCost / 3.75                          //Converted in US$ from SAR
    var output_Profit = (output_TotalCostUSD) * 1;   //100% Profit
    var output_Charges = (output_TotalCostUSD) + output_Profit;

    var output_costingPriority = pu[0][_priority];
    output_Charges += output_Additional;
    output_Charges = output_Charges * output_costingPriority;

    output_costingDiscount = output_costingDiscount * output_Charges;

    var output_Net = output_Charges - output_costingDiscount;
    var output_cpi = output_Net / _baseSize;

    var output_COS = output_Additional + output_TotalCost;              //For COS additional cost is converted to SAR as is
    var output_GP = 100 - (output_COS / (output_Net * 3.75)) * 100;
    
    $("#incidenceLevel").text(_incidenceLevelName);

    $("#costingAdditional").text("US$ " + output_Additional.toFixed(0));
    $("#costingTotal").text("US$ " + output_Charges.toFixed(0));
    $("#costingAfterDiscount").text("US$ " + output_Net.toFixed(0));

    $("#total-NetFees").text("US$ " + output_Net.toFixed(0));
    $("#total-CPI").text("US$ " + output_cpi.toFixed(0));

    $("#costingOfSales").text("SAR " + output_COS.toFixed(0));
    $("#grossProfit").text(output_GP.toFixed(1) + "%");

}

$(document).ready(function () {
    
    // Attach a submit handler to the form
    $("input").on('focusout', onChange);
    $("select").change(onChange);
    setTimeout(function () {
        $('#noofGroups').focus();
    }, 0);
});

function validateBaseSize() {
    var invalid = false;
    var _noofGroups = parseInt($("#noofGroups").val());

    if (_noofGroups < 1) {
        alert("Group size must be greater than 0");
        invalid = true;
    }
    if (invalid) {
        setTimeout(function () {
            $('#noofGroups').focus();
        }, 0);
        return false;
    }
    return true;
}