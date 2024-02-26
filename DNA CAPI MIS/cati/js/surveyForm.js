function surveyForm(jsonData) {
  this.qno = 1;    //Current question being displayed. 1 based index
  this.formData = JSON.parse(jsonData);
  this.formControls = new Array();        //Form objects
  this.formView = '';
  this.connectedFormControls=new Array();
  this.connectedFormView='';                      //Rendered HTML of all elements
  this.SectionName='';
  this.sectionView='';
  this.sectionViewsArr=new Array();
  this.sectNo=1;
  this.sectionWithControls=new Array();
}





surveyForm.prototype.build = function() 
{
//  varFormControls = new Array();        //Form objects
varFormControls = new Array();        //Form objects


  varFormView = '';


  varConnectedFormControls = new Array();        //Form objects
  varConnectedFormView = '';

  varSectionsControl=new Array();


  varSectionWithControl=new Array();


 // console.log(this.formData);
 var fno=0;
 var CQno=0;

  $.each(this.formData, function(key1, valueSect) {

    varSectionWithControl[key1]=new Array();

    varSectionsControl[key1]='';

    if(valueSect['SectionName']!='undefined') {


   //   var currentSectIter=valueSect['SectionName'];
   //   currentSectIter=currentSectIter.replace(/\s/g, '');
     // varFormControls[key1]=new Array();


      $.each(valueSect['Questions'], function(key, value) {



      if (!((typeof value['Visible']!='undefined') && (value['Visible']==false || value['Visible']=='false' || value['Visible']=='False'))) {

      //    var fno = key+1;
        fno++;

       // varFormControls[key1]=new Array();

        if(value['FieldType']=='SLT' || value['FieldType']=='phone' || value['FieldType']=='email') {

        //  varFormControls[key1][fno] = new textboxControls(fno, value['Id'], value['FieldName']);

        //  varFormControls[key1][fno] = new stopwatchControls(fno, value['Id'], value['FieldName']);
         
           varFormControls[fno] = new textboxControls(fno, value['Id'], value['FieldName'],true);      
           
        }

        if(value['FieldType']=='XXX' || value['FieldType']=='') {
           varFormControls[fno] = new bodytext(fno, value['Id'], value['FieldName']); 


          /*
           if ((typeof value['Visible']!='undefined') && value['Visible']==false) {
              varFormControls[fno].visible=false;
           }
           */
                
        }

       /*   if(value['FieldType']=='SRO') {
              varFormControls[fno] = new soundrecordControls(fno, value['Id'], value['FieldName'],value['EndOnField'],value['UI'],value['AutoStart']);                
          }
        */

        if(value['FieldType']=='CTR') {
           varFormControls[fno] = new numberCounterControls(fno, value['Id'], value['FieldName']);
        }


        if(value['FieldType']=='RGS') {
          varFormControls[fno] = new rangeSliderControl(fno, value['Id'], value['FieldName'],1,3);
        }

        if(value['FieldType']=='DAT') {
          varFormControls[fno] = new datepickerControls(fno, value['Id'], value['FieldName']);
        }

        if (value['FieldType']=='NUM') {
          varFormControls[fno] = new integerTextboxControls(fno, value['Id'], value['FieldName']);      
        }

        if (value['FieldType']=='PIC') {
          varFormControls[fno] = new textboxControls(fno, value['Id'], value['FieldName']);
          varFormControls[fno].type="file";
        }

        if(value['FieldType']=='RDO') {
          varFormControls[fno] = new radioboxControls(fno, value['Id'], value['FieldName'], value['Options']);

          if ((typeof value['IncludeField']!='undefined') && value['IncludeField']==false) {
              varFormControls[fno].visible=false;
          }
          
        }

        if(value['FieldType']=='STW') {
          varFormControls[fno] = new stopwatchControls(fno, value['Id'], value['FieldName']);
        }

        if(value['FieldType']=='DDN') {
          varFormControls[fno] = new dropdownControls(fno, value['Id'], value['FieldName'], value['Options']);
        }

        if(value['FieldType']=='SCD') {
          varFormControls[fno] = new singlechoiceGridControl(fno, value['Id'], value['FieldName'], value['Questions'],value['Options']);
        }


        if(value['FieldType']=='SCG') {
          varFormControls[fno] = new singlechoiceGridRadioControl(fno, value['Id'], value['FieldName'], value['Questions'],value['Options']);
        }
      
        if(value['FieldType']=='MCG') {
          varFormControls[fno] = new multichoiceGridCheckboxControl(fno, value['Id'], value['FieldName'], value['Questions'],value['Options']);
        }

        if(value['FieldType']=='MLT') {
          varFormControls[fno] = new openendedTextControl(fno, value['Id'], value['FieldName'], value['Questions']);
        }

        if(value['FieldType']=='ONG') {
          varFormControls[fno] = new openendedIntegerControl(fno, value['Id'], value['FieldName'], value['Questions']);
        }


        if(value['FieldType']=='CHK') {
          varFormControls[fno] = new checkboxControls(fno, value['Id'], value['FieldName'], value['Options']);
        //  if (true) {};
        } 


        if (value['FieldType']=='XXX' || value['FieldType']=='') {
          varFormControls[fno].mandatory=false;
        }

        else if ((value['FieldType']=='RDO' || value['FieldType']=='CHK') && (typeof value['Options']=='undefined' || value['Options']==null)) {
          varFormControls[fno].mandatory=false;
        }
      

        else if ((typeof value['Mandatory']!='undefined') && value['Mandatory']!=null && (value['Mandatory']==true || value['Mandatory']=='TRUE' || value['Mandatory']=='true' || value['Mandatory']=='True')) {
          varFormControls[fno].mandatory=true;
        }    

        else {
          varFormControls[fno].mandatory=false;
        }
        
        varFormView += varFormControls[fno].header(fno) + varFormControls[fno].display(fno) + varFormControls[fno].footer(fno);

        varSectionWithControl[key1][key]=fno;

      //  varFormView += varFormControls[fno].header(key) + varFormControls[key1+key].display(key) + varFormControls[key1+key].footer(key);

      //  varSectionsControl[key1]+= varFormControls[key1+key].header(key) + varFormControls[key1+key].display(key) + varFormControls[key1+key].footer(key);

      }

      else {
        CQno++;

        

          

          if(value['FieldType']=='SLT' || value['FieldType']=='phone' || value['FieldType']=='email') {

          //  varFormControls[key1][fno] = new textboxControls(fno, value['Id'], value['FieldName']);

          //  varFormControls[key1][fno] = new stopwatchControls(fno, value['Id'], value['FieldName']);
           
             varConnectedFormControls[CQno] = new textboxControls(CQno, value['Id'], value['FieldName'],false);      
             
          }


          if(value['FieldType']=='SRO') {
              varConnectedFormControls[CQno] = new soundrecordControls(CQno, value['Id'], value['FieldName'],value['EndOnField'],value['UI'],value['AutoStart']);                
          }


          if(value['FieldType']=='XXX' || value['FieldType']=='') {
             varConnectedFormControls[CQno] = new bodytext(CQno, value['Id'], value['FieldName']); 

            /*
             if ((typeof value['Visible']!='undefined') && value['Visible']==false) {
                varConnectedFormControls[CQno].visible=false;
             }
             */
                  
          }

          if(value['FieldType']=='CTR') {
             varConnectedFormControls[CQno] = new numberCounterControls(CQno, value['Id'], value['FieldName']);
          }


          if(value['FieldType']=='RGS') {
            varConnectedFormControls[CQno] = new rangeSliderControl(CQno, value['Id'], value['FieldName'],1,3);
          }

          if(value['FieldType']=='DAT') {
            varConnectedFormControls[CQno] = new datepickerControls(CQno, value['Id'], value['FieldName']);
          }

          if (value['FieldType']=='NUM') {
            varConnectedFormControls[CQno] = new integerTextboxControls(CQno, value['Id'], value['FieldName']);      
          }

          if (value['FieldType']=='PIC') {
            varConnectedFormControls[CQno] = new textboxControls(CQno, value['Id'], value['FieldName']);
            varConnectedFormControls[CQno].type="file";
          }

          if(value['FieldType']=='RDO') {
            varConnectedFormControls[CQno] = new radioboxControls(CQno, value['Id'], value['FieldName'], value['Options']);

            if ((typeof value['IncludeField']!='undefined') && value['IncludeField']==false) {
                varConnectedFormControls[CQno].visible=false;
            }
            
          }

          if(value['FieldType']=='STW') {
            varConnectedFormControls[CQno] = new stopwatchControls(CQno, value['Id'], value['FieldName']);
          }

          if(value['FieldType']=='DDN') {
            varConnectedFormControls[CQno] = new dropdownControls(CQno, value['Id'], value['FieldName'], value['Options']);
          }

          if(value['FieldType']=='SCD') {
            varConnectedFormControls[CQno] = new singlechoiceGridControl(CQno, value['Id'], value['FieldName'], value['Questions'],value['Options']);
          }


          if(value['FieldType']=='SCG') {
            varConnectedFormControls[CQno] = new singlechoiceGridRadioControl(CQno, value['Id'], value['FieldName'], value['Questions'],value['Options']);
          }
        
          if(value['FieldType']=='MCG') {
            varConnectedFormControls[CQno] = new multichoiceGridCheckboxControl(CQno, value['Id'], value['FieldName'], value['Questions'],value['Options']);
          }

          if(value['FieldType']=='MLT') {
            varConnectedFormControls[CQno] = new openendedTextControl(CQno, value['Id'], value['FieldName'], value['Questions']);
          }

          if(value['FieldType']=='ONG') {
            varConnectedFormControls[CQno] = new openendedIntegerControl(CQno, value['Id'], value['FieldName'], value['Questions']);
          }


          if(value['FieldType']=='CHK') {
            varConnectedFormControls[CQno] = new checkboxControls(CQno, value['Id'], value['FieldName'], value['Options']);
          //  if (true) {};
          } 

          varConnectedFormView += varConnectedFormControls[CQno].header(CQno) + varConnectedFormControls[CQno].display(CQno) + varConnectedFormControls[CQno].footer(CQno);
        

      }
       

    

    });

  

   // varSectionsControl[key1]=valueSect['SectionName'];
   // varSectionsControl[key1]=varFormView;
}

});


  this.sectionWithControls=varSectionWithControl;

  this.formControls = varFormControls;
  this.formView = varFormView;


  this.connectedFormControls = varConnectedFormControls;
  this.connectedFormView = varConnectedFormView;

 // this.SectionName=varSectionsControl;

  this.sectionViewsArr=varSectionsControl;

  

}

