function formControl(fno, id, title) {
  this.qno = fno;
  this.value = null;
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
  return true;
}

formControl.prototype.onExit = function() {
  if (this.id == '2842') {
    return OnExit_2842();
  }

/*  else if (this.id == '2843') {
//    return OnExit_2843();
  }
*/
  else if (this.id == '3879') {
    return OnExit_3879();
  }  

  else {
    return 1;
  }
}

/*
formControl.prototype.onBack = function() {
  if (this.id == '2843') {
    OnBack_2843();
  }

  if (this.id == '3879') {
    OnBack_3879();
  }

  if (this.id == '3880') {
    OnBack_3880();
  }
} */

formControl.prototype.onEntry=function() {



  if (this.id == '3879') {
 //   sf.HideAllQuestions('2842',3);
    OnEntry_3879();
  }


 if (this.id == '3880') {
 //   sf.HideAllQuestions('2842',3);
    OnEntry_3880();
  }


  if (this.id == '2855') {
 //   sf.HideAllQuestions('2842',3);
    OnEntry_2855();
  }

  

/*
  if (this.id == '2843') {
    OnEntry_2843();
  } 
*/

/*  if (this.id == '3879') {
  //  sf.HideAllQuestions('2843',2);
    OnEntry_3879();
  } 

*/

/*  if (this.id == '3880') {
    OnEntry_3880();
  } */

 /* if (this.id == '2842') {
    OnEntry_2842();
  } */
}


formControl.prototype.HideOptions = function(optionsToHide) {
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

   var a = sf.GetValue('2842');

  var allValues=a.split(',');


  $.each(allValues,function(index,value) {

  //  if (flag) {

      if (value!='2') {
        sf.HideQuestions('3879');
     //   sf.HideAllQuestions('3879',2);
       // flag=false;
      }

      else {
        sf.ShowQuestions('3879');
        return false;
      }


    }); 

  


  var a = sf.GetValue('2843');

  for (var i = 1; i <= 24; i++) {
    if (a == i) {
    //sf.HideOption('Q5', 'Q5_1');
    sf.HideOptionsAll('3879');
    sf.ShowOptions('3879', i+',');
    return false;
   
  }  else {
      sf.ShowOptionsAll('3879');
    }
  } 

} 
/*
function OnEntry_3879() {


  var a = sf.GetValue('2842');

  var allValues=a.split(',');

sf.HideQuestions('3879');

  $.each(allValues,function(index,value) {

  //  if (flag) {

      if (value==2) {
        sf.ShowQuestions('3879');
     //   sf.HideAllQuestions('3879',2);
       // flag=false;
      }

});



  var a = sf.GetValue('2843');

  if (a == '1') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.ShowOptionsAll('3879');
    sf.HideOptions('3879', '2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24');

   
  } else if (a == '2') {
    sf.ShowOptionsAll('3879');
    sf.HideOptions('3879', '1,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24');
  

  } else if (a == '3') {
    sf.ShowOptionsAll('3879');
    sf.HideOptions('3879', '1,2,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24');
  

  } else if (a == '4') {
    sf.ShowOptionsAll('3879');
    sf.HideOptions('3879', '1,2,3,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24');
  

  } else if (a == '5') {
    sf.ShowOptionsAll('3879');
    sf.HideOptions('3879', '1,2,3,4,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24');
  

  } else if (a == '6') {
    sf.ShowOptionsAll('3879');
    sf.HideOptions('3879', '1,2,3,4,5,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24');
  

  } else if (a == '7') {
    sf.ShowOptionsAll('3879');
    sf.HideOptions('3879', '1,2,3,4,5,6,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24');
  

  } else if (a == '8') {
    sf.ShowOptionsAll('3879');
    sf.HideOptions('3879', '1,2,3,4,5,6,7,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24');
  } else if (a == '9') {
    sf.ShowOptionsAll('3879');
    sf.HideOptions('3879', '1,2,3,4,5,6,7,8,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24');
  } else if (a == '10') {
    sf.ShowOptionsAll('3879');
    sf.HideOptions('3879', '1,2,3,4,5,6,7,8,9,11,12,13,14,15,16,17,18,19,20,21,22,23,24');
  } else if (a == '11') {
    sf.ShowOptionsAll('3879');
    sf.HideOptions('3879', '1,2,3,4,5,6,7,8,9,10,12,13,14,15,16,17,18,19,20,21,22,23,24');
  } else if (a == '12') {
    sf.ShowOptionsAll('3879');
    sf.HideOptions('3879', '1,2,3,4,5,6,7,8,9,10,11,13,14,15,16,17,18,19,20,21,22,23,24');
  } else if (a == '13') {
    sf.ShowOptionsAll('3879');
    sf.HideOptions('3879', '1,2,3,4,5,6,7,8,9,10,11,12,14,15,16,17,18,19,20,21,22,23,24');
  }  else if (a == '14') {
    sf.ShowOptionsAll('3879');
    sf.HideOptions('3879', '1,2,3,4,5,6,7,8,9,10,11,12,13,15,16,17,18,19,20,21,22,23,24');
  } else if (a == '15') {
    sf.ShowOptionsAll('3879');
    sf.HideOptions('3879', '1,2,3,4,5,6,7,8,9,10,11,12,13,14,16,17,18,19,20,21,22,23,24');
  }  else if (a == '16') {
    sf.ShowOptionsAll('3879');
    sf.HideOptions('3879', '1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,17,18,19,20,21,22,23,24');
  } else if (a == '17') {
    sf.ShowOptionsAll('3879');
    sf.HideOptions('3879', '1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,18,19,20,21,22,23,24');
  }  else if (a == '18') {
    sf.ShowOptionsAll('3879');
    sf.HideOptions('3879', '1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,19,20,21,22,23,24');
  } else if (a == '19') {
    sf.ShowOptionsAll('3879');
    sf.HideOptions('3879', '1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,20,21,22,23,24');
  }  else if (a == '20') {
    sf.ShowOptionsAll('3879');
    sf.HideOptions('3879', '1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,21,22,23,24');
  } else if (a == '21') {
    sf.ShowOptionsAll('3879');
    sf.HideOptions('3879', '1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,22,23,24');
  } else if (a == '22') {
    sf.ShowOptionsAll('3879');
    sf.HideOptions('3879', '1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,23,24');
  } else if (a == '23') {
    sf.ShowOptionsAll('3879');
    sf.HideOptions('3879', '1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,24');
  } else if (a == '24') {
    sf.ShowOptionsAll('3879');
    sf.HideOptions('3879', '1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23');
  }

  else {
    sf.ShowOptionsAll('3879');
  }  



} */


