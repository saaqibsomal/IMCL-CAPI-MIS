function formControl(fno, id, title) {
  this.qno = fno;  
  this.nextqno=null;
  this.prevqno=null;
  this.value = null;
  this.visible=true;
  this.title=title;  
  this.type = "text";
  this.id=id;
  this.name='field_'+id;
  this.mandatory = false; 
  this.uiElement = null; 
  this.OnValidateScript = "";
}
formControl.prototype.constructor = formControl.prototype;

formControl.prototype.header = function() {
    var field='<div class="form-group formControl" id="field_'+this.id+'">';
    return field;
}
formControl.prototype.footer = function() {
    var field='</div>';
    return field;
}
formControl.prototype.displayLabel = function() {
    var field='<label>'+this.title+'</label>';
  //  this.value=this.getValues(-1);
    return field;
}


formControl.prototype.getValues = function() {
  var values=$('#'+'field_'+this.id+'_text').val();

  return values;
}

formControl.prototype.onValidate = function() {


/*  if (this.type=="SRO") {
    return onSROValidate(this.id);
  }
*/

  if (this.id=='2937') {
    return onValidate_2937();
   // return false;
  }


  if (this.id=='2842') {
    return onValidate_2842();
   // return false;
  }

   if (this.id=='4473') {
    return onValidate_4473();
   // return false;
  }


  if (this.id=='2835') {
    return onValidate_2835();
  }

   if(this.id=='2857') {
    return onValidate_2857();
   }


   if (this.id=='2859') {
    return onValidate_2859();
   }

   if (this.id=='2858') {
    return onValidate_2858();
   }
   

/*
  if (this.id=='3876') {
   return OnValidate_3876();
  } */

  else
    return true;
}

formControl.prototype.onExit = function() {
 /* if (this.id == '2842') {
    return OnExit_2842();
  }
*/

 /* else if (this.id == '3879') {
    return OnExit_3879();
  } */  

  //else {
   // this.nextqno=this.qno;
    // return 1;

    return 1;
 // }
}



formControl.prototype.onEntry=function() {


/*
  if (this.id=='2835') {
    OnEntry_2835();
  }

  if (this.id=='4473') {
    OnEntry_4473();
  }
*/

/*
  if (typeof this.endOnField!="undefined") {
    if (this.type==) {

    }
  }
*/


  


  if (this.visible==false) {
    sf.hideQuestion(this.id);
  }


  if (this.id == '4533') {
    OnEntry_4533();
  }


  if (this.id == '4534') {
    OnEntry_4534();
  }


/*


  if (this.id == '2843') {
    OnEntry_2843();
  }

  if (this.id == '3879') {
    OnEntry_3879();
  }


 if (this.id == '3880') {
    OnEntry_3880();
  }


  if (this.id == '2855') {
    OnEntry_2855();
  }

  if (this.id=='2856') {
    OnEntry_2856();
  }

  if (this.id=='2875') {
    OnEntry_2875();
  }

  if (this.id=='2921') {
    OnEntry_2921();
  }

  */

/*
  if (this.id=='3876') {
    OnEntry_3876();
  } */

  /*
  
  if (this.id=='2867') {
    OnEntry_2867();
  }

  if (this.id=='3877') {
    OnEntry_3877();
  }


  if (this.id=='3878') {
    OnEntry_3878();
  }
  */
  
}


formControl.prototype.HideOptions = function(optionsToHide) {
}



