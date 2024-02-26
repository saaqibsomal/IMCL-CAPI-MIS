$(document).ready(function () {
    $("#fromdate").datepicker();
    $("#todate").datepicker();
    $("#btnsubmit").on('click', function (event) {
        event.preventDefault();
        
        var path = document.location.toString();
        var spl = path.split("?");
        path = spl[0] + "?";
        var url = path;  //"~/Survey/Map/3390?"; //http://localhost:54050

        if ($("#fromdate").val() != "" && $("#todate").val() != "") {

            var fromdate = $("#fromdate").val(); //.replace('0','')
            var spl_fromdate = fromdate.split('/');

            var day_fromdate = spl_fromdate[1];
            var month_fromdate = spl_fromdate[0];
            //if (spl_fromdate[1] != '10' && spl_fromdate[1] != '20' && spl_fromdate[1] != '30') {
            //    day_fromdate = spl_fromdate[1].replace('0', '');
            //}
            //if (month_fromdate != '10') {
            //    month_fromdate = spl_fromdate[0].replace('0', '');
            //}

            var todate = $("#todate").val();
            var spl_todate = todate.split('/');

            var day_todate = spl_todate[1];
            var month_todate = spl_todate[0];
            //if (spl_todate[1] != '10' && spl_todate[1] != '20' && spl_todate[1] != '30') {
            //    day_todate = spl_todate[1].replace('0', '');
            //}
            //if (month_todate != '10') {
            //    month_todate = spl_todate[0].replace('0', '');
            //}

            //City
            var selectedOptions = $('#fctlCity option:selected')
            var cityIDs = "";
            selectedOptions.each(function (index, option) {
                cityIDs += option.value.substr(1) + ',';        //Discard _
            });
            if (cityIDs.length > 0) {
                cityIDs = cityIDs.substring(0, cityIDs.length - 1);
            }
            //Surveyors
            var selectedOptions = $('#fctlSurveyor option:selected')
            var SurveyorIDs = "";
            selectedOptions.each(function () {
                SurveyorIDs += selectedOptions.text() + ',';
            });
            if (SurveyorIDs.length > 0) {
                SurveyorIDs = SurveyorIDs.substring(0, SurveyorIDs.length - 1);
            }

            //Format URL
            url += "&fromdate=" + spl_fromdate[2] + month_fromdate + day_fromdate;
            url += "&todate=" + spl_todate[2] + month_todate + day_todate;
            if (cityIDs.length > 0) {
                url += "&c=" + cityIDs;
            }
            if (SurveyorIDs.length > 0) {
                url += "&surveyor=" + SurveyorIDs;
            }
            window.location = url;
        }
    });

    $('#fctlCity').multiselect({
        enableCaseInsensitiveFiltering: true,
        enableClickableOptGroups: true,
        maxHeight: 400
    });
    if (getParameterByName("c") != "") {
        $('#fctlCity').multiselect('select', '_' + getParameterByName("c"));
    }

    $('#fctlSurveyor').multiselect({
        enableCaseInsensitiveFiltering: true,
        maxHeight: 400
    });
    if (getParameterByName("surveyor") != "") {
        $('#fctlSurveyor').multiselect('select', getParameterByName("surveyor"));
    }

    $('.mappg-dtpck-area').show();

    //if (getParameterByName("country") != "") {
    //    url += "&country=" + getParameterByName("country");
    //}

});


var liveTrackTimer;
var live_date, live_id;