function OnEntry_3880() {

   var a = sf.GetValue('2842');

  var allValues=a.split(',');


  $.each(allValues,function(index,value) {

  //  if (flag) {

      if (value!='3') {
        sf.HideQuestions('3880');
     //   sf.HideAllQuestions('3880',2);
       // flag=false;
      }

      else {
        sf.ShowQuestions('3880');
        return false;
      }
    }); 

  

    var a = sf.GetValue('2843');

  for (var i = 1; i <= 24; i++) {
    if (a == i) {
    //sf.HideOption('Q5', 'Q5_1');
    sf.HideOptionsAll('3880');
    sf.ShowOptions('3880', i+',');
    return false;
   
  }  else {
      sf.ShowOptionsAll('3880');
    } 
  }

  
}



function OnEntry_2855() {


  var a = sf.GetValue('2843');
  sf.HideOptionsAll('2855');


  if (a == '1') {
    //sf.HideOption('Q5', 'Q5_1');
    sf.ShowOptions('2855', '1,23,13');

   
  } else if (a == '2') {
    sf.ShowOptions('2855', '4,19,35,36,37,38');
  

  } else if (a == '5') {
    sf.ShowOptions('2855', '3');
  

  } else if (a == '6') {
    sf.ShowOptions('2855', '5');
  

  } else if (a == '7') {
    sf.ShowOptions('2855', '15,17');
  

  } else if (a == '8') {
    sf.ShowOptions('2855', '2');
  

  } else if (a == '9') {
    sf.ShowOptions('2855', '11,14,16');
  

  } else if (a == '10') {
    sf.ShowOptions('2855', '12,13,28');
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
    sf.ShowOptions('2855', '22,26,27');
  } else if (a == '17') {
    sf.ShowOptions('2855', '9');
  }  else if (a == '18') {
    sf.ShowOptions('2855', '6');
  } else if (a == '19') {
    sf.ShowOptions('2855', '8,20,32');
  }  else if (a == '20') {
    sf.ShowOptions('2855', '37,40,38');
  } else if (a == '21') {
    sf.ShowOptions('2855', '33');
  }  else if (a == '22') {
    sf.ShowOptions('2855', '29');
  } else if (a == '23') {
    sf.ShowOptions('2855', '31,34');
  } else if (a == '24') {
    sf.ShowOptions('2855', '30');
  }  /*else if (a == '23') {
    sf.HideOptionsAll('2855');
    sf.HideOptions('2855', '1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40');
  } else if (a == '24') {
    sf.ShowOptionsAll('2855');
    sf.HideOptions('2855', '1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23');
  } */

  else {
    sf.ShowOptionsAll('2855');
  }  

  

}


