function multiSelectImage(fno, id, title, BeforeImage, BeforeImageRows, BeforeImageColumns, BeforeImageTimeout, BeforeImageShuffle, AfterImage, AfterImageRows, AfterImageColumns, AfterImageShuffle, Images, ImagesToSelect) {
    formControl.call(this, fno, id, title);
    this.type = "IRT";
 //   this.controlType=controlType;
    this.OnValidateScript = "";
    
	this.BeforeImage = BeforeImage;
	this.BeforeImageRows = BeforeImageRows;
	this.BeforeImageColumns = BeforeImageColumns;
	this.BeforeImageTimeout = BeforeImageTimeout;
    this.BeforeImageShuffle = BeforeImageShuffle;
    this.AfterImage = AfterImage;
	this.AfterImageRows = AfterImageRows;
	this.AfterImageColumns = AfterImageColumns;
    this.AfterImageShuffle = AfterImageShuffle;
    this.ImagesToSelect = ImagesToSelect;
    this.Images = Images;
    this.imagesArr=null;
    this.displayMode = "before";
	
	this.imagesBefore = new Array();
	this.imagesAfter = new Array();
	this.imagesPreloaded = new Array();
}

multiSelectImage.prototype = Object.create(formControl.prototype); 
multiSelectImage.prototype.constructor = multiSelectImage;

multiSelectImage.prototype.PutinSlot = function(arr, item) {
	var emptySlot = false;
	for (var i = 0; i < arr.length; i++) {
		if (arr[i] == null) { 
			emptySlot = true;
			break;
		}
	}
	if (emptySlot) {
		do {
			var s = Math.floor(Math.random() * arr.length);
			if (arr[s] == null) {
				arr[s] = item;
				break;
			}
		} while(true);
	}
}

multiSelectImage.prototype.display = function() {
    var field = this.displayLabel();

    var iname = this.name;
    var qno = this.qno;
    var iid = this.id;

    var cType=this.controlType;
      	
   	var ctrlId = 'field_'+this.id+'_image';

	field += '<div class="row" id="' + 'viewButton_' + this.id + '" style="margin: 10px 0px 0px 0px;"><button type="button" class="btn btn-default btn-lg btn-info" onclick="multiSelectImage.prototype.ViewBeforeImage(' + this.qno + ')">Click here to View</button></div>';
	
	field += '<div class="row" id="' + ctrlId + '" style="display:none"><div class="prnt-wrapper"><div class="shelf-wrapper"><div class="col-lg-12 shelf-bordr" style="background-image: url(' + sf.SurveyMediaUrl + this.BeforeImage + ')">';

	var afterImage=this.AfterImage;
	var beforeImageTimeout=this.BeforeImageTimeout;
	if (afterImage != null && afterImage.length > 0) {
	    this.PreLoad(sf.SurveyMediaUrl + this.AfterImage);
	}
	
	//multiSelectImage.prototype.afterShelf(afterImage,beforeImageTimeout);


	var i = 0;
	var imagesArr = new Array();
	var imagesAfter = this.imagesAfter;
	var imagesBefore = this.imagesBefore;
	
	var slotsBeforeImage = 	this.BeforeImageRows * this.BeforeImageColumns;
	for (i = 0; i < slotsBeforeImage; i++) {
		imagesBefore[i] = null;
	}
	var slotsAfterImage = 	this.AfterImageRows * this.AfterImageColumns;
	for (i = 0; i < slotsAfterImage; i++) {
		imagesAfter[i] = null;
	}

	if (this.Images) {
	    $.each(this.Images, function (key, value) {
	        if (typeof (imagesArr[value["row"]]) == "undefined") {
	            imagesArr[value["row"]] = new Array();
	        }
	        imagesArr[value["row"]][value["column"]] = new Object();
	        imagesArr[value["row"]][value["column"]].imgSrc = value["imageURL"];
	        imagesArr[value["row"]][value["column"]].imgId = value["imageID"];
	        imagesArr[value["row"]][value["column"]].BeforeImage = value["BeforeImage"];
	        imagesArr[value["row"]][value["column"]].AfterImage = value["AfterImage"];
	        //	field+='<div name="'+iname+'" id="'+value['imageID']+'" class="col-lg-1 col-xs-1 col-sm-1 col-md-1"><img src="'+value['imageURL']+'" onClick="multiSelectImage.prototype.setImage(this,'+fieldName+','+iid+')"></div>';

	        if (value.BeforeImage == "True") {
	            multiSelectImage.prototype.PutinSlot(imagesBefore, imagesArr[value["row"]][value["column"]]);
	        }
	        if (value.AfterImage == "True") {
	            multiSelectImage.prototype.PutinSlot(imagesAfter, imagesArr[value["row"]][value["column"]]);
	        }
	    });
	}

	field+="<div class='shuffleImg'>";
	i = 0;
	for (var r = 0; r < this.BeforeImageRows; r++) {
		field += '<div class="row prd_row_wrap">';
		for (var c = 0; c < this.BeforeImageColumns; c++) {
			if (imagesBefore[i] != null) {
				var value = imagesBefore[i];
				field += '<div name="' + iname + '" id="' + value.imgId + '" class="prd-img-wrap"><img src="' + sf.SurveyMediaUrl + value.imgSrc + '"></div>';
			}
			i++;
		}
		field += '</div>';
	} 	
	field+='</div></div></div></div></div>';

    //	field+='<div id="'+ctrlId+'" class="ctrlIdimg" onclick="multiSelectImage.prototype.setImage('+this.id+')"><img src="./img/default.jpg"></div>';

    return field;
}

