
    
        $.extend({

        /*    Counter: {
                

                current: function(iter) {
                    var counter=iter;
                    counter++;
                    return counter;
                },

                inc: function(id){
                    counter++;
                    $('#'+id).val(counter);
                }
            } */
            
            StopWatch : {                
                
                formatTimer : function(a) {
                    if (a < 10) {
                        a = '0' + a;
                    }                              
                    return a;
                },    
                
                startTimer : function(dir) {
                    
                    var a;
                    
                    // save type
                    $.StopWatch.dir = dir;
                    
                    // get current date
                    $.StopWatch.d1 = new Date();
                    
                    switch($.StopWatch.state) {
                            
                        case 'pause' :
                            
                            // resume timer
                            // get current timestamp (for calculations) and
                            // substract time difference between pause and now
                            $.StopWatch.t1 = $.StopWatch.d1.getTime() - $.StopWatch.td;                            
                            
                        break;
                            
                        default :
                            
                            // get current timestamp (for calculations)
                            $.StopWatch.t1 = $.StopWatch.d1.getTime(); 
                            
                            // if countdown add ms based on seconds in textfield
                            if ($.StopWatch.dir === 'cd') {
                                $.StopWatch.t1 += parseInt($('#cd_seconds').val())*1000;
                            }    
                        
                        break;
                            
                    }                                   
                    
                    // reset state
                    $.StopWatch.state = 'alive';   
                    $('#' + $.StopWatch.dir + '_status').html('Running');
                    
                    // start loop
                    $.StopWatch.loopTimer();
                    
                },
                
                pauseTimer : function() {
                    
                    // save timestamp of pause
                    $.StopWatch.dp = new Date();
                    $.StopWatch.tp = $.StopWatch.dp.getTime();
                    
                    // save elapsed time (until pause)
                    $.StopWatch.td = $.StopWatch.tp - $.StopWatch.t1;
                    
                    // change button value
                    $('#' + $.StopWatch.dir + '_start').val('Resume');
                    
                    // set state
                    $.StopWatch.state = 'pause';
                    $('#' + $.StopWatch.dir + '_status').html('Paused');
                    
                },
                
                stopTimer : function() {
                    
                    // change button value
                    $('#' + $.StopWatch.dir + '_start').val('Restart');                    
                    
                    // set state
                    $.StopWatch.state = 'stop';
                    $('#' + $.StopWatch.dir + '_status').html('Stopped');
                    
                },
                
                resetTimer : function() {

                    // reset display
                    $('#' + $.StopWatch.dir + '_ms,#' + $.StopWatch.dir + '_s,#' + $.StopWatch.dir + '_m,#' + $.StopWatch.dir + '_h').html('00');                 
                    
                    // change button value
                    $('#' + $.StopWatch.dir + '_start').val('Start');                    
                    
                    // set state
                    $.StopWatch.state = 'reset';  
                    $('#' + $.StopWatch.dir + '_status').html('Reset & Idle again');
                    
                },
                
                endTimer : function(callback) {
                   
                    // change button value
                    $('#' + $.StopWatch.dir + '_start').val('Restart');
                    
                    // set state
                    $.StopWatch.state = 'end';
                    
                    // invoke callback
                    if (typeof callback === 'function') {
                        callback();
                    }    
                    
                },    
                
                loopTimer : function() {
                    
                    var td;
                    var d2,t2;
                    
                    var ms = 0;
                    var s  = 0;
                    var m  = 0;
                    var h  = 0;
                    
                    if ($.StopWatch.state === 'alive') {
                                
                        // get current date and convert it into 
                        // timestamp for calculations
                        d2 = new Date();
                        t2 = d2.getTime();   
                        
                        // calculate time difference between
                        // initial and current timestamp
                        if ($.StopWatch.dir === 'sw') {
                            td = t2 - $.StopWatch.t1;
                        // reversed if countdown
                        } else {
                            td = $.StopWatch.t1 - t2;
                            if (td <= 0) {
                                // if time difference is 0 end countdown
                                $.StopWatch.endTimer(function(){
                                    $.StopWatch.resetTimer();
                                    $('#' + $.StopWatch.dir + '_status').html('Ended & Reset');
                                });
                            }    
                        }    
                        
                        // calculate milliseconds
                        ms = td%1000;
                        if (ms < 1) {
                            ms = 0;
                        } else {    
                            // calculate seconds
                            s = (td-ms)/1000;
                            if (s < 1) {
                                s = 0;
                            } else {
                                // calculate minutes   
                                var m = (s-(s%60))/60;
                                if (m < 1) {
                                    m = 0;
                                } else {
                                    // calculate hours
                                    var h = (m-(m%60))/60;
                                    if (h < 1) {
                                        h = 0;
                                    }                             
                                }    
                            }
                        }
                      
                        // substract elapsed minutes & hours
                        ms = Math.round(ms/100);
                        s  = s-(m*60);
                        m  = m-(h*60);                                
                        
                        // update display
                        $('#' + $.StopWatch.dir + '_ms').html($.StopWatch.formatTimer(ms));
                        $('#' + $.StopWatch.dir + '_s').html($.StopWatch.formatTimer(s));
                        $('#' + $.StopWatch.dir + '_m').html($.StopWatch.formatTimer(m));
                        $('#' + $.StopWatch.dir + '_h').html($.StopWatch.formatTimer(h));
                        
                        // loop
                        $.StopWatch.t = setTimeout($.StopWatch.loopTimer,1);
                    
                    } else {
                    
                        // kill loop
                        clearTimeout($.StopWatch.t);
                        return true;
                    
                    }  
                    
                }
                    
            } 

        
        });

