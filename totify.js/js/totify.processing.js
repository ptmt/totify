$(document).ready(function () {
		
		util.getLastDraft('#editable');
		totify.enableHovers();
		util.setCaret(0);

		$('#editable').live('mousedown focus', function() {
		   $('#editable').addClass('changing');
		}).live('blur', function() {
		   totify.hideVariants();
		   util.saveDraft('#editable');		  
    	   $('#editable').removeClass('changing');		    
		}).live('click', function () {//HideVariants();
		}).live('keyup paste',function (eventData) {
			 totify.hideVariants();			
			 var isNewWordTyped = eventData.keyCode == 32; // temporary if only space pressed
			 if (isNewWordTyped) {	
			 	 log('new word detected');
			 	 totify.process();
			 }
         });
		 totify.process();
    });


(function(totify, $, undefined ) {

	markup={};
	markup.result = function() {
	  html='<span id="word' + this.Token.Id + '" class="word with-select main">'+this.Token.Content +'</span>';
	 // html='<div class="bubble"><div class="pointer"><div class="one"></div><div class="two"></div></div><div class="content">';
	  html+='<ul id="changes' + this.Token.Id + '" class="variants"></ul>';
	 // html+='</div></div>';
	  return html;
	}


	totify.process = function () {
		 $('#editable .variants').remove();
		 var text = $('#editable').text();
		 if (text.length > 3) {
		 	 log("function Process() text= " + text);		 	 
		 	 jQuery.ajax({
	                type: "POST",
	                url: "totify.fs",
	                dataType:"json",
	                timeout: 5000,
	                data:text,
	                success:function(response){                   

	                    RenderResults(response, '<span>&shy;</span>' );//&shy;');
	                },
	                error:function (xhr, ajaxOptions, thrownError){
	                    log(xhr.status);
	                    log(thrownError);
	                }    
	            });	 
	 	 }
	}

	totify.enableHovers = function() {
		$('#editable .with-select').hover(function () {$(this).addClass("hover")}, function () {$(this).removeClass("hover")})
		$('#editable .with-select').click(function () {
			var pos = $(this).offset();
			var thisword = this;
			$('ul.variants').hide();
			var key = $(this).attr('id').replace('word','')
			log ('key ' + key);
			$('#changes' + key).css({"left": pos.left-20 + "px"}); $('#changes'+key).show();
			$('.variants li').hover(function () {$(this).addClass("hover")}, function () {$(this).removeClass("hover")});
			$('.variants li span').click(function () {ApplySynonym(this.id)});
		})
	}

	totify.hideVariants = function() {
		$('ul.variants').hide();
	}

	totify.applySynonym = function(elem_id) {
		var elem =  $('#' + elem_id);
		log('applying from span with id ' + elem_id + ' to word with tid ' + elem.attr('tid'));
		$('#word' + elem.attr('tid')).html(elem.html());
		totify.hideVariants();
		util.SaveDraft();
	}

	totify.renderResults  = function  (data, appendix) {
         var savedCaret = util.getCaret();
    	 log ("data = " + JSON.stringify(data));
    	 $('#editable').empty();
    	 
    	 $.each(data, function(index, a)
    	 {
    	 
	    	 	var compiled_span = markup.result.apply(a);
	    	 	$('#editable').append(compiled_span);
	    	 	var compiled_lis = "";
	    	 	$.each(a.Changes, function (index, s) { 
	    	 		$.each(s.Variants, function (i, v) {
	    	 			compiled_lis += "<li class='variants'><span id='" + i + "' tid='" + a.Token.Id + "'>" + v+ "</span></li>";
	    	 		});
	    	 		
	    	 	});
	    	 	$('#changes'+a.Token.Id).append(compiled_lis + appendix);
    	 
    	 });    	 
    	// $('#editable').append(appendix);
    	 totify.enableHovers();    	 
    	 util.setCaret(savedCaret);
    }    

    totify.htmlToText = function() {
    	var result_data = "";
    	$('#editable span.main').each(function() {    		
    		result_data += $(this).html();
    	});
    	return return_data;
    }  
    


}(window.totify = window.totify || {}, jQuery));