function Surveyorlist() {
    if (surveydates.length > 0) {
        //var out = '<input class="parent-chk-dt" type="checkbox" checked="checked" onClick="toggle(this)" />Surveyor Date<br/>';
        var daySelect = document.getElementById('mctlTrackDates');
        for (i = 0; i < surveydates.length; i++) {
            var color = polyLines[i].strokeColor;
            daySelect.options[daySelect.options.length] = new Option(surveydates[i], '_'+i);
            //out += "<div class='rgt-panel-chk-area'>" +
            //    "<input id='" + i + "' class='chksurveyordate' type='checkbox' checked='checked' name='foo'>" +
            //            "<span>" + surveydates[i] + "</span></div>";
            //// out += '<li>'+surveydates[i]+'</li>';
        }
        //$("#sidedata").html(out);
        $(".panelSurveyordates").show();

        $('#mctlTrackDates').multiselect({
            includeSelectAllOption: true,
            maxHeight: 400,
            onChange: function (option, checked, select) {
                if (typeof (option) != "undefined") {
                    var index = $(option).val().toString().substr(1);
                    var i = parseInt(index);
                    if (checked == true) {
                        runSnapToRoad(polyLines[i].getPath());
                        polyLines[i].setMap(map);
                    } else {
                        polyLines[i].setMap(null);
                    }
                    SetupLiveTracker($(option).text(), i, checked);
                } else {
                    clearInterval(liveTrackTimer);
                    if (checked == true) {
                        ShowAllTracks();
                        //SnapToRoad();
                    } else {
                        ClearAllTracks();
                    }
                }
            }
        });
        $('#mctlTrackDates').multiselect('selectAll', false);
        $('#mctlTrackDates').multiselect('refresh');

        $('#mctlOptions').multiselect({
            onChange: function (option, checked, select) {
                if ($(option).val() == 'SE') {
                    showStartEndMarkers = checked;
                    if (showStartEndMarkers) {
                        var value = map;
                    } else {
                        var value = null;
                    }
                    for (var i = 0; i < seMarkersArray.length; i++) {
                        seMarkersArray[i].setMap(value);
                    }
                } else if ($(option).val() == 'TP') {
                    showTrackPointMarkers = checked;
                    if (showTrackPointMarkers) {
                        var value = map;
                    } else {
                        var value = null;
                    }
                    for (var i = 0; i < pathPointArray.length; i++) {
                        pathPointArray[i].setMap(value);
                    }
                }
            }
        });
        $('#mctlOptions').multiselect('select', ['SE']);

    } else {
        $(".panelSurveyordates").hide();
    }
}

function getTodaysDate() {
    var today = new Date();
    var dd = today.getDate();
    var mm = today.getMonth() + 1; //January is 0!
    var yyyy = today.getFullYear();

    today = dd + '-' + mm + '-' + yyyy;
    return today;
}

function SetupLiveTracker(selectedDate, dateIndex, isChecked) {
    var today = getTodaysDate();
    selectedDate = selectedDate.substring(0, selectedDate.indexOf('('));

    if (isChecked == true) {
        live_date = today;
        live_id = dateIndex;

        if (selectedDate == today) {
            liveTrackTimer = setInterval(updateSurveyorTracks, 1000 * 60);
        }

        polyLines[dateIndex].setMap(map);
    } else {
        if (selectedDate == today) {
            clearInterval(liveTrackTimer);
        }
        polyLines[dateIndex].setMap(null);
    }
}

function updateSurveyorTracks() {
    var pSurv = getUrlVars()["surveyor"];
    var pCity = getUrlVars()["c"];
    var pCnry = getUrlVars()["country"];

    $.ajax({
        type: "POST",
        url: "/Survey/TrackData?surveyor=" + pSurv + "&c=" + pCity + "&country=" + pCnry + "&fordate=" + live_date,
    })
    .done(function (data) {
        addSurveyorTracks(data, true);
    });
}


var MIN_DISTANCE_FOR_DIRECTIONS = 200;
var MIN_DISTANCE_PER_SECOND_AS_VALID = 30;