surveyForm.prototype.setPage = function(pageNo) {
  $('.counter_text').text(pageNo + ' of ' + this.formData.length);
  this.qno = pageNo;
}


/*
surveyForm.prototype.NextPage = function() {

  //Validate current page
  if (this.validate(this.qno) && varFormControls[this.qno].onValidate()) {
    if (this.qno <= (this.formControls.length-2)) {

      var jumpQ = varFormControls[this.qno].onExit();
      varFormControls[this.qno].nextqno = jumpQ;
      var currentQ = this.qno;

      this.qno += jumpQ;

      this.refreshPage();

      varFormControls[this.qno].prevqno = currentQ;
    }

  //  else {
      this.saveData();
  //  }
  }   
}
*/



surveyForm.prototype.getSectionQno = function(sectNo) {

 // var currentSectionName=this.getSectionName(sectNo);

 var sectionQno=new Array();

  for (var i = 0; i < this.sectionWithControls[sectNo-1].length; i++) {
    //  if (index>0) {
      sectionQno[i]=this.sectionWithControls[sectNo-1][i];
        
  }

  return sectionQno;
}

surveyForm.prototype.NextPage = function() {


  //      this.sectionView=this.sectionViewsArr[this.sectNo-1];

//   $('.counter_text').html('<span>' + this.sectNo + '</span> of ' + (this.formData.length-1));

 this.getSectionQno(1);

  //Validate current page
  this.qno=1;
 
  var flag=true;

 // for (var i = 0; i < varFormControls.length; i++) {
  $.each(this.getSectionQno(this.sectNo),function(index,value) { 
    if (index>0) {

       $('.err_msg_field').remove();

      if (sf.validate(value) && varFormControls[value].onValidate()) {
      //  $('#field_'+varFormControls[value].id+' p').text('');
     //  $('#field_'+varFormControls[value].id).after('label').html('');
      //  $('').insertAfter('#field_'+varFormControls[value].id+' label').first();
  //    $('#field_'+varFormControls[value].id+' .err_msg_field').hide();
     //   $('#field_'+varFormControls[value].id+' #err_msg_field').hide();
            $('#field_'+varFormControls[value].id+' label').first().append('');

      }

      else {
        //  $('<div class="alert alert-danger err_msg_field" role="alert"><span class="glyphicon glyphicon-exclamation-sign" aria-hidden="true"></span><span class="sr-only">Error:</span>This field is required.!</div>').insertAfter('#field_'+varFormControls[value].id+' label').first();

        $('#field_'+varFormControls[value].id+' label').first().append('<div class="alert alert-danger err_msg_field" role="alert"><span class="glyphicon glyphicon-exclamation-sign" aria-hidden="true"></span><span class="sr-only">Error:</span>This field is required.!</div>');

      //   $('#field_'+varFormControls[value].id+' p').text('this field is required');
     //    $('#field_'+varFormControls[value].id+' #err_msg_field').show();

        flag=false;
        return false;
      }
    }
  });


  if (flag) {  
    if (this.sectNo<this.formData.length) {  
      this.sectNo=this.sectNo+1;
      this.refreshPage();   
    } 

    else {
      this.saveData();
    }

  }


 



}

