function integerTextboxControls(fno, id, title, MinLength, MaxLength) {
    formControl.call(this, fno, id, title);
    this.type = "number";
    this.MinLength = (MinLength ? (parseInt(MinLength) > 0 ? MinLength : 0) : 0);
    this.MaxLength = (MaxLength ? (parseInt(MaxLength) > 0 ? MaxLength : 0) : 0);
}

integerTextboxControls.prototype = Object.create(formControl.prototype); 
integerTextboxControls.prototype.constructor = integerTextboxControls;

integerTextboxControls.prototype.display = function () {
    var field = this.displayLabel();

    // field += '<div><input type="'+this.type+'" name="'+this.name+'" id="field_'+this.id+'_text" onblur="textboxControls.prototype.getValues(this,'+this.qno+')"></div>';
    field += '<input class="form-control txt-fnt-mob" type="' + this.type + '" name="' + this.name + '" id="field_' + this.id + '_number" onkeyup="integerTextboxControls.prototype.getValues(this,' + this.qno + ')">';
    field += '<span class="txt-info" id="field_' + this.id + '_info"></span>';

    return field;
}

integerTextboxControls.prototype.getValues= function(obj,fno) {
    var ctl = buildData.varFormControls[fno];
    ctl.value = obj.value;

    var info = 'Count: ' + ctl.value.length;
    if (ctl.MinLength > 0) {
        info += ' | Required: ' + ctl.MinLength;
    }
    if (ctl.MaxLength > 0) {
        info += ' | Max Chars: ' + ctl.MaxLength;
    }
    $('#field_' + ctl.id + '_info').html(info);
}


integerTextboxControls.prototype.number_validation = function(min,max) {
  var ctlId = this.id;
  var maxVal=max+'';

//  $("#field_" + ctlId+'_text').mask(max);
 
    /*  $("#field_" + ctlId+'_text').keyup(function() {

        var currentVal=parseInt($(this).val());

        if (currentVal<0 && currentVal>999999999) {

          var checkVal=parseInt($(this).val());
           $("#field_" + ctlId+'_text').val(checkVal);
        }

        
      });

*/
 
// + "_" + value.Id

}



integerTextboxControls.prototype.setValues = function (value) {
    this.value = value;
    $('#field_' + this.id + '_number').val(value);
}

integerTextboxControls.prototype.onValidate = function () {
    if (this.MinLength > 0) {
        if (this.value.length < this.MinLength) {
            sf.alert("Minimum number of characters required are " + this.MinLength.toString());
            return false;
        }
    }
    if (this.MaxLength > 0) {
        if (this.value.length > this.MaxLength) {
            sf.alert("Maximum number of characters allowed are " + this.MaxLength.toString());
            return false;
        }
    }
    var evalValue = true;
    if (this.ScriptOnValidate.length > 0) {
        try {
            evalValue = (new Function(this.ScriptOnValidate)());
            if (typeof (evalValue) != "boolean") {
                evalValue = true;
            }
        } catch (e) {
            sf.alert("Contact Administrator: Error in script");
        }
    }
    return evalValue;
}