/*
function onValidate_2857() {
  var field_value=sf.GetValue('2857');
  var allValues=field_value.split(',');


  var isDone=true;

  $.each(allValues,function(index,value) {
    if (value==10) {
      if ($('#field_4444_text').val()=='' || $('#field_4444_text').val()==null) {
        android.alert('Other Field Required');
        isDone=false;        
      }      
    }

    
  });

  return isDone;

}


function onSROValidate(id) {
  android.startHiddenRecording(id);
}




function onValidate_2937() {
  var recValue=sf.GetValue('2937');

  if (recValue==1) {
    android.startHiddenRecording('5555');
  }

  return true;
  
}

function StopRecording() {
  android.stopHiddenRecording('5555');
}

function onValidate_2859() {
  var field_value=sf.GetValue('2859');
  var allValues=field_value.split(',');


  var isDone=true;

  $.each(allValues,function(index,value) {
    if (value==7) {
      if ($('#field_4446_text').val()=='' || $('#field_4446_text').val()==null) {
        android.alert('Other Field Required');
        isDone=false;        
      }      
    }

    
  });

  return isDone;

}

function onValidate_2858() {
  var field_value=sf.GetValue('2858');
  var isDone=true;
  
    if (field_value==4) {
      if ($('#field_4445_text').val()=='' || $('#field_4445_text').val()==null) {
        android.alert('Other Field Required');
        isDone=false;        
      }      
    }

    
  

  return isDone;

}

function OnExit_2842() {

  var a = sf.GetValue('2842');
  var allValues=a.split(',');
  var counter=2;

  $.each(allValues,function(index,value) {
    if (value==1) {
          counter=1;   
    }

  });

  return counter;
}

function OnExit_2843() {

  var a = sf.GetValue('2842');
  var allValues=a.split(',');
  var counter=2;

  $.each(allValues,function(index,value) {
    if (value==2) {
          counter=1;   
    }

  });

  return counter;
}

function OnExit_3879() {

  var a = sf.GetValue('2842');
  var allValues=a.split(',');
  var counter=2;

  $.each(allValues,function(index,value) {
    if (value==3) {
          counter=1;   
    }

  });

  return counter;
}




function OnEntry_3879() {


  var a = sf.GetValue('2836');
  sf.ShowOptionsAll('3879');


  if (a == '1') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.HideOptions('3879', '3,4');

   
  } else if (a == '2') {
    sf.HideOptions('3879', '4');
  

  } else if (a == '3') {
    sf.HideOptions('3879', '3,24');


} else if (a == '4') {
    sf.HideOptions('3879', '3,4,6,8,12,13,14,15,16,19,20,21,22,23,24');
  

  } else if (a == '5') {
    sf.HideOptions('3879', '3,4,5,6,8,12,13,15,16,19,20,21,22,23,24');
}  

else if (a == '6') {
    sf.HideOptions('3879', '2,3,4,6,8,12,13,15,16,19,20,21,22,23,24');
  

  } else if (a == '7') {
    sf.HideOptions('3879', '3,4,6,8,12,13,15,16,19,20,21,22,23,24');
}    


  else {
    sf.ShowOptionsAll('3879');
  }  

  
*/














  /*********************************************/

/*

 var a = sf.GetValue('2842');

  var allValues=a.split(',');

  var found=false;
    

  $.each(allValues,function(index,value) {

  //  if (flag) {

    if (value=='2') {
      found=true;
      return false;       
      }
  
  });

if (!found) {
    sf.ShowQuestions('3880');

}



  /////////////////////////////////////////

 

  


  var a = sf.GetValue('2843');

  for (var i = 1; i <= 24; i++) {
    if (a == i) {
    //sf.HideOption('Q5', 'Q5_1');
    sf.HideOptionsAll('3879');
    sf.ShowOptions('3879', i+',');
    return false;
   
  }  else {
     // sf.ShowOptionsAll('3879');
    }
  } 

} 


function OnEntry_3880() {

  
   var a = sf.GetValue('2836');
  sf.ShowOptionsAll('3880');


  if (a == '1') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.HideOptions('3880', '3,4');

   
  } else if (a == '2') {
    sf.HideOptions('3880', '4');
  

  } else if (a == '3') {
    sf.HideOptions('3880', '3,24');


} else if (a == '4') {
    sf.HideOptions('3880', '3,4,6,8,12,13,14,15,16,19,20,21,22,23,24');
  

  } else if (a == '5') {
    sf.HideOptions('3880', '3,4,5,6,8,12,13,15,16,19,20,21,22,23,24');
}  

else if (a == '6') {
    sf.HideOptions('3880', '2,3,4,6,8,12,13,15,16,19,20,21,22,23,24');
  

  } else if (a == '7') {
    sf.HideOptions('3880', '3,4,6,8,12,13,15,16,19,20,21,22,23,24');
}    


  else {
    sf.ShowOptionsAll('3880');
  }  

  */















  /*********************************************/
/*

   var a = sf.GetValue('2842');

    var allValues=a.split(',');

    var found=false;
      

    $.each(allValues,function(index,value) {

    //  if (flag) {

      if (value=='3') {
        found=true;
        return false;       
        }
    
    });

  if (!found) {
      sf.ShowQuestions('2844');

  }






  ///////////////////////////////////////////

  

    var a = sf.GetValue('2843');

  for (var i = 1; i <= 24; i++) {
    if (a == i) {
    //sf.HideOption('Q5', 'Q5_1');
    sf.HideOptionsAll('3880');
    sf.ShowOptions('3880', i+',');
    return false;
   
  }  else {
 //     sf.ShowOptionsAll('3880');
    } 
  }

  
}
*/