surveyForm.prototype.showError = function(qno) {
  $('.err_msg').text('');
  var id=varFormControls[this.qno].id;
  $('#field_'+id+' p').text('This field is required.');
}



surveyForm.prototype.PrevPage = function() {

  if (this.sectNo > 1) {
     /* if (varFormControls[this.qno].prevqno != null) {
        var prevQ = varFormControls[this.sectNo].prevqno;
        this.sectNo = prevQ;
      }
      else {
    */    this.sectNo--;
    //  }

      this.refreshPage();
    }
   /* if (this.qno > 1) {
      if (varFormControls[this.qno].prevqno != null) {
        var prevQ = varFormControls[this.qno].prevqno;
        this.qno = prevQ;
      }
      else {
        this.qno--;
      }

      this.refreshPage();
    } */
}



surveyForm.prototype.GetValue = function(id) {

    var matchedValue='';

    $.each(this.formControls,function (index, control) {

      if (index>0) {

        if (id == control.id) {
          matchedValue = control.value;
        }
      }      
    //  console.log(values);
    /*  if (index==2) {
        return false;
      }
      */
      
    });

    return matchedValue;
 
  //  return $('#'+id).val();
}

surveyForm.prototype.Getqno = function(id) {

    var matchedValue='';

    $.each(this.formControls,function (index, control) {

      if (index>0) {

        if (id == control.id) {
          matchedValue = control.qno;
        }
      }      
    //  console.log(values);
    /*  if (index==2) {
        return false;
      }
      */
      
    });

    return matchedValue;
 
  //  return $('#'+id).val();
}