function addSurveyorTracks(trackdata, isUpdate) {
    if (trackdata == null) {
        trackdata = markers;
    }
    if (isUpdate == null) {
        isUpdate = false;
        drawUserPaths();
    }
    if (trackdata.length > 0) {
        var currentDate = "";
        var currentUser = "";
        var path;
        var lineno = (isUpdate ? live_id : 0);
        var latlngbounds = new google.maps.LatLngBounds();
        var myLatlng;
        var lastRouteEnd = null;

        if (isUpdate) {
            polyLines[live_id].setMap(null);
        }

        for (i = 0; i < trackdata.length; i++) {
            var data = trackdata[i];
            if (data.Cat == "" && data.ID == 0) {
                if ((currentDate.length == 0 || currentDate != data.dt) || (currentUser.length == 0 || currentUser != data.user)) {
                    //Intialize the Path Array
                    path = new google.maps.MVCArray();
                    currentDate = data.dt;
                    currentUser = data.user;

                    //Set the Path Stroke Color
                    color = '#11' + (((lineno + 25) * 5) % 255).toString(16) + (((lineno + 50) * 50) % 255).toString(16);//'#'+Math.floor(Math.random()*16777215).toString(16);
                    polyLines[lineno] = new google.maps.Polyline({ path: path, map: map, strokeColor: color, strokeOpacity: 0.5, strokeWeight: 2 });
                    polyLineFix[lineno] = 0;
                    if (!isUpdate) {
                        surveydates[lineno] = currentDate + ' (' + currentUser + ')';
                    }
                    // Add a new marker at the new plotted point on the polyline.
                    if (i > 0) {
                        var datap = trackdata[i - 1];
                        myLatlng = new google.maps.LatLng(datap.lat, datap.lng);
                        var marker = new google.maps.Marker({
                            position: myLatlng,
                            title: 'End of day for date: ' + datap.dt + '  ' + datap.tm + ' by User: ' + datap.user
                        });
                        seMarkersArray.push(marker);
                        if (showStartEndMarkers) {
                            marker.setMap(map);
                        }
                    }
                    myLatlng = new google.maps.LatLng(data.lat, data.lng);
                    var marker = new google.maps.Marker({
                        position: myLatlng,
                        title: 'Start of day for date: ' + currentDate + '  ' + data.tm + ' by User: ' + currentUser
                    });
                    seMarkersArray.push(marker);
                    if (showStartEndMarkers) {
                        marker.setMap(map);
                    }

                    if (!isUpdate) {
                        trackdata[i].lineno = lineno;
                    }
                    lineno++;
                }

                //Optimize track by removing invalid points
                dist = 0;
                data.inv = 0;     //inv = Invalid
                if (i > 0) {
                    var prevdata = trackdata[i - 1];
                    p = i;
                    do {
                        if (trackdata[p - 1].inv == 0) {
                            prevdata = trackdata[p - 1];
                            break;
                        } else {
                            //Console.log(trackdata[p-1]);
                            p = p;
                        }
                        p--;
                    } while (p > 0);

                    if (data.lineno == prevdata.lineno) {

                        if (lastRouteEnd != null) {
                            prevdata = lastRouteEnd;
                        }
                        dist = getDistance(prevdata, data);

                        //Seconds from last position
                        sfcp = getMarkerSeconds(data.tm);
                        sflp = sfcp - getMarkerSeconds(prevdata.tm);

                        //Seconds from next position
                        if (i < trackdata.length - 1) {
                            nextdata = trackdata[i + 1];
                            distn = getDistance(nextdata, data);
                            disto = getDistance(nextdata, prevdata);
                            sfnp = getMarkerSeconds(nextdata.tm) - sfcp;

                            if (dist > (sflp * MIN_DISTANCE_PER_SECOND_AS_VALID)) {
                                data.inv = 1;
                            }
                        }

                        //dist = getDistance(prevdata, data);
                        //if (dist > MIN_DISTANCE_FOR_DIRECTIONS) {       //If distance is greater than 250m
                        //    var markerTitle = prevdata.yy + "-" + prevdata.mm + "-" + prevdata.dd + "#" + i + "_" + dist;
                        //    var prevLatlng = new google.maps.LatLng(prevdata.lat, prevdata.lng);
                        //    var smarker = new google.maps.Marker({
                        //        position: prevLatlng,
                        //        title: 'Route start date: ' + markerTitle + '  ' + prevdata.hh + ":" + prevdata.mins + ":" + prevdata.ss,
                        //        map: map
                        //    });
                        //    markerTitle = data.yy + "-" + data.mm + "-" + data.dd + "#" + i + "_" + dist;
                        //    var newLatlng = new google.maps.LatLng(data.lat, data.lng);
                        //    var emarker = new google.maps.Marker({
                        //        position: newLatlng,
                        //        title: 'Route end date: ' + markerTitle + '  ' + data.hh + ":" + data.mins + ":" + data.ss,
                        //        map: map
                        //    });

                        //    getDirectionsAsync(lineno-1, path, path.length, prevLatlng, newLatlng);
                        //}
                    }
                }
                if (dist < MIN_DISTANCE_FOR_DIRECTIONS && data.inv == 0) {
                    //Add current post to current Path
                    myLatlng = new google.maps.LatLng(data.lat, data.lng);

                    path.push(myLatlng);

                    //Build the area of the points, so that it can be centered on the map
                    var marker = new google.maps.Marker({
                        position: new google.maps.LatLng(data.lat, data.lng),
                        //map: map,
                        //title: data.title
                    });

                    latlngbounds.extend(marker.position);
                }
                var pathPoint = new google.maps.Marker({
                    position: myLatlng,
                    title: "User: " + data.user + ", Date-Time: " + data.dt+' '+data.tm,
                    icon: trackPoint
                });
                pathPointArray.push(pathPoint);
                if (showTrackPointMarkers) {
                    pathPoint.setMap(map);
                }
            }
        }
        if (!isUpdate) {
            Surveyorlist();
        }
        if (i > 0) {
            var datap = markers[trackdata.length - 1];
            myLatlng = new google.maps.LatLng(datap.lat, datap.lng);
            var marker = new google.maps.Marker({
                position: myLatlng,
                title: 'End of day for date: ' + datap.dt + '  ' + datap.tm + ' by User: ' + datap.user,
                map: map
            });
            seMarkersArray.push(marker);
            if (showStartEndMarkers) {
                marker.setMap(map);
            }
        }
        map.setCenter(latlngbounds.getCenter());
        map.fitBounds(latlngbounds);

    }

}

