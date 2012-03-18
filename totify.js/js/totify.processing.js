
markup={};
markup.result = function() {
  html='<span id="word' + this.tid + '" class="word with-select main">'+this.word+'</span>';
  html+='<ul id="changes' + this.tid + '" class="variants"></ul>';
  return html;
}

$(document).ready(function () {
		
		GetLastDraft();
		EnableHovers();
		
		$('#editable').live('mousedown focus', function() {
		   $('#editable').addClass('changing');
		}).live('blur', function() {
		   HideVariants();
		   SaveDraft();		  
    	   $('#editable').removeClass('changing');		    
		}).live('click', function () {//HideVariants();
		}).live('keyup paste',function (eventData) {
			 HideVariants();
			 log('keyCode pressed ' + eventData.keyCode);
			 log('caret position ' +  getCaret());
			 var isNewWordTyped = eventData.keyCode == 32; // temporary if only space pressed
			 if (isNewWordTyped) {	
			 	 log('new word detected');
			 	 $.post('/totify.fs', {text:"John"}, function(jsonAnswer) {
  						log('jsonAnswer = ' + JSON.stringify(jsonAnswer))
				 		// TODO convert eventData.keyCode to char and append
				 		RenderResults(jsonAnswer, '&shy;');
				 }).error(function(error) { console.log(error.message); });  			  	 
				
				
			 }
         });
    });

	function EnableHovers() {
		$('#editable .with-select').hover(function () {$(this).addClass("hover")}, function () {$(this).removeClass("hover")})
		$('#editable .with-select').click(function () {
			var pos = $(this).offset();
			var thisword = this;
			$('ul.variants').hide();
			var key = $(this).attr('id').replace('word','')
			log ('key ' + key);
			$('#changes' + key).css({"left": pos.left + "px"}); $('#changes'+key).show();
			$('.variants li').hover(function () {$(this).addClass("hover")}, function () {$(this).removeClass("hover")});
			$('.variants li span').click(function () {ApplySynonym(this.id)});
		})
	}
	function HideVariants() {
		$('ul.variants').hide();
	}
	function ApplySynonym(elem_id) {
		var elem =  $('#' + elem_id);
		log('applying from span with id ' + elem_id + ' to word with tid ' + elem.attr('tid'));
		$('#word' + elem.attr('tid')).html(elem.html());
		HideVariants();
		SaveDraft();
	}
    function RenderResults (data, appendix) {
         HtmlToTTY()
    	 log("try to render " + data);
    	 var savedCaret = getCaret();
    	 //log(savedSelection.focusOffset);
    	 $('#editable').empty();
    	 $.each(data.tokens, function(index, a)
    	 {
    	 	if (a.type == "word") {    	 	
	    	 	var compiled_span = markup.result.apply(a);
	    	 	$('#editable').append(compiled_span);
	    	 	var compiled_lis = "";
	    	 	$.each(a.synonyms, function (index, s) { compiled_lis += "<li><span id= " + s.fwid + " tid=" + a.tid + ">" + s.word + "</span></li>"; });
	    	 	$('#changes'+a.tid).append(compiled_lis);
    	 	}
    	 	if (a.type == "other") {
    	 		$('#editable').append(a.content);
    	 	}
    	 });
    	 $('#editable').append(appendix);
    	 EnableHovers();    	 
    	 setCaret(savedCaret);
    }      
    
    function HtmlToTTY() {
    	var result_data = new Object();
    	result_data.words = [];
    	$('#editable span.main').each(function() {    		
    		var newspan = new Object();
    		newspan.tid = this.id;
    		newspan.word = $(this).html();
    		newspan.type = "word";
    		result_data.words.push(newspan);//[index] = newspan; 
    	});
    	log (JSON.stringify(result_data));
    }

