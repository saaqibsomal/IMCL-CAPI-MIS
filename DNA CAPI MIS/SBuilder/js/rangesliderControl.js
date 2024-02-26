function rangeSliderControl(fno, id, title, min, max, questionData) {
    formControl.call(this, fno, id, title);
    this.type = "slider";
    this.min=min;
    this.max = max;
    this.questionData = questionData;

}
rangeSliderControl.prototype = Object.create(formControl.prototype); 
rangeSliderControl.prototype.constructor = rangeSliderControl;

rangeSliderControl.prototype.display = function() {
    var field = this.displayLabel();
    field += '<div id="field_' + this.id + '">';

    questions = this.questionData;
    var lmin = this.min;
    var lmax = this.max;
    var ctlId = this.id;
    var fno = this.qno;

    if (questions != null) {
        $.each(questions, function (key, value) {

            field += '<p>' + value['Option'] + '</p>';
            field += '<div id="field_' + ctlId + '_' + value['Id'] + '_slider" class="rangeslider"></div>';
            field += '<script>$(function() { $("#field_' + ctlId + '_' + value['Id'] + '_slider").slider({ range: "max", min: ' + lmin + ', max: ' + lmax + ', value: 1, slide: function(event,ui) { $("#field_' + ctlId + '_' + value['Id'] + '_slider").val(ui.value); rangeSliderControl.prototype.setValue("' + ctlId + '", "' + value['Id'] + '", ' + fno + '); } }); }); </script>';
        });
    }
    else {
        field += '<div id="field_' + this.id + '_slider" class="rangeslider"></div>';
        field += '<script>$(function() { $("#field_' + ctlId + '_' + this.id + '_slider").slider({ range: "max", min: ' + this.min + ', max: ' + this.max + ', value: 1, slide: function(event,ui) { $("#field_' + ctlId + '_' + this.id + '_slider").val(ui.value); rangeSliderControl.prototype.setValue("' + ctlId + '", "' + this.id + '", ' + fno + '); } }); }); </script>';
    }

    field += '</div>';

    return field;
}

rangeSliderControl.prototype.setValue = function (ctlId, sliderId, fno) {
    var values = "";

    $.each($('#field_' + ctlId + ' div.rangeslider'), function (key, value) {
        values += $(value).val() + ",";
    });

    $('#field_' + ctlId).value = values;
    buildData.varFormControls[fno].value = values;
}

rangeSliderControl.prototype.getValues= function(obj,fno) {
	
	buildData.varFormControls[fno].value=obj.value;
}