function getMarkerSeconds(markerTime) {
    var secOfDay = 0;
    var mtime = markerTime.split(':');
    if (mtime.length == 3) {
        secOfDay = (mtime[0] * 60 * 60) + (mtime[1] * 60) + mtime[2];
    }
    return secOfDay;
}

var DirectionsServiceRunning = 0;
function getDirectionsAsync(atLineno, path, pathpos, prevLatlng, newLatlng) {
    var service = new google.maps.DirectionsService();

    setTimeout(function () {
        if (DirectionsServiceRunning == 0) {
            DirectionsServiceRunning = 1;
            service.route({
                origin: prevLatlng,
                destination: newLatlng,
                travelMode: google.maps.DirectionsTravelMode.DRIVING
            }, function (result, status) {
                if (status == google.maps.DirectionsStatus.OK) {
                    var ir = 0;
                    pathpos += polyLineFix[atLineno];
                    for (ir = 0, len = result.routes[0].overview_path.length; ir < len; ir++) {
                        path.insertAt(pathpos + ir, result.routes[0].overview_path[ir]);
                    }
                    polyLineFix[atLineno] += result.routes[0].overview_path.length;
                    DirectionsServiceRunning = 0;
                }
            });
        }
        else {
            getDirectionsAsync(atLineno, path, pathpos, prevLatlng, newLatlng);
        }
    }, 1000);
}

var rad = function (x) {
    return x * Math.PI / 180;
};

var getDistance = function (p1, p2) {
    var R = 6378137; // Earth’s mean radius in meter
    var dLat = rad(p2.lat - p1.lat);
    var dLong = rad(p2.lng - p1.lng);
    var a = Math.sin(dLat / 2) * Math.sin(dLat / 2) +
      Math.cos(rad(p1.lat)) * Math.cos(rad(p2.lat)) *
      Math.sin(dLong / 2) * Math.sin(dLong / 2);
    var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
    var d = R * c;
    return d; // returns the distance in meter
};

function ClearAllTracks() {
    for (var i = 0; i < polyLines.length; i++) {
        polyLines[i].setMap(null);
    }
}
function ShowAllTracks() {
    for (var i = 0; i < polyLines.length; i++) {
        polyLines[i].setMap(map);
    }
}

////////////////////////////////////////////////////////////////////////////
var placeIdArray = [];
var snappedCoordinates = [];
var roadPolylines = [];

//var apiKey = "AIzaSyCTEYMi7LuB6wk8runMAWUFt53_uukaQR8";
var apiKey = "AIzaSyChoiI359ZNHAyqd_WWQbVxKhMmZ4hZewk";

function SnapToRoad() {
    for (var i = 0; i < polyLines.length; i++) {
        polyLines[i].setMap(null);
        runSnapToRoad(polyLines[i].getPath());
        polyLines[i].setMap(map);
    }
}

