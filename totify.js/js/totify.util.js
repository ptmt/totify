// http://enterprisejquery.com/2010/10/how-good-c-habits-can-encourage-bad-javascript-habits-part-1/
(function( util, $, undefined ) {
    
    util.log = function (message) {
        console.log(message);
    };

    util.getCaret = function () {
        var savedRange;
        if(window.getSelection)//non IE Browsers
        {
            savedRange = window.getSelection();
        }
        else if(document.selection)//IE
        { 
            savedRange = document.selection.createRange();  
        } 
        return savedRange.focusOffset;
    };

    util.setCaret = function (pos) {
        ctrl = document.getElementById('editable');
        ctrl.focus();
        //pos = ctrl.value.length;   
        util.log ("moving to pos = " + pos);
        var range,selection;
        if(document.createRange)//Firefox, Chrome, Opera, Safari, IE 9+
        {
            range = document.createRange();//Create a range (a range is a like the selection but invisible)
            range.selectNodeContents(ctrl);//Select the entire contents of the element with the range
            //range.focusOffset = pos;
            //range.setStart(ctrl , pos-1);
            //range.setEnd(ctrl , pos);
            range.collapse(false);//collapse the range to the end point. false means collapse to end rather than the start
            selection = window.getSelection();//get the selection object (allows you to change selection)
            selection.removeAllRanges();//remove any selections already made
            selection.addRange(range);//make the range you have just created the visible selection
        }
        else if(document.selection)//IE 8 and lower
        { 
            range = document.body.createTextRange();//Create a range (a range is a like the selection but invisible)
            range.moveToElementText(ctrl);//Select the entire contents of the element with the range
            range.collapse(false);//collapse the range to the end point. false means collapse to end rather than the start
            range.select();//Select the range (make it the visible selection
        }
    };

    util.saveDraft = function(id) {
        util.log ('saving...' + $(id).html());
        localStorage.setItem('contenteditable', $(id).html());
    };

    util.getLastDraft = function(id) {
        if (localStorage.getItem('contenteditable')) 
        {
            var content = localStorage.getItem('contenteditable');
            if (content === "")
              $(id).html('<span></span>');
            else
              $(id).html(localStorage.getItem('contenteditable'));
        }
        else
            $(id).html("<span>Привет, круто, что ты с нами</span>"); //. Используйте всплывающие окна, включайте и выключайте фильтры</p>");
    };    
    
}(window.util = window.util || {}, jQuery));