multiSelectImage.prototype.PreLoad = function (newImage) {
    var img = new Image();
    img.src = newImage;
    this.imagesPreloaded.push(img);
}

multiSelectImage.prototype.setImage = function (obj, fno, iid) {
    if ($(obj).hasClass('selected')) {
        $(obj).removeClass('selected');
        $(obj).parent().find('.mask').hide();

        this.getValues(fno, 'field_' + iid);
    }
    else {
        var ImagesToSelect = (buildData.varFormControls[fno].ImagesToSelect > 0 ? buildData.varFormControls[fno].ImagesToSelect : 1);

        if ($(obj).parent().parent().parent().find('img.selected').length < ImagesToSelect) {
            $(obj).addClass('selected');
            $(obj).parent().find('.mask').show();
        }

        this.getValues(fno, 'field_' + iid);

        if (ImagesToSelect == 1) {
            sf.NextPage();
        }
    }
}

multiSelectImage.prototype.onClickImageMask = function (obj, fno, iid) {
    multiSelectImage.prototype.setImage($(obj).parent().find('img'), fno, iid);
}

multiSelectImage.prototype.getValues = function (index, name) {
    buildData.varFormControls[index].value = '';
    $.each($('div[name="' + name + '"]'), function (iter, value) {
        if ($(this).find('img').hasClass('selected')) {
            buildData.varFormControls[index].value += this.id + ',';
        }
    });
}



/*
multiSelectImage.prototype.setImage = function(controlId) {
		android.imageFunction(controlId);
}*/


multiSelectImage.prototype.setValues = function(path) {
 	$('#field_'+this.id+'_image').children("img").attr("src",path);
 }


multiSelectImage.prototype.ViewBeforeImage = function (fno) {
	var obj = buildData.varFormControls[fno];
	
	var ctrlId = '#viewButton_' + obj.id;
	$(ctrlId).css('display', 'none');
	
	resizeShelfImages();

	ctrlId = '#field_'+obj.id+'_image';
	$(ctrlId).css('display', 'block');

	obj.Stopwatch.start();

	if (parseInt(obj.BeforeImageTimeout) > 0) {
	    setTimeout(function () {
	        multiSelectImage.prototype.shuffleAfterShelfImage(obj)
	    }, parseInt(obj.BeforeImageTimeout) * 1000);
	} else {
	    multiSelectImage.prototype.shuffleAfterShelfImage(obj);
	}
}


multiSelectImage.prototype.shuffleAfterShelfImage = function(obj) {
	var imageUrl = obj.AfterImage;
	var imgArray = obj.imagesArr;
	var name = obj.name;
	var qno = obj.qno;
	var id = obj.id;

	var ctrlId = 'field_' + id + '_image';

	$('#' + ctrlId + ' .shelf-bordr').css('background-image', 'url(' + sf.SurveyMediaUrl + imageUrl + ')');

	shuffleReplace='';
	i = 0;
	var elemStyle = "";
	for (var r = 0; r < obj.AfterImageRows; r++) {
	    if (r > 0) {
	        //elemStyle = 'style="top: -1%; position: relative;"';
	    }
		shuffleReplace += '<div class="row prd_row_wrap" ' + elemStyle + '>';
		for (var c = 0; c < obj.AfterImageColumns; c++) {
			if (obj.imagesAfter[i] != null) {
				var value = obj.imagesAfter[i];
				shuffleReplace += '<div name="' + name + '" id="' + value.imgId + '" class="prd-img-wrap"><img src="' + sf.SurveyMediaUrl + value.imgSrc + '" onClick="multiSelectImage.prototype.setImage(this,' + qno + ',' + id + ')"><span class="mask" onClick="multiSelectImage.prototype.onClickImageMask(this,' + qno + ',' + id + ')"></span></div>';
			}
			i++;
		}
		shuffleReplace += '</div>';
	} 	

	$('#' + ctrlId + ' .shuffleImg').replaceWith(shuffleReplace);

	resizeShelfImages();

}