// Snap a user-created polyline to roads and draw the snapped path
function runSnapToRoad(path) {
    var pathValues = [];
    for (var i = 0; i < path.getLength() && i < 100; i++) {
        pathValues.push(path.getAt(i).toUrlValue());
    }
    var url = 'https://roads.googleapis.com/v1/snapToRoads?interpolate=true&key=' + apiKey + '&path=' + pathValues.join('|');
    
    jsonp(url, function (data) {
        processSnapToRoadResponse(data);
        drawSnappedPolyline();
        //getAndDrawSpeedLimits();
    });

    //$.ajax({
    //    url: 'https://roads.googleapis.com/v1/snapToRoads?interpolate=true&key=' + apiKey + '&path=' + pathValues.join('|'),
    //    type: "GET",
    //    dataType: 'jsonp',
    //    cache: false,
    //    success: function (data) {
    //        processSnapToRoadResponse(data);
    //        drawSnappedPolyline();
    //        getAndDrawSpeedLimits();
    //    }
    //});
}

// Store snapped polyline returned by the snap-to-road method.
function processSnapToRoadResponse(data) {
    snappedCoordinates = [];
    placeIdArray = [];
    for (var i = 0; i < data.snappedPoints.length; i++) {
        var latlng = new google.maps.LatLng(
            data.snappedPoints[i].location.latitude,
            data.snappedPoints[i].location.longitude);
        snappedCoordinates.push(latlng);
        placeIdArray.push(data.snappedPoints[i].placeId);
    }
}

// Draws the snapped polyline (after processing snap-to-road response).
function drawSnappedPolyline() {
    var snappedPolyline = new google.maps.Polyline({
        path: snappedCoordinates,
        strokeColor: 'black',
        strokeWeight: 2
    });

    snappedPolyline.setMap(map);
    roadPolylines.push(snappedPolyline);
}

// Gets speed limits (for 100 segments at a time) and draws a polyline
// color-coded by speed limit. Must be called after processing snap-to-road
// response.
function getAndDrawSpeedLimits() {
    for (var i = 0; i <= placeIdArray.length / 100; i++) {
        // Ensure that no query exceeds the max 100 placeID limit.
        var start = i * 100;
        var end = Math.min((i + 1) * 100 - 1, placeIdArray.length);

        drawSpeedLimits(start, end);
    }
}

// Gets speed limits for a 100-segment path and draws a polyline color-coded by
// speed limit. Must be called after processing snap-to-road response.
function drawSpeedLimits(start, end) {
    var placeIdQuery = '';
    for (var i = start; i < end; i++) {
        placeIdQuery += '&placeId=' + placeIdArray[i];
    }

    $.get('https://roads.googleapis.com/v1/speedLimits',
        'key=' + apiKey + placeIdQuery,
        function (speedData) {
            processSpeedLimitResponse(speedData, start);
        }
    );
}

// Draw a polyline segment (up to 100 road segments) color-coded by speed limit.
function processSpeedLimitResponse(speedData, start) {
    var end = start + speedData.speedLimits.length;
    for (var i = 0; i < speedData.speedLimits.length - 1; i++) {
        var speedLimit = speedData.speedLimits[i].speedLimit;
        var color = getColorForSpeed(speedLimit);

        // Take two points for a single-segment polyline.
        var coords = snappedCoordinates.slice(start + i, start + i + 2);

        var snappedPolyline = new google.maps.Polyline({
            path: coords,
            strokeColor: color,
            strokeWeight: 6
        });
        snappedPolyline.setMap(map);
        roadPolylines.push(snappedPolyline);
    }
}

function getColorForSpeed(speed_kph) {
    if (speed_kph <= 40) {
        return 'purple';
    }
    if (speed_kph <= 50) {
        return 'blue';
    }
    if (speed_kph <= 60) {
        return 'green';
    }
    if (speed_kph <= 80) {
        return 'yellow';
    }
    if (speed_kph <= 100) {
        return 'orange';
    }
    return 'red';
}


function drawUserPaths() {
    for (var i = 0; i < userPaths.length; i++) {
        snappedCoordinates = [];
        var coords = userPaths[i].path.split('|');
        for (var c = 0; c < coords.length; c++) {
            var coord = coords[c].split(',');
            var latlng = new google.maps.LatLng(coord[0], coord[1]);
            snappedCoordinates.push(latlng);
        }
        drawSnappedPolyline();
    }
}