surveyForm.prototype.HideOptions = function(id, optionsToHide) {

    $.each(this.formControls, function (index, control) {

      if (index > 0) {

        if (id == control.id) {
          control.HideOptions(optionsToHide);
        }
      }      
    });


}

surveyForm.prototype.ShowOptions = function(id, optionsToHide) {

    $.each(this.formControls, function (index, control) {

      if (index > 0) {

        if (id == control.id) {
          control.ShowOptions(optionsToHide);
        }
      }      
    });


}

surveyForm.prototype.ShowOptionsAll = function(id) {
  $.each(this.formControls, function (index, control) {

      if (index > 0) {

        if (id == control.id) {
          control.ShowOptionsAll();
        }
      }      
    });
}


surveyForm.prototype.HideOptionsAll = function(id) {
  $.each(this.formControls, function (index, control) {

      if (index > 0) {

        if (id == control.id) {
          control.HideOptionsAll();
        }
      }      
    });
}






surveyForm.prototype.HideAllQuestions = function(id,iter) {



    var pageMove=this.qno;
    var formPointer=this.formControls;

    $.each(this.formControls, function (index, control) {

      if (index > 0) {

        if (id == control.id) {
         // this.qno=control.qno+1;
          pageMove=pageMove+iter;
        // sf.setPage(control.qno);
        //  control.HideQuestions(id);
        }
      }      
    });

    this.qno=pageMove;
   // sf.refreshPage();

}