/*
if (value=='1') {
      found=true;
      return false;       
      }
  
  });

if (!found) {
    sf.ShowQuestions('3879');

}

*/
/*
function OnEntry_3878() {

  var a = sf.GetValue('3875');

  if (a==1) {
    sf.ShowQuestions('3876');
  }

}
*/
/*
function OnEntry_3876() {
  var a = sf.GetValue('3875');

  if (a==2) {
    sf.ShowQuestions('3878');
  }

} */

/*

function OnValidate_3876() {
  var a = sf.GetValue('3876');

  var isDone=true;

  if (a==2) {
     try {
        android.setData();
       // return false;
      } catch (e) {
      } 
      isDone=false;
  }

  return isDone;

}

function onValidate_4473() {
  var a = sf.GetValue('4473');

  var isDone=false;

//  var num=parseInt(a);
var a=a+'';

  if(a.length==4 && a.indexOf('-')==(-1) && a.indexOf('+')==(-1) && a.indexOf('*')==(-1) && a.indexOf('/')==(-1) && a.indexOf('.')==(-1) && a.indexOf(' ')==(-1) && a.indexOf(',')==(-1)) {

//if(a.length!=4) {
 // if (!(num<=9999 && num>999)) {
     
      isDone=true;
  }

  else {
        android.alert('0000-9999 allowed Only');
  }

  return isDone;

}

function onValidate_2835() {
  var a = sf.GetValue('2835');

  var isDone=false;

  var a=a+'';


 // var num=parseInt(a);

    if(a.length==10 && a.indexOf('-')==(-1) && a.indexOf('+')==(-1) && a.indexOf('*')==(-1) && a.indexOf('/')==(-1) && a.indexOf('.')==(-1) && a.indexOf(' ')==(-1) && a.indexOf(',')==(-1)) {
 //if(a.length!=10) {

  //if (!(num>999999999 && num<=9999999999)) {
     
      isDone=true;
  }

  else {
        android.alert('0000000000-9999999999 allowed Only');
  }

  return isDone;

}


function OnEntry_2855() {


  var a = sf.GetValue('2843');
  sf.HideOptionsAll('2855');


  if (a == '1') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.ShowOptions('2855', '1,23,24');

   
  } else if (a == '2') {

    if (sf.GetValue('2836')==2 || sf.GetValue('2836')==3)
        sf.ShowOptions('2855', '19,35,36,37,38');

      else
        sf.ShowOptions('2855', '4,19,35,36,37,38');   
  

  } else if (a == '5') {
    sf.ShowOptions('2855', '3');
  

  } else if (a == '6') {
    sf.ShowOptions('2855', '5');
  

  } else if (a == '7') {
    sf.ShowOptions('2855', '15,17');
  

  } else if (a == '8') {

      if (sf.GetValue('2836')==1)
        sf.ShowOptions('2855', '2,34');

      else
        sf.ShowOptions('2855', '2');
  

  } else if (a == '9') {
    sf.ShowOptions('2855', '11,14,16');
  

  } else if (a == '10') {
    sf.ShowOptions('2855', '12,13,41');
  } else if (a == '11') {
    sf.ShowOptions('2855', '11,14');
  } else if (a == '12') {
    sf.ShowOptions('2855', '10,28,34');
  } else if (a == '13') {
    sf.ShowOptions('2855', '11,14');
  } else if (a == '14') {
    sf.ShowOptions('2855', '2');
  } else if (a == '15') {
    sf.ShowOptions('2855', '18');
  }  else if (a == '16') {
    sf.ShowOptions('2855', '26,27');
  } else if (a == '17') {
    sf.ShowOptions('2855', '9');
  }  else if (a == '18') {
    sf.ShowOptions('2855', '6');
  } else if (a == '19') {
    sf.ShowOptions('2855', '8,20,32');
  }  else if (a == '20') {
    sf.ShowOptions('2855', '37,38');
  } else if (a == '21') {
    sf.ShowOptions('2855', '33');
  }  else if (a == '22') {
    sf.ShowOptions('2855', '29');
  } else if (a == '23') {
    sf.ShowOptions('2855', '31,34');
  } else if (a == '24') {
    sf.ShowOptions('2855', '30');
  }  

  else {
    sf.ShowOptionsAll('2855');
  }  

  

}

function OnEntry_2856() {
    var a = sf.GetValue('2842');

    var allValues=a.split(',');

    var found=false;
      

    $.each(allValues,function(index,value) {

    //  if (flag) {

      if (value=='1') {
        found=true;
        return false;       
        }
    
    });

  if (!found) {
      sf.ShowQuestions('2875');

  }
}

function OnEntry_2875() {
    var a = sf.GetValue('2842');

    var allValues=a.split(',');

    var found=false;
      

    $.each(allValues,function(index,value) {

    //  if (flag) {

      if (value=='2') {
        found=true;
        return false;       
        }
    
    });

  if (!found) {
      sf.ShowQuestions('2921');

  }
}

function OnEntry_2921() {
    var a = sf.GetValue('2842');

    var allValues=a.split(',');

    var found=false;
      

    $.each(allValues,function(index,value) {

    //  if (flag) {

      if (value=='3') {
        found=true;
        return false;       
        }
    
    });

  if (!found) {
      sf.ShowQuestions('2865');

  }
}

function OnEntry_3877() {

  var a = sf.GetValue('3876');

  if (a==2) {
    sf.ShowQuestions('2874');
  }
}

*/


