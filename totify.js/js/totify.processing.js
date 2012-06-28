var global_lock_timeout = false;

$(document).ready(function () {
		
		util.getLastDraft('#editable');
		totify.enableHovers();
		util.setCaret(0);
		totify.quotechange();
		setTimeout(totify.quotechange, 10000);


		$('#editable').live('mousedown focus', function() {
		   $('#editable').addClass('changing');
		}).live('blur', function() {
		   totify.hideVariants();
		   util.saveDraft('#editable');		  
    	   $('#editable').removeClass('changing');		    
		}).live('click', function () {   if (!$(event.target).hasClass('with-select')) {totify.hideVariants();}
		}).live('keyup paste',function (eventData) {
			 //totify.lockWindow();
			 if(global_lock_timeout !== ""){
				window.clearTimeout(global_lock_timeout);
				global_lock_timeout = "";
			 }
			 global_lock_timeout = setTimeout(totify.process,1000);	
			 totify.hideVariants();	
			
         });
		 totify.process();
    });


(function(totify, $, undefined ) {

	var temp_flag = false;

	markup={};
	markup.result = function() {
	  html='<span id="word' + this.Token.Id + '" class="word ';
	  if (this.Changes.length > 0 && this.Changes[0].Variants.length > 0) { 
	  	html+='with-select';
	  } 
	  html+= ' main">'+this.Token.Content +'</span>';	 
	  html+='<ul id="changes' + this.Token.Id + '" class="variants"></ul>';	 
	  return html;
	}


	totify.isLocked = function() {
		return global_lock;
	}

	

	totify.process = function () {
		// if (!totify.isLocked()) {
			 $('#editable .variants').remove();
			 var text = totify.htmlToText();//$('#editable').text();
			 if (text.length > 3) {
			 	 $('.loading').show();
			 	 //util.log("function Process() text= " + text);		 	 
			 	 jQuery.ajax({
		                type: "POST",
		                url: "totify.fs",
		                scriptCharset: "utf-8" ,
		                contentType: "application/json; charset=utf-8",
		                dataType:"json",
		                timeout: 15000,
		                data:text,
		                success:function(response){                  
		                	$('.loading').hide();
		                    totify.renderResults(response);//, '<span>&shy;</span>' );//&shy;');
		                },
		                error:function (xhr, ajaxOptions, thrownError){
		                	$('.loading').hide();
		                    util.log(xhr.status);
		                    util.log(thrownError);
		                }    
		            });	 
		 	 }
	 	// }
	}

	totify.enableHovers = function() {
		$('#editable .with-select').hover(function () {$(this).addClass("hover")}, function () {$(this).removeClass("hover")})
		$('#editable .with-select').click(function () {
			var pos = $(this).offset();
			var thisword = this;
			$('ul.variants').hide();
			var key = $(this).attr('id').replace('word','')
			util.log ('key ' + key);
			$('#changes' + key).css({"left": pos.left-20 + "px"}); $('#changes'+key).show();
			$('.variants li').hover(function () {$(this).addClass("hover")}, function () {$(this).removeClass("hover")});
			$('.variants li span').click(function () { totify.applySynonym(this.id)});
		})
	}

	totify.hideVariants = function() {
		//if( $('ul.variants').is(":visible") ) {
			$('ul.variants').hide();
		//}
	}

	totify.applySynonym = function(elem_id) {
		var elem =  $('#' + elem_id);
		util.log('applying from span with id ' + elem_id + ' to word with tid ' + elem.attr('tid'));
		$('#word' + elem.attr('tid')).html(elem.html());
		totify.hideVariants();
		util.saveDraft();
	}

	totify.renderFilterName = function(filtername) {
		var rus_name = "";
		if (filtername == "synonyms")
			rus_name = "Синонимы";
		if (filtername == "replaces")
			rus_name = "Замены";
		return "<li class='highlighted'>" + rus_name + "</li>";		
	}

	totify.renderResults  = function  (data, appendix) {
         var savedCaret = util.getCaret();
    	 //util.log ("data = " + JSON.stringify(data));
    	 $('#editable').empty();    	 
    	 $.each(data, function(index, a)
    	 {
    	 		if (a.Token.Class == 1) {
		    	 	var compiled_span = markup.result.apply(a);
		    	 	$('#editable').append(compiled_span + "<span class='main space'>&nbsp;</span>");
		    	 	var compiled_lis = "";

		    	 	$.each(a.Changes, function (index, s) { 	
		    	 		//util.log(s.FilterName);	    	 		
		    	 		compiled_lis += totify.renderFilterName(s.FilterName);
		    	 		if (s.Variants.length > 0) {
			    	 		$.each(s.Variants, function (i, v) {			    	 			
			    	 			compiled_lis += "<li class='variants'><span id='" + a.Token.Id + "_" + i + "' tid='" + a.Token.Id + "'>" + v+ "</span></li>";
			    	 		});
		    	 		}
		    	 	});
		    	 	$('#changes'+a.Token.Id).append(compiled_lis);
	    	 	}
	    	 	else {
	    	 		// remove last space
	    	 		$('#editable .space').filter(":last").remove();
	    	 		$('#editable').append(a.Token.Content + "<span class='main space'>&nbsp;</span>");
	    	 	}
	    	 	
    	 		
    	 });    	 
    	 $('#editable').append("<span class='main'></span>");
    	 totify.enableHovers();    	 
    	 util.setCaret(savedCaret);
    }  

    totify.string_replace = function (haystack, find, sub) {
    	return haystack.split(find).join(sub);
	}  

    totify.htmlToText = function() {
    	var result_data = "";
    

    	result_data = totify.string_replace( $('#editable').text(), String.fromCharCode(160), String.fromCharCode(32));
    	//for(i =0; i < result_data.length; i++){
		  //var chr = result.charAt(i);
		//  var hexval = result_data.charCodeAt(i);
		//  util.log(hexval + " ");
		//}
    	return result_data;//.string_replace(result_data," ","<s>");//.string_replace(" ", "<sp>");
    }  
    
    totify.quotechange = function() {
    	 jQuery.ajax({
		                type: "GET",
		                url: "quote.fs?random",
		                scriptCharset: "utf-8" ,
		                contentType: "application/json; charset=utf-8",
		                dataType:"json",
		                timeout: 5000,		               
		                success:function(response){                  
		                	util.log ("data = " + JSON.stringify(response));
				    		$('.quote-text').html('«' + response.Quote + '»');
				    		$('.quote-text').append('<div class="quote-author">&mdash; ' + response.Author + '</div>');
		                },
		                error:function (xhr, ajaxOptions, thrownError){
		                	$('.loading').hide();
		                    util.log(xhr.status);
		                    util.log(thrownError);
		                }    
		            });	 
    	
    }

}(window.totify = window.totify || {}, jQuery));