surveyForm.prototype.number_validation = function(id,min,max) {

  var formPointer=this.formControls;

    $.each(this.formControls, function (index, control) {

      if (index > 0) {

        if (id == control.id) {
         // this.qno=control.qno+1;
        //  pageMove=pageMove-1;
        // sf.setPage(control.qno);
          control.number_validation(min,max);
        //pageMove=control.qno;
        }
      }      
    });
}


surveyForm.prototype.ShowQuestions = function(id) {

   // var prevqqnnoo=this.Getqno(pqno);
 //  var prevqqnnoo=pqno;

    var pageMove=this.qno;
    var formPointer=this.formControls;



    $.each(this.formControls, function (index, control) {

      if (index > 0) {

        if (id == control.id) {
         // this.qno=control.qno+1;
        //  pageMove=pageMove-1;
        // sf.setPage(control.qno);
        //  control.HideQuestions(id);
 //        varFormControls[pageMove].nextqno=control.qno;
//         varFormControls[control.qno].prevqno=prevqqnnoo;




        pageMove=control.qno;
        }
      }      
    });



    this.qno=pageMove;
   // sf.refreshPage();

}


surveyForm.prototype.alert = function(msg) {
  alert(msg);
}

surveyForm.prototype.showError = function(id,msg) {

  $('.err_msg_field').remove();

  $('#field_'+id+' label').first().append('<div class="alert alert-danger err_msg_field" role="alert"><span class="glyphicon glyphicon-exclamation-sign" aria-hidden="true"></span><span class="sr-only">Error:</span>'+msg+'</div>');

 // alert(msg);
}


surveyForm.prototype.BackQuestions = function(id) {



    var pageMove=this.qno;
    var formPointer=this.formControls;

    $.each(this.formControls, function (index, control) {

      if (index > 0) {

        if (id == control.id) {
         // this.qno=control.qno+1;
          pageMove=pageMove-1;
        // sf.setPage(control.qno);
        //  control.HideQuestions(id);
        }
      }      
    });

    this.qno=pageMove;
   // sf.refreshPage();

}


surveyForm.prototype.hideQuestion = function(id) {


    var currentQno=this.Getqno(id);

    this.qno=currentQno+1;

    this.refreshPage();    

}

surveyForm.prototype.setRecordingValue = function(id,filename,path) {


    var pageMove=this.qno;
    var formPointer=this.connectedFormControls;

    $.each(formPointer, function (index, control) {

      if (index > 0) {

        if (id == control.id) {
         // this.qno=control.qno+1;
          control.value=filename;
        // sf.setPage(control.qno);
        //  control.HideQuestions(id);
        }
      }      
    });

   

}


surveyForm.prototype.onEntryFire = function() {



  for (var i = 0; i < varFormControls[this.sectNo-1].length; i++) {
 //  console.log(varFormControls[this.sectNo-1][i].id);
   varFormControls[this.sectNo-1][i].onEntry();
   // Things[i]
  }

 // console.log(varFormControls[1].length);
/*
  varFormControls[this.sectNo].each(function( index ) {
 // console.log(this.value);
});
*/
/*  var sectEntryId=this.sectNo;

  $.each(varFormControls, function(key, value) {

     console.log(value[sectEntryId].id);
    //  value.onEntry();
  });
*/

}