function OnEntry_4534() {

  var a = sf.GetValue('4533');
  sf.ShowOptionsAll('4534');


  if (a == '55') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.HideOptions('4534', '5,6,7');   
  }


  else if (a == '56') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.HideOptions('4534', '1,2,3,4');   
  }


   else if (a == '42') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.HideOptions('4534', '1,2,3,4');   
  }


   else if (a == '43') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.HideOptions('4534', '1,5,6,7');   
  }


   else if (a == '44') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.HideOptions('4534', '1,2,3,4');   
  }

   else if (a == '63') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.HideOptions('4534', '1,2');   
  }

   else if (a == '64') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.HideOptions('4534', '3,4,5,6,7');   
  }

   else if (a == '65') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.HideOptions('4534', '3,4,5,6,7');   
  }

   else if (a == '101') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.HideOptions('4534', '1');   
  }

  else if (a == '102') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.HideOptions('4534', '1');   
  }
  else if (a == '103') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.HideOptions('4534', '1');   
  }
  else if (a == '104') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.HideOptions('4534', '1');   
  }
  else if (a == '105') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.HideOptions('4534', '1');   
  }
  else if (a == '106') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.HideOptions('4534', '1');   
  }

 else if (a == '107') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.HideOptions('4534', '1,3,4,5,6,7');   
  }

  else if (a == '261') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.HideOptions('4534', '4,5,6,7');   
  }


else if (a == '262') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.HideOptions('4534', '4,5,6,7');   
  }


  else if (a == '263') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.HideOptions('4534', '4,5,6,7');   
  }


  else if (a == '151') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.HideOptions('4534', '1,2,3');   
  }

  else if (a == '152') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.HideOptions('4534', '1,2,3');   
  }

  else if (a == '153') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.HideOptions('4534', '1,2,3');   
  }

   else if (a == '154') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.HideOptions('4534', '1,2,3');   
  }


   else if (a == '131') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.HideOptions('4534', '1,2,3');   
  }


   else if (a == '132') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.HideOptions('4534', '1,2,3');   
  }


   else if (a == '133') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.HideOptions('4534', '1,2,3');   
  }


   else if (a == '134') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.HideOptions('4534', '1,2,3');   
  }

   else if (a == '21') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.HideOptions('4534', '4,5,6,7');   
  }

   else if (a == '22') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.HideOptions('4534', '4,5,6,7');   
  }

    else if (a == '121') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.HideOptions('4534', '3,4,5,6,7');   
  }  

    else if (a == '122') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.HideOptions('4534', '3,4,5,6,7');   
  }

   else if (a == '123') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.HideOptions('4534', '3,4,5,6,7');   
  } 

   else if (a == '124') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.HideOptions('4534', '3,4,5,6,7');   
  }


 else {
    sf.ShowOptionsAll('4534');
  }  




}