/*Question Hide*/
/*
function OnBack_2843() {
  sf.BackQuestions('2843');
}

function OnBack_3879() {
  sf.BackQuestions('3879');
}

function OnBack_3880() {
  sf.BackQuestions('3879');
} */

function OnEntry_2843() {

  var a = sf.GetValue('2842');

  var allValues=a.split(',');


     // sf.HideAllQuestions('2842',3);
     
   //   sf.HideQuestions('3879');  
  
   //   sf.HideQuestions('3880');
  

 // sf.HideQuestions('2843');
 // sf.HideQuestions('3879');
 // sf.HideQuestions('3880');

 //sf.HideQuestions('2843');

  $.each(allValues,function(index,value) {

  //  if (flag) {

    if (value!='1') {
        sf.HideQuestions('2843');
     //   sf.HideAllQuestions('3879',2);
       // flag=false;
      }

      else {
        sf.ShowQuestions('2843');
        return false;
      }

     

    


      /*
      if (value==2) {
        sf.ShowQuestions('3879');
        flag=false;
      }

    

      if (value==3) {
        sf.ShowQuestions('3880');
        flag=false;
      } */

 //   }

  
  });


 /*   if (!($.inArray('1',allValues)>-1)) {
    //  sf.ShowQuestions('2843');
      sf.HideQuestions('2843');
    }


    
    else {
      sf.ShowQuestions('2843');
    } 

      if (!($.inArray('2',allValues)>-1)) {
        sf.HideQuestions('3879');
      }

      
      else {
        sf.ShowQuestions('3879');
      } 

      if (!($.inArray('3',allValues)>-1)) {
        sf.HideQuestions('3880');
      }

      
      else {
        sf.ShowQuestions('3880');
      } 
   */ 

  /*  else {
      sf.HideQuestions('2843');
    }
*/
    /*
    var chkOperation=false;
    if (value==1) {
        chkOperation=1;
      sf.ShowQuestions('2843');
    }

    else {

      sf.HideQuestions('2843');
    }
  */

    /*
    if (value==2) {
      sf.ShowQuestions('3879');
    }

    if (value==3) {
      sf.ShowQuestions('3880');
    }
    */

//  });

 /* if (a == '1') {
    //sf.HideOption('Q5', 'Q5_1');
   // sf.ShowQuestionsAll('2843');
    sf.HideQuestions('2843');
    sf.HideQuestions('2844');
  } else if (a == '2') {
    sf.ShowQuestionsAll('2843');
    sf.HideQuestions('2843', '1,2');
  }

  else {
    sf.ShowQuestionsAll('2843');
  }  
*/


}

/*
function OnEntry_3879() {

  var a = sf.GetValue('2842');

  var allValues=a.split(',');


   //   sf.HideAllQuestions('2843',2);
     
   //   sf.HideQuestions('3879');  
  
   //   sf.HideQuestions('3880');
  

 // sf.HideQuestions('2843');
 // sf.HideQuestions('3879');
 // sf.HideQuestions('3880');

 var flag=true;

  $.each(allValues,function(index,value) {

    

      if (value==2) {
        sf.ShowQuestions('3879');
   //     sf.HideAllQuestions('3880',1);
        //flag=false;
      }   

  
  });



}
*/

/*
function OnEntry_3879() {

  var a = sf.GetValue('2842');

  var allValues=a.split(',');


  if (!$.inArray('2',allValues)!=-1) {
      sf.HideQuestions('3879');
    }

    else {
      sf.HideQuestions('3879');
    } 
}


function OnEntry_3880() {

  var a = sf.GetValue('2842');

  var allValues=a.split(',');


  if (!$.inArray('3',allValues)!=-1) {
      sf.HideQuestions('3880');
    }

    else {
      sf.HideQuestions('3880');
    }

    
} */


/*
function OnEntry_2842() {

   var a = sf.GetValue('2842');

  var allValues=a.split(',');

  $.each(allValues,function() {
    if ($.inArray('4',allValues)!=-1) {
      this.type='NONE';
    }
  });

  
   
} */