surveyForm.prototype.refreshPage = function() {




//this.onEntryFire();


//console.log(this.sectionWithControls);


    $.each(this.getSectionQno(this.sectNo), function (index, value) {
        try {
            //  if (index>0) {
            if (typeof (value) != "undefined" && typeof (varFormControls[value]) != "undefined") {
                varFormControls[value].onEntry();
            }
            //  }
        }
        catch (err) {
            sf.alert("On refreshPage.getSectionQno: " + this.sectNo + ", " + index + ", " + value + ".\n\r" + err.stack);
        }
    });


 // varFormControls[this.sectNo].onEntry();



  //$(".formControl input[type='checkbox']").on('change', checkboxControls.prototype.onChange);
 /* var totalLength=this.formControls.length;

  if (this.qno==totalLength) {
    this.saveData();
  }

  else {
  */ 
 // varFormControls[this.qno].value=$('#'+'field_'+varFormControls[this.qno].id+'_text').val();
 // this.value=$()

 //   this.sectionView=this.sectionViewsArr[this.sectNo-1];

  //  this.sectNo

//   $('.counter_text').html('<span>' + this.qno + '</span> of ' + (this.formData.length-1));

 //  var currentSectionName=this.getSectionName(this.sectNo);

   $('.counter_text').html('<span>' + this.sectNo + '</span> of ' + (this.formData.length));

  // $('#fData').html(this.formView);

  // this.qno=1;

    $('.formControl').hide();

    $.each(this.getSectionQno(this.sectNo), function (index, value) {
        try {
            //  if (index>0) {
            if (typeof (value) != "undefined" && typeof (varFormControls[value]) != "undefined") {
                var name = varFormControls[value].id;
                $('.formControl#field_' + name).show();
            }
            //  }
        }
        catch (err) {
            sf.alert("On refreshPage.getSectionQno2: " + this.sectNo + ", " + index + ", " + value + ".\n\r" + err.stack);
        }
    });
  //  }
   
    
  
  
    //console.log(name);

  $('#section_name').text(this.getSectionNameHeading(this.sectNo));
    
//  }

 
}

//console.log(this.getSectionName('1'));

surveyForm.prototype.getSectionNameHeading = function(i) {

  //$.each(this.formData,function(index,value) {

  var currentSectIter=this.formData[i-1]['SectionName'];
 // currentSectIter=currentSectIter.replace(/\s/g, '');

  return currentSectIter;

  //  console.log(this.formData[i-1]['SectionName']);
 // });

//  console.log(this.formData.length);

//  console.log(this.sectNo);

}

surveyForm.prototype.getSectionName = function(i) {

  //$.each(this.formData,function(index,value) {

  var currentSectIter=this.formData[i-1]['SectionName'];
  currentSectIter=currentSectIter.replace(/\s/g, '');

  return currentSectIter;

  //  console.log(this.formData[i-1]['SectionName']);
 // });

//  console.log(this.formData.length);

//  console.log(this.sectNo);

}

surveyForm.prototype.validate = function(index) {
  if(varFormControls[index].mandatory && (varFormControls[index].value==null || varFormControls[index].value=='')) {
    //  try {
      //  android.alert('Please input values.');
        return false;
   /*   } catch (e) {
      } 
   */  // return false;
  }
  return true;  
  
}


/*
surveyForm.prototype.goto = function(index) {
  this.gno=3;
}
*/

/*

android.getFormData(this.savePartialFormData());



surveyForm.prototype.savePartialFormData = function() {

  var finalData=[];

for (var i = 0; i <= varFormControls.length-2; i++) {
    finalData[i]=new Object(); 
    finalData[i].id=varFormControls[i+1].id;
    finalData[i].value=varFormControls[i+1].value;
};




 var save_final_data=JSON.stringify(finalData);
// console.log();

  return save_final_data;

}

*/



surveyForm.prototype.saveData = function() {

  var finalData=[];

  var conntectedFormBeginPointer=0;

for (var i = 0; i <= varFormControls.length-2; i++) {
    finalData[i]=new Object(); 
    finalData[i].id=varFormControls[i+1].id;
    finalData[i].value=varFormControls[i+1].value;

};


for (var i = 1; i < varConnectedFormControls.length; i++) {
    finalData[varFormControls.length+(i-2)]=new Object(); 
    finalData[varFormControls.length+(i-2)].id=varConnectedFormControls[i].id;
    finalData[varFormControls.length+(i-2)].value=varConnectedFormControls[i].value;
};



//console.log(finalData);

/*
for (var i = 0; i < this.formData.length; i++) {
  //getSectionName
  for (var j = 0; j < finalData.length; j++) {
    finalData[j].id=finalData[j].id.replace(this.getSectionName(i+1),'');
  }
   // replaceString = replaceString.replace(find[i], replace[i]);
  }
*/
//console.log(finalData);


/*
  $.each(varFormControls,function(index,value) {
    finalData[index+1]=[]; 
    finalData[index+1]['id']=value.id;
    finalData[index+1]['value']=value.value;
  });
*/

 var save_final_data=JSON.stringify(finalData);
// console.log();

  console.log(save_final_data);
  android.setData(save_final_data);

}