function OnEntry_4533() {

  var a = sf.GetValue('4532');
  sf.HideOptionsAll('4533');


  if (a == '22') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.ShowOptions('4533', '221,223,224');   
  }

  else if (a == '18') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.ShowOptions('4533', '181,182,183');   
  }

  else if (a == '5') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.ShowOptions('4533', '51,52,53,54,55,56');   
  }


  else if (a == '4') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.ShowOptions('4533', '41,42,43,44,45,46');   
  }


  else if (a == '28') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.ShowOptions('4533', '281,282,283');   
  }


  else if (a == '11') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.ShowOptions('4533', '111,112,113,114');   
  }

  else if (a == '1') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.ShowOptions('4533', '11,12,13,14,15,16');   
  }

  else if (a == '23') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.ShowOptions('4533', '231,232,233');   
  }

  else if (a == '6') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.ShowOptions('4533', '61,62,63,64,65');   
  }

  else if (a == '8') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.ShowOptions('4533', '81,82,83,84,85,86,87');   
  }


  else if (a == '10') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.ShowOptions('4533', '101,102,103,104,105,106,107');   
  }


   else if (a == '25') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.ShowOptions('4533', '251,252,253');   
  }


   else if (a == '14') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.ShowOptions('4533', '141,142,143,144');   
  }


   else if (a == '26') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.ShowOptions('4533', '261,262,263');   
  }


  else if (a == '15') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.ShowOptions('4533', '151,152,153,154');   
  }

   else if (a == '13') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.ShowOptions('4533', '131,132,133,134');   
  }


   else if (a == '7') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.ShowOptions('4533', '71,72,73,74,75');   
  }



  else if (a == '16') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.ShowOptions('4533', '161,162,163,164');   
  }


  else if (a == '21') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.ShowOptions('4533', '211,212,213');   
  }


  else if (a == '19') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.ShowOptions('4533', '191,192,193');   
  }


   else if (a == '20') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.ShowOptions('4533', '201,202,203');   
  }


 else if (a == '3') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.ShowOptions('4533', '31,32,33,34,35,36');   
  }

  else if (a == '24') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.ShowOptions('4533', '241,242,243');   
  }


  else if (a == '2') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.ShowOptions('4533', '21,22');   
  }


  else if (a == '30') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.ShowOptions('4533', '301,302,303,304,305,306,307,308,309');   
  }


  else if (a == '9') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.ShowOptions('4533', '91,92,93,94,95,96,97');   
  }


   else if (a == '12') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.ShowOptions('4533', '121,122,123,124');   
  }


   else if (a == '27') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.ShowOptions('4533', '271,272,273');   
  }

   else if (a == '17') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.ShowOptions('4533', '171,172,173,174');   
  }


   else if (a == '31') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.ShowOptions('4533', '311,312');   
  }



 else {
    sf.HideOptionsAll('4533');
  }  




}





function onValidate_4473() {
  var a = sf.GetValue('4473');

  var isDone=false;

//  var num=parseInt(a);
var a=a+'';

  if(a.length==4 && a.indexOf('-')==(-1) && a.indexOf('+')==(-1) && a.indexOf('*')==(-1) && a.indexOf('/')==(-1) && a.indexOf('.')==(-1) && a.indexOf(' ')==(-1) && a.indexOf(',')==(-1)) {

//if(a.length!=4) {
 // if (!(num<=9999 && num>999)) {
     
      isDone=true;
  }

  else {
        android.alert('0000-9999 allowed Only');
  }

  return isDone;

}

/*

function OnEntry_2843() {





  var a = sf.GetValue('2836');
  sf.ShowOptionsAll('2843');


  if (a == '1') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.HideOptions('2843', '3,4');

   
  } else if (a == '2') {
    sf.HideOptions('2843', '4');
  

  } else if (a == '3') {
    sf.HideOptions('2843', '3,24');


} else if (a == '4') {
    sf.HideOptions('2843', '3,4,6,8,12,13,14,15,16,19,20,21,22,23,24');
  

  } else if (a == '5') {
    sf.HideOptions('2843', '3,4,5,6,8,12,13,15,16,19,20,21,22,23,24');
}  

else if (a == '6') {
    sf.HideOptions('2843', '2,3,4,6,8,12,13,15,16,19,20,21,22,23,24');
  

  } else if (a == '7') {
    sf.HideOptions('2843', '3,4,6,8,12,13,15,16,19,20,21,22,23,24');
}    


  else {
    sf.ShowOptionsAll('2843');
  }  

  
















  var a = sf.GetValue('2842');

  var allValues=a.split(',');

  var found=false;
    

  $.each(allValues,function(index,value) {

  //  if (flag) {

    if (value=='1') {
      found=true;
      return false;       
      }
  
  });

if (!found) {
    sf.ShowQuestions('3879');

}
 

}


function onValidate_2842() {

  var isNotNone=true;

  var X2=sf.GetValue('2842');

  var X2_array=X2.split(',');

  $.each(X2_array,function(index,value) {
  
    if(value=='4') {
      isNotNone=false;
       try {
        android.cancelSurvey();
       // return false;
      } catch (e) {
      }
      
      return false;
    }

  });


  return isNotNone;

}


function OnEntry_2835() {

    sf.number_validation('2835',0,999999999);
}


function OnEntry_2833() {

    sf.number_validation('2833',0,9999);
}



function OnEntry_2867() {
  
var a = sf.GetValue('2866');

  if (a==2 || a==3) {
    sf.ShowQuestions('2870');
  }


} */