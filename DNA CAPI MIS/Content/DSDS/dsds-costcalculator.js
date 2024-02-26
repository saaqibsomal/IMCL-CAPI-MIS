// On Focus out from every input and select field
function onChange() {
    if ($("#qnrLength").val().length == 0) {
        return;
    }

    var i;
    var _baseSizeM = parseInt($("#baseSizeM").val());
    var _baseSizeP = parseInt($("#baseSizeP").val());
    var _baseSizeR = parseInt($("#baseSizeR").val());
    var _baseSize = _baseSizeM + _baseSizeP + _baseSizeR;

    var _incidence = parseInt($("#incidencePercent").val());
    var _incidenceLevelId = 0;
    var _incidenceLevelName = "";
    var _qnrLengthMins = parseInt($("#qnrLength").val());
    var _qnrType = parseInt($("#qnrType").val());
    _qnrType = (_qnrType >= 0 ? _qnrType : 0);
    var _translate = parseInt($("#translate").val())
    var _dpcapi = parseInt($("#dpcapi").val());
    var _dataconv = parseInt($("#dataconv").val());
    var _clientServHrs = parseInt($("#clientServHrs").val());
    _clientServHrs = (_clientServHrs >= 0 ? _clientServHrs : 0);
    var _analysisType = parseInt($("#analysisType").val());
    _analysisType = (_analysisType >= 0 ? _analysisType : 0);
    var _giftsCost = parseFloat($("#costGift").val());
    var _travelKm = parseFloat($("#travelkm").val());
    var _nightnos = parseFloat($("#nightnos").val());
    var _venueType = parseInt($("#venueType").val());
    var _venueDays = parseInt($("#venueDays").val());
    var _priority = parseInt($("#priority").val());
    _priority = (_priority >= 0 ? _priority : 0);
    var _heavyClientDisc = 0;
    var _Discount = false;
    if ($("#heavyClientDisc").val()) {
        _Discount = true;
        _heavyClientDisc = parseInt($("#heavyClientDisc").val());
    }
    var _others = parseFloat($("#others").val());
    var _slabId = 1;
    var _slabIdBase = 1;

    if (!(_giftsCost >= 0)) _giftsCost = 0;
    if (!(_others >= 0)) _others = 0;

    if (_qnrLengthMins >= 1 && _qnrLengthMins <= 10) {
        _slabId = 1;
    } else if (_qnrLengthMins > 10 && _qnrLengthMins <= 25) {
        _slabId = 2;
    } else if (_qnrLengthMins > 26 && _qnrLengthMins <= 40) {
        _slabId = 3;
    } else {
        if (_qnrLengthMins < 1) {
            alert("1Questionnaire length must be greater than 0 min");
            $("#qnrLength").val(0);
        } else {
            alert("1Questionnaire length cannot be greater than 40 mins in this Cost Calculator");
            $("#qnrLength").val(40);
        }
        return false;
    }

    if (_baseSize < 500) {
        _slabIdBase = 0;
    } else if (_baseSize >= 500 && _baseSize < 1500) {
        _slabIdBase = 1;
    } else if (_baseSize > 1500) {
        _slabIdBase = 2;
    }

    for (i = 0; i < incidenceLevel.length; i++) {
        if (_incidence >= incidenceLevel[i][0]) {
            _incidenceLevelId = i;
            _incidenceLevelName = incidenceLevel[i][1];
        }
    }

    var _nightnos = parseFloat($("#nightnos").val());
    var _travelKm = parseFloat($("#travelkm").val());

    var output_costingInc = incidence[_incidenceLevelId][_slabId] * _baseSize;
    var output_costingQnr = qnr[_qnrType][_slabId];
    var output_costingTrans = trqnr[_translate][_slabId];
    var output_costingDPCAPI = dp[_dpcapi][_slabId] * _baseSize;
    var output_costingDataConv = datcnv[_dataconv][_slabId];
    var output_costingCSH = csh[_clientServHrs][_slabId];
    var output_costingAnalysis = afee[_analysisType][_slabId];
    var output_costingDiscount = (pu[0][_priority] / 100);
    if (_Discount) {
        output_costingDiscount = (pu[0][_priority] / 100) * (lbd[_slabIdBase][_slabId] / 100) * (hcd[_heavyClientDisc][_slabId] / 100);
    }
    var output_costingVenue = venuet[0][_venueType] * _venueDays;
    var output_costingTravel = fomrate[0][_slabId] * _travelKm;
    var output_costingNights = fomrate[1][_slabId] * _nightnos;
    var output_costingGifts = _giftsCost;

    var costPerField = (output_costingInc / _baseSize);
    output_costingInc = (costPerField * _baseSizeM * fom[0][_slabId] / 100) + (costPerField * _baseSizeP * fom[1][_slabId] / 100) + (costPerField * _baseSizeR * fom[2][_slabId] / 100);

    var output_totalcost = output_costingInc + output_costingQnr + output_costingTrans + output_costingDPCAPI +
                            output_costingDataConv + output_costingCSH + output_costingAnalysis +
                            output_costingGifts + output_costingVenue + output_costingTravel + output_costingNights + _others;

    output_costingDiscount = output_costingDiscount * output_totalcost;

    $("#incidenceLevel").text(_incidenceLevelName);
    $("#baseSize").val(_baseSize);

    $("#costingInc").text("US$ " + output_costingInc.toFixed(0));
    $("#costingQnr").text("US$ " + output_costingQnr.toFixed(0));
    $("#costingTrans").text("US$ " + output_costingTrans.toFixed(0));
    $("#costingDPCAPI").text("US$ " + output_costingDPCAPI.toFixed(0));
    $("#costingDataConv").text("US$ " + output_costingDataConv.toFixed(0));
    $("#costingCSH").text("US$ " + output_costingCSH.toFixed(0));
    $("#costingAnalysis").text("US$ " + output_costingAnalysis.toFixed(0));
    $("#costingGifts").text("US$ " + output_costingGifts.toFixed(0));
    $("#costingVenue").text("US$ " + output_costingVenue.toFixed(0));
    $("#costingTravel").text("US$ " + output_costingTravel.toFixed(0));
    $("#costingNights").text("US$ " + output_costingNights.toFixed(0));
    $("#costingOthers").text("US$ " + _others.toFixed(0));

    $("#costingTotal").text("US$ " + output_totalcost.toFixed(0));
    $("#costingAfterDiscount").text("US$ " + output_costingDiscount.toFixed(0));

    var output_netfees = output_costingDiscount;
    var output_cpi = output_netfees / _baseSize;

    $("#total-NetFees").text("US$ " + output_netfees.toFixed(0));
    $("#total-CPI").text("US$ " + output_cpi.toFixed(0));


    //Calculate COS
    var input_from = "fees";
    if (event.target.name == "VenuePerDayCOS" || event.target.name == "travelkmCOS" || event.target.name == "nightRateCOS" || event.target.name == "costGiftCOS" || event.target.name == "othersCOS")
    {
        input_from = "cos";
    }

    var _VenuePerDayCOS = 0;
    var _travelkmCOS = 0;
    var _nightRateCOS = 0;

    if (input_from == "fees")
    {
        //_giftsCost = _giftsCost * 3.75;                //Conversion to SAR will use same rate
        _giftsCost = _giftsCost.toFixed(2);
        //_others = _others * 3.75;
        _others = _others.toFixed(2);

        _VenuePerDayCOS = venusPdCOS[0][_venueType] * _venueDays;
        _travelkmCOS = fomrate[0][_slabId] * _travelKm;
        _nightRateCOS = fomrate[1][_slabId] * _nightnos;

        if (!(_VenuePerDayCOS >= 0)) _VenuePerDayCOS = 0;
        if (!(_costGiftCOS >= 0)) _costGiftCOS = 0;
        if (!(_othersCOS >= 0)) _othersCOS = 0;

        $("#VenuePerDayCOS").val(_VenuePerDayCOS.toFixed(0));
        $("#travelkmCOS").val(_travelkmCOS.toFixed(0));
        $("#nightRateCOS").val(_nightRateCOS.toFixed(0));
        $("#costGiftCOS").val(_giftsCost);
        $("#othersCOS").val(_others);
    }
    _VenuePerDayCOS = parseInt($("#VenuePerDayCOS").val());
    _travelkmCOS = parseFloat($("#travelkmCOS").val());
    _nightRateCOS = parseFloat($("#nightRateCOS").val());
    var _costGiftCOS = parseFloat($("#costGiftCOS").val());
    var _othersCOS = parseFloat($("#othersCOS").val());

    if (!(_VenuePerDayCOS >= 0)) _VenuePerDayCOS = 0;
    if (!(_costGiftCOS >= 0)) _costGiftCOS = 0;
    if (!(_othersCOS >= 0)) _othersCOS = 0;

    var cos_costingVenue = _VenuePerDayCOS;
    var cos_costingTravel = _travelkmCOS;
    var cos_costingNights = _nightRateCOS;

    //Now calculate
    output_totalcost = output_costingInc + output_costingDPCAPI + output_costingDataConv + 
                            _costGiftCOS + cos_costingVenue + cos_costingTravel + cos_costingNights + _othersCOS;

    //$("#costingTotalCOS").text("SAR " + output_totalcost.toFixed(0));

    var output_GP = output_totalcost / 3.75;            //Convert in $, so that % can be correctly calculated. Netfees is in $
    var output_GP = 100 - (output_GP / output_netfees) * 100;

    output_netfees = output_totalcost;
    output_cpi = output_netfees / _baseSize;

    $("#total-NetFeesCOS").text("SAR " + output_netfees.toFixed(0));
    $("#total-CPICOS").text("SAR " + output_cpi.toFixed(0));
    $("#total-GP").text(output_GP.toFixed(1) + "%");

}



$(document).ready(function () {
    
    // Attach a submit handler to the form
    $("input").on('focusout', onChange);
    $("select").change(onChange);

    setTimeout(function () {
        $('#qnrLength').focus();
    }, 0);
});

function validateQnrLength() {
    var invalid = false;
    var _qnrLengthMins = parseInt($("#qnrLength").val());
 //   if ($("#qnrLength").val().length > 0) {
    if ($("#qnrLength").val().length == 0 || _qnrLengthMins < 1) {
            alert("Questionnaire length must be greater than 0 min");
            //$("#qnrLength").val();
            invalid = true;
        } else if (_qnrLengthMins > 40) {
            alert("Questionnaire length cannot be greater than 40 mins in this Cost Calculator");
            $("#qnrLength").val(40);
            invalid = true;
        }
        if (invalid) {
            setTimeout(function () {
                $('#qnrLength').focus();
            }, 0);
            return false;
        }
//    }
    return true;
}