surveyForm.prototype.saveDatajq = function() {
//  console.log(this.value);

var data_textbox=[];
//var data_textbox[]=[];
var data_checkbox=[];
var data_radiobox=[];

//ar abcd_json='';

$.each($(".formControl input[type='text']"),function(index,value) {
  data_textbox[index]=new Object();
  data_textbox[index].fieldType='text';
  data_textbox[index].Value=this.value;

 //abcd_json+=JSON.stringify(data_textbox);

});

//console.log(abcd_json);

var checkBoxCVS='';
var chkboxName='';

$.each($(".formControl div input[type='checkbox']:checked"),function(index,value) {
  
  chkboxName=this.name;
 
  data_checkbox[index]=new Object();
  data_checkbox[index].fieldType=this.name;
  checkBoxCVS+=this.value+',';
  data_checkbox[index].Value=checkBoxCVS;
 // console.log(value);
});


$.each($(".formControl input[type='radio']:checked"),function(index,value) {

  data_radiobox[index]=new Object();
  data_radiobox[index].fieldType='radiobox';
  data_radiobox[index].Value=this.value+',';
});



//data_textbox.push(data_fields);
var finalData=[];

finalData.push(data_textbox);
finalData.push(data_radiobox);
finalData.push(data_checkbox);


console.log('start-------------');

console.log(finalData);

var abec=JSON.stringify(finalData);

console.log(abec);

 }






surveyForm.prototype.saveDataArray = function() {
//  console.log(this.value);

var data_textbox=[];
//var data_textbox[]=[];
var data_checkbox=[];
var data_radiobox=[];



$.each($(".formControl input[type='text']"),function(index,value) {
  data_textbox[index]=[];
  data_textbox[index]['fieldType']='text';
  data_textbox[index]['Value']=this.value;

var abcd_json=JSON.stringify(data_textbox);

});

$.each($(".formControl input[type='checkbox']:checked"),function(index,value) {
  data_checkbox[index]=[];
  data_checkbox[index]['fieldType']='checkbox';
  data_checkbox[index]['Value']=this.value;
 // console.log(value);
});


$.each($(".formControl input[type='radio']:checked"),function(index,value) {

  data_radiobox[index]=[];
  data_radiobox[index]['fieldType']='radiobox';
  data_radiobox[index]['Value']=this.value;
});



//data_textbox.push(data_fields);
var finalData=[];

finalData.push(data_textbox);
finalData.push(data_radiobox);
finalData.push(data_checkbox);


console.log('start-------------');

console.log(finalData);

var abec=JSON.stringify(finalData);

console.log(abec);

/*console.log(data_textbox);

console.log('now checkbox');

console.log(data_checkbox);

console.log('now radio');

console.log(data_radiobox);

*/
/*$.each(data_textbox,function(index,value) {

  console.log(value);  
});
*/


//  console.log($(".formControl input").val());

/*  this.formControls.length-1

  for (var i = 0; i < this.formControls.length-1; i++) {
    console.log(this.value);
    this.formControls.length-1;
*/}



/*
surveyForm.prototype.setNone = function(index) {

  if(varFormControls[index].type=="checkbox" && varFormControls[index].noneSelection) {
    varFormControls[index].noneSelection=true;
  }

}
*/

//setNone();




surveyForm.prototype.updateValues = function(updateValues) 
{

  var updatedValues=JSON.parse(updateValues);

  $.each(updatedValues, function(key, value) {
    sf.searchField(value.id,value.value);
  //  var qno=sf.Getqno(value.id);
  //  $('input[name="field_'+value.id+'"]').val(value.value);
  //  varFormControls[qno].value=value.value;
  });

  sf.refreshPage();
}


surveyForm.prototype.searchField = function(id,value) 
{
  //  varFormControls[key1][fno].value;
  $.each(this.formControls, function(key, control) {
    
    if (key>0) {
      if (id==control.id && control.type!="bodyText") {
        varFormControls[control.qno].setValues(value);
        varFormControls[control.qno].value=value;
      }
    